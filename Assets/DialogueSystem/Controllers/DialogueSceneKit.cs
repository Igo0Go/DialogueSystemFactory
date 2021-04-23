using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    public int firstNodeIndex = 0;
    public string sceneName;
    public List<DialogueNode> nodes = new List<DialogueNode>();
    public List<Color> dialogueCharactersColors = new List<Color>();
    public List<string> camerasPositions = new List<string>();
    public List<string> inSceneInvokeObjects = new List<string>();

    public void Clear() => nodes.Clear();

    /// <summary>
    /// удаление узла с обрезанием связей
    /// </summary>
    /// <param name="node">удаляемый узел</param>
    public void Remove(DialogueNode node)
    {
        if (nodes.Contains(node))
        {
            ClearNextRelations(node);
            ClearPreviousRelations(node);
            int indexBufer;
            indexBufer = node.index;
            nodes.Remove(node);
            CheckIndexForAll(indexBufer);
        }
    }
    /// <summary>
    /// корректировка номеров узлов после удаления
    /// </summary>
    /// <param name="removedIndex"></param>
    public void CheckIndexForAll(int removedIndex)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].CheckIndexes(removedIndex);
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
            nodes[from.previousNodesNumbers[i]].RemoveThisNodeFromNext(from);
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
            nodes[from.nextNodesNumbers[i]].RemoveThisNodeFromPrevious(from);
        }
    }
    /// <summary>
    /// добавление узла в предыдущие к другому узлу
    /// </summary>
    /// <param name="from"></param>
    /// <param name="node"></param>
    public void AddInPreviousRelations(DialogueNode from, DialogueNode node)
    {
        if (!from.previousNodesNumbers.Contains(node.index))
        {
            from.previousNodesNumbers.Add(node.index);
        }
    }
    /// <summary>
    /// Назначить узел стартовым
    /// </summary>
    /// <param name="node"></param>
    public void SetAsFirst(DialogueNode node)
    {
        firstNodeIndex = node.index;
    }
}

