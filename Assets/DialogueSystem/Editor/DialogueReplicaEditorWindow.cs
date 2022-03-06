using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueReplicaEditorWindow : EditorWindow
{
    public ReplicInfo replica;
    public DialogueSceneKit kit;
    private Vector2 verticalScrollPosition;

    private GUIStyle style;

    public static DialogueReplicaEditorWindow GetReplicaWindow(ReplicInfo replica, DialogueSceneKit sceneKit)
    {
        var window = GetWindow<DialogueReplicaEditorWindow>();
        window.replica = replica;
        window.kit = sceneKit;
        return window;
    }

    private void OnGUI()
    {
        DrawReplica();
    }

    private void DrawReplica()
    {
        if(style == null)
        {
            style = new GUIStyle(new GUISkin().textArea);
            style.normal.textColor = Color.gray;
            style.hover.textColor = Color.white;
            style.focused.textColor = Color.white;
            style.wordWrap = true;
        }

        EditorGUILayout.BeginVertical();
        verticalScrollPosition = EditorGUILayout.BeginScrollView(verticalScrollPosition);

        replica.character = (DialogueCharacter)EditorGUILayout.ObjectField(replica.character, typeof(DialogueCharacter), allowSceneObjects: true);
        replica.replicaText = EditorGUILayout.TextArea(replica.replicaText, style, GUILayout.MinHeight(50), GUILayout.MaxWidth(position.width - 10));
        replica.clip = (AudioClip)EditorGUILayout.ObjectField(replica.clip, typeof(AudioClip), allowSceneObjects: true);
        replica.animType = (DialogueAnimType)EditorGUILayout.EnumPopup(replica.animType, GUILayout.MinWidth(80), GUILayout.MinHeight(20));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ракурс:");
        replica.camPositionNumber = EditorGUILayout.Popup(replica.camPositionNumber, kit.camerasPositions.ToArray());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
