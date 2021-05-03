using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Tooltip("������ ��� ���������")] public GameObject subsPanel;
    [Tooltip("����� ���������")] public Text subsText;
    [Tooltip("����� ��������")] public GameObject skipTip;
    [Tooltip("������-���������")] public GameObject tipPanel;
    [Tooltip("UI-����� ���������"), SerializeField] private Text tipText;
    [Tooltip("������ ��� ���������"), SerializeField] private UISlidePanel messagePanel;
    [Tooltip("����� ���������"), SerializeField] private Text messageText;
    [Tooltip("�������� ������")] public List<AnswerUI> answers = new List<AnswerUI>();



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
