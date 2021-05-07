using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEventEditorWindow : EditorWindow
{
    public EventNode eventNode;
    public DialogueSceneKit kit;
    private Vector2 horizontalScrollPosition;

    public static DialogueEventEditorWindow GetEventWindow(EventNode node, DialogueSceneKit sceneKit)
    {
        var window = GetWindow<DialogueEventEditorWindow>();
        window.eventNode = node;
        window.kit = sceneKit;
        return window;
    }

    private void OnGUI()
    {
        DrawDialogueEvent();
    }

    private void DrawDialogueEvent()
    {
        EditorGUILayout.BeginVertical();
        eventNode.parameter = (ParameterPack)EditorGUILayout.ObjectField(eventNode.parameter, typeof(ParameterPack),
            allowSceneObjects: true);
        eventNode.character = (DialogueCharacter)EditorGUILayout.ObjectField(eventNode.character, typeof(DialogueCharacter),
            allowSceneObjects: true);

        if (eventNode.parameter != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Менять параметр");
            eventNode.changeParameter = EditorGUILayout.Toggle(eventNode.changeParameter);
            EditorGUILayout.EndHorizontal();

            if (eventNode.changeParameter)
            {
                EditorGUILayout.BeginHorizontal();
                eventNode.changeingParameterIndex = EditorGUILayout.Popup(eventNode.changeingParameterIndex,
                    eventNode.parameter.GetCharacteristic());

                if (eventNode.parameter.parametres[eventNode.changeingParameterIndex].type == ParameterType.Bool)
                {
                    EditorGUILayout.LabelField("Значение после события");
                    eventNode.targetBoolValue = EditorGUILayout.Toggle(eventNode.targetBoolValue);
                }
                else
                {
                    EditorGUILayout.LabelField("Сместить на");
                    eventNode.changeIntValue = EditorGUILayout.IntField(eventNode.changeIntValue);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Событие в игровой сцене");
        eventNode.inSceneInvoke = EditorGUILayout.Toggle(eventNode.inSceneInvoke);
        EditorGUILayout.EndHorizontal();

        if (eventNode.inSceneInvoke)
        {
            horizontalScrollPosition = EditorGUILayout.BeginScrollView(horizontalScrollPosition, GUILayout.MaxHeight(100));
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < eventNode.reactorsNumbers.Count; i++)
            {
                EditorGUILayout.BeginVertical();
                eventNode.reactorsNumbers[i] = EditorGUILayout.Popup(eventNode.reactorsNumbers[i],
                    kit.inSceneInvokeObjects.ToArray(),
             GUILayout.MinWidth(100));
                GUILayout.Space(3);
                if (GUILayout.Button("x", GUILayout.MaxWidth(20)))
                {
                    eventNode.reactorsNumbers.Remove(eventNode.reactorsNumbers[i]);
                    EditorUtility.SetDirty(kit);
                    break;
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
            {
                eventNode.reactorsNumbers.Add(0);
                EditorUtility.SetDirty(kit);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ракурс во время события:");
            eventNode.eventCamPositionNumber = EditorGUILayout.Popup(eventNode.eventCamPositionNumber,
                kit.camerasPositions.ToArray(), GUILayout.MinWidth(40));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Время фокусировки на событии");
            eventNode.eventTime = EditorGUILayout.Slider(eventNode.eventTime, 0, 120);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Сообщение при событии");
        eventNode.useMessage = EditorGUILayout.Toggle(eventNode.useMessage);
        EditorGUILayout.EndHorizontal();

        if (eventNode.useMessage)
        {
            eventNode.messageText = EditorGUILayout.TextArea(eventNode.messageText, GUILayout.MaxWidth(position.width - 10));
        }

        if (eventNode.character != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Менять параметр персонажа");
            eventNode.changeCharacter = EditorGUILayout.Toggle(eventNode.changeCharacter);
            EditorGUILayout.EndHorizontal();

            if (eventNode.changeCharacter)
            {
                EditorGUILayout.BeginHorizontal();
                eventNode.changeCharacterStatIndex = EditorGUILayout.Popup(eventNode.changeCharacterStatIndex,
                    eventNode.character.GetStatsName(), GUILayout.MinWidth(80));
                EditorGUILayout.LabelField("Сместить на", GUILayout.MaxWidth(80));
                eventNode.changeCharacterStatValue = EditorGUILayout.IntField(eventNode.changeCharacterStatValue);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();
    }

}
