using UnityEngine;

/// <summary>
/// Узел-условие
/// </summary>
[System.Serializable]
public class ConditionNode : DialogueNode
{
    #region Поля и свойства

    /// <summary>
    /// Цвет подложки узла
    /// </summary>
    public override Color СolorInEditor
    {
        get
        {
            foreach (var item in nextNodesNumbers)
            {
                if (item == -1 && !finalNode)
                    return Color.red;
            }
            return Color.cyan;
        }
    }

    /// <summary>
    /// Набор для работы с параметром
    /// </summary>
    public ConditionItem conditionItem;

    /// <summary>
    /// ссылка на следующий узел для позитивного исхода
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
    /// ссылка на следующий узел для негативного исхода
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

    #region Конструкторы

    /// <summary>
    /// Создать дилоговый узел с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public ConditionNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 180, 65);
        //        _rightPointOffset = new Vector3(160, 21);
        nextNodesNumbers.Add(-1);
        nextNodesNumbers.Add(-1);
    }

    public ConditionNode(){}

    #endregion
}

[System.Serializable]
public class ConditionItem
{
    /// <summary>
    /// Пакет уловий (создаётся отдельно)
    /// </summary>
    public ParameterPack parameter;

    /// <summary>
    /// номер проверяемого условия (из сиска условий в пакете)
    /// </summary>
    public int conditionNumber;

    /// <summary>
    /// тип проверки условия (зависит от типа условия)
    /// </summary>
    public CheckType checkType;

    /// <summary>
    /// Целевое значение для сравнения bool
    /// </summary>
    public bool checkBoolValue;

    /// <summary>
    /// Целевое значение для сравнения int
    /// </summary>
    public int checkIntValue;

    public bool CheckCondition()
    {
        bool result = false;

        if (parameter.GetType(conditionNumber, out ParameterType type) && type == ParameterType.Bool)
        {
            if (parameter.Check(conditionNumber, checkType, checkBoolValue))
            {
                result = true;
            }
        }
        else
        {
            if (parameter.GetType(conditionNumber, out type) && type == ParameterType.Int)
            {
                if (parameter.Check(conditionNumber, checkType, checkIntValue))
                {
                    result = true;
                }
            }
        }


        return result;
    }
}
