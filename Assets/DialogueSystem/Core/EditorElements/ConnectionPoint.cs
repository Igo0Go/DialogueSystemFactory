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
    public GUIStyle style;

    public ConnectionPointType PointType { get; set; }
    public Action<IConnectionPoint> OnClickConnectionPoint { get; set; }

    private Vector2 offset;

    public ConnectionPoint(Vector2 offset, DialogueNode node, ConnectionPointType type, GUIStyle style,
        Action<IConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.PointType = type;
        this.style = style;
        this.offset = offset;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        _rect = new Rect(0, 0, 10f, 20f);
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
            return node.index == point.node.index;
        }
        return false;
    }
}