using UnityEngine;
using System;
using System.Collections.Generic;

public class StartNode : IDragableElement, IConnectionPoint, IHaveNextNodes, IHaveOneNextNode
{
    private GUIStyle style;
    public ConnectionPointType PointType { get; set; }
    public Action<IConnectionPoint> OnClickConnectionPoint { get; set; }

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

    public int NodeIndex => -2;

    public Connection CurrentConnection { get; set; }
    public Action<int> OnRemoveNext { get; set ; }
    public Action<int> OnRemovePrevoius { get; set; } = (item) => { };
    public List<int> NextNodesNumbers { get; set; }

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
            if (NextNodesNumbers == null)
                NextNodesNumbers = new List<int>();

            NextNodesNumbers[0] = value;
        }
    }

    private Rect _rect = new Rect(0, 0, 70, 30);

    public StartNode(GUIStyle style, Action<IConnectionPoint> OnClickInPoint)
    {
        this.style = style;
        PointType = ConnectionPointType.Out;
        UpdateDelegates(OnClickInPoint);
    }
    public void UpdateDelegates(Action<IConnectionPoint> OnClickInPoint)
    {
        OnClickConnectionPoint = OnClickInPoint;
    }

    public void Drag(Vector2 delta)
    {
        _rect.position += delta;
    }

    public void Draw()
    {
        if (GUI.Button(_rect, "", style))
        {
            OnClickConnectionPoint?.Invoke(this);
        }

        Rect bufer = _rect;
        bufer.position += new Vector2(10, 0);
        GUI.Label(bufer, "СТАРТ");
    }

    public bool Equals(IConnectionPoint other)
    {
        return false;
    }

    public void RemoveThisNodeFromPrevious(int nodeForRemoving)
    {
        NextNodesNumbers.RemoveAll(item => item == nodeForRemoving);
    }
}
