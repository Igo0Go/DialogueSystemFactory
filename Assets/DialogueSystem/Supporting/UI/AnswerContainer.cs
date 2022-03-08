using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerContainer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Объект, который будет являться родительским для всех кнопок-выборов, которые будут создаваться динамически." +
        " Стандартно - объект Content.")]
    private Transform answerContainerContent;

    [SerializeField]
    [Tooltip("Объект, который содержит весь UI, связанный с вариантами ответа: поле для прокрутки и т.д.")]
    private GameObject answerContainerObject;

    [SerializeField]
    [Tooltip("Префаб кнопки для выбора варианта. Будет использоваться, чтобы сгенерировать список вариантов.")]
    private GameObject answerUIprefab;

    private List<AnswerUI> answers;

    public void PrepareAllAnswers(List<AnswerItem> answerItems, DialogueScenePoint point, DialogueCharacter character)
    {
        ClearAllAnswers();
        answerContainerObject.SetActive(true);
        answers = new List<AnswerUI>();
        for (int i = 0; i < answerItems.Count; i++)
        {
            if(AnserIsAvailable(answerItems[i], character))
            {
                answers.Add(Instantiate(answerUIprefab, answerContainerContent).GetComponent<AnswerUI>());
                answers[answers.Count-1].PrepareAnswer(answerItems[i], i, point);
            }
        }
    }

    public void ClearAllAnswers()
    {
        if(answers != null)
        {
            foreach (var item in answers)
            {
                item.PrepareToDestroy();
            }

            answers.Clear();
        }
        answerContainerObject.SetActive(false);
    }

    private bool AnserIsAvailable(AnswerItem answerItem, DialogueCharacter character)
    {
        bool result = true;

        for (int i = 0; i < answerItem.answerStats.Count; i++)
        {
            if(answerItem.answerMode == AnswerMode.AutoChoiсe)
            {
                switch (answerItem.answerStats[i].mode)
                {
                    case AnswerStatMode.Игнорируется:
                        break;
                    case AnswerStatMode.Цель:
                        break;
                    case AnswerStatMode.БольшеИлиРавно:
                        result = character.characterStats[i].statValue >= answerItem.answerStats[i].value;
                        break;
                    case AnswerStatMode.МеньшеИлиРавно:
                        result = character.characterStats[i].statValue <= answerItem.answerStats[i].value;
                        break;
                    case AnswerStatMode.Больше:
                        result = character.characterStats[i].statValue > answerItem.answerStats[i].value;
                        break;
                    case AnswerStatMode.Меньше:
                        result = character.characterStats[i].statValue < answerItem.answerStats[i].value;
                        break;
                    default:
                        break;
                }
            }
            else if(answerItem.answerMode == AnswerMode.Condition)
            {
                result = answerItem.conditionItem.CheckCondition();
            }

            if (!result)
                return result;
        }

        return result;
    }
}
