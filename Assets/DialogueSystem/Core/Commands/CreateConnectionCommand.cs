public class CreateConnectionCommand : ICommand
{
    private Connection connectionBufer;
    private Connection oldConnectionForInPoint;
    private Connection oldConnectionForOutPoint;

    public CreateConnectionCommand(Connection connection, Connection oldInConnection, Connection oldOutConnection)
    {
        oldConnectionForInPoint = oldInConnection;
        oldConnectionForOutPoint = oldOutConnection;
        connectionBufer = connection;
    }

    public void Execute()
    {
        CommandManager.connections.Add(connectionBufer);
    }

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
