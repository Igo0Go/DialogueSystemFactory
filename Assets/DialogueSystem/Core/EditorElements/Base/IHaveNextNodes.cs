public interface IHaveNextNodes
{
    /// <summary>
    /// ������� ������ �� ��������� ���� �� ������ ���������
    /// </summary>
    /// <param name="indexOfNodeForRemoving">������ ���������� ����</param>
    void RemoveThisNodeFromNext(int indexOfNodeForRemoving);

    /// <summary>
    /// ��������� ����� � ���������� ���� (�������� = -1) �� ������� ������
    /// </summary>
    /// <param name="indexOfNextConnectionPoint">������ ������ � ���������� ����, ������� ������
    /// ����� ������� ��� ����� (= -1)</param>
    void ClearNextByIndex(int indexOfNextConnectionPoint);

    /// <summary>
    /// �������� ������ �� ��������� ���� � ����� � ��������� �������
    /// </summary>
    /// <param name="newNodeIndex">������ ������������ ����</param>
    /// <param name="outPoinIndex">������ ������</param>
    void AddThisNodeInNext(int newNodeIndex, int outPoinIndex);
}
