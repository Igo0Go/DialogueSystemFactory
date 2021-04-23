using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Узел - выбор
/// </summary>
public class ChoiceNode : DialogueNode
{
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

    private readonly int answerLimit = 3;
    private readonly Vector2 exitOffset = new Vector3(180, 21);

    /// <summary>
    /// Создать узел выбора с указанным индексом
    /// </summary>
    /// <param name="index">индекс узла в схеме</param>
    public ChoiceNode(int index) : base(index)
    {
        transformRect = new Rect(0, 0, 200, 50);
        colorInEditor = Color.grey;
        exitPointOffsetList = new List<Vector2>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    /// <summary>
    /// Создать узел выбора с указанным индексом в указанной позиции
    /// </summary>
    /// <param name="pos">позиция узла в координатах схемы</param>
    /// <param name="index">индекс узла в схеме</param>
    public ChoiceNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 200, 50);
        colorInEditor = Color.grey;
        exitPointOffsetList = new List<Vector2>();
        for (int i = 0; i < answerLimit; i++)
        {
            nextNodesNumbers.Add(-1);
        }
    }

    private ChoiceNode(){}

    /// <summary>
    /// Добавить заготовку для нового варианта ответа
    /// </summary>
    public void AddAnswer()
    {
        if(answers.Count < answerLimit)
        {
            answers.Add(new AnswerItem());
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
}

/// <summary>
/// Пакет информации о варианте ответа
/// </summary>
public class AnswerItem
{
    /// <summary>
    /// Текст, который будет выводиться на кнопке выбора реплики (может отличаться от реальной реплики, к примеру, для сокращения)
    /// </summary>
    public string answerTip;

    /// <summary>
    /// Реальная реплика ответа
    /// </summary>
    public ReplicInfo answerReplica;

    public AnswerItem()
    {
        answerReplica = new ReplicInfo();

    }
}
