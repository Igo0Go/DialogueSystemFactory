using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����-������������
/// </summary>
public class RandomizerNode : DialogueNode
{
    /// <summary>
    /// �������� ������� ������ ����� ���������� ������
    /// </summary>
    public List<Vector2> exitPointsOffsetList;

    /// <summary>
    /// ������� �� ������ �� ���� ����� ���������� ������
    /// </summary>
    public bool withRemoving;

    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

    /// <summary>
    /// ������ �� ���� �� ���������
    /// </summary>
    public int defaultNextNodeNumber
    {
        get
        {
            return nextNodesNumbers[0];
        }
        set
        {
            nextNodesNumbers[0] = value;
        }
    }

    /// <summary>
    /// ������� ����-������������ � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public RandomizerNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 65);
        colorInEditor = Color.magenta;
        exitPointsOffsetList = new List<Vector2>();
        nextNodesNumbers.Add(-1);
    }

    protected RandomizerNode() { }

    /// <summary>
    /// �������� ���� � ������ ��� ���������� ������
    /// </summary>
    /// <param name="index">������ ����</param>
    public void AddLinkNumber(int index)
    {
        nextNodesNumbers.Add(index);
        CheckExitOffsetForRandomLink();
    }

    /// <summary>
    /// ������� ���� �� ������ ��� ���������� ������
    /// </summary>
    /// <param name="numberInList">������ ����</param>
    public void RemoveLinkNumber(int numberInList)
    {
        nextNodesNumbers.RemoveAt(numberInList);
        CheckExitOffsetForRandomLink();
    }

    /// <summary>
    /// �������� ��������� ��������� ���� ��� �������� ���� �� ���������, ���� ������ ���������� ������ ����
    /// </summary>
    /// <returns>������ ���������� ����</returns>
    public int GetNextLink()
    {
        if (nextNodesNumbers.Count == 1)
        {
            return defaultNextNodeNumber;
        }

        int resultPositionInList = Random.Range(1, nextNodesNumbers.Count - 1);
        int resultIndex = nextNodesNumbers[resultPositionInList];
        nextNodesNumbers.RemoveAt(resultPositionInList);
        return resultIndex;
    }

    private void CheckExitOffsetForRandomLink()
    {
        exitPointsOffsetList.Clear();
        for (int i = 1; i < nextNodesNumbers.Count; i++)
        {
            exitPointsOffsetList.Add(exitPointOffset + new Vector2(0, (i-1) * 21));
        }
    }
}
[System.Serializable]
public class RandomLinlkPair
{
    /// <summary>
    /// ������ �� ��������� ����
    /// </summary>
    public int nextNodeIndex;

    /// <summary>
    /// ����������� ��������� ����
    /// </summary>
    public bool access;

    /// <summary>
    /// ������� ���� (����, ������). �� ��������� ���� ����� ��������
    /// </summary>
    /// <param name="linkIndex">������ ����</param>
    public RandomLinlkPair(int linkIndex)
    {
        nextNodeIndex = linkIndex;
        access = true;
    }
}
