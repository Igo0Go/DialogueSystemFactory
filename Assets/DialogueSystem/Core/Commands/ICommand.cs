/// <summary>
/// ������� ��������� ��� ���� �������, ��������������� ������ ���������� ������� � ������
/// </summary>
public interface ICommand
{
    void Execute();
    void Undo();
}
