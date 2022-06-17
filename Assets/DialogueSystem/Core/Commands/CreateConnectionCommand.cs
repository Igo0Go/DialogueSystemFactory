/// <summary>
/// ������� �������� �����
/// </summary>
public class CreateConnectionCommand : ICommand
{
    private readonly Connection connectionBufer;
    private readonly Connection oldConnectionForInPoint;
    private readonly Connection oldConnectionForOutPoint;

    public CreateConnectionCommand(Connection connection, Connection oldInConnection, Connection oldOutConnection)
    {
        oldConnectionForInPoint = oldInConnection;
        oldConnectionForOutPoint = oldOutConnection;
        connectionBufer = connection;
    }

    /// <summary>
    /// ������ ��������� �����. ���� ���� ������ �����, ����� �������� ��
    /// </summary>
    public void Execute()
    {
        CommandManager.connections.Add(connectionBufer);
    }

    /// <summary>
    /// ������� ��������� ����� ����� � ���������� ������, ���� ��� ����
    /// </summary>
    public void Undo()
    {
        connectionBufer.OnClickRemoveConnection(connectionBufer);
        if (oldConnectionForInPoint != null)
        {
            oldConnectionForInPoint.inPoint.SaveReferenceToNode(oldConnectionForInPoint.outPoint.NodeIndex);
            oldConnectionForInPoint.outPoint.SaveReferenceToNode(oldConnectionForInPoint.inPoint.NodeIndex);
            CommandManager.connections.Add(oldConnectionForInPoint);
        }
        if (oldConnectionForOutPoint != null)
        {
            oldConnectionForOutPoint.inPoint.SaveReferenceToNode(oldConnectionForOutPoint.outPoint.NodeIndex);
            oldConnectionForOutPoint.outPoint.SaveReferenceToNode(oldConnectionForOutPoint.inPoint.NodeIndex);
            CommandManager.connections.Add(oldConnectionForOutPoint);
        }
    }
}
