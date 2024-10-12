using System;
using UnityEngine;

public class DialogueNodeReplica : DialogueNode
{
    public string text;

    public DialogueNodeReplica(Vector2 position, Vector2 size, int index, GUISkin skin,
        Action<DialogueNode> onRemove, Action<IConnectionPoint> OnClickInPoint, 
        Action<DialogueNode> onNodeSelect, Action<DialogueNode> onNodeDeselect) : 
        base(position, size, index, skin, onRemove, OnClickInPoint, onNodeSelect, onNodeDeselect)
    {
        
    }

    public override void Draw()
    {
        base.Draw();
        Rect bufer = new Rect(Rect.position.x + 10, Rect.position.y + 20, Rect.width - 40, 40);
        GUI.Label(bufer, text);
        bufer = new Rect(Rect.position.x + Rect.width - 50, Rect.position.y + 5, 20, 22);
        GUI.Button(bufer, "=");
    }
}
