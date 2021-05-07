using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    public int firstNodeIndex = 0;
    public string sceneName;
    public List<DialogueNode> Nodes { get; set; } = new List<DialogueNode>();
    public List<Color> dialogueCharactersColors = new List<Color>();
    public List<string> camerasPositions = new List<string>();
    public List<string> inSceneInvokeObjects = new List<string>();

    public List<DialogueNodePack> savedNodes;

    private List<DialogueNodePack> editorCopy;

    /// <summary>
    /// Создать временную копию хранилища
    /// </summary>
    public void CreateEditorCopy()
    {
        if(savedNodes != null)
            editorCopy = new List<DialogueNodePack>(savedNodes);
    }
    /// <summary>
    /// Восстановить хранилище из временной копии
    /// </summary>
    public void RepairSavedNodes()
    {
        savedNodes = new List<DialogueNodePack>(editorCopy);
    }
    /// <summary>
    /// Удалить все текущие узлы из списка быстрого доступа
    /// </summary>
    public void Clear() => Nodes.Clear();
    /// <summary>
    /// Сохранить рабочую копию узлов в постоянное хранилище
    /// </summary>
    public void SaveAllNodes()
    {
        savedNodes = new List<DialogueNodePack>();
        foreach (var item in Nodes)
        {
            if(item is ReplicaNode replica)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Replica, replicaNode = replica });
            }
            else if (item is ChoiceNode choice)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Choice, choiceNode = choice });
            }
            else if (item is EventNode eventNode)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Event, eventNode = eventNode });
            }
            else if (item is ConditionNode condition)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Condition, conditionNode = condition });
            }
            else if (item is LinkNode link)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Link, linkNode = link });
            }
            else if (item is RandomizerNode randomizer)
            {
                savedNodes.Add(new DialogueNodePack() { nodeType = DialogueNodeType.Randomizer, randomizerNode = randomizer });
            }
        }
        Nodes.Clear();
    }
    /// <summary>
    /// Привисти рабочую копию в соответствие с постоянным хранилищем
    /// </summary>
    public void LoadAllNodes()
    {
        if(savedNodes != null)
        {
            Nodes.Clear();
            foreach (var item in savedNodes)
            {
                switch (item.nodeType)
                {
                    case DialogueNodeType.Replica:
                        Nodes.Add(item.replicaNode);
                        break;
                    case DialogueNodeType.Choice:
                        Nodes.Add(item.choiceNode);
                        break;
                    case DialogueNodeType.Condition:
                        Nodes.Add(item.conditionNode);
                        break;
                    case DialogueNodeType.Event:
                        Nodes.Add(item.eventNode);
                        break;
                    case DialogueNodeType.Link:
                        Nodes.Add(item.linkNode);
                        break;
                    case DialogueNodeType.Randomizer:
                        Nodes.Add(item.randomizerNode);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    /// <summary>
    /// удаление узла с обрезанием связей
    /// </summary>
    /// <param name="node">удаляемый узел</param>
    public void Remove(DialogueNode node)
    {
        if (Nodes.Contains(node))
        {
            ClearNextRelations(node);
            ClearPreviousRelations(node);
            int indexBufer;
            indexBufer = node.index;
            Nodes.Remove(node);
            CheckIndexForAll(indexBufer);
        }
    }
    /// <summary>
    /// корректировка номеров узлов после удаления
    /// </summary>
    /// <param name="removedIndex"></param>
    public void CheckIndexForAll(int removedIndex)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].CheckIndexes(removedIndex);
        }
    }
    /// <summary>
    /// зачистить все предыдущие связи
    /// </summary>
    /// <param name="from"></param>
    public void ClearPreviousRelations(DialogueNode from)
    {
        for (int i = 0; i < from.previousNodesNumbers.Count; i++)
        {
            if (from.previousNodesNumbers[i] > -1)
                Nodes[from.previousNodesNumbers[i]].RemoveThisNodeFromNext(from);
        }
    }
    /// <summary>
    /// зачистить следующие связи
    /// </summary>
    /// <param name="from"></param>
    public void ClearNextRelations(DialogueNode from)
    {
        for (int i = 0; i < from.nextNodesNumbers.Count; i++)
        {
            if(from.nextNodesNumbers[i] > -1)
            {
                Nodes[from.nextNodesNumbers[i]].RemoveThisNodeFromPrevious(from);
                from.nextNodesNumbers[i] = -1;
            }
        }
    }
    /// <summary>
    /// добавление узла в предыдущие к другому узлу
    /// </summary>
    /// <param name="thisNode">Какой узел добавить</param>
    /// <param name="nextNode">Какому узлу добавить</param>
    public void AddInPreviousRelations(DialogueNode thisNode, DialogueNode nextNode)
    {
        if (!nextNode.previousNodesNumbers.Contains(thisNode.index))
        {
            nextNode.previousNodesNumbers.Add(thisNode.index);
        }
    }
    public void ClearOneNextNumber(DialogueNode node, int answerNumber)
    {
        Nodes[node.nextNodesNumbers[answerNumber]].RemoveThisNodeFromPrevious(node);
    }
    /// <summary>
    /// Назначить узел стартовым
    /// </summary>
    /// <param name="node"></param>
    public void SetAsFirst(DialogueNode node)
    {
        firstNodeIndex = node.index;
    }
    /// <summary>
    /// Добавить новый узел указанного типа с указанными координатами
    /// </summary>
    /// <param name="type">Тип создаваемого узла</param>
    /// <param name="pos">Координаты создаваемого узла</param>
    public void CreateNode(DialogueNodeType type , Vector2 pos)
    {
        int index = Nodes.Count;
        switch (type)
        {
            case DialogueNodeType.Replica:
                Nodes.Add(new ReplicaNode(pos, index));
                break;
            case DialogueNodeType.Choice:
                Nodes.Add(new ChoiceNode(pos, index));
                break;
            case DialogueNodeType.Condition:
                Nodes.Add(new ConditionNode(pos, index));
                break;
            case DialogueNodeType.Event:
                Nodes.Add(new EventNode(pos, index));
                break;
            case DialogueNodeType.Link:
                Nodes.Add(new LinkNode(pos, index));
                break;
            case DialogueNodeType.Randomizer:
                Nodes.Add(new RandomizerNode(pos, index));
                break;
            default:
                break;
        }

        if (index == 0)
        {
            SetAsFirst(Nodes[0]);
        }
    }
    /// <summary>
    /// Создать новую точку камеры
    /// </summary>
    public void CreateCameraPoint()
    {
        camerasPositions.Add("Новый ракурс " + (camerasPositions.Count + 1));
    }
}

[System.Serializable]
public class DialogueNodePack
{
    public DialogueNodeType nodeType;

    public ReplicaNode replicaNode;

    public ChoiceNode choiceNode;

    public EventNode eventNode;

    public ConditionNode conditionNode;

    public LinkNode linkNode;

    public RandomizerNode randomizerNode;
}
