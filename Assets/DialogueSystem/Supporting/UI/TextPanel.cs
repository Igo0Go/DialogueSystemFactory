using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TextPanel
{
    [Tooltip("Объект панели")]
    public GameObject panel;
    [Tooltip("Сам текст")]
    public Text contentText;
}
