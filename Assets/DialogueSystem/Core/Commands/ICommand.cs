/// <summary>
/// Ѕазовый интерфейс дл€ всех комманд, предоставл€ющий методы выполнени€ команды и отмены
/// </summary>
public interface ICommand
{
    void Execute();
    void Undo();
}
