using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Tooltip("Панель для субтитров")] 
    [SerializeField]
    private TextPanel subsPanel;

    [Tooltip("Текст с именем говорящего")]
    [SerializeField]
    private TextPanel characterNamePanel;

    [Tooltip("Текст пропуска")]
    [SerializeField]
    private TextPanel skipPanel;

    [Tooltip("Панель-подсказка")]
    [SerializeField]
    private TextPanel infoTipPanel;

    [Tooltip("Панель для сообщения")]
    [SerializeField]
    private UISlidePanel messagePanel;

    [Tooltip("Текст сообщения")]
    [SerializeField]
    private Text messageText;

    [Tooltip("Варианты ответа")]
    public List<AnswerUI> answers = new List<AnswerUI>();

    [SerializeField]
    [Tooltip("Выводить ли имя говорящего во время реплик")]
    private bool showName = false;


    private void Start()
    {
        skipPanel.panel.SetActive(false);
        subsPanel.panel.SetActive(false);
        infoTipPanel.panel.SetActive(false);
        characterNamePanel.panel.SetActive(false);
    }

    public void SetInfoTipString(string tipString)
    {
        infoTipPanel.contentText.text = tipString;
    }

    public void PrepareSubs(ReplicInfo info)
    {
        subsPanel.panel.SetActive(true);

        subsPanel.contentText.color = info.character.color;
        subsPanel.contentText.text = info.replicaText;
        if(showName)
        {
            characterNamePanel.panel.SetActive(true);
            characterNamePanel.contentText.color = info.character.color;
            characterNamePanel.contentText.text = info.character.characterName;
        }
    }
    public void HideSubs()
    {
        subsPanel.panel.SetActive(false);
    }

    public void SetSkipTipState(bool value) => skipPanel.panel.SetActive(value);
    public void SetInfoTipState(bool value) => infoTipPanel.panel.SetActive(value);
    public void SetNamePanelState(bool value) => characterNamePanel.panel.SetActive(value);


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
