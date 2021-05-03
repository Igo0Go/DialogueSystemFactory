using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Tooltip("Панель для субтитров")] public GameObject subsPanel;
    [Tooltip("Текст субтитров")] public Text subsText;
    [Tooltip("Текст пропуска")] public GameObject skipTip;
    [Tooltip("Панель-подсказка")] public GameObject tipPanel;
    [Tooltip("UI-Текст посдказки"), SerializeField] private Text tipText;
    [Tooltip("Панель для сообщения"), SerializeField] private UISlidePanel messagePanel;
    [Tooltip("Текст сообщения"), SerializeField] private Text messageText;
    [Tooltip("Варианты ответа")] public List<AnswerUI> answers = new List<AnswerUI>();



    private void Start()
    {
        skipTip.SetActive(false);
        subsPanel.SetActive(false);
        tipPanel.SetActive(false);
    }

    public void SetTipString(string tipString)
    {
        tipText.text = tipString;
    }

    public void PrepareSubs(ReplicInfo info)
    {
        subsText.color = info.character.color;
        subsText.text = info.replicaText;
    }
    public void UseMessage(string messageString)
    {
        messageText.text = messageString;
        messagePanel.OpenPanel();
    }
    public void HideMessage()
    {
        messagePanel.HidePanel();
    }
    public void CheckVariants(bool value)
    {
        foreach (var item in answers)
        {
            item.variantButton.SetActive(value);
        }
    }
    public void ShowVariants(AnswerUI answer, AnswerItem item)
    {
        ReplicInfo inf = item.answerReplica;
        if (!inf.replicaText.Equals(string.Empty))
        {
            answer.variantButton.SetActive(true);
            answer.variantText.color = inf.character.color;
            answer.variantText.text = inf.replicaText;
        }
    }
}
