using System.Collections.Generic;
using UnityEngine;

public static class CommandManager
{
    public static Vector2 dragBufer;
    public static DialogueSceneKit sceneKit;
    public static List<Connection> connections;
    public static Stack<ICommand> commandHistory = new Stack<ICommand>();

    public static void AddCommandAndExecute(ICommand command)
    {
        commandHistory.Push(command);
        command.Execute();
    }

    public static void AddCommand(ICommand command)
    {
        commandHistory.Push(command);
    }

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
