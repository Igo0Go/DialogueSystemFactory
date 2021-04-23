using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� - �����
/// </summary>
public class ChoiceNode : DialogueNode
{
    /// <summary>
    /// ������� ����� �����������
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// �������� �������
    /// </summary>
    public List<Vector2> exitPointOffsetList;

    /// <summary>
    /// �������� ������
    /// </summary>
    public List<AnswerItem> answers;

    /// <summary>
    /// ������ ������� ������ � �����
    /// </summary>
    public int defaultCameraPositionIndex;

    private readonly int answerLimit = 3;
    private readonly Vector2 exitOffset = new Vector3(180, 21);

    /// <summary>
    /// ������� ���� ������ � ��������� ��������
    /// </summary>
    /// <param name="index">������ ���� � �����</param>
    public ChoiceNode(int index) : base(index)
    {
        transformRect = new Rect(0, 0, 200, 50);
        colorInEditor = Color.grey;
        exitPointOffsetList = new List<Vector2>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    /// <summary>
    /// ������� ���� ������ � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public ChoiceNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 200, 50);
        colorInEditor = Color.grey;
        exitPointOffsetList = new List<Vector2>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    private ChoiceNode(){}

    /// <summary>
    /// �������� ��������� ��� ������ �������� ������
    /// </summary>
    public void AddAnswer()
    {
        if(answers.Count < answerLimit)
        {
            answers.Add(new AnswerItem());
            CheckExitOffset();
        }
    }
    /// <summary>
    /// ������� ������� ������ �� ������
    /// </summary>
    /// <param name="number">����� �������� � ������ �������</param>
    public void RemoveAnsver(int number)
    {
        nextNodesNumbers[number] = -1;
        answers.RemoveAt(number);
        CheckExitOffset();
    }
    /// <summary>
    /// �������� ������� �������� ���� � ������������ � ����������� ��������� �������
    /// </summary>
    private void CheckExitOffset()
    {
        exitPointOffsetList.Clear();
        for (int i = 0; i < answers.Count; i++)
        {
            exitPointOffsetList.Add(exitOffset + new Vector2(0, i * 21));
        }
    }
}

/// <summary>
/// ����� ���������� � �������� ������
/// </summary>
public class AnswerItem
{
    /// <summary>
    /// �����, ������� ����� ���������� �� ������ ������ ������� (����� ���������� �� �������� �������, � �������, ��� ����������)
    /// </summary>
    public string answerTip;

    /// <summary>
    /// �������� ������� ������
    /// </summary>
    public ReplicInfo answerReplica;

    public AnswerItem()
    {
        answerReplica = new ReplicInfo();

    }
}
