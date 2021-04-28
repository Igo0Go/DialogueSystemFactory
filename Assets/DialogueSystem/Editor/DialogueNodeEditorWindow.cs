using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueNodeEditorWindow : EditorWindow
{
    public DialogueSceneKit kit;
    public DialogueNode node;

    public static DialogueNodeEditorWindow GetNodeEditor()
    {
        return GetWindow<DialogueNodeEditorWindow>();
    }
    //private void OnGUI()
    //{
    //    if(node is ChoiceNode choice)
    //    {
    //        DrawChoice(choice);
    //    }
    //    else if(node is EventNode eventNode)
    //    {
    //        DrawDialogueEvent(eventNode);
    //    }
    //}


    //private void DrawDialogueEvent(DialogueNode eventContainer)
    //{
    //    EditorGUILayout.BeginVertical();
    //    verticalScrollPosition = EditorGUILayout.BeginScrollView(verticalScrollPosition);
    //    ParameterPack pack = eventContainer.Parameter;
    //    eventContainer.Parameter = (ParameterPack)EditorGUILayout.ObjectField(eventContainer.Parameter,
    //        typeof(ParameterPack), allowSceneObjects: true);
    //    if (pack != eventContainer.Parameter)
    //    {
    //        EditorUtility.SetDirty(kit);
    //    }

    //    if (eventContainer.Parameter != null)
    //    {
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Менять параметр");
    //        bool toggleBufer = eventContainer.ChangeCondition;
    //        eventContainer.ChangeCondition = EditorGUILayout.Toggle(eventContainer.ChangeCondition);
    //        if (toggleBufer != eventContainer.ChangeCondition)
    //        {
    //            EditorUtility.SetDirty(kit);
    //        }
    //        EditorGUILayout.EndHorizontal();
    //        if (eventContainer.ChangeCondition)
    //        {
    //            EditorGUILayout.BeginHorizontal();
    //            int number = eventContainer.ConditionNumber;
    //            eventContainer.ConditionNumber = EditorGUILayout.Popup(eventContainer.ConditionNumber, eventContainer.Parameter.GetCharacteristic());
    //            if (number != eventContainer.ConditionNumber)
    //            {
    //                EditorUtility.SetDirty(kit);
    //            }

    //            if (eventContainer.Parameter.parametres[eventContainer.ConditionNumber].type == ParameterType.Bool)
    //            {
    //                EditorGUILayout.LabelField("Значение после события");
    //                toggleBufer = eventContainer.ChangeBoolValue;
    //                eventContainer.ChangeBoolValue = EditorGUILayout.Toggle(eventContainer.ChangeBoolValue);
    //                if (toggleBufer != eventContainer.ChangeBoolValue)
    //                {
    //                    EditorUtility.SetDirty(kit);
    //                }
    //            }
    //            else
    //            {
    //                EditorGUILayout.LabelField("Сместить на");
    //                number = eventContainer.ChangeIntValue;
    //                eventContainer.ChangeIntValue = EditorGUILayout.IntField(eventContainer.ChangeIntValue);
    //                if (number != eventContainer.ChangeIntValue)
    //                {
    //                    EditorUtility.SetDirty(kit);
    //                }
    //            }
    //            EditorGUILayout.EndHorizontal();

    //        }
    //    }


    //    EditorGUILayout.BeginHorizontal();
    //    EditorGUILayout.LabelField("Событие в игровой сцене");
    //    bool toggle = eventContainer.InSceneInvoke;
    //    eventContainer.InSceneInvoke = EditorGUILayout.Toggle(eventContainer.InSceneInvoke);
    //    if (toggle != eventContainer.InSceneInvoke)
    //    {
    //        EditorUtility.SetDirty(kit);
    //    }
    //    EditorGUILayout.EndHorizontal();
    //    if (eventContainer.InSceneInvoke)
    //    {
    //        horizontalScrollPosition = EditorGUILayout.BeginScrollView(horizontalScrollPosition, GUILayout.MaxHeight(100));
    //        EditorGUILayout.BeginHorizontal();
    //        for (int i = 0; i < eventContainer.ReactorsNumbers.Count; i++)
    //        {
    //            EditorGUILayout.BeginVertical();
    //            eventContainer.ReactorsNumbers[i] = EditorGUILayout.Popup(eventContainer.ReactorsNumbers[i], kit.inSceneInvokeObjects.ToArray(),
    //         GUILayout.MinWidth(100));
    //            if (numberBufer != eventContainer.ReactorsNumbers[i])
    //            {
    //                EditorUtility.SetDirty(kit);
    //            }
    //            GUILayout.Space(3);
    //            if (GUILayout.Button("x", GUILayout.MaxWidth(20)))
    //            {
    //                eventContainer.ReactorsNumbers.Remove(eventContainer.ReactorsNumbers[i]);
    //                EditorUtility.SetDirty(kit);
    //                break;
    //            }
    //            EditorGUILayout.EndVertical();
    //        }
    //        GUILayout.Space(5);
    //        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
    //        {
    //            eventContainer.ReactorsNumbers.Add(0);
    //            EditorUtility.SetDirty(kit);
    //        }
    //        EditorGUILayout.EndHorizontal();
    //        EditorGUILayout.EndScrollView();
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Ракурс во время события:");
    //        eventContainer.EventCamPositionNumber = EditorGUILayout.Popup(eventContainer.EventCamPositionNumber, kit.camerasPositions.ToArray(),
    //            GUILayout.MinWidth(40));
    //        if (replica.CamPositionNumber != replica.CamPositionNumber)
    //        {
    //            EditorUtility.SetDirty(this);
    //        }
    //        EditorGUILayout.LabelField("Ракурс № во время события");
    //        int number = eventContainer.EventCamPositionNumber;
    //        eventContainer.EventCamPositionNumber = EditorGUILayout.IntField(eventContainer.EventCamPositionNumber, GUILayout.MinWidth(40));
    //        if (number != eventContainer.EventCamPositionNumber)
    //        {
    //            EditorUtility.SetDirty(kit);
    //        }
    //        EditorGUILayout.EndHorizontal();
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Время фокусировки на событии");
    //        float time = eventContainer.EventTime;
    //        eventContainer.EventTime = EditorGUILayout.Slider(eventContainer.EventTime, 0, 120);
    //        if (time != eventContainer.EventTime)
    //        {
    //            EditorUtility.SetDirty(kit);
    //        }
    //        EditorGUILayout.EndHorizontal();
    //    }

    //    EditorGUILayout.BeginHorizontal();
    //    EditorGUILayout.LabelField("Сообщение при событии");
    //    toggle = eventContainer.IsMessage;
    //    eventContainer.IsMessage = EditorGUILayout.Toggle(eventContainer.IsMessage);
    //    if (toggle != eventContainer.IsMessage)
    //    {
    //        EditorUtility.SetDirty(kit);
    //    }
    //    EditorGUILayout.EndHorizontal();
    //    if (eventContainer.IsMessage)
    //    {
    //        string text = eventContainer.MessageText;
    //        eventContainer.MessageText = EditorGUILayout.TextArea(eventContainer.MessageText, GUILayout.MaxWidth(position.width - 10));
    //        if (text != null && !text.Equals(eventContainer.MessageText))
    //        {
    //            EditorUtility.SetDirty(kit);
    //        }
    //    }
    //    EditorGUILayout.EndScrollView();
    //    EditorGUILayout.EndVertical();
    //}
}
