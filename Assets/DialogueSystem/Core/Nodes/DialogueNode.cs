using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class DialogueNode : IHaveIndexCheckFunctions, IDrawableElement, IDragableElement, IHavePreviousNodes, IHaveOneNextNode
{
    #region ���� � ��������

    #region ������
    /// <summary>
    /// ����� � ����� �������
    /// </summary>
    public int index;

    /// <summary>
    /// ���� ���� ����������� ������/������
    /// </summary>
    public bool finalNode;

    /// <summary>
    /// ������ ���������� ����
    /// </summary>
    public int NextNodeNumber
    {
        get
        {
            if (NextNodesNumbers != null && NextNodesNumbers.Count > 0)
                return NextNodesNumbers[0];
            return -1;
        }
        set
        {
            NextNodesNumbers[0] = value;
        }
    }

    /// <summary>
    /// ������� ��������� �����
    /// </summary>
    public List<int> NextNodesNumbers;
    /// <summary>
    /// ������� ���������� �����
    /// </summary>
    public List<int> PreviousNodeNumbers;
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
    [SerializeField]
    private Rect _rect;

    public bool isSelected;
    private bool isDragged;

    public Action<DialogueNode> OnRemoveNode;
    public event Action<DialogueNode> OnNodeSelect;
    public event Action<DialogueNode> OnNodeDeselect;

    public ConnectionPoint InPoint { get; private set; }
    public List<ConnectionPoint> OutPoints { get; private set; }

    private GUIStyle style;
    private GUISkin skin;
    private Vector2 position;
    private Vector2 size;
    private Action<DialogueNode> onRemove;
    private Action<IConnectionPoint> onClickInPoint;
    #endregion

    #endregion

    #region �������� � ����������
    /// <summary>
    /// ������� ��������� ���� � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public DialogueNode(Vector2 position, Vector2 size, int index, GUISkin skin,
        Action<DialogueNode> onRemove, Action<IConnectionPoint> OnClickInPoint, 
        Action<DialogueNode> onNodeSelect, Action<DialogueNode> onNodeDeselect)
    {
        this.index = index;
        PreviousNodeNumbers = new List<int>();
        NextNodesNumbers = new List<int>() { -1 };
        style = StylePack.nodeStyleReplica_default;
        this.skin = skin;
        Rect = new Rect(position.x, position.y, size.x, size.y);
        UpdateNodeData(onRemove, OnClickInPoint, StylePack.inPointStyle, StylePack.outPointStyle);
        OnNodeSelect += onNodeSelect;
        OnNodeDeselect += onNodeDeselect;
    }

    public DialogueNode(Vector2 position, Vector2 size, int index, GUISkin skin, Action<DialogueNode> onRemove, Action<IConnectionPoint> onClickInPoint)
    {
        this.position = position;
        this.size = size;
        this.index = index;
        this.skin = skin;
        this.onRemove = onRemove;
        this.onClickInPoint = onClickInPoint;
    }

    public void UpdateNodeData(Action<DialogueNode> onRemove, Action<IConnectionPoint> OnClickInPoint,
        GUIStyle inPointStyle, GUIStyle outPointStyle)
    {
        InPoint = new ConnectionPoint(new Vector2(-Rect.width/2 - 10, -Rect.height/2 + 10), new Vector2(20, Rect.height-20),
            this, 0, ConnectionPointType.In, inPointStyle,
            OnClickInPoint);
        OutPoints = new List<ConnectionPoint>()
        {
            new ConnectionPoint(new Vector2(Rect.width/2 - 10, -Rect.height/2 + 10), new Vector2(20, Rect.height-20), 
            this, 0, ConnectionPointType.Out, outPointStyle, OnClickInPoint)
        };
        OnRemoveNode = onRemove;
    }
    #endregion

    #region ������

    #region ������

    /// <summary>
    /// ������� ��������� ���� �� ������ ���������
    /// </summary>
    public void RemoveThisNodeFromNext(int nodeForRemoving)
    {
        NextNodesNumbers.RemoveAll(item => item == nodeForRemoving);
    }
    /// <summary>
    /// ��������� ��������� ����� � ��������� �����
    /// </summary>
    /// <param name="indexOfNextConnectionPoint">������ ������, ������� ����� ���������</param>
    public void ClearNextByIndex(int indexOfNextConnectionPoint)
    {
        NextNodesNumbers[indexOfNextConnectionPoint] = -1;
    }
    /// <summary>
    /// �������� ����� ���� � ������ ��������� (� ������ ������ ��� ������ ������������� ����������)
    /// </summary>
    /// <param name="newNode">������ ������ ����</param>
    /// <param name="outPoinIndex">��� ������ ������ ������� ������ (� ������ ������ ������������)</param>
    public void AddThisNodeInNext(int newNode, int outPoinIndex)
    {
        NextNodeNumber = newNode;
    }

    /// <summary>
    /// ������� ��������� ���� �� ������ ����������
    /// </summary>
    /// <param name="nodeForRemoving">��������� ����</param>
    public void RemoveThisNodeFromPrevious(int nodeForRemoving)
    {
        PreviousNodeNumbers.RemoveAll(item => item == nodeForRemoving);
    }
    /// <summary>
    /// �������� ���� � ������ ���������
    /// </summary>
    /// <param name="newNode">������ ����� ����</param>
    public void AddThisNodeInPrevious(int newNode)
    {
        if (PreviousNodeNumbers == null)
        {
            PreviousNodeNumbers = new List<int>();
        }
        if (!PreviousNodeNumbers.Contains(newNode))
        {
            PreviousNodeNumbers.Add(newNode);
        }
    }

    /// <summary>
    /// ������������� ������� ����� ����� ��������
    /// </summary>
    /// <param name="removedIndex">������ ������ ��� ��������� ����</param>
    public void CheckIndexesAfterRemovingNodeWithIndex(int removedIndex)
    {
        if (index > removedIndex)
        {
            index--;
        }
        for (int i = 0; i < PreviousNodeNumbers.Count; i++)
        {
            if (PreviousNodeNumbers[i] > removedIndex)
            {
                PreviousNodeNumbers[i]--;
            }
        }
        for (int i = 0; i < NextNodesNumbers.Count; i++)
        {
            if (NextNodesNumbers[i] > removedIndex)
            {
                NextNodesNumbers[i]--;
            }
        }
    }
    /// <summary>
    /// ������������� ������� ����� ����� ��������
    /// </summary>
    /// <param name="removedIndex">������ ������ ��� ��������� ����</param>
    public void CheckIndexesAfterInsertingNodeWithIndex(int insertedIndex)
    {
        if (index >= insertedIndex)
        {
            index++;
        }
        for (int i = 0; i < PreviousNodeNumbers.Count; i++)
        {
            if (PreviousNodeNumbers[i] >= insertedIndex)
            {
                PreviousNodeNumbers[i]++;
            }
        }
        for (int i = 0; i < NextNodesNumbers.Count; i++)
        {
            if (NextNodesNumbers[i] >= insertedIndex)
            {
                NextNodesNumbers[i]++;
            }
        }
    }

    #endregion

    #region ���������
    public virtual void Draw()
    {
        GUI.skin = this.skin;
        InPoint.Draw();
        OutPoints[0].Draw();
        GUI.Box(Rect, string.Empty, style);
        Rect bufer = new Rect(Rect.position.x + 10, Rect.position.y + 10, 17, 17);

        skin.box.fontSize = 8;

        GUI.Box(bufer, index.ToString());

        if (PreviousNodeNumbers != null && PreviousNodeNumbers.Count > 0)
        {
            GUI.Label(new Rect(Rect.position, new Vector2(20, 20)), PreviousNodeNumbers[0].ToString());
        }
        if(NextNodesNumbers != null && NextNodesNumbers.Count > 0)
        {
            GUI.Label(new Rect(Rect.position + new Vector2(Rect.width - 22, 0), new Vector2(20, 20)),
                NextNodesNumbers[0].ToString());
        }
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
                        OnNodeSelect?.Invoke(this);
                        style = StylePack.nodeStyleReplica_selected;
                        CommandManager.dragBufer = _rect.position;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        OnNodeDeselect?.Invoke(this);
                        style = StylePack.nodeStyleReplica_default;
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
                if(e.button == 0 && Rect.Contains(e.mousePosition))
                {
                    CommandManager.dragBufer = _rect.position - CommandManager.dragBufer;
                    CommandManager.AddCommand(new MoveNodeCommand(CommandManager.dragBufer, this));
                    CommandManager.dragBufer = Vector2.zero;
                }
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
        OutPoints[0]?.CurrentConnection?.OnClickRemoveConnection(OutPoints[0].CurrentConnection);
        InPoint?.CurrentConnection?.OnClickRemoveConnection(InPoint.CurrentConnection);
        OnNodeDeselect?.Invoke(this);
        OnRemoveNode?.Invoke(this);
    }
    #endregion

    #endregion
}
