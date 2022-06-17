/// <summary>
/// ������� �������� ����
/// </summary>
public class AddNodeCommand : ICommand
{
    private readonly DialogueNode node;

    public AddNodeCommand(DialogueNode node)
    {
        this.node = node;
    }

    /// <summary>
    /// �������� ��������� ����
    /// </summary>
    public void Execute()
    {
        CommandManager.sceneKit.AddNewNode(node);
    }

    /// <summary>
    /// ������� ��������� ����� ����
    /// </summary>
    public void Undo()
    {
        CommandManager.sceneKit.RemoveNode(node);
    }
}
