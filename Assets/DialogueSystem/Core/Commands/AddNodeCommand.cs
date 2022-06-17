using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNodeCommand : ICommand
{
    DialogueNode node;

    public AddNodeCommand(DialogueNode node)
    {
        this.node = node;
    }

    public void Execute()
    {
        CommandManager.sceneKit.AddNewNode(node);
    }

    public void Undo()
    {
        CommandManager.sceneKit.RemoveNode(node);
    }
}
