using UnityEngine;

[CreateAssetMenu(menuName = "IgoGoTools/DialogueSceneKit")]
public class DialogueSceneKit : ScriptableObject
{
    [ContextMenu("������� ��������")]
    public void ShowEditor()
    {
        Debug.Log("�������� ������");
    }
}
