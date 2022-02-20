using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueChoiceEditorWindow : EditorWindow
{
    public ChoiceNode choiceNode;
    public DialogueSceneKit kit;
    private DialogueNodeEditorWindow nodeEditorWindow;
    private Rect bufer;
    private Vector2 horizontalScrollPosition;
    private Vector2 verticalScrollPosition;

    public static DialogueChoiceEditorWindow GetReplicaWindow(ChoiceNode choice, DialogueSceneKit sceneKit)
    {
        var window = GetWindow<DialogueChoiceEditorWindow>();
        window.choiceNode = choice;
        window.kit = sceneKit;
        return window;
    }

    private void OnGUI()
    {
        DrawChoice();
    }

    private void DrawChoice()
    {
        EditorGUILayout.BeginVertical();
        verticalScrollPosition = EditorGUILayout.BeginScrollView(verticalScrollPosition);

        choiceNode.character = (DialogueCharacter)EditorGUILayout.ObjectField(choiceNode.character, typeof(DialogueCharacter),
            allowSceneObjects: true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ракурс:");
        choiceNode.defaultCameraPositionIndex = EditorGUILayout.Popup(choiceNode.defaultCameraPositionIndex,
            kit.camerasPositions.ToArray());
        EditorGUILayout.EndHorizontal();

        if(choiceNode.character != null)
        {
            choiceNode.useStats = EditorGUILayout.Toggle("Использовать автовыбор", choiceNode.useStats);
        }

        for (int i = 0; i < choiceNode.answers.Count; i++)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginVertical();
            if (choiceNode.character != null)
            {
                if (choiceNode.useStats)
                {
                    if(choiceNode.answers[i].answerStats == null ||
                        choiceNode.character.characterStats.Count != choiceNode.answers[i].answerStats.Count)
                    {
                        choiceNode.answers[i].answerStats = new List<float>();
                        for (int j = 0; j < choiceNode.character.characterStats.Count; j++)
                        {
                            choiceNode.answers[i].answerStats.Add(0);
                        }
                    }

                    for (int j = 0; j < choiceNode.character.characterStats.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(choiceNode.character.characterStats[j].statName);
                        choiceNode.answers[i].answerStats[j] = EditorGUILayout.Slider(choiceNode.answers[i].answerStats[j], -100, 100);
                        EditorGUILayout.EndHorizontal();
                    }
                }

            }
            EditorGUILayout.BeginHorizontal();
            choiceNode.answers[i].answerTip = EditorGUILayout.TextField(choiceNode.answers[i].answerTip);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
