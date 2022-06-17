using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����-��������� ���� ������ � ������� �����.
/// </summary>
public static class CommandManager
{
    public static Vector2 dragBufer;
    public static DialogueSceneKit sceneKit;
    public static List<Connection> connections;
    public static Stack<ICommand> commandHistory = new Stack<ICommand>();

    /// <summary>
    /// �������� ������� � ����� ��������� �
    /// </summary>
    /// <param name="command"></param>
    public static void AddCommandAndExecute(ICommand command)
    {
        commandHistory.Push(command);
        command.Execute();
    }

    /// <summary>
    /// �������� ������� ��� ����������
    /// (��� ������, ������� ���������� ����������� �������������� �� �������.
    /// ���� ������������ ������ �������� ��������������. ������: ������� �����������.
    /// ���������� �� ������� � ������� �������, �� ������� ������� ������� �����������, ����� �������� ������)
    /// </summary>
    /// <param name="command"></param>
    public static void AddCommand(ICommand command)
    {
        commandHistory.Push(command);
    }

    /// <summary>
    /// �������� ��������� ������� � ����� ������� � �� �����
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
