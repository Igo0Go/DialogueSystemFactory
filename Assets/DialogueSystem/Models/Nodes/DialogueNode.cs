using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Узел диалоговой схемы
/// </summary>
[System.Serializable]
public class DialogueNode
{
    #region Поля и свойства

    public Vector2 InverseEnterPointOffset => new Vector2(transformRect.width - 21, enterPointOffset.y);

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

    /// <summary>
    /// Отрисовка слева направа
    /// </summary>
    public bool leftToRight;

    /// <summary>
    /// Прямоугольник, в котором отрисовывается узел
    /// </summary>
    public Rect transformRect;

    /// <summary>
    /// Цвет подложки узла
    /// </summary>
    public virtual Color СolorInEditor
    {
        get
        {
            foreach (var item in nextNodesNumbers)
            {
                if (item == -1 && !finalNode)
                    return Color.red;
            }
            return Color.gray;
        }
    }

    /// <summary>
    /// Смещение входа
    /// </summary>
    public readonly Vector2 enterPointOffset = new Vector2(0, 21);
    #endregion

    #region Конструкторы
    /// <summary>
    /// Создать дилоговый узел с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public DialogueNode(Vector2 pos, int index)
    {
        leftToRight = true;
        this.index = index;
        previousNodesNumbers = new List<int>();
        nextNodesNumbers = new List<int>();
    }

    protected DialogueNode() { }
    #endregion

    #region Методы

    /// <summary>
    /// Удалить указанный узел из списка следующих
    /// </summary>
    public void RemoveThisNodeFromNext(DialogueNode nodeForRemoving)
    {
        for (int i = 0; i < nextNodesNumbers.Count; i++)
        {
            if(nextNodesNumbers[i] == nodeForRemoving.index)
            {
                nextNodesNumbers[i] = -1;
            }
        }
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
        if(index > removedIndex)
        {
            index--;
        }
        for (int i = 0; i < previousNodesNumbers.Count; i++)
        {
            if(previousNodesNumbers[i] > removedIndex)
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
}
