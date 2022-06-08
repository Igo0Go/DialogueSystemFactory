using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    public List<DialogueNode> nodes;
    public List<Connection> connections;
    public StartNode startNode;
}
