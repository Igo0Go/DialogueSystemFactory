using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DialogueController : MonoBehaviour
{
    public Transform playerTransform;
    public DialogueCharacter dialogueCharacter;
    public bool useAutoChoice = false;

    protected Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Выставляет персонажа в позицию для начала диалога и передаёт в аниматор команду для перехода анимации к разговору
    /// </summary>
    /// <param name="positioning"></param>
    public virtual void ToDialoguePosition(Transform positioning)
    {
        playerTransform.position = positioning.position;
        playerTransform.rotation = positioning.rotation;
    }
    public virtual void ToDialogueAnimation()
    {
        anim.SetInteger("TalkType", 0);
        anim.SetInteger("TalkStatus", 1);
    }
    /// <summary>
    /// Возвращает аниматор к статусу управления игроком
    /// </summary>
    public virtual void ToDefault()
    {
        anim.SetInteger("TalkType", 0);
        anim.SetInteger("TalkStatus", 0);
    }
    /// <summary>
    /// Заставляет аниматор прекратить анимацию активного участия в диалоге и перейти к стандартной анимации персонажа в диалоге
    /// </summary>
    public void StopReplic()
    {
        anim.SetInteger("TalkType", 0);
        anim.SetInteger("TalkStatus", 1);
    }
    /// <summary>
    /// Задаёт тип анимации персонажу во время разговора
    /// </summary>
    /// <param name="type"></param>
    public void SetTalkType(DialogueAnimType type)
    {
        anim.SetInteger("TalkStatus", 2);
        anim.SetInteger("TalkType", InverseTypeToInt(type));
    }
    /// <summary>
    /// Переводит значения типа анимации в целочисленные значения для аниматора
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected int InverseTypeToInt(DialogueAnimType type)
    {
        switch (type)
        {
            case DialogueAnimType.Talk:
                return 1;
            case DialogueAnimType.Yes:
                return 2;
            case DialogueAnimType.No:
                return 3;
            case DialogueAnimType.Quastion:
                return 4;
            case DialogueAnimType.InSurprise:
                return 5;
            case DialogueAnimType.Nervously:
                return 6;
            case DialogueAnimType.Fear:
                return 7;
        }
        return 0;
    }
}
