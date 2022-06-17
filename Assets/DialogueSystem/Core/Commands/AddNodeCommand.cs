/// <summary>
/// Команда создания узла
/// </summary>
public class AddNodeCommand : ICommand
{
    private readonly DialogueNode node;

    public AddNodeCommand(DialogueNode node)
    {
        this.node = node;
    }

    /// <summary>
    /// Добавить указанный узел
    /// </summary>
    public void Execute()
    {
        CommandManager.sceneKit.AddNewNode(node);
    }

    /// <summary>
    /// Удалить созданный ранее узел
    /// </summary>
    public void Undo()
    {
        CommandManager.sceneKit.RemoveNode(node);
    }
}
