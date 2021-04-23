using UnityEngine;
using System;

public class DialogueTeamDirector : MonoBehaviour
{
    [HideInInspector] public bool inDialog;

    public event Action<int> OnChooseAnswer = null;

    
    public void ReturnFromDialogue()
    {
        //в данном методе прописывается логика для возвращения всего игрового процесса к обычному геймплею
    }

    
    public void SetToDialogue()
    {
        //в данном методе прописывается логика для перевода геймплея в режим диалога
    }
}