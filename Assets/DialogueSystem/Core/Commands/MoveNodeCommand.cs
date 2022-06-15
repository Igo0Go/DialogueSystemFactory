using UnityEngine;

public class MoveNodeCommand : ICommand
{
    private Vector3 offset;
    private DialogueNode dialogueNode;

    public MoveNodeCommand(Vector3 offset, DialogueNode dialogueNode)
    {
        this.dialogueNode = dialogueNode;
        this.offset = offset;
    }

    public void Execute()
    {
        dialogueNode?.Drag(offset);
    }

    public void Undo()
    {
        dialogueNode?.Drag(-offset);
    }
}