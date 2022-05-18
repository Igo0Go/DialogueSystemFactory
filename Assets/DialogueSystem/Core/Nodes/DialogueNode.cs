using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class DialogueNode : IDrawableElement, IDragableElement
{
    #region ���� � ��������

    #region ������

    /// <summary>
    /// ����� � ����� �������
    /// </summary>
    public int index;

    /// <summary>
    /// ������� ���������� �����
    /// </summary>
    public List<int> previousNodesNumbers;

    /// <summary>
    /// ������� ��������� �����
    /// </summary>
    public List<int> nextNodesNumbers;

    /// <summary>
    /// ���� ���� ����������� ������/������
    /// </summary>
    public bool finalNode;

    #endregion

    #region ���������
    /// <summary>
    /// �������������, � ������� �������������� ����
    /// </summary>
    public Rect Rect
    {
        get
        {
            return _rect;
        }
        set
        {
            _rect = value;
        }
    }
    private Rect _rect;

    private bool isSelected;
    private bool isDragged;

    public Action<DialogueNode> OnRemoveNode;

    private ConnectionPoint inPoint;
    private ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    #endregion

    #endregion

    #region ������������
    /// <summary>
    /// ������� ��������� ���� � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public DialogueNode(Vector2 position, int index, GUIStyle defaultStyle, GUIStyle selectedStyle,
        Action<DialogueNode> onRemove,
        GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<IConnectionPoint> OnClickInPoint)
    {
        this.index = index;
        previousNodesNumbers = new List<int>();
        nextNodesNumbers = new List<int>();

        Rect = new Rect(position.x, position.y, 110, 40);
        defaultNodeStyle = style = defaultStyle;
        selectedNodeStyle = selectedStyle;

        inPoint = new ConnectionPoint(new Vector2(-55, -10), this, ConnectionPointType.In, inPointStyle,
            OnClickInPoint);
        outPoint = new ConnectionPoint(new Vector2(45, -10), this, ConnectionPointType.Out, outPointStyle,
            OnClickInPoint);

        OnRemoveNode = onRemove;
    }
    #endregion

    #region ������

    #region ������

    /// <summary>
    /// ������� ��������� ���� �� ������ ���������
    /// </summary>
    public void RemoveThisNodeFromNext(DialogueNode nodeForRemoving)
    {
        nextNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// ������� ��������� ���� �� ������ ����������
    /// </summary>
    /// <param name="nodeForRemoving">��������� ����</param>
    public void RemoveThisNodeFromPrevious(DialogueNode nodeForRemoving)
    {
        previousNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// ������������� ������� ����� ����� ��������
    /// </summary>
    /// <param name="removedIndex">������ ������ ��� ��������� ����</param>
    public void CheckIndexes(int removedIndex)
    {
        if (index > removedIndex)
        {
            index--;
        }
        for (int i = 0; i < previousNodesNumbers.Count; i++)
        {
            if (previousNodesNumbers[i] > removedIndex)
            {
                previousNodesNumbers[i]--;
            }
        }
        for (int i = 0; i < nextNodesNumbers.Count; i++)
        {
            if (nextNodesNumbers[i] > removedIndex)
            {
                nextNodesNumbers[i]--;
            }
        }
    }

    #endregion

    #region ���������

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(Rect, "", style);
    }

    public void Drag(Vector2 delta)
    {
        _rect.position += delta;
    }

    #endregion

    #region ��������� �������

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (Rect.Contains(e.mousePosition))
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
                if (e.button == 1 && isSelected && Rect.Contains(e.mousePosition))
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
        OnRemoveNode?.Invoke(this);
    }

    #endregion

    #endregion
}
