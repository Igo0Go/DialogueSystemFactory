using UnityEngine;
using UnityEditor;
using System;

public class Connection
{
    public IConnectionPoint inPoint;
    public IConnectionPoint outPoint;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(IConnectionPoint inPoint, IConnectionPoint outPoint,
        Action<Connection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        inPoint.CurrentConnection = outPoint.CurrentConnection = this;
        inPoint.SaveReferenceToNode(outPoint.NodeIndex);
        outPoint.SaveReferenceToNode(inPoint.NodeIndex);
        UpdateDelegates(OnClickRemoveConnection);
    }

    public void UpdateDelegates(Action<Connection> OnClickRemoveConnection)
    {
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            inPoint.Rect.center,
            outPoint.Rect.center,
            inPoint.Rect.center + Vector2.left * 50f,
            outPoint.Rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((inPoint.Rect.center + outPoint.Rect.center) * 0.5f, Quaternion.identity, 4, 8,
            Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}
