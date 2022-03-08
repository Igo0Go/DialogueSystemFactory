using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Color color;
    [TextArea(10,100)]
    public string description;
    public List<CharacterStat> characterStats;

    /// <summary>
    /// Получить вариант ответа 
    /// </summary>
    /// <param name="answers">Варианты ответа</param>
    /// <returns>Выбранный вариант ответа</returns>
    public int GetAutoChoiceAnswerIndex(List<AnswerItem> answers)
    {
        float resultDistance = float.MaxValue;
        int resultIndex = 0;

        MultidimensionalPoint persPoint = new MultidimensionalPoint(characterStats);
        MultidimensionalPoint answerPoint;
        for (int i = 0; i < answers.Count; i++)
        {
            if(answers[i].answerMode == AnswerMode.AutoChoiсe)
            {
                if (!answers[i].answerStats.Exists(o => (int)o.mode > 1))
                {
                    answerPoint = new MultidimensionalPoint(answers[i], this);
                    float bufer = persPoint.GetDistance(answerPoint);
                    if (bufer < resultDistance)
                    {
                        resultDistance = bufer;
                        resultIndex = i;
                    }
                }
            }
        }
        answers[resultIndex].variantForAutoChoise = true;
        return resultIndex;
    }

    /// <summary>
    /// Получить список названий параметров персонажа
    /// </summary>
    /// <returns></returns>
    public string[] GetStatsName()
    {
        string[] result = new string[characterStats.Count];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = characterStats[i].statName;
        }

        return result;
    }
}

[System.Serializable]
public class CharacterStat
{
    public string statName;
    [Range(-100,100)]
    public float statValue;
}

public class MultidimensionalPoint
{
    List<float> coordinate;

    public MultidimensionalPoint(AnswerItem answer, DialogueCharacter dialogueCharacter)
    {
        coordinate = new List<float>();

        for (int i = 0; i < answer.answerStats.Count; i++)
        {
            if (answer.answerStats[i].mode == AnswerStatMode.Цель)
            {
                coordinate.Add(answer.answerStats[i].value);
            }
            else if (answer.answerStats[i].mode == AnswerStatMode.Игнорируется)
            {
                coordinate.Add(dialogueCharacter.characterStats[i].statValue);
            }
        }
    }
    public MultidimensionalPoint(List<CharacterStat> stats)
    {
        coordinate = new List<float>();
        foreach (var item in stats)
        {
            coordinate.Add(item.statValue);
        }
    }

    /// <summary>
    /// Найти расстояние от этой точки до другой в многомерном пространстве
    /// </summary>
    /// <param name="secondPoint">Точка, до которой нужно измерить расстояние</param>
    /// <returns>Расстояние до точки</returns>
    public float GetDistance(MultidimensionalPoint secondPoint)
    {
        float bufer = -1;
        if (secondPoint.coordinate.Count != coordinate.Count)
        {
            Debug.LogError("Размерности многомерных точек на совпадают");
        }
        else
        {
            bufer = 0;
            for (int i = 0; i < coordinate.Count; i++)
            {
                bufer += Mathf.Pow(coordinate[i] - secondPoint.coordinate[i], 2);
            }
            bufer = Mathf.Sqrt(bufer);
        }
        return bufer;
    }
}
