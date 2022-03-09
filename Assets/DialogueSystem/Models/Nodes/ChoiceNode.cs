using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Узел - выбор
/// </summary>
[System.Serializable]
public class ChoiceNode : DialogueNode
{
    #region Поля и свойства

    /// <summary>
    /// Цвет подложки узла
    /// </summary>
    public override Color СolorInEditor
    {
        get
        {
            for (int i = 0; i < answers.Count; i++)
            {
                if (nextNodesNumbers[i] == -1)
                    return Color.red;
            }
            return Color.gray;
        }
    }

    /// <summary>
    /// Ролевой пакет выбирающего
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// Смещения выходов
    /// </summary>
    public List<Vector2> exitPointOffsetList;

    /// <summary>
    /// Варианты ответа
    /// </summary>
    public List<AnswerItem> answers;

    /// <summary>
    /// Индекс позиции камеры в сцене
    /// </summary>
    public int defaultCameraPositionIndex;

    public readonly int answerLimit = 20;
    private readonly Vector2 exitOffset = new Vector3(180, 21);
    #endregion

    #region Конструкторы
    /// <summary>
    /// Создать узел выбора с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public ChoiceNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 200, 50);
        exitPointOffsetList = new List<Vector2>();
        answers = new List<AnswerItem>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    private ChoiceNode(){}
    #endregion

    #region Методы
    /// <summary>
    /// Добавить заготовку для нового варианта ответа
    /// </summary>
    public void AddAnswer()
    {
        if(answers.Count < answerLimit)
        {
            answers.Add(new AnswerItem(character));
            CheckExitOffset();
        }
    }
    /// <summary>
    /// удалить вариант ответа по номеру
    /// </summary>
    /// <param name="number">номер варианта в списке ответов</param>
    public void RemoveAnsver(int number)
    {
        nextNodesNumbers.RemoveAt(number);
        nextNodesNumbers.Add(-1);
        answers.RemoveAt(number);
        CheckExitOffset();
    }
    /// <summary>
    /// изменить размеры отриовки узла в соответствии с количеством вариантов ответов
    /// </summary>
    private void CheckExitOffset()
    {
        exitPointOffsetList.Clear();
        for (int i = 0; i < answers.Count; i++)
        {
            exitPointOffsetList.Add(exitOffset + new Vector2(0, i * 21));
        }
    }
    #endregion
}

/// <summary>
/// Пакет информации о варианте ответа
/// </summary>
[System.Serializable]
public class AnswerItem
{
    /// <summary>
    /// Метка, показывающая, что данный вариант будет выбран системой автовыбора
    /// </summary>
    public bool variantForAutoChoise;

    /// <summary>
    /// Настройки для этого варианта ответа
    /// </summary>
    public AnswerMode answerMode = AnswerMode.AutoChoiсe;

    /// <summary>
    /// Текст, который будет выводиться на кнопке выбора реплики (может отличаться от реальной реплики, к примеру, для сокращения)
    /// </summary>
    public string answerTip;

    /// <summary>
    /// Роль говорящего
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// Пакет для работы с диалоговоым параметром
    /// </summary>
    public ConditionItem conditionItem;

    /// <summary>
    /// Для каждого ответа можно задать идеальные характеристики, которые беруться из характеристик выбирающего. В случае работы бота
    /// выбор будет сделан в пользу того ответа, к идеальным характеристикам которого персонаж ближе
    /// </summary>
    public List<StatItem> answerStats;

    public AnswerItem(DialogueCharacter dialogueCharacter)
    {
        character = dialogueCharacter;
    }
}

[System.Serializable]
public class StatItem
{
    public float value = 0;
    public AnswerStatMode mode;

    public StatItem(float value)
    {
        mode = AnswerStatMode.Цель;
        this.value = value;
    }
}

public enum AnswerMode
{
    Manual = 0,
    AutoChoiсe = 1,
    Condition = 2
}

public enum AnswerStatMode : int
{
    Игнорируется = 0,
    Цель = 1,
    БольшеИлиРавно = 2,
    МеньшеИлиРавно = 3,
    Больше = 4,
    Меньше = 5,
}
