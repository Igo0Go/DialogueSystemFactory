using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    public List<DialogueNode> nodes;
    public int startNodeIndex = -1;

    public GUISkin replicaSkin;

    public void AddNewNode(DialogueNode node)
    {
        if (nodes == null)
        {
            nodes = new List<DialogueNode>();
        }
        nodes.Add(node);
    }
    public void InsertNodeAsId(DialogueNode node, int id)
    {
        if (nodes == null)
        {
            nodes = new List<DialogueNode>();
        }
    }
    public void RemoveNode(DialogueNode node)
    {
        RemoveNodeWithId(node.index);
    }
    public void RemoveNodeWithId(int index)
    {
        nodes.RemoveAt(index);
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].CheckIndexesAfterRemovingNodeWithIndex(index);
        }
    }
}