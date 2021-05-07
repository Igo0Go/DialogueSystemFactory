using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Color color;

    public List<CharacterStat> characterStats;
}

[System.Serializable]
public class CharacterStat
{
    public string statName;
    [Range(-100,100)]
    public float statValue;
}
