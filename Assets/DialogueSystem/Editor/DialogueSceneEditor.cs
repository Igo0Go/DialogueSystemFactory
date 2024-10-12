using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DialogueSceneEditor : EditorWindow
{
    #region œŒÀﬂ

    private DialogueSceneKit scene;

    private Vector2 drag;
    private Vector2 offset;

    private List<Connection> connections;

    private IConnectionPoint selectedInPoint;
    private IConnectionPoint selectedOutPoint;
    private DialogueNode selectedNode;
    private StartNode startNode;

    #endregion

    #region Œ—ÕŒ¬Õ€≈ Ã≈“Œƒ€
    public static DialogueSceneEditor GetEditor(DialogueSceneKit sceneKit)
    {
        DialogueSceneEditor window = GetWindow<DialogueSceneEditor>();
        window.scene = sceneKit;
        window.UpdateSceneData();
        return window;
    }

    private void OnEnable()
    {
        selectedNode = null;
        ClearConnectionSelection();
    }

    private void OnGUI()
    {
        if(scene== null)
        {
            return;
        }
        else if (startNode == null)
        {
            UpdateSceneData();
        }

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);



        DrawConnections();
        startNode.Draw();
        DrawNodes();

        DrawConnectionLine(Event.current);

        DrawInspector();

        ProcessEvents(Event.current);
        ProcessNodeEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    #endregion

    #region Œ“–»—Œ¬ ¿

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (scene.nodes != null)
        {
            for (int i = 0; i < scene.nodes.Count; i++)
            {
                scene.nodes[i].Draw();
                if (scene.nodes[i].isSelected)
                {
                    selectedNode = scene.nodes[i];
                }
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.Rect.center,
                e.mousePosition,
                selectedInPoint.Rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.Rect.center,
                e.mousePosition,
                selectedOutPoint.Rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawInspector()
    {
        if(selectedNode != null)
        {
            Rect inspectorRect = new Rect(position.width - 200, 0, 200, position.height);
            GUI.Box(inspectorRect, string.Empty, StylePack.nodeStyleReplica_default);
            Rect bufer = new Rect(position.width - 200, 0, 200, 20);
            GUI.Label(bufer, selectedNode.index.ToString());
        }
    }
    #endregion

    #region Œ¡–¿¡Œ“ ¿ —Œ¡€“»…

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if(selectedNode != null)
                    {
                        Rect inspectorRect = new Rect(position.width - 200, 0, 200, position.height);
                        Vector2 mouse = Event.current.mousePosition;
                        if (mouse.x > inspectorRect.x && mouse.x < inspectorRect.x + inspectorRect.width &&
                            mouse.y > 0 && mouse.y < inspectorRect.height)
                        {
                            e.Use();
                            return;
                        }
                    }
                    ClearConnectionSelection();
                }
                else if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;
            case EventType.KeyDown:
                if (e.control && e.keyCode == KeyCode.Z)
                {
                    CommandManager.Undo();
                    GUI.changed = true;
                }
                else if(e.control && e.keyCode == KeyCode.C)
                {
                    //copy
                }
                else if (e.control && e.keyCode == KeyCode.V)
                {
                    //paste
                }
                break;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("  ÚÓ˜ÍÂ ‚ıÓ‰‡"), false, () => OnToStartClick());
        genericMenu.AddItem(new GUIContent("—ÓÁ‰‡Ú¸ ÂÔÎËÍÛ"), false, () => CreateNewNode(mousePosition));
        genericMenu.AddItem(new GUIContent("—ÚÂÂÚ¸ ËÒÚÓË˛"), false, () => CommandManager.commandHistory = new Stack<ICommand>());

        genericMenu.ShowAsContext();
    }

    private void ProcessNodeEvents(Event e)
    {
        if (scene.nodes != null)
        {
            for (int i = scene.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = scene.nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        startNode.Drag(delta);
        if (scene.nodes != null)
        {
            for (int i = 0; i < scene.nodes.Count; i++)
            {
                scene.nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnToStartClick()
    {
        Vector2 delta = - (startNode.Rect.position - new Vector2(position.size.x/2, + position.size.y/2));
        OnDrag(delta);
    }

    private void OnClickRemoveNode(DialogueNode node)
    {
        scene.RemoveNode(node);
    }

    private void OnClickConnectionPoint(IConnectionPoint point)
    {
        if(point.PointType == ConnectionPointType.In)
        {
            selectedInPoint = point;

            if (selectedOutPoint != null)
            {
                if (selectedOutPoint != selectedInPoint)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }
        else
        {
            selectedOutPoint = point;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint != selectedInPoint)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connection.inPoint.CurrentConnection = connection.outPoint.CurrentConnection = null;
        connection.inPoint.ClearReferenceToNodeByValue(connection.outPoint.NodeIndex);
        connection.outPoint.ClearReferenceToNodeByValue(connection.inPoint.NodeIndex);
        connections.Remove(connection);
    }

    private void OnChangeFirstNode(int newFirstNode)
    {
        scene.startNodeIndex = newFirstNode;
    }

    private void OnNodeSelected(DialogueNode node)
    {
        selectedNode = node;
    }

    private void OnNodeDeselected(DialogueNode node)
    {
        if(selectedNode == node)
        {
            selectedNode = null;
        }
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    #endregion

    #region ÀŒ√» ¿

    private void CreateConnection()
    {
        var oldInConnection = selectedInPoint.CurrentConnection;
        var oldOutConnection = selectedOutPoint.CurrentConnection;

        ICommand command = new CreateConnectionCommand(new Connection(selectedInPoint, selectedOutPoint, 
            OnClickRemoveConnection), oldInConnection, oldOutConnection);

        CommandManager.AddCommandAndExecute(command);
    }

    private void CreateConnection(IConnectionPoint outPoint, IConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;
        selectedOutPoint = outPoint;
        CreateConnection();
    }

    public void CreateNewNode(Vector2 position)
    {
        ICommand addCommand = new AddNodeCommand(
            new DialogueNodeReplica(position, new Vector2(200, 80), scene.nodes.Count, scene.replicaSkin,
            OnClickRemoveNode, OnClickConnectionPoint, OnNodeSelected, OnNodeDeselected));
        CommandManager.AddCommandAndExecute(addCommand);
    }

    private void UpdateSceneData()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
            CommandManager.connections = connections;
        }

        CommandManager.commandHistory = new Stack<ICommand>();

        if (scene != null)
        {
            CommandManager.sceneKit = scene;

            startNode = new StartNode(StylePack.startNodeStyle, OnClickConnectionPoint, scene.startNodeIndex, OnChangeFirstNode);

            if (scene.nodes == null)
            {
                scene.nodes = new List<DialogueNode>();
            }

            foreach (var item in scene.nodes)
            {
                item.UpdateNodeData(OnClickRemoveNode, OnClickConnectionPoint, StylePack.inPointStyle, StylePack.outPointStyle);
            }
        }

        if(startNode.NextNodeNumber >= 0)
        {
            CreateConnection(startNode, scene.nodes[scene.startNodeIndex].InPoint);
        }
        if(scene.nodes != null)
        {
            for (int i = 0; i < scene.nodes.Count; i++)
            {
                DialogueNode node = scene.nodes[i];
                for (int j = 0; j < node.OutPoints.Count; j++)
                {
                    if (node.NextNodesNumbers[j] >= 0)
                    {
                        CreateConnection(node.OutPoints[j], scene.nodes[node.NextNodesNumbers[j]].InPoint);
                    }
                }
            }
        }
    }
    #endregion
}
