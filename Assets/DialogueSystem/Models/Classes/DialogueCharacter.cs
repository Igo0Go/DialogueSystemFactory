using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Color color;

    public List<CharacterStat> characterStats;

    /// <summary>
    /// Получить вариант ответа 
    /// </summary>
    /// <param name="answers">Варианты ответа</param>
    /// <returns>Выбранный вариант ответа</returns>
    public int GetAutoChoiceAnswerIndex(List<AnswerItem> answers)
    {
        float resultDistance = float.MaxValue;
        int resultIndex = -1;

        MultidimensionalPoint persPoint = new MultidimensionalPoint(characterStats);
        MultidimensionalPoint answerPoint;
        for (int i = 0; i < answers.Count; i++)
        {
            answerPoint = new MultidimensionalPoint(answers[i]);
            float bufer = persPoint.GetDistance(answerPoint);
            if (bufer < resultDistance)
            {
                resultDistance = bufer;
                resultIndex = i;
            }
        }
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

    public MultidimensionalPoint(AnswerItem answer)
    {
        coordinate = new List<float>();
        foreach (var item in answer.answerStats)
        {
            coordinate.Add(item);
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
