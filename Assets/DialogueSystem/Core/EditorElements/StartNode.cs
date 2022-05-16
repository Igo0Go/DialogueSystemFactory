using UnityEngine;
using System;

public class StartNode
{
    public int firstNodeNomber = -1;

    private Rect startRect = new Rect(0, 0, 70, 30);
    private GUIStyle style;
    public Action OnClickStartPoint;

    public StartNode(GUIStyle style)
    {
        this.style = style;
    }

    public void Draw()
    {
        if (GUI.Button(startRect, "", style))
        {
            OnClickStartPoint?.Invoke();
        }

        Rect bufer = startRect;
        bufer.position += new Vector2(10, 0);
        GUI.Label(bufer, "бвРав");
    }
    public void Drag(Vector2 delta)
    {
        startRect.position += delta;
    }
}
