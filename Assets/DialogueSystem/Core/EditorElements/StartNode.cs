using UnityEngine;
using System;
using System.Collections.Generic;

public class StartNode : IHaveIndexCheckFunctions, IDragableElement, IConnectionPoint, IHaveNextNodes, IHaveOneNextNode
{
    public int NodeIndex => -2;

    /// <summary>
    /// ѕр€моугольник, в котором отрисовываетс€ узел
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

    public int NextNodeNumber
    {
        get
        {
            return NextNodesNumbers[0];
        }
        set
        {
            NextNodesNumbers[0] = value;
            OnChangeFirstNode?.Invoke(NextNodesNumbers[0]);
        }
    }
    public List<int> NextNodesNumbers;

    public int PointIndex => 0;

    public ConnectionPointType PointType { get; set; }
    public Connection CurrentConnection { get; set; }

    public Action<IConnectionPoint> OnClickConnectionPoint { get; set; }
    public Action<int> OnChangeFirstNode { get; set; }
    public Action<int> OnRemoveNext { get; set; }
    public Action<int> OnRemovePrevoius { get; set; } = (item) => { };

    private readonly GUIStyle style;

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
        GUI.Label(bufer, NextNodeNumber.ToString());
    }

    public bool Equals(IConnectionPoint other)
    {
        return false; //поскольку нет второго такого стартового узла в схеме, метод всегда возвращает false
    }

    public void UpdateData(Action<IConnectionPoint> OnClickConnectionPoint)
    {
        this.OnClickConnectionPoint = OnClickConnectionPoint;
    }

    public void SaveReferenceToNode(int nodeReference)
    {
        NextNodeNumber = nodeReference;
    }
    public void ClearReferenceToNodeByValue(int nodeReference)
    {
        ClearNextByIndex(0);
    }

    public void AddThisNodeInNext(int newNode, int outPoinIndex)
    {
        NextNodeNumber = newNode;
    }
    public void ClearNextByIndex(int indexOfNextConnectionPoint)
    {
        NextNodeNumber = -1;
    }
    public void RemoveThisNodeFromNext(int nodeForRemoving)
    {
        NextNodesNumbers = new List<int> { -1 };
        OnChangeFirstNode?.Invoke(NextNodesNumbers[0]);
    }

    public void CheckIndexesAfterRemovingNodeWithIndex(int removedIndex)
    {
        if (NextNodeNumber > removedIndex)
        {
            NextNodeNumber--;
        }
    }
    public void CheckIndexesAfterInsertingNodeWithIndex(int insertedIndex)
    {
        if (NextNodeNumber >= insertedIndex)
        {
            NextNodeNumber++;
        }
    }

}
