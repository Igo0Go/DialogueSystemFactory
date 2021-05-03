using System;
using UnityEngine;

[Serializable]
public class DialogueActorPointItem
{
    [Tooltip("Позиция персонажа в диалоге")] public Transform actorPoint;
    [Tooltip("Роль персонажа в диалоге")] public DialogueCharacter actorRole;

    public DialogueActorPointItem(Transform point)
    {
        actorPoint = point;
    }
}
