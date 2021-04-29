using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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
    public bool useVoice;
    public bool once;
    public bool debug;
    public bool useAnimations;
    public bool teleportPlayersToPositions = true;

    [Space(20)]
    public bool clearUsingInReplics;

    public bool useNetwork = false; 

    public DialogueCharacter playerRole;
    public event Action<int> ActivateNodeWithIDEvent;
    public event Action StopDialogueEvent;
    public event Action ClearPlayersBuffer;

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
        audioSource.playOnAwake = false;
        audioSource.Stop();
        currentIndex = scene.firstNodeIndex;
        skipTip.SetActive(false);

        CheckVariants(false);
        HideMessage();
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
            PreparePlayersToDialogue();
            StartNode(currentIndex);
        }
    }
    public void StartNode(int nodeIndex)
    {
        StopAllCoroutines();
        skip = 1;

        currentIndex = nodeIndex; 
        node = scene.nodes[nodeIndex];
        CheckVariants(false);

        if (node is LinkNode link)
        {
            dialogueStatus = DialogueState.Disactive;
            currentIndex = link.NextNodeNumber;
            if (useNetwork)
            {
                ActivateNodeWithIDEvent?.Invoke(currentIndex);
            }
            else
            {
                StartNode(currentIndex);
            }
        }
        else if (node is RandomizerNode randomizer)
        {
            dialogueStatus = DialogueState.Disactive;
            currentIndex = randomizer.GetNextLink();
            if (useNetwork)
            {
                ActivateNodeWithIDEvent?.Invoke(currentIndex);
            }
            else
            {
                StartNode(currentIndex);
            }
        }
        else if (node is ReplicaNode replica)
        {
            ReplicInfo info = replica.replicaInformation;
            PrepareReplica(info);
            StartCoroutine(ShowSkipTipAfterTimeCoroutine());
        }
        else if (node is ChoiceNode choice)
        {
            sceneCamera.position = cameraPoints[choice.defaultCameraPositionIndex].position;
            sceneCamera.rotation = cameraPoints[choice.defaultCameraPositionIndex].rotation;
            sceneCamera.parent = cameraPoints[choice.defaultCameraPositionIndex];

            skip = 0;
            if (!useNetwork || playerRole.Equals(choice.character))
            {
                dialogueStatus = DialogueState.WaitChoose;
                for (int i = 0; i < choice.answers.Count; i++)
                {
                    ShowVariants(answers[i], choice.answers[i]);
                }
            }
        }
        else if(node is ConditionNode condition)
        {
            if (condition.parameter.GetType(condition.conditionNumber, out ParameterType type) && type == ParameterType.Bool)
            {
                if (condition.parameter.Check(condition.conditionNumber, condition.checkType, condition.checkBoolValue))
                {
                    currentIndex = condition.PositiveNextNumber;
                    if (useNetwork)
                    {
                        ActivateNodeWithIDEvent?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
                else
                {
                    currentIndex = condition.NegativeNextNumber;
                    if (useNetwork)
                    {
                        ActivateNodeWithIDEvent?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
            }
            else
            {
                if (condition.parameter.GetType(condition.conditionNumber, out type) && type == ParameterType.Int)
                {
                    if (condition.parameter.Check(condition.conditionNumber, condition.checkType, condition.checkIntValue))
                    {
                        if (useNetwork)
                        {
                            ActivateNodeWithIDEvent?.Invoke(condition.PositiveNextNumber);
                        }
                        else
                        {
                            StartNode(condition.PositiveNextNumber);
                        }
                    }
                    else
                    {
                        if (useNetwork)
                        {
                            ActivateNodeWithIDEvent?.Invoke(condition.NegativeNextNumber);
                        }
                        else
                        {
                            StartNode(condition.NegativeNextNumber);
                        }
                    }
                }
            }
        }
        else if(node is EventNode eventNode)
        {
            if (eventNode.useMessage)
            {
                messagePanel.SetActive(true);
                messageText.text = eventNode.messageText;
                StartCoroutine(HideMessageCoroutine(4));
            }


            if (!eventNode.changeParameter && !eventNode.inSceneInvoke)
            {
                dialogueStatus = DialogueState.EventComplete;
            }
            else
            {
                if (eventNode.changeParameter)
                {
                    if (eventNode.parameter.GetType(eventNode.changeingParameterIndex, out ParameterType type) && type == ParameterType.Bool)
                    {
                        eventNode.parameter.SetBool(eventNode.changeingParameterIndex, eventNode.targetBoolValue);
                    }
                    else
                    {
                        if (eventNode.operation == OperationType.AddValue)
                        {
                            eventNode.parameter.SetInt(eventNode.changeingParameterIndex,
                                eventNode.parameter.GetInt(eventNode.changeingParameterIndex) + eventNode.changeIntValue);
                        }
                        else
                        {
                            eventNode.parameter.SetInt(eventNode.changeingParameterIndex, eventNode.changeIntValue);
                        }
                    }
                    if (!eventNode.inSceneInvoke)
                    {
                        dialogueStatus = DialogueState.EventComplete;
                    }
                }
                if (eventNode.inSceneInvoke)
                {
                    foreach (var item in eventNode.reactorsNumbers)
                    {
                        if (reactors[item] != null)
                        {
                            reactors[item].OnEvent();
                        }
                    }
                    dialogueStatus = DialogueState.WaitEvent;
                    sceneCamera.position = cameraPoints[eventNode.eventCamPositionNumber].position;
                    sceneCamera.rotation = cameraPoints[eventNode.eventCamPositionNumber].rotation;
                    sceneCamera.parent = cameraPoints[eventNode.eventCamPositionNumber];
                    StartCoroutine(StopInSceneEventCoroutine(eventNode.eventTime));
                }
            }
        }
    }
    /// <summary>
    /// Срабатывает, когда игрок нажимает кнопку выбора варианта ответа. В параметр передаётся номер этой кнопки.
    /// </summary>
    /// <param name="number">номер нажатой кнопки</param>
    public void UseAnswer(int number)
    {
        answerNumber = number;
        StartNode(node.nextNodesNumbers[answerNumber]);
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
            case DialogueState.TalkReplic:
                CheckTalkReplicState();
                break;
            case DialogueState.LastClick:
                if (skip == 2)
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
        teamDirector.OnChooseAnswer -= UseAnswer;
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
    /// Показать подсказку - зона диалога
    /// </summary>
    public void ShowTip()
    {
        tipPanel.SetActive(true);
        Debug.Log("1 "+ tipString);
        tipText.text = tipString;
        Debug.Log("2 "+ tipString);

    }
    /// <summary>
    /// Скрыть подсказку - зона диалога
    /// </summary>
    public void CloseTip()
    {
        tipText.text = string.Empty;
        tipPanel.SetActive(false);
    }

    //public int GetNodeIndexByAnswer(int choiseNodeIndex, int answerNumber)
    //{
    //    //return scene.nodes[choiseNodeIndex].AnswerChoice[answerNumber];
    //}
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

    private void CheckVariants(bool value)
    {
        foreach (var item in answers)
        {
            item.variantButton.SetActive(value);
        }
    }
    private void HideMessage()
    {
        messagePanel.SetActive(false);
    }
    private void AddAnswerEvents()
    {
        foreach (var item in answers)
        {
            item.variantButton.GetComponent<AnswerButtonReactor>().TakeAnswerEvent += UseAnswer;
        }
    }
    private void RemoveAnswerEvents()
    {
        foreach (var item in answers)
        {
            item.variantButton.GetComponent<AnswerButtonReactor>().TakeAnswerEvent -= UseAnswer;
        }
    }
    private void PreparePlayersToDialogue()
    {
        teamDirector.OnChooseAnswer += UseAnswer;
        teamDirector.inDialog = true;
        for (int i = 0; i < actors.Count; i++)
        {
            if (teleportPlayersToPositions)
                actors[i].ToDialoguePosition(FindPointByRole(actors[i].dialogueCharacter));
            if (useAnimations)
                actors[i].ToDialogueAnimation();
        }
    }
    private void PrepareReplica(ReplicInfo info)
    {
        sceneCamera.position = cameraPoints[info.camPositionNumber].position;
        sceneCamera.rotation = cameraPoints[info.camPositionNumber].rotation;
        sceneCamera.parent = cameraPoints[info.camPositionNumber];

        if (useVoice)
        {
            audioSource.clip = info.clip;
            audioSource.Play();
        }

        dialogueStatus = DialogueState.TalkReplic;

        subsPanel.SetActive(true);
        subsText.color = info.character.color;
        subsText.text = info.replicaText;

        if (useAnimations)
        {
            if (FindController(info.character))
            {
                activeDialogueController.SetTalkType(info.animType);
            }
            else
            {
                Debug.LogError("Контроллер не найден");
            }
        }
    }
    private void ShowVariants(AnswerUI answer, AnswerItem item)
    {
        ReplicInfo inf = item.answerReplica;
        if (!inf.replicaText.Equals(string.Empty))
        {
            answer.variantButton.SetActive(true);
            answer.variantText.color = inf.character.color;
            answer.variantText.text = inf.replicaText;
        }
    }

    private void CheckTalkReplicState()
    {
        if (useVoice)
        {
            if (!audioSource.isPlaying || skip == 2)
            {
                audioSource.Stop();
                activeDialogueController.StopReplic();
                CloseSkipTip();
                if (node.finalNode)
                {
                    ExitScene();
                    dialogueStatus = DialogueState.Disactive;
                }
                else
                {
                    currentIndex = node.nextNodesNumbers[0];
                    if (useNetwork)
                    {
                        ActivateNodeWithIDEvent?.Invoke(currentIndex);
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
            if (skip == 2)
            {
                if (useAnimations)
                    activeDialogueController.StopReplic();
                CloseSkipTip();

                if (node.finalNode)
                {
                    skip = 0;
                    ExitScene();
                }
                else
                {
                    currentIndex = node.nextNodesNumbers[0];
                    if (useNetwork)
                    {
                        ActivateNodeWithIDEvent?.Invoke(currentIndex);
                    }
                    else
                    {
                        StartNode(currentIndex);
                    }
                }
            }
        }
    }

    private IEnumerator HideMessageCoroutine(float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        HideMessage();
    }
    private IEnumerator ShowSkipTipAfterTimeCoroutine()
    {
        yield return new WaitForSeconds(2);
        skipTip.SetActive(true);
        skip = 1;
    }
    private IEnumerator StopInSceneEventCoroutine(float time)
    {
        float t = 0;
        while(t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (node.finalNode)
        {
            ExitScene();
            dialogueStatus = DialogueState.Disactive;
        }
        else
        {
            currentIndex = node.nextNodesNumbers[0];
            if (useNetwork)
            {
                ActivateNodeWithIDEvent?.Invoke(currentIndex);
            }
            else
            {
                StartNode(currentIndex);
            }
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
    }
}