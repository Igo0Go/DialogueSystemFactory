using UnityEngine;
using System;

public class StartNode : IDragableElement, IConnectionPoint
{
    public int firstNodeNomber = -1;

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

    private Rect _rect = new Rect(0, 0, 70, 30);

    public StartNode(GUIStyle style, Action<IConnectionPoint> OnClickInPoint)
    {
        this.style = style;
        PointType = ConnectionPointType.Out;
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
}
