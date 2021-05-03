using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventReactorSample : DialogueEventReactor
{
    [SerializeField]
    private string message;

    public override void OnEvent()
    {
        Debug.Log(message);
    }
}
