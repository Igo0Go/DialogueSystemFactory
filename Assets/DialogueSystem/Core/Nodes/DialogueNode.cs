using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    #region ���� � ��������

    /// <summary>
    /// ����� � ����� �������
    /// </summary>
    public int index;

    /// <summary>
    /// ������� ���������� �����
    /// </summary>
    public List<int> previousNodesNumbers;

    /// <summary>
    /// ������� ��������� �����
    /// </summary>
    public List<int> nextNodesNumbers;

    /// <summary>
    /// ���� ���� ����������� ������/������
    /// </summary>
    public bool finalNode;

    #endregion

    #region ������������
    /// <summary>
    /// ������� ��������� ���� � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public DialogueNode(Vector2 pos, int index)
    {
        this.index = index;
        previousNodesNumbers = new List<int>();
        nextNodesNumbers = new List<int>();
    }
    #endregion

    #region ������

    /// <summary>
    /// ������� ��������� ���� �� ������ ���������
    /// </summary>
    public void RemoveThisNodeFromNext(DialogueNode nodeForRemoving)
    {
        nextNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// ������� ��������� ���� �� ������ ����������
    /// </summary>
    /// <param name="nodeForRemoving">��������� ����</param>
    public void RemoveThisNodeFromPrevious(DialogueNode nodeForRemoving)
    {
        previousNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// ������������� ������� ����� ����� ��������
    /// </summary>
    /// <param name="removedIndex">������ ������ ��� ��������� ����</param>
    public void CheckIndexes(int removedIndex)
    {
        if (index > removedIndex)
        {
            index--;
        }
        for (int i = 0; i < previousNodesNumbers.Count; i++)
        {
            if (previousNodesNumbers[i] > removedIndex)
            {
                previousNodesNumbers[i]--;
            }
        }
        for (int i = 0; i < nextNodesNumbers.Count; i++)
        {
            if (nextNodesNumbers[i] > removedIndex)
            {
                nextNodesNumbers[i]--;
            }
        }
    }
    #endregion
}
