using UnityEngine;

public interface IDrawableElement
{
    public Rect Rect { get; set; }

    void Draw();
}
