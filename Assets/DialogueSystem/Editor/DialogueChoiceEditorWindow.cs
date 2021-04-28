using UnityEngine;
using UnityEditor;

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

        choiceNode.character = (DialogueCharacter)EditorGUILayout.ObjectField(choiceNode.character, typeof(DialogueCharacter), allowSceneObjects: true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ракурс:");
        choiceNode.defaultCameraPositionIndex = EditorGUILayout.Popup(choiceNode.defaultCameraPositionIndex, kit.camerasPositions.ToArray());
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < choiceNode.answers.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                choiceNode.RemoveAnsver(i);
                EditorUtility.SetDirty(kit);
                break;
            }
            choiceNode.answers[i].answerReplica.replicaText = EditorGUILayout.TextField(choiceNode.answers[i].answerReplica.replicaText);
            if (GUILayout.Button("="))
            {
                DialogueReplicaEditorWindow.GetReplicaWindow(choiceNode.answers[i].answerReplica, kit).Show();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
