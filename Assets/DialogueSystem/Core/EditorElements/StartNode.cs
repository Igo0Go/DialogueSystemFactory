using UnityEngine;
using System;
using System.Collections.Generic;

public class StartNode : IDragableElement, IConnectionPoint, IHaveNextNodes, IHaveOneNextNode
{
    public int NodeIndex => -2;

    private GUIStyle style;
    public ConnectionPointType PointType { get; set; }
    public Connection CurrentConnection { get; set; }

    public Action<IConnectionPoint> OnClickConnectionPoint { get; set; }
    public Action<int> OnChangeFirstNode { get; set; }
    public Action<int> OnRemoveNext { get; set; }
    public Action<int> OnRemovePrevoius { get; set; } = (item) => { };

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
    private Rect _rect = new Rect(0, 0, 70, 30);

    public List<int> NextNodesNumbers;

    public int NextNodeNumber
    {
        get 
        {
            return NextNodesNumbers[0];
        }
        set
        {
            NextNodesNumbers[0] = value;
            OnChangeFirstNode?.Invoke(value);
        }
    }

    public int PointIndex => 0;

    public StartNode(GUIStyle style, Action<IConnectionPoint> OnClickInPoint, int nextNodeIndex, Action<int> OnChangeFirstNode)
    {
        this.style = style;
        PointType = ConnectionPointType.Out;

        this.OnChangeFirstNode = OnChangeFirstNode;
        OnClickConnectionPoint = OnClickInPoint;
        NextNodesNumbers = new List<int>() { nextNodeIndex };
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

    public void RemoveThisNodeFromNext(int nodeForRemoving)
    {
        NextNodesNumbers.RemoveAll(item => item == nodeForRemoving);
    }

    public void SaveReferenceToNode(int nodeReference)
    {
        NextNodeNumber = nodeReference;
    }

    public void AddThisNodeInNext(int newNode, int outPoinIndex)
    {
        NextNodeNumber = newNode;
    }

    public void UpdateData(Action<IConnectionPoint> OnClickConnectionPoint)
    {
        this.OnClickConnectionPoint = OnClickConnectionPoint;
    }
}
