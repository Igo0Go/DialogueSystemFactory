using UnityEngine;

/// <summary>
/// ����-������
/// </summary>
public class LinkNode : DialogueNode
{
    /// <summary>
    /// ������ ����, �� ������� ����� ������������� ��� �������
    /// </summary>
    public int NextNodeNumber => nextNodesNumbers[0];
    /// <summary>
    /// �������� ������
    /// </summary>
    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

    /// <summary>
    /// ������� ����-������ � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public LinkNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 50);
        colorInEditor = Color.blue;
        nextNodesNumbers.Add(-1);
    }

    protected LinkNode() { }
}
