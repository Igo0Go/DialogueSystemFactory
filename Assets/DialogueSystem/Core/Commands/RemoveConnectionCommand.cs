public class RemoveConnectionCommand : ICommand
{
    private readonly Connection connectionBufer;

    public RemoveConnectionCommand(Connection connection)
    {
        connectionBufer = connection;
    }

    /// <summary>
    /// удаляет созданную ранее связь и возвращает старые, если они были
    /// </summary>
    public void Execute()
    {
        connectionBufer.OnClickRemoveConnection(connectionBufer);
    }

    /// <summary>
    /// восстанавливает удалённую связь
    /// </summary>
    public void Undo()
    {
        CommandManager.connections.Add(connectionBufer);
    }
}
