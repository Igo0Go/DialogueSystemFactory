using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ”зел - выбор
/// </summary>
[System.Serializable]
public class ChoiceNode : DialogueNode
{
    #region ѕол€
    /// <summary>
    /// –олевой пакет выбирающего
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// —мещени€ выходов
    /// </summary>
    public List<Vector2> exitPointOffsetList;

    /// <summary>
    /// ¬арианты ответа
    /// </summary>
    public List<AnswerItem> answers;

    /// <summary>
    /// »ндекс позиции камеры в сцене
    /// </summary>
    public int defaultCameraPositionIndex;

    /// <summary>
    /// »спользовать ли идеальные характеристики персонажа дл€ автовыбора ответа
    /// </summary>
    public bool useStats = false;

    public readonly int answerLimit = 20;
    private readonly Vector2 exitOffset = new Vector3(180, 21);
    #endregion

    #region  онструкторы
    /// <summary>
    /// —оздать узел выбора с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позици€ узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public ChoiceNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 200, 50);
        colorInEditor = Color.grey;
        exitPointOffsetList = new List<Vector2>();
        answers = new List<AnswerItem>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    private ChoiceNode(){}
    #endregion

    #region ћетоды
    /// <summary>
    /// ƒобавить заготовку дл€ нового варианта ответа
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
        nextNodesNumbers[number] = -1;
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
/// ѕакет информации о варианте ответа
/// </summary>
[System.Serializable]
public class AnswerItem
{
    /// <summary>
    /// “екст, который будет выводитьс€ на кнопке выбора реплики (может отличатьс€ от реальной реплики, к примеру, дл€ сокращени€)
    /// </summary>
    public string answerTip;

    /// <summary>
    /// –оль говор€щего
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// ƒл€ каждого ответа можно задать идеальные характеристики, которые берутьс€ из характеристик выбирающего. ¬ случае работы бота
    /// выбор будет сделан в пользу того ответа, к идеальным характеристикам которого персонаж ближе
    /// </summary>
    public List<float> answerStats;

    public AnswerItem(DialogueCharacter dialogueCharacter)
    {
        character = dialogueCharacter;
    }
}
