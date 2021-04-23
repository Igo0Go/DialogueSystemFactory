using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� - �������
/// </summary>
public class EventNode : DialogueNode
{
    public int NextNodeNumber => nextNodesNumbers[0];

    #region ��������� ��� �������
    /// <summary>
    /// �������� �� ��������� ��������� ��� �������
    /// </summary>
    public bool useMessage;

    /// <summary>
    /// ����� ��������� ��� �������
    /// </summary>
    public string messageText;
    #endregion

    #region ��������� ���������
    /// <summary>
    /// ������ �� �������� ������-�� ���������
    /// </summary>
    public bool changeCondition;

    /// <summary>
    /// �� ����� �������� bool �������� ��������, ���� ������ ����� �������
    /// </summary>
    public bool targetBoolValue;

    /// <summary>
    /// �������� int, ������� ����� ��������/�������� �������� ���������, ���� ������ ����� ������� (����� ���� �������������)
    /// </summary>
    public int changeIntValue;

    /// <summary>
    /// ���� �������� ��������� � ����������
    /// </summary>
    public OperationType operation;
    #endregion

    #region ������� � �����
    /// <summary>
    /// ������� �� ������� � ������� �����
    /// </summary>
    public bool inSceneInvoke;

    /// <summary>
    /// ������ �������� � ������� �����, ����������� �� ������� (����� ����������� ������ ������ � ������)
    /// </summary>
    public List<int> reactorsNumbers;

    /// <summary>
    /// ����� �������� ������� ��� ������� �������� � ������� �����
    /// </summary>
    public float eventTime;

    /// <summary>
    /// ������ ������ ��� �������� �� ������� � ������� �����
    /// </summary>
    public int eventCamPositionNumber;
    #endregion

    #region ������������
    /// <summary>
    /// ������� ����-������� � ��������� ��������
    /// </summary>
    /// <param name="index">������ ���� � �����</param>
    public EventNode(int index) : base(index)
    {
        reactorsNumbers = new List<int>();
        transformRect = new Rect(0, 0, 150, 90);
        colorInEditor = Color.yellow;
        //        _rightPointOffset = new Vector2(130, 21);
        nextNodesNumbers.Add(-1);
    }

    /// <summary>
    /// ������� ����-������� � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public EventNode(Vector2 pos, int index) : base(pos, index)
    {
        reactorsNumbers = new List<int>();
        transformRect = new Rect(pos.x, pos.y, 150, 90);
        colorInEditor = Color.yellow;
        //        _rightPointOffset = new Vector2(130, 21);
        nextNodesNumbers.Add(-1);
    }

    protected EventNode() { }
    #endregion
}
