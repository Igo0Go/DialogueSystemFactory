using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueSceneKit))]
public class DialogueSceneKitEditor : Editor
{
    private DialogueSceneKit sceneKit;

    private void OnEnable()
    {
        sceneKit = (DialogueSceneKit)target;
    }

    public override void OnInspectorGUI()
    {
        if(sceneKit.nodes != null)
        {
            GUILayout.Label("Количество узлов: " + sceneKit.nodes.Count.ToString());
        }


        GUI.color = Color.cyan;
        if (GUILayout.Button("Редактировать", GUILayout.MinWidth(80)))
        {
            DialogueSceneEditor sceneEditor = DialogueSceneEditor.GetEditor(sceneKit);
            sceneEditor.minSize = new Vector2(400, 300);
            sceneEditor.Show();
        }
        GUI.color = Color.magenta;
        if (GUILayout.Button("Сохранить", GUILayout.MinWidth(80)))
        {
            EditorUtility.SetDirty(sceneKit);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
