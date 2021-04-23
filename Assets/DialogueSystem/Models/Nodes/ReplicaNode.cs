using UnityEngine;

/// <summary>
/// ���� - �������
/// </summary>
public class ReplicaNode : DialogueNode
{
    #region ���� � ��������

    /// <summary>
    /// ��������� ����
    /// </summary>
    public int NextNodeNumber => nextNodesNumbers[0];

    /// <summary>
    /// ������������ ���� ���������� � �������
    /// </summary>
    public ReplicInfo replicaInformation;

    /// <summary>
    /// �������� ������
    /// </summary>
    public readonly Vector2 exitPointOffset = new Vector3(130, 21);

    #endregion

    #region ������������

    /// <summary>
    /// ������� ���� �������
    /// </summary>
    /// <param name="index">������ ����</param>
    public ReplicaNode(int index) : base(index)
    {
        transformRect = new Rect(0, 0, 150, 50);
        colorInEditor = Color.gray;
        replicaInformation = new ReplicInfo();
        nextNodesNumbers.Add(-1);
    }

    /// <summary>
    /// ������� ���� ������� � ��������� �������� � ��������� �����������
    /// </summary>
    /// <param name="pos">���������� ���� � �����</param>
    /// <param name="index">������ ����</param>
    public ReplicaNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 50);
        colorInEditor = Color.gray;
        replicaInformation = new ReplicInfo();
        nextNodesNumbers.Add(-1);
    }

    private ReplicaNode(){}

    #endregion
}

/// <summary>
/// ����� � ����������� �� �������. ����������� ��� ����� ��������, ��� � � �������.
/// </summary>
public class ReplicInfo
{
    /// <summary>
    /// ������� ��� �������
    /// </summary>
    public bool alreadyUsed;

    /// <summary>
    /// ������� ����� ����������
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// ��������� ������� ������� (����� ���� ������, ���� �� �� ����������� ������� � �������)
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// ������������ ��������� ���������� �� ����� �������
    /// </summary>
    public DialogueAnimType animType;

    /// <summary>
    /// ������� ������� ������ �� ����� �������
    /// </summary>
    public int camPositionNumber;

    /// <summary>
    /// ����� ���������
    /// </summary>
    public string replicaText;

    /// <summary>
    /// ������� ����� ���������� � �������. ��� ����� �������� ��� �������������
    /// </summary>
    public ReplicInfo()
    {
        alreadyUsed = false;
    }
}
