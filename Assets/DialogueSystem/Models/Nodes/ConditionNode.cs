using UnityEngine;

/// <summary>
/// ”зел-условие
/// </summary>
[System.Serializable]
public class ConditionNode : DialogueNode
{
    #region ѕол€ и свойства

    /// <summary>
    /// ѕакет уловий (создаЄтс€ отдельно)
    /// </summary>
    public ParameterPack parameter;

    /// <summary>
    /// номер провер€емого услови€ (из сиска условий в пакете)
    /// </summary>
    public int conditionNumber;

    /// <summary>
    /// тип проверки услови€ (зависит от типа услови€)
    /// </summary>
    public CheckType checkType;

    /// <summary>
    /// ÷елевое значение дл€ сравнени€ bool
    /// </summary>
    public bool checkBoolValue;

    /// <summary>
    /// ÷елевое значение дл€ сравнени€ int
    /// </summary>
    public int checkIntValue;

    /// <summary>
    /// ссылка на следующий узел дл€ позитивного исхода
    /// </summary>
    public int PositiveNextNumber
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
    /// ссылка на следующий узел дл€ негативного исхода
    /// </summary>
    public int NegativeNextNumber
    {
        get
        {
            return nextNodesNumbers[1];
        }
        set
        {
            nextNodesNumbers[1] = value;
        }
    }

    public readonly Vector2 positiveExitPointOffset = new Vector2(150, 21);
    public readonly Vector2 negativeExitPointOffset = new Vector2(150, 42);

    #endregion

    #region  онструкторы

    /// <summary>
    /// —оздать дилоговый узел с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позици€ узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public ConditionNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 180, 65);
        //        _rightPointOffset = new Vector3(160, 21);
        colorInEditor = Color.cyan;
        nextNodesNumbers.Add(-1);
        nextNodesNumbers.Add(-1);
    }

    public ConditionNode(){}

    #endregion
}
