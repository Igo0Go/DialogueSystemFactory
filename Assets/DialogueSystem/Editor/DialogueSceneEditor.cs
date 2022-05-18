using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DialogueSceneEditor : EditorWindow
{
    #region Поля

    private DialogueSceneKit scene;
    private List<Connection> connections;

    private Vector2 drag;
    private Vector2 offset;

    private StartNode startNode;
    private IConnectionPoint selectedInPoint;
    private IConnectionPoint selectedOutPoint;

    #region Стили
    private GUIStyle startNodeStyle;
    private GUIStyle nodeStyleReplica_default;
    private GUIStyle nodeStyleReplica_selected;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    #endregion

    #endregion

    #region Основные методы
    public static DialogueSceneEditor GetEditor(DialogueSceneKit sceneKit)
    {
        DialogueSceneEditor window = GetWindow<DialogueSceneEditor>();
        window.scene = sceneKit;
        return window;
    }

    private void OnEnable()
    {
        startNodeStyle = new GUIStyle();
        startNodeStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
        startNodeStyle.border = new RectOffset(0, 0, 0, 0);
        startNodeStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node5 on.png") as Texture2D;
        startNodeStyle.border = new RectOffset(0, 0, 0, 0);

        nodeStyleReplica_default = new GUIStyle();
        nodeStyleReplica_default.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyleReplica_default.border = new RectOffset(12, 12, 12, 12);

        nodeStyleReplica_selected = new GUIStyle();
        nodeStyleReplica_selected.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D;
        nodeStyleReplica_selected.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 4, 4);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        startNode = new StartNode(startNodeStyle, OnClickConnectionPoint);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawConnections();
        startNode.Draw();
        DrawNodes();

        DrawConnectionLine(Event.current);

        ProcessEvents(Event.current);
        ProcessNodeEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    #endregion

    #region События

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
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
        }
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

    #endregion

    #region ОТРИСОВКА

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
    #endregion

    #region ОБРАБОТКА СОБЫТИЙ

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("К точке входа"), false, () => OnToStartClick());
        genericMenu.AddItem(new GUIContent("Создать узел"), false, () => CreateNewNode(mousePosition));

        genericMenu.ShowAsContext();
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
        Vector2 delta = -startNode.Rect.position;
        OnDrag(delta);
    }

    private void OnClickRemoveNode(DialogueNode node)
    {
        scene.nodes.Remove(node);
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

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    #endregion

    #region Логика

    public void CreateNewNode(Vector2 position)
    {
        if(scene.nodes == null)
        {
            scene.nodes = new List<DialogueNode>();
        }

        scene.nodes.Add(new DialogueNode(position, scene.nodes.Count,
            nodeStyleReplica_default, nodeStyleReplica_selected, OnClickRemoveNode, inPointStyle, outPointStyle,
            OnClickConnectionPoint));
    }

    #endregion
}
