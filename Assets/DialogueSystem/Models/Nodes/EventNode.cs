using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� - �������
/// </summary>
[System.Serializable]
public class EventNode : DialogueNode
{
    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

    public int NextNodeNumber
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
    /// ����� ���������� ��� ���������
    /// </summary>
    public ParameterPack parameter;

    /// <summary>
    /// ������ �� �������� ������-�� ���������
    /// </summary>
    public bool changeParameter;

    /// <summary>
    /// �� ����� �������� bool �������� ��������, ���� ������ ����� �������
    /// </summary>
    public bool targetBoolValue;

    /// <summary>
    /// �������� int, ������� ����� ��������/�������� �������� ���������, ���� ������ ����� ������� (����� ���� �������������)
    /// </summary>
    public int changeIntValue;

    /// <summary>
    /// ������ ����������� ��������� � ������
    /// </summary>
    public int changeingParameterIndex;

    /// <summary>
    /// ���� �������� ��������� � ����������
    /// </summary>
    public OperationType parameterOperation;

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

    #region ��������� ������ ���������

    /// <summary>
    /// ����� ���������
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// ������ �� �������������� ���������
    /// </summary>
    public bool changeCharacter;

    /// <summary>
    /// ���� �������� ��������� � ����������
    /// </summary>
    public OperationType characterOperation;

    /// <summary>
    ///����� �������� ��������� �������� 
    /// </summary>
    public int changeCharacterStatIndex;

    /// <summary>
    ///�� ����� �������� �������� �������� ��������� 
    /// </summary>
    public int changeCharacterStatValue;
    #endregion

    #region ������������
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
        nextNodesNumbers.Add(-1);
    }

    protected EventNode() { }
    #endregion
}
