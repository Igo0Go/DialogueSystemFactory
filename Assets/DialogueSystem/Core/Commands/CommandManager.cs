using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс-хранилище всех команд в формате стека.
/// </summary>
public static class CommandManager
{
    public static Vector2 dragBufer;
    public static DialogueSceneKit sceneKit;
    public static List<Connection> connections;
    public static Stack<ICommand> commandHistory = new Stack<ICommand>();

    /// <summary>
    /// Добавить команду и сразу выполнить её
    /// </summary>
    /// <param name="command"></param>
    public static void AddCommandAndExecute(ICommand command)
    {
        commandHistory.Push(command);
        command.Execute();
    }

    /// <summary>
    /// Добавить команду без выполнения
    /// (для команд, которые фактически выполняются продолжительно по времени.
    /// Сюда записывается только итоговое преобразование. Пример: команда перемещения.
    /// Фактически мы двигаем её мелкими рывками, но запишем команду полного перемещения, когда отпустим кнопку)
    /// </summary>
    /// <param name="command"></param>
    public static void AddCommand(ICommand command)
    {
        commandHistory.Push(command);
    }

    /// <summary>
    /// Отменить последнюю команду и сразу удалить её из стека
    /// </summary>
    public static void Undo()
    {
        if(commandHistory.Count > 0)
        {
            ICommand command = commandHistory.Peek();
            command.Undo();
            commandHistory.Pop();
        }
    }
}
