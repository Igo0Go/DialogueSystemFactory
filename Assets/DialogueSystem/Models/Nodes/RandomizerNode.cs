using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ”зел-рандомизатор
/// </summary>
public class RandomizerNode : DialogueNode
{
    /// <summary>
    /// смещени€ выходов списка узлов случайного выбора
    /// </summary>
    public List<Vector2> exitPointsOffsetList;

    /// <summary>
    /// ”дал€ть ли ссылку на узел после случайного выбора
    /// </summary>
    public bool withRemoving;

    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

    /// <summary>
    /// —сылка на узел по умолчанию
    /// </summary>
    public int defaultNextNodeNumber
    {
        get
        {
            return nextNodesNumbers[0];
        }
        set
        {
            nextNodesNumbers[0] = value;
        }
    }

    /// <summary>
    /// —оздать узел-рандомизатор с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позици€ узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public RandomizerNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 65);
        colorInEditor = Color.magenta;
        exitPointsOffsetList = new List<Vector2>();
        nextNodesNumbers.Add(-1);
    }

    protected RandomizerNode() { }

    /// <summary>
    /// ƒобавить узел в список дл€ случайного выбора
    /// </summary>
    /// <param name="index">индекс узла</param>
    public void AddLinkNumber(int index)
    {
        nextNodesNumbers.Add(index);
        CheckExitOffsetForRandomLink();
    }

    /// <summary>
    /// ”далить узел из списка дл€ случайного выбора
    /// </summary>
    /// <param name="numberInList">индекс узла</param>
    public void RemoveLinkNumber(int numberInList)
    {
        nextNodesNumbers.RemoveAt(numberInList);
        CheckExitOffsetForRandomLink();
    }

    /// <summary>
    /// ѕолучить следующий случайный узел или получить узел по умолчанию, если список случайного выбора пуст
    /// </summary>
    /// <returns>»ндекс следующего узла</returns>
    public int GetNextLink()
    {
        if (nextNodesNumbers.Count == 1)
        {
            return defaultNextNodeNumber;
        }

        int resultPositionInList = Random.Range(1, nextNodesNumbers.Count - 1);
        int resultIndex = nextNodesNumbers[resultPositionInList];
        nextNodesNumbers.RemoveAt(resultPositionInList);
        return resultIndex;
    }

    private void CheckExitOffsetForRandomLink()
    {
        exitPointsOffsetList.Clear();
        for (int i = 1; i < nextNodesNumbers.Count; i++)
        {
            exitPointsOffsetList.Add(exitPointOffset + new Vector2(0, (i-1) * 21));
        }
    }
}
[System.Serializable]
public class RandomLinlkPair
{
    /// <summary>
    /// —сылка на следующий узел
    /// </summary>
    public int nextNodeIndex;

    /// <summary>
    /// ƒоступность следущего узла
    /// </summary>
    public bool access;

    /// <summary>
    /// —оздать пару (узел, доступ). ѕо умолчанию узел будет доступен
    /// </summary>
    /// <param name="linkIndex">»ндекс узла</param>
    public RandomLinlkPair(int linkIndex)
    {
        nextNodeIndex = linkIndex;
        access = true;
    }
}
