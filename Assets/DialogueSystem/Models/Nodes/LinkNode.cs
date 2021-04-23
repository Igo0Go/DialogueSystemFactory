using UnityEngine;

/// <summary>
/// Узел-ссылка
/// </summary>
public class LinkNode : DialogueNode
{
    /// <summary>
    /// Индекс узла, на который нужно перенаправить ход диалога
    /// </summary>
    public int NextNodeNumber => nextNodesNumbers[0];
    /// <summary>
    /// Смещение выхода
    /// </summary>
    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

    /// <summary>
    /// Создать узел-ссылку с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public LinkNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 50);
        colorInEditor = Color.blue;
        nextNodesNumbers.Add(-1);
    }

    protected LinkNode() { }
}
