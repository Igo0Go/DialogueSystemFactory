using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DialogueSceneEditor : EditorWindow
{
    #region Поля

    private DialogueSceneKit scene;
    private StartNode startNode;

    private Vector2 drag;
    private Vector2 offset;

    #region Стили
    private GUIStyle startNodeStyle;
    private GUIStyle nodeStyleReplica_default;
    private GUIStyle nodeStyleReplica_selected;
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

        startNode = new StartNode(startNodeStyle);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        startNode.Draw();
        DrawNodes();

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

        GUI.changed = true;
    }

    private void OnToStartClick()
    {
        Vector2 delta = -startNode.rect.position;
        OnDrag(delta);
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
            nodeStyleReplica_default, nodeStyleReplica_selected));
    }

    #endregion
}
