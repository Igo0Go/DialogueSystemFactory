using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScenePoint : MonoBehaviour
{
    [Tooltip("Контроллер персонажей")] public DialogueTeamDirector teamDirector;
    [Tooltip("Схема диалога")] public DialogueSceneKit scene;
    [Tooltip("Подсказка, которая будет высвечиваться.")] public string tip;

    [Space(20), Header("UI"), Tooltip("Панель для субтитров")] public GameObject subsPanel;
    [Tooltip("Текст субтитров")] public Text subsText;
    [Tooltip("Панель для сообщения")] public GameObject messagePanel;
    [Tooltip("Текст сообщения")] public Text messageText;
    [Tooltip("Текст пропуска")] public GameObject skipTip;
    [Tooltip("Варианты ответа")] public List<AnswerUI> answers = new List<AnswerUI>();
    [Tooltip("Панель-подсказка")] public GameObject tipPanel;
    [Tooltip("UI-Текст посдказки")] public Text tipText;
    [Tooltip("Текст подсказки")] public string tipString;

    [Space(10), Header("Постановка сцены"), Tooltip("Позиции, куда будут поставлены игроки")]
    public List<DialogueActorPointItem> actorsPoints = new List<DialogueActorPointItem>();
    [Tooltip("Основная камера")] public Transform sceneCamera;
    [Tooltip("Позиции для ракурса камеры")] public List<Transform> cameraPoints;
    [Tooltip("Источник звука для проигрывания реплик")] public AudioSource audioSource;
    [Tooltip("Участники диалога")] public List<DialogueController> actors;
    [Tooltip("Если реагируют объекты из сцены")] public List<DialogueEventReactor> reactors;

    [Tooltip("Кнопка пропуска реплики")] public KeyCode skipButton = KeyCode.R;
    [Space(20)]
    public bool noVoice;
    public bool once;
    public bool debug;
    public bool useAnimations;
    public bool teleportPlayersToPositions = true;

    [Space(20)]
    public bool clearUsingInReplics;

    public bool useNetwork = false; 

    public DialogueCharacter playerRole;
    public event System.Action<int> ToNodeWithEventId;
    public event System.Action StopDialogueEvent;
    public event System.Action ClearPlayersBuffer;

    private DialogueController activeDialogueController;
    private int currentIndex;
    private DialogueState dialogueStatus;
    private int answerNumber;
    private int skip;
    private Vector3 camPosBufer;
    private Quaternion camRotBufer;
    private DialogueNode node;

    void Start()
    {
        skip = 0;
        dialogueStatus = DialogueState.Disactive;
        subsPanel.SetActive(false);
        CheckVariants(false);
        HideMessage();
        audioSource.playOnAwake = false;
        audioSource.Stop();
        currentIndex = scene.firstNodeIndex;
        skipTip.SetActive(false);
    }

    void Update()
    {
        CheckReplic();
        if(Input.GetKeyDown(skipButton) && skip == 1)
        {
            skip = 2;
        }
    }

    #region Управление диалогом
    public void StartScene()
    {
        CloseTip();
        camPosBufer = sceneCamera.position;
        camRotBufer = sceneCamera.rotation;
        AddAnswerEvents();
        if (currentIndex >= 0)
        {
            teamDirector.OnChooseAnswer += CheckAnswer;
            teamDirector.inDialog = true;
            for (int i = 0; i < actors.Count; i++)
            {
                if(teleportPlayersToPositions)
                    actors[i].ToDialoguePosition(FindPointByRole(actors[i].dialogueCharacter));
                if (useAnimations)
                    actors[i].ToDialogueAnimation();
            }
            StartNode(currentIndex);
        }
    }
    public void StartNode(int nodeIndex)
    {
        skip = 1;
        currentIndex = nodeIndex; 
        node = scene.nodes[nodeIndex];
        CheckVariants(false);

        if(node.Type == NodeType.Link)
        {
            dialogueStatus = DialogueState.Disactive;
            currentIndex = node.NextNodeNumber;
            if (useNetwork)
            {
                ToNodeWithEventId?.Invoke(currentIndex);
            }
            else
            {
                StartNode(currentIndex);
            }
        }
        else if(node.Type == NodeType.RandomLink)
        {
            dialogueStatus = DialogueState.Disactive;
            currentIndex = node.GetNextLink();
            if (useNetwork)
            {
                ToNodeWithEventId?.Invoke(currentIndex);
            }
            else
            {
                StartNode(currentIndex);
            }
        }

        if (node.Type == NodeType.Replica || node.Type == NodeType.Choice)
        {
            sceneCamera.position = cameraPoints[node.CamPositionNumber].position;
            sceneCamera.rotation = cameraPoints[node.CamPositionNumber].rotation;
            sceneCamera.parent = cameraPoints[node.CamPositionNumber];

            if (node.Type == NodeType.Choice)
            {
                skip = 0;
                if(!useNetwork || playerRole.Equals(node.Character))
                {
                    dialogueStatus = DialogueState.WaitChoose;
                    for (int i = 0; i < node.AnswerChoice.Count; i++)
                    {
                        CheckVariant(answers[i], scene.nodes[node.AnswerChoice[i]]);
                    }
                }
                
            }
            else
            {
                if(noVoice)
                {
                    if(!node.finalNode && !(scene.nodes[node.NextNodeNumber].Type == NodeType.Choice))
                    {
                        skipTip.SetActive(true);
                        skip = 1;
                    }
                }
                else
                {
                    audioSource.clip = node.Clip;
                    audioSource.Play();
                    if (node.alreadyUsed)
                    {
                        skipTip.SetActive(true);
                        skip = 1;
                    }
                }

                dialogueStatus = DialogueState.TalkReplic;

                subsPanel.SetActive(true);
                subsText.color = node.Character.color;
                subsText.text = node.ReplicText;
                
                if(useAnimations)
                {
                    if (FindController(node.Character))
                    {
                        activeDialogueController.SetTalkType(node.AnimType);
                    }
                    else
                    {
                        Debug.LogError("Контроллер не найден");
                    }
                }

                if(!node.finalNode && scene.nodes[node.NextNodeNumber].Type == NodeType.Choice)
                {
                    StartNode(node.NextNodeNumber);
                }
            }
        }
        else if (node.Type == NodeType.Condition)
        {
            if (node.Parameter.GetType(node.ConditionNumber, out ParameterType type) && type == ParameterType.Bool)
            {
                if (node.Parameter.Check(node.ConditionNumber, node.CheckType, node.CheckBoolValue))
                {
                    currentIndex = node.PositiveNextNumber;
                    if (useNetwork)
                    {
                        ToNodeWithEventId?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
                else
                {
                    currentIndex = node.NegativeNextNumber;
                    if (useNetwork)
                    {
                        ToNodeWithEventId?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
            }
            else
            {
                if (node.Parameter.GetType(node.ConditionNumber, out type) && type == ParameterType.Int)
                {
                    if (node.Parameter.Check(node.ConditionNumber, node.CheckType, node.CheckIntValue))
                    {
                        if (useNetwork)
                        {
                            ToNodeWithEventId?.Invoke(node.PositiveNextNumber);
                        }
                        else
                        {
                            StartNode(node.PositiveNextNumber);
                        }
                    }
                    else
                    {
                        if (useNetwork)
                        {
                            ToNodeWithEventId?.Invoke(node.NegativeNextNumber);
                        }
                        else
                        {
                            StartNode(node.NegativeNextNumber);
                        }
                    }
                }
            }
        }
        else if (node.Type == NodeType.Event)
        {
            if(node.IsMessage)
            {
                messagePanel.SetActive(true);
                messageText.text = node.MessageText;
                Invoke("HideMessage", 4);
            }


            if (!node.ChangeCondition && !node.InSceneInvoke)
            {
                dialogueStatus = DialogueState.EventComplete;
            }
            else
            {
                if (node.ChangeCondition)
                {
                    if (node.Parameter.GetType(node.ConditionNumber, out ParameterType type) && type == ParameterType.Bool)
                    {
                        node.Parameter.SetBool(node.ConditionNumber, node.ChangeBoolValue);
                    }
                    else
                    {
                        if(node.Operation == OperationType.AddValue)
                        {
                            node.Parameter.SetInt(node.ConditionNumber, node.Parameter.GetInt(node.ConditionNumber) + node.ChangeIntValue);
                        }
                        else
                        {
                            node.Parameter.SetInt(node.ConditionNumber, node.ChangeIntValue);
                        }
                    }
                    if (!node.InSceneInvoke)
                    {
                        dialogueStatus = DialogueState.EventComplete;
                    }
                }
                if (node.InSceneInvoke)
                {
                    foreach (var item in node.ReactorsNumbers)
                    {
                        if (reactors[item] != null)
                        {
                            reactors[item].OnEvent();
                        }
                    }
                    dialogueStatus = DialogueState.WaitEvent;
                    sceneCamera.position = cameraPoints[node.EventCamPositionNumber].position;
                    sceneCamera.rotation = cameraPoints[node.EventCamPositionNumber].rotation;
                    sceneCamera.parent = cameraPoints[node.EventCamPositionNumber];
                    Invoke("StopEvent", node.EventTime);
                }
            }
        }
    }
    public void CloseSkipTip()
    {
        skipTip.SetActive(false);
        skip = 0;
    }
    private void CheckReplic()
    {
        switch (dialogueStatus)
        {
            case DialogueState.Disactive:
                break;
            case DialogueState.TalkReplic:
                if(noVoice)
                {
                    if(skip == 2)
                    {
                        if(useAnimations)
                            activeDialogueController.StopReplic();
                        CloseSkipTip();

                        if (node.finalNode)
                        {
                            skip = 0;
                            ExitScene();
                            //skip = 2;
                            //dialogueStatus = DialogueState.LastClick;
                        }
                        else
                        {
                            currentIndex = node.Type == NodeType.RandomLink ? node.GetNextLink() : node.NextNodeNumber;
                            if (useNetwork)
                            {
                                ToNodeWithEventId?.Invoke(currentIndex);
                            }
                            else
                            {
                                
                                StartNode(currentIndex);
                            }
                        }
                    }
                }
                else
                {
                    if (!audioSource.isPlaying || skip == 2)
                    {
                        audioSource.Stop();
                        activeDialogueController.StopReplic();
                        CloseSkipTip();
                        if (node.Type == NodeType.Replica)
                        {
                            node.alreadyUsed = true;
                        }

                        if (node.finalNode)
                        {
                            ExitScene();
                            dialogueStatus = DialogueState.Disactive;
                        }
                        else
                        {
                            currentIndex = node.NextNodeNumber;
                            if (useNetwork)
                            {
                                ToNodeWithEventId?.Invoke(currentIndex);
                            }
                            else
                            {
                                StartNode(currentIndex);
                            }
                        }
                    }
                }
                break;
            case DialogueState.ChooseComplete:
                currentIndex = scene.nodes[currentIndex].AnswerChoice[answerNumber];

                if(noVoice)
                {
                    currentIndex = scene.nodes[currentIndex].NextNodeNumber;
                }

                if (useNetwork)
                {
                    ToNodeWithEventId?.Invoke(currentIndex);
                }
                else
                {
                    StartNode(currentIndex);
                }
                break;
            case DialogueState.EventComplete:
                if(node.finalNode)
                {
                    ExitScene();
                    dialogueStatus = DialogueState.Disactive;
                }
                else
                {
                    currentIndex = node.Type == NodeType.RandomLink ? node.GetNextLink() : node.NextNodeNumber;
                    if (useNetwork)
                    {
                        ToNodeWithEventId?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
                break;
            case DialogueState.LastClick:
                if(skip == 2)
                {
                    skip = 0;
                    ExitScene();
                }
                break;
            default:
                break;
        }    
    }
    public void StopScene()
    {
        currentIndex = -1;
        sceneCamera.parent = null;
        sceneCamera.position = camPosBufer;
        sceneCamera.rotation = camRotBufer;
        subsPanel.SetActive(false);
        skipTip.SetActive(false);
        CheckVariants(false);
        if (useAnimations)
        {
            foreach (var item in actors)
            {
                item.ToDefault();
            }
        }
        teamDirector.inDialog = false;
        teamDirector.ReturnFromDialogue();
        teamDirector.OnChooseAnswer -= CheckAnswer;
        if(once)
        {
            ClearPlayersBuffer?.Invoke();
            GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject, 6);
        }
        RemoveAnswerEvents();
        CloseTip();
    }
    private void ExitScene()
    {
        if (useNetwork)
        {
            StopDialogueEvent?.Invoke();
        }
        else
        {
            StopScene();
        }
    }
    #endregion

    #region Вспомогательные
    /// <summary>
    /// Помечает все узлы как ещё не просмотренные. Теперь их нельзя пропускать.
    /// </summary>
    public void ClearUsings()
    {
        if(scene != null)
        {
            foreach (var item in scene.nodes)
            {
                item.alreadyUsed = false;
            }
            Debug.Log("Узлы очищены!");
        }
        Debug.LogWarning("Диалоговая сцена не указана!");
    }
    public void ShowTip()
    {
        tipPanel.SetActive(true);
        Debug.Log("1 "+ tipString);
        tipText.text = tipString;
        Debug.Log("2 "+ tipString);

    }
    public void CloseTip()
    {
        tipText.text = string.Empty;
        tipPanel.SetActive(false);
    }

    public int GetNodeIndexByAnswer(int choiseNodeIndex, int answerNumber)
    {
        return scene.nodes[choiseNodeIndex].AnswerChoice[answerNumber];
    }
    private Transform FindPointByRole(DialogueCharacter role)
    {
        foreach (var item in actorsPoints)
        {
            if (role == item.actorRole)
                return item.actorPoint;
        }
        Debug.LogError("Не удалось найти точки для " + role.characterName + " в диалоге " + gameObject.name);
        return null;
    }
    private bool FindController(DialogueCharacter character)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i].dialogueCharacter == character)
            {
                activeDialogueController = actors[i];
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Скрывает (false) или показывает (true) кнопки, используемые в вариантах ответов
    /// </summary>
    /// <param name="value"></param>
    private void CheckVariants(bool value)
    {
        foreach (var item in answers)
        {
            item.variantButton.SetActive(value);
        }
    }
    /// <summary>
    /// В соответствии с указанным вариантом ответа, представленным как DialogueNode, включает кнопку для выбора данного варианта ответа и устанавливает ей нужный текст
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="node"></param>
    private void CheckVariant(AnswerUI answer, DialogueNode node)
    {
        if (!node.ReplicText.Equals(string.Empty))
        {
            answer.variantButton.SetActive(true);
            answer.variantText.color = node.Character.color;
            answer.variantText.text = node.ReplicText;
        }
    }
    /// <summary>
    /// Срабатывает, когда игрок нажимает кнопку выбора варианта ответа. В параметр передаётся номер этой кнопки.
    /// </summary>
    /// <param name="number"></param>
    public void CheckAnswer(int number)
    {
        answerNumber = number;
        dialogueStatus = DialogueState.ChooseComplete;
    }
    private void HideMessage() => messagePanel.SetActive(false);
    private void StopEvent() => dialogueStatus = DialogueState.EventComplete;
    //Используется!!! в StartReplic() для события (Invke("StopEvent"))
    private void AddAnswerEvents()
    {
        foreach (var item in answers)
        {
            item.variantButton.GetComponent<AnswerButtonReactor>().TakeAnswerEvent += CheckAnswer;
        }
    }
    private void RemoveAnswerEvents()
    {
        foreach (var item in answers)
        {
            item.variantButton.GetComponent<AnswerButtonReactor>().TakeAnswerEvent -= CheckAnswer;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.green;
            foreach (var item in actorsPoints)
            {
                if(item.actorPoint != null)
                {
                    Gizmos.DrawCube(item.actorPoint.position, new Vector3(0.2f, 0.1f, 0.2f));
                }
                else
                {
                    Debug.LogError("Одна из заявленных точек персонажей не задана!");
                }
               
            }
            Gizmos.color = Color.cyan;
            foreach (var item in cameraPoints)
            {
                if(item != null)
                {
                    Gizmos.DrawLine(item.position, item.position + item.forward + item.right * 0.3f + item.up * 0.3f);
                    Gizmos.DrawLine(item.position, item.position + item.forward + item.right * 0.3f - item.up * 0.3f);
                    Gizmos.DrawLine(item.position, item.position + item.forward - item.right * 0.3f + item.up * 0.3f);
                    Gizmos.DrawLine(item.position, item.position + item.forward - item.right * 0.3f - item.up * 0.3f);
                }
                else
                {
                    Debug.LogError("Одна из заявленных точек камеры на задана!");
                }
            }
        }
        if(clearUsingInReplics)
        {
            ClearUsings();
            clearUsingInReplics = false;
        }
    }
}