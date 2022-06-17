using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class DialogueNode : IDrawableElement, IDragableElement, IHavePreviousNodes, IHaveOneNextNode
{
    #region Поля и свойства

    #region Логика
    /// <summary>
    /// Номер в схеме диалога
    /// </summary>
    public int Index;

    /// <summary>
    /// Этот узел заканчивает диалог/группу
    /// </summary>
    public bool finalNode;

    /// <summary>
    /// Индекс следующего узла
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
    /// Индексы следующих узлов
    /// </summary>
    public List<int> NextNodesNumbers;
    /// <summary>
    /// Индексы предыдущих узлов
    /// </summary>
    public List<int> PreviousNodeNumbers;
    #endregion

    #region Отрисовка
    /// <summary>
    /// Прямоугольник, в котором отрисовывается узел
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

    private bool isSelected;
    private bool isDragged;

    public Action<DialogueNode> OnRemoveNode;

    public ConnectionPoint InPoint { get; private set; }
    public List<ConnectionPoint> OutPoints { get; private set; }

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    #endregion

    #endregion

    #region Создание и обновление
    /// <summary>
    /// Создать дилоговый узел с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public DialogueNode(Vector2 position, int index, GUIStyle defaultStyle, GUIStyle selectedStyle,
        Action<DialogueNode> onRemove,
        GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<IConnectionPoint> OnClickInPoint)
    {
        this.Index = index;
        PreviousNodeNumbers = new List<int>();
        NextNodesNumbers = new List<int>() { -1 };

        Rect = new Rect(position.x, position.y, 110, 40);
        defaultNodeStyle = style = defaultStyle;
        selectedNodeStyle = selectedStyle;
        UpdateNodeData(onRemove, OnClickInPoint, inPointStyle, outPointStyle);

    }

    public void UpdateNodeData(Action<DialogueNode> onRemove, Action<IConnectionPoint> OnClickInPoint,
        GUIStyle inPointStyle, GUIStyle outPointStyle)
    {
        InPoint = new ConnectionPoint(new Vector2(-55, -10), this, 0, ConnectionPointType.In, inPointStyle,
            OnClickInPoint);
        OutPoints = new List<ConnectionPoint>()
        {
            new ConnectionPoint(new Vector2(45, -10), this, 0, ConnectionPointType.Out, outPointStyle,
            OnClickInPoint)
        };
        OnRemoveNode = onRemove;
    }
    #endregion

    #region Методы

    #region Логика

    /// <summary>
    /// Удалить указанный узел из списка следующих
    /// </summary>
    public void RemoveThisNodeFromNext(int nodeForRemoving)
    {
        NextNodesNumbers.RemoveAll(item => item == nodeForRemoving);
    }
    /// <summary>
    /// Зачистить указанный выход к следующим узлам
    /// </summary>
    /// <param name="indexOfNextConnectionPoint">индекс выхода, который нужно зачистить</param>
    public void ClearNextByIndex(int indexOfNextConnectionPoint)
    {
        NextNodesNumbers[indexOfNextConnectionPoint] = -1;
    }
    /// <summary>
    /// Добавить новый узел в список следующих (в данном случае идёт замена единственного следующего)
    /// </summary>
    /// <param name="newNode">индекс нового узла</param>
    /// <param name="outPoinIndex">для какого выхода сделать ссылку (в данном случае игнорируется)</param>
    public void AddThisNodeInNext(int newNode, int outPoinIndex)
    {
        NextNodeNumber = newNode;
    }

    /// <summary>
    /// Удалить указанный узел из списка предыдущих
    /// </summary>
    /// <param name="nodeForRemoving">удаляемый узел</param>
    public void RemoveThisNodeFromPrevious(int nodeForRemoving)
    {
        PreviousNodeNumbers.RemoveAll(item => item == nodeForRemoving);
    }
    /// <summary>
    /// Добавить узел в список следующих
    /// </summary>
    /// <param name="newNode">индекс нвого узла</param>
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
    /// корректировка номеров узлов после удаления
    /// </summary>
    /// <param name="removedIndex">индекс только что удалённого узла</param>
    public void CheckIndexesAfterRemovingNodeWithIndex(int removedIndex)
    {
        if (Index > removedIndex)
        {
            Index--;
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
    /// корректировка номеров узлов после удаления
    /// </summary>
    /// <param name="removedIndex">индекс только что удалённого узла</param>
    public void CheckIndexesAfterInsertingNodeWithIndex(int insertedIndex)
    {
        if (Index >= insertedIndex)
        {
            Index++;
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

    #region Отрисовка

    public virtual void Draw()
    {
        InPoint.Draw();
        OutPoints[0].Draw();
        GUI.Box(Rect, Index.ToString() , style);

        if(PreviousNodeNumbers != null && PreviousNodeNumbers.Count > 0)
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

    #region Обработка событий
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
                        CommandManager.dragBufer = _rect.position;
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
        OnRemoveNode?.Invoke(this);
    }


    #endregion

    #endregion
}
