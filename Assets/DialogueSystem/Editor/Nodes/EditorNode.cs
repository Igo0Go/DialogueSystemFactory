using System;
using UnityEditor;
using UnityEngine;

public class EditorNode
{
    public Rect rect;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public bool isDragged;
    public bool isSelected;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<EditorNode> OnRemoveNode;

    public EditorNode(Vector2 position, float width, float height, 
        GUIStyle defaultStyle, GUIStyle selectedStyle,
        GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
        Action<EditorNode> OnClickRemoveNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = defaultNodeStyle = defaultStyle;
        selectedNodeStyle = selectedStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        OnRemoveNode = OnClickRemoveNode;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, "", style);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}
