using UnityEngine;

/// <summary>
/// Команда передвижения узла
/// </summary>
public class MoveNodeCommand : ICommand
{
    private readonly Vector3 offset;
    private readonly DialogueNode dialogueNode;

    public MoveNodeCommand(Vector3 offset, DialogueNode dialogueNode)
    {
        this.dialogueNode = dialogueNode;
        this.offset = offset;
    }

    /// <summary>
    /// Передвигает узел на указанную позицию
    /// </summary>
    public void Execute()
    {
        dialogueNode?.Drag(offset);
    }

    /// <summary>
    /// Передвигает узел обратно
    /// </summary>
    public void Undo()
    {
        dialogueNode?.Drag(-offset);
    }
}