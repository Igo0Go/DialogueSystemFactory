using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnswerUI : MonoBehaviour
{
    [Tooltip("Текст варианта")] public Text variantText;

    private int answerNumber = 0;
    private UnityEvent<int> TakeAnswerEvent = new UnityEvent<int>();

    public void PrepareAnswer(AnswerItem item, int number, DialogueScenePoint scenePoint)
    {
        variantText.text = item.answerTip;
        variantText.color = item.character.color;
        answerNumber = number;
        TakeAnswerEvent.AddListener(scenePoint.UseAnswer);
    }

    public void OnButtonClick()
    {
        TakeAnswerEvent?.Invoke(answerNumber);
    }

    public void PrepareToDestroy()
    {
        TakeAnswerEvent.RemoveAllListeners();
        Destroy(gameObject, Time.deltaTime);
    }
}
