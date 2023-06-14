public class RemoveConnectionCommand : ICommand
{
    private readonly Connection connectionBufer;

    public RemoveConnectionCommand(Connection connection)
    {
        connectionBufer = connection;
    }

    /// <summary>
    /// ������� ��������� ����� ����� � ���������� ������, ���� ��� ����
    /// </summary>
    public void Execute()
    {
        connectionBufer.OnClickRemoveConnection(connectionBufer);
    }

    /// <summary>
    /// ��������������� �������� �����
    /// </summary>
    public void Undo()
    {
        CommandManager.connections.Add(connectionBufer);
    }
}
