using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButtonReactor : MonoBehaviour
{
    [Range(0,2)]
    public int answerNumber = 0;

   private Button answerButton;
    public event System.Action<int> TakeAnswerEvent;

    private void Awake()
    {
        answerButton = GetComponent<Button>();
    }

    public void OnButtonClick()
    {
        TakeAnswerEvent?.Invoke(answerNumber);
    }
}
