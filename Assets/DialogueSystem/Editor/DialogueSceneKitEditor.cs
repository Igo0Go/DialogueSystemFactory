using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueSceneKit))] 
public class DialogueSceneKitEditor : Editor 
{
    private DialogueSceneKit sceneKit;

    private Vector2 camScroll = Vector2.zero;
    private Vector2 eventScroll = Vector2.zero;

    private void OnEnable()
    {
        sceneKit = (DialogueSceneKit)target;
        sceneKit.CreateEditorCopy();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Название сцены: " + sceneKit.sceneName);
        GUI.color = Color.cyan;
        EditorGUILayout.LabelField("Количество узлов рабочей копии: " + sceneKit.Nodes.Count);
        if(sceneKit.savedNodes != null)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("Количество узлов хранилища: " + sceneKit.savedNodes.Count);
        }
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.cyan;
        if(GUILayout.Button("Редактировать", GUILayout.MinWidth(80)))
        {
            DialogueSceneEditor sceneEditor = DialogueSceneEditor.GetEditor();
            sceneEditor.sceneKit = sceneKit;
            sceneEditor.minSize = new Vector2(400, 300);
            sceneEditor.Show();
        }
        GUI.color = Color.yellow;
        if (GUILayout.Button("Сохранить", GUILayout.MinWidth(80)))
        {
            sceneKit.SaveAllNodes();
            EditorUtility.SetDirty(sceneKit);
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUI.color = Color.red;
        if (GUILayout.Button("Загрузить", GUILayout.MinWidth(80)))
        {
            sceneKit.LoadAllNodes();
            EditorUtility.SetDirty(sceneKit);
        }
        GUI.color = Color.magenta;
        if (GUILayout.Button("Восстановить", GUILayout.MinWidth(80)))
        {
            sceneKit.RepairSavedNodes();
            EditorUtility.SetDirty(sceneKit);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUI.color = Color.white;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Ракурсы камеры:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.CreateCameraPoint();
        }
        GUILayout.EndHorizontal();
        camScroll = GUILayout.BeginScrollView(camScroll);
        if (sceneKit.camerasPositions.Count > 0)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sceneKit.camerasPositions.Count; i++)
            {
                sceneKit.camerasPositions[i] = GUILayout.TextField(sceneKit.camerasPositions[i],
                    GUILayout.MaxWidth(100), GUILayout.MinWidth(80));

                if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                {
                    sceneKit.camerasPositions.Remove(sceneKit.camerasPositions[i]);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("События в игровой сцене:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.inSceneInvokeObjects.Add("Новое событие " + (sceneKit.inSceneInvokeObjects.Count + 1));
        }
        GUILayout.EndHorizontal();
        eventScroll = GUILayout.BeginScrollView(eventScroll);
        if (sceneKit.inSceneInvokeObjects.Count > 0)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sceneKit.inSceneInvokeObjects.Count; i++)
            {
                sceneKit.inSceneInvokeObjects[i] = GUILayout.TextField(sceneKit.inSceneInvokeObjects[i],
                    GUILayout.MaxWidth(100), GUILayout.MinWidth(80));

                if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                {
                    sceneKit.inSceneInvokeObjects.Remove(sceneKit.inSceneInvokeObjects[i]);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}

