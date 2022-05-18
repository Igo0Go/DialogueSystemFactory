using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class DialogueNode
{
    #region Поля и свойства

    #region Логика

    /// <summary>
    /// Номер в схеме диалога
    /// </summary>
    public int index;

    /// <summary>
    /// Индексы предыдущих узлов
    /// </summary>
    public List<int> previousNodesNumbers;

    /// <summary>
    /// Индексы следующих узлов
    /// </summary>
    public List<int> nextNodesNumbers;

    /// <summary>
    /// Этот узел заканчивает диалог/группу
    /// </summary>
    public bool finalNode;

    #endregion

    #region Отрисовка
    /// <summary>
    /// Прямоугольник, в котором отрисовывается узел
    /// </summary>
    public Rect rect;

    private bool isSelected;
    private bool isDragged;

    public Action<DialogueNode> OnRemoveNode;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    #endregion

    #endregion

    #region Конструкторы
    /// <summary>
    /// Создать дилоговый узел с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public DialogueNode(Vector2 position, int index, GUIStyle defaultStyle, GUIStyle selectedStyle,
        Action<DialogueNode> onRemove)
    {
        this.index = index;
        previousNodesNumbers = new List<int>();
        nextNodesNumbers = new List<int>();

        rect = new Rect(position.x, position.y, 110, 40);
        defaultNodeStyle = style = defaultStyle;
        selectedNodeStyle = selectedStyle;

        OnRemoveNode = onRemove;
    }
    #endregion

    #region Методы

    #region Логика

    /// <summary>
    /// Удалить указанный узел из списка следующих
    /// </summary>
    public void RemoveThisNodeFromNext(DialogueNode nodeForRemoving)
    {
        nextNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// Удалить указанный узел из списка предыдущих
    /// </summary>
    /// <param name="nodeForRemoving">удаляемый узел</param>
    public void RemoveThisNodeFromPrevious(DialogueNode nodeForRemoving)
    {
        previousNodesNumbers.RemoveAll(item => item == nodeForRemoving.index);
    }

    /// <summary>
    /// корректировка номеров узлов после удаления
    /// </summary>
    /// <param name="removedIndex">индекс только что удалённого узла</param>
    public void CheckIndexes(int removedIndex)
    {
        if (index > removedIndex)
        {
            index--;
        }
        for (int i = 0; i < previousNodesNumbers.Count; i++)
        {
            if (previousNodesNumbers[i] > removedIndex)
            {
                previousNodesNumbers[i]--;
            }
        }
        for (int i = 0; i < nextNodesNumbers.Count; i++)
        {
            if (nextNodesNumbers[i] > removedIndex)
            {
                nextNodesNumbers[i]--;
            }
        }
    }

    #endregion

    #region Отрисовка

    public void Draw()
    {
        GUI.Box(rect, "", style);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    #endregion

    #region Обработка событий

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        OnRemoveNode?.Invoke(this);
    }

    #endregion

    #endregion
}
