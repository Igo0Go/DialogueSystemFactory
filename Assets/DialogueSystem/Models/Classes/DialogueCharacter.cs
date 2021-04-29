﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Color color;
}