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
        verticalScrollPosition = EditorGUILayout.BeginScrollView(verticalScrollPosition);

        choiceNode.character = (DialogueCharacter)EditorGUILayout.ObjectField(choiceNode.character, typeof(DialogueCharacter),
            allowSceneObjects: true);


        EditorGUILayout.LabelField("Ракурс:");
        choiceNode.defaultCameraPositionIndex = EditorGUILayout.Popup(choiceNode.defaultCameraPositionIndex,
            kit.camerasPositions.ToArray());

        if (choiceNode.character != null)
        {
            for (int i = 0; i < choiceNode.answers.Count; i++)
            {
                if (choiceNode.answers[i].answerStats == null ||
                    choiceNode.character.characterStats.Count != choiceNode.answers[i].answerStats.Count)
                {
                    choiceNode.answers[i].answerStats = new List<StatItem>();
                    for (int j = 0; j < choiceNode.character.characterStats.Count; j++)
                    {
                        choiceNode.answers[i].answerStats.Add(new StatItem(0));
                    }
                }

                EditorGUILayout.BeginHorizontal();
                choiceNode.answers[i].answerTip = EditorGUILayout.TextField(choiceNode.answers[i].answerTip);
                if (i > 0)
                {
                    choiceNode.answers[i].useAutoChoiseForThisAnswer = EditorGUILayout.ToggleLeft("Автовыбор",
                        choiceNode.answers[i].useAutoChoiseForThisAnswer);
                }
                EditorGUILayout.EndHorizontal();

                if(choiceNode.answers[i].useAutoChoiseForThisAnswer)
                {
                    for (int j = 0; j < choiceNode.character.characterStats.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(choiceNode.character.characterStats[j].statName);
                        choiceNode.answers[i].answerStats[j].mode =
                            (AnswerStatMode)EditorGUILayout.EnumPopup(choiceNode.answers[i].answerStats[j].mode);

                        if (choiceNode.answers[i].answerStats[j].mode > 0)
                        {
                            choiceNode.answers[i].answerStats[j].value =
                                EditorGUILayout.Slider(choiceNode.answers[i].answerStats[j].value, -100, 100);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space(10);
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
