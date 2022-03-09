using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Узел - событие
/// </summary>
[System.Serializable]
public class EventNode : DialogueNode
{
    #region Основные поля и свойства
    public readonly Vector2 exitPointOffset = new Vector2(130, 21);

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
            return Color.yellow;
        }
    }

    public int NextNodeNumber
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
    #endregion

    #region Сообщение при событии
    /// <summary>
    /// выводить ли текстовое сообщение при событии
    /// </summary>
    public bool useMessage;

    /// <summary>
    /// текст сообщения при событии
    /// </summary>
    public string messageText;
    #endregion

    #region Изменение параметра
    /// <summary>
    /// Пакет параметров для изменения
    /// </summary>
    public ParameterPack parameter;

    /// <summary>
    /// менять ли значение какого-то параметра
    /// </summary>
    public bool changeParameter;

    /// <summary>
    /// на какое значение bool изменить параметр, если менять пакет условий
    /// </summary>
    public bool targetBoolValue;

    /// <summary>
    /// Значение int, которое будет добавлен/присвоен значению параметра, если менять пакет условий (может быть отрицательным)
    /// </summary>
    public int changeIntValue;

    /// <summary>
    /// Индекс изменяемого параметра в пакете
    /// </summary>
    public int changeingParameterIndex;

    /// <summary>
    /// Каую операцию проводить с параметром
    /// </summary>
    public OperationType parameterOperation;

    #endregion

    #region Событие в сцене
    /// <summary>
    /// пускать ли событие в ИГРОВУЮ сцену
    /// </summary>
    public bool inSceneInvoke;

    /// <summary>
    /// номера объектов в ИГРОВОЙ сцене, реагирующих на событие (здесь указываются только номера в списке)
    /// </summary>
    public List<int> reactorsNumbers;

    /// <summary>
    /// время задержки диалога при реакции объектов в ИГРОВОЙ сцене
    /// </summary>
    public float eventTime;

    /// <summary>
    /// ракурс камеры при фиксации на событии в ИГРОВОЙ сцене
    /// </summary>
    public int eventCamPositionNumber;
    #endregion

    #region Изменение пакета персонажа

    /// <summary>
    /// Пакет персанажа
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// Менять ли характеристики персонажа
    /// </summary>
    public bool changeCharacter;

    /// <summary>
    /// Каую операцию проводить с параметром
    /// </summary>
    public OperationType characterOperation;

    /// <summary>
    ///Какой параметр персонажа изменить 
    /// </summary>
    public int changeCharacterStatIndex;

    /// <summary>
    ///На какое значение изменить параметр персонажа 
    /// </summary>
    public int changeCharacterStatValue;
    #endregion

    #region Конструкторы
    /// <summary>
    /// Создать узел-событие с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public EventNode(Vector2 pos, int index) : base(pos, index)
    {
        reactorsNumbers = new List<int>();
        transformRect = new Rect(pos.x, pos.y, 150, 90);
        nextNodesNumbers.Add(-1);
    }

    protected EventNode() { }
    #endregion
}
