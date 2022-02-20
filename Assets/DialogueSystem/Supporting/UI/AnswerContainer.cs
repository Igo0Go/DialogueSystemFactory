using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerContainer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("������, ������� ����� �������� ������������ ��� ���� ������-�������, ������� ����� ����������� �����������." +
        " ���������� - ������ Content.")]
    private Transform answerContainerContent;

    [SerializeField]
    [Tooltip("������, ������� �������� ���� UI, ��������� � ���������� ������: ���� ��� ��������� � �.�.")]
    private GameObject answerContainerObject;

    [SerializeField]
    [Tooltip("������ ������ ��� ������ ��������. ����� ��������������, ����� ������������� ������ ���������.")]
    private GameObject answerUIprefab;

    private List<AnswerUI> answers;

    public void PrepareAllAnswers(List<AnswerItem> answerItems, DialogueScenePoint point)
    {
        ClearAllAnswers();
        answerContainerObject.SetActive(true);
        answers = new List<AnswerUI>();
        for (int i = 0; i < answerItems.Count; i++)
        {
            answers.Add(Instantiate(answerUIprefab, answerContainerContent).GetComponent<AnswerUI>());
            answers[i].PrepareAnswer(answerItems[i], i, point);
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
}
