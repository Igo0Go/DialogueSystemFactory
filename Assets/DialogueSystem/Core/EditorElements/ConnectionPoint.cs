using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint : IConnectionPoint
{
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
    private Rect _rect;

    public DialogueNode node;
    public int NodeIndex => node.Index;
    public int PointIndex { get; private set; }
    public ConnectionPointType PointType { get; set; }
    public Connection CurrentConnection { get; set; }

    public GUIStyle style;

    public Action<IConnectionPoint> OnClickConnectionPoint { get; set; }
    public Action<int> OnRemoveNext { get; set; }
    public Action<int> OnRemovePrevoius { get; set; }

    private Vector2 offset;

    public ConnectionPoint(Vector2 offset, DialogueNode node, int indexOfPoint, ConnectionPointType type, GUIStyle style,
        Action<IConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        PointIndex = indexOfPoint;
        PointType = type;
        this.style = style;
        this.offset = offset;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        _rect = new Rect(0, 0, 10f, 20f);

        OnRemoveNext = node.RemoveThisNodeFromPrevious;
        OnRemovePrevoius = node.RemoveThisNodeFromNext;

    }
    public void UpdateData(Action<IConnectionPoint> OnClickConnectionPoint)
    {
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        OnRemoveNext = node.RemoveThisNodeFromPrevious;
    }

    public void Draw()
    {
        _rect.position = node.Rect.center + offset;

        if (GUI.Button(_rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }

    public bool Equals(IConnectionPoint other)
    {
        if(other is StartNode)
        {
            return false;
        }
        else if (other is ConnectionPoint point)
        {
            return node.Index == point.node.Index;
        }
        return false;
    }

    public void SaveReferenceToNode(int saveNodeIndex)
    {
        if(PointType == ConnectionPointType.In)
        {
            node.AddThisNodeInPrevious(saveNodeIndex);
        }
        else
        {
            node.AddThisNodeInNext(saveNodeIndex, PointIndex);
        }
    }

    public void ClearReferenceToNodeByValue(int nodeReference)
    {
        if (PointType == ConnectionPointType.In)
        {
            node.RemoveThisNodeFromPrevious(nodeReference);
        }
        else
        {
            node.ClearNextByIndex(PointIndex);
        }
    }
}