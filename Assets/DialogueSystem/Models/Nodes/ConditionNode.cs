using UnityEngine;

/// <summary>
/// ����-�������
/// </summary>
[System.Serializable]
public class ConditionNode : DialogueNode
{
    #region ���� � ��������

    /// <summary>
    /// ����� ������ (�������� ��������)
    /// </summary>
    public ParameterPack parameter;

    /// <summary>
    /// ����� ������������ ������� (�� ����� ������� � ������)
    /// </summary>
    public int conditionNumber;

    /// <summary>
    /// ��� �������� ������� (������� �� ���� �������)
    /// </summary>
    public CheckType checkType;

    /// <summary>
    /// ������� �������� ��� ��������� bool
    /// </summary>
    public bool checkBoolValue;

    /// <summary>
    /// ������� �������� ��� ��������� int
    /// </summary>
    public int checkIntValue;

    /// <summary>
    /// ������ �� ��������� ���� ��� ����������� ������
    /// </summary>
    public int PositiveNextNumber
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
    /// ������ �� ��������� ���� ��� ����������� ������
    /// </summary>
    public int NegativeNextNumber
    {
        get
        {
            return nextNodesNumbers[1];
        }
        set
        {
            nextNodesNumbers[1] = value;
        }
    }

    public readonly Vector2 positiveExitPointOffset = new Vector2(150, 21);
    public readonly Vector2 negativeExitPointOffset = new Vector2(150, 42);

    #endregion

    #region ������������

    /// <summary>
    /// ������� ��������� ���� � ��������� �������� � ��������� �������
    /// </summary>
    /// <param name="pos">������� ���� � ����������� �����</param>
    /// <param name="index">������ ���� � �����</param>
    public ConditionNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 180, 65);
        //        _rightPointOffset = new Vector3(160, 21);
        colorInEditor = Color.cyan;
        nextNodesNumbers.Add(-1);
        nextNodesNumbers.Add(-1);
    }

    public ConditionNode(){}

    #endregion
}
