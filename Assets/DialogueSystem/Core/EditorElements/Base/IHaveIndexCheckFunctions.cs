public interface IHaveIndexCheckFunctions
{
    /// <summary>
    /// ��������������� ������� ����� �������� ���� � ��������� �������� 
    /// (��� ������� ������� ��������� ����� ��������� �� �������, ������� ����� ���������)
    /// </summary>
    /// <param name="removedNodeIndex">������ ��������� ����</param>
    void CheckIndexesAfterRemovingNodeWithIndex(int removedNodeIndex);

    /// <summary>
    /// ��������������� ������� ����� ������� ���� � ��������� ����� ������ 
    ///(������������ ��� ������ �������� ��������. ���� ������������ ����� � ������������ �������.
    /// ��� �������, ������� ��� ������ ������� ������������, ����� ��������� �� �������, ���������� ��� �����)
    /// </summary>
    /// <param name="insertedNodeIndex">������ ������������ ����</param>
    void CheckIndexesAfterInsertingNodeWithIndex(int insertedNodeIndex);
}
