public interface IHavePreviousNodes
{
    /// <summary>
    /// ������� ������ �� ��������� ���� �� ����� ����������
    /// </summary>
    /// <param name="indexOfNodeForRemoving">������ ���������� ����</param>
    void RemoveThisNodeFromPrevious(int indexOfNodeForRemoving);

    /// <summary>
    /// �������� ������ �� ��������� ���� � ������ ����������
    /// </summary>
    /// <param name="newNodeIndex">������ ������ ����</param>
    void AddThisNodeInPrevious(int newNodeIndex);
}
