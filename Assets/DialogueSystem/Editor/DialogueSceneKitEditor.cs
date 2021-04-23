using System.Collections;
using System.Collections.Generic;
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
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        sceneKit.sceneName=  GUILayout.TextField(sceneKit.sceneName);
        GUILayout.Label("Количество узлов: " + sceneKit.nodes.Count);
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Редактировать", GUILayout.MinWidth(80)))
        {
            DialogueSceneEditor sceneEditor = DialogueSceneEditor.GetEditor();
            sceneEditor.sceneKit = sceneKit;
            sceneEditor.minSize = new Vector2(400, 300);
            sceneEditor.Show();
        }
        if (GUILayout.Button("Сохранить", GUILayout.MinWidth(80)))
        {
            EditorUtility.SetDirty(sceneKit);
        }
        GUILayout.Space(80);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Ракурсы камеры:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.camerasPositions.Add("Новый ракурс " + (sceneKit.camerasPositions.Count + 1));
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

