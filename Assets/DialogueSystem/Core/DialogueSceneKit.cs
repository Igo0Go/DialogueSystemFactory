using UnityEngine;

[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    [ContextMenu("Открыть редактор")]
    public void ShowEditor()
    {
        Debug.Log("Редактор открыт");
    }
}
