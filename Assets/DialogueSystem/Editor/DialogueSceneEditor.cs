﻿using UnityEngine;
using UnityEditor;

public class DialogueSceneEditor : EditorWindow
{
    #region Поля

    /// <summary>
    /// ссылка на диалоговую схему
    /// </summary>
    public DialogueSceneKit sceneKit;

    private DialogueNodeType nodeType = DialogueNodeType.Replica;
    private DialogueNode beginRelationNodeBufer;
    private DialogueNode dragNodeBufer;
    private Rect buferRect, scrollViewRect;
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 clickPoint;

    private bool dragNode;
    private bool mouseScroll;
    private int exitBufer;
    private int currentCamPos = 0;
    private int currentEvent = 0;

    private const float mainInfoYSize = 50;

    #endregion

    #region Основные методы

    [MenuItem("Window/IgoGoTools/DialogueEditor %>")]
    public static void Init() 
    {
        DialogueSceneEditor sceneEditor = GetWindow<DialogueSceneEditor>();
        sceneEditor.Show();
    }
    public static DialogueSceneEditor GetEditor()
    {
        return GetWindow<DialogueSceneEditor>();
    }

    private void OnEnable()
    {
        scrollViewRect = new Rect(0, mainInfoYSize, position.width, position.height - mainInfoYSize);
    }

    private void OnGUI()
    {
        DrawMainInfo();
        if (sceneKit != null)
        {
            DrawOptions();
            DrawNodes();
            DragNode();
        }
        MouseScroll();
    }
    #endregion
    #region Методы отрисовки
    private void DrawMainInfo()
    {
        buferRect = new Rect(0, 0, position.width, 20);
        GUILayout.BeginVertical();
        GUI.Box(buferRect, GUIContent.none);
        GUILayout.BeginHorizontal();
        sceneKit = (DialogueSceneKit)EditorGUILayout.ObjectField(sceneKit, typeof(DialogueSceneKit), false, GUILayout.MaxWidth(200));
        if (sceneKit != null)
        {
            GUILayout.Label("Сцена: " + sceneKit.sceneName);
            GUILayout.Label("Узлов: " + sceneKit.Nodes.Count);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    private void DrawOptions()
    {
        buferRect = new Rect(0, 20, position.width, 25);
        GUILayout.BeginVertical();
        GUI.Box(buferRect, GUIContent.none);
        GUILayout.BeginHorizontal();
        nodeType = (DialogueNodeType)EditorGUILayout.EnumPopup(nodeType, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
        if (GUILayout.Button("Создать узел", GUILayout.MaxWidth(100), GUILayout.MinWidth(100)))
        {
            Vector2 pos = new Vector2(position.width / 2 + scrollPosition.x, position.height / 2 + scrollPosition.y);
            sceneKit.CreateNode(nodeType, pos);
        }
        DrawCameraPositionsMenu();
        DrawInSceneEventsMenu();
        if (GUILayout.Button("Удалить всё", GUILayout.MaxWidth(100), GUILayout.MinWidth(100)))
        {
            sceneKit.Clear();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }
    private void DrawNodes()
    {                       
        scrollViewRect = GetScrollViewZone();
        scrollPosition = GUI.BeginScrollView(new Rect(0, mainInfoYSize, position.width, position.height - mainInfoYSize), scrollPosition,
            scrollViewRect, false, false);
        DrawRelations();
        for (int i = 0; i < sceneKit.Nodes.Count; i++)
        {
            DrawNode(sceneKit.Nodes[i]);
        }
        GUI.EndScrollView();
    }

    private void DrawCameraPositionsMenu()
    {
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(320), GUILayout.MinWidth(200));
        GUILayout.Label("Ракурсы камеры:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.camerasPositions.Add("Новый ракурс " + (sceneKit.camerasPositions.Count + 1));
            currentCamPos = sceneKit.camerasPositions.Count - 1;
        }
        if (sceneKit.camerasPositions.Count > 0)
        {
            if (GUILayout.Button("<<", GUILayout.MaxWidth(30), GUILayout.MinWidth(20)))
            {
                currentCamPos--;
                if (currentCamPos < 0)
                    currentCamPos = sceneKit.camerasPositions.Count - 1;
            }
            sceneKit.camerasPositions[currentCamPos] = GUILayout.TextField(sceneKit.camerasPositions[currentCamPos],
                GUILayout.MaxWidth(100), GUILayout.MinWidth(40));
            if (GUILayout.Button(">>", GUILayout.MaxWidth(30), GUILayout.MinWidth(20)))
            {
                currentCamPos++;
                if (currentCamPos > sceneKit.camerasPositions.Count - 1)
                    currentCamPos = 0;
            }

            if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
            {
                sceneKit.camerasPositions.Remove(sceneKit.camerasPositions[currentCamPos]);
                currentCamPos--;
                if (currentCamPos < 0)
                    currentCamPos = sceneKit.camerasPositions.Count - 1;
            }
        }
        GUILayout.EndHorizontal();
    }
    private void DrawInSceneEventsMenu()
    {
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(sceneKit.inSceneInvokeObjects.Count > 0 ? 360 : 100), GUILayout.MinWidth(100));
        GUILayout.Label("События в сцене:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.inSceneInvokeObjects.Add("Новое событие " + (sceneKit.inSceneInvokeObjects.Count + 1));
            currentEvent = sceneKit.inSceneInvokeObjects.Count - 1;
        }
        if (sceneKit.inSceneInvokeObjects.Count > 0)
        {
            if (GUILayout.Button("<<", GUILayout.MaxWidth(30), GUILayout.MinWidth(20)))
            {
                currentEvent--;
                if (currentEvent < 0)
                    currentEvent = sceneKit.inSceneInvokeObjects.Count - 1;
            }
            sceneKit.inSceneInvokeObjects[currentEvent] = GUILayout.TextField(sceneKit.inSceneInvokeObjects[currentEvent],
                GUILayout.MaxWidth(140), GUILayout.MinWidth(80));
            if (GUILayout.Button(">>", GUILayout.MaxWidth(30), GUILayout.MinWidth(20)))
            {
                currentEvent++;
                if (currentEvent > sceneKit.inSceneInvokeObjects.Count - 1)
                    currentEvent = 0;
            }

            if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
            {
                sceneKit.inSceneInvokeObjects.Remove(sceneKit.inSceneInvokeObjects[currentEvent]);
                currentEvent--;
                if (currentEvent < 0)
                    currentEvent = sceneKit.inSceneInvokeObjects.Count - 1;
            }
        }
        GUILayout.EndHorizontal();
    }
    private void DrawNode(DialogueNode node)
    {
        if (node.transformRect.x < 0)
        {
            node.transformRect = new Rect(0, node.transformRect.y, node.transformRect.width, node.transformRect.height);
        }
        if (node.transformRect.y < 0)
        {
            node.transformRect = new Rect(node.transformRect.x, 0, node.transformRect.width, node.transformRect.height);
        }

        Rect nodeTransform = new Rect(node.transformRect.x + 5, node.transformRect.y + 33, 30, 25);

        EditorGUI.DrawRect(nodeTransform, node.colorInEditor);
        EditorGUI.LabelField(nodeTransform, node.index.ToString()); ;

        nodeTransform = new Rect(node.transformRect.x, node.transformRect.y + mainInfoYSize, node.transformRect.width,
            node.transformRect.height);

        if (node.index == sceneKit.firstNodeIndex)
        {
            EditorGUI.DrawRect(nodeTransform, Color.green);
        }
        else
        {
            EditorGUI.DrawRect(nodeTransform, node.colorInEditor);
        }
        Rect bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 1, 20, 20);
        if (GUI.Button(bufer, "1"))
        {
            sceneKit.SetAsFirst(node);
            return;
        }
        bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 1, 24, 20);
        if (GUI.Button(bufer, "X"))
        {
            sceneKit.Remove(node);
            return;
        }

        bufer = new Rect(nodeTransform.x + 22, nodeTransform.y + 1, 30, 20);
        if (node.leftToRight)
        {
            if (GUI.Button(bufer, ">>"))
            {
                node.leftToRight = !node.leftToRight;
            }
            bufer = new Rect(nodeTransform.x + node.enterPointOffset.x, nodeTransform.y + node.enterPointOffset.y, 20, 20);
        }
        else
        {
            if (GUI.Button(bufer, "<<"))
            {
                node.leftToRight = !node.leftToRight;
            }
            bufer = new Rect(nodeTransform.x + nodeTransform.width - node.enterPointOffset.x - 21,
                nodeTransform.y + node.enterPointOffset.y, 20, 20);
        }
        if (GUI.Button(bufer, "O"))
        {
            if (beginRelationNodeBufer != null)
            {
                AddRelation(node);
            }
        }

        if (node is ReplicaNode replicaNode)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 1, 24, 20);
            bufer.x -= 21;
            if (GUI.Button(bufer, "="))
            {
                DialogueReplicaEditorWindow.GetReplicaWindow(replicaNode.replicaInformation, sceneKit).Show();
            }
            DrawReplica(replicaNode, nodeTransform);
        }
        else if (node is ChoiceNode choiceNode)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 1, 24, 20);
            bufer.x -= 21;
            if (GUI.Button(bufer, "="))
            {
                DialogueChoiceEditorWindow.GetReplicaWindow(choiceNode, sceneKit).Show();
            }
            DrawChoice(choiceNode, nodeTransform);
        }
        else if (node is ConditionNode conditionNode)
        {
            DrawCondition(conditionNode, nodeTransform);
        }
        else if (node is EventNode eventNode)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 1, 24, 20);
            bufer.x -= 21;
            if (GUI.Button(bufer, "="))
            {
                DialogueEventEditorWindow.GetEventWindow(eventNode, sceneKit).Show();
            }
            DrawEvent(eventNode, nodeTransform);
        }
        else if (node is LinkNode linkNode)
        {
            DrawLink(linkNode, nodeTransform);
        }
        else if (node is RandomizerNode randomizer)
        {
            DrawRandomizer(randomizer, nodeTransform);
        }
    }

    private void DrawReplica(ReplicaNode replica, Rect nodeTransform)
    {
        Rect bufer;
        ReplicInfo info = replica.replicaInformation;

        if (info.character != null)
        {
            bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + nodeTransform.height - 8, nodeTransform.width / 2, 7);
            EditorGUI.DrawRect(bufer, Color.grey);
            bufer = new Rect(nodeTransform.x + 2, nodeTransform.y + nodeTransform.height - 7, nodeTransform.width / 2 - 2, 5);
            EditorGUI.DrawRect(bufer, info.character.color);
        }

        if (replica.leftToRight)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 63, nodeTransform.y + 1, 21, 21);
            if (GUI.Button(bufer, replica.finalNode ? "-|" : "->"))
            {
                replica.finalNode = !replica.finalNode;
                if (replica.finalNode)
                {
                    sceneKit.ClearNextRelations(replica);
                }
            }
            if (!replica.finalNode)
            {
                bufer = new Rect(nodeTransform.x + replica.exitPointOffset.x, nodeTransform.y + replica.exitPointOffset.y, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = replica;
                    exitBufer = 0;
                }
            }
            bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
            info.replicaText = EditorGUI.TextField(bufer, info.replicaText);
        }
        else
        {
            bufer = new Rect(nodeTransform.x + 53, nodeTransform.y + 1, 21, 21);
            if (GUI.Button(bufer, replica.finalNode ? "|-" : "<-"))
            {
                replica.finalNode = !replica.finalNode;
                if (replica.finalNode)
                {
                    sceneKit.ClearNextRelations(replica);
                }
            }
            if (!replica.finalNode)
            {
                bufer = new Rect(nodeTransform.x + replica.enterPointOffset.x, nodeTransform.y + replica.enterPointOffset.y, 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = replica;
                    exitBufer = 0;
                }
            }
            bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
            info.replicaText = EditorGUI.TextField(bufer, info.replicaText);
        }
    }
    private void DrawChoice(ChoiceNode choice, Rect nodeTransform)
    {
        Rect bufer;
        if (choice.character != null)
        {
            bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + nodeTransform.height - 8, nodeTransform.width / 2, 7);
            EditorGUI.DrawRect(bufer, Color.grey);
            bufer = new Rect(nodeTransform.x + 2, nodeTransform.y + nodeTransform.height - 7, nodeTransform.width / 2 - 2, 5);
            EditorGUI.DrawRect(bufer, choice.character.color);
        }
        for (int i = 0; i < choice.answers.Count; i++)
        {
            if (choice.leftToRight)
            {
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    choice.RemoveAnsver(i);
                    if (choice.answers.Count < 2)
                    {
                        choice.transformRect = new Rect(choice.transformRect.x, choice.transformRect.y, choice.transformRect.width,
                            choice.transformRect.height - 22);
                    }
                    break;
                }
                bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                choice.answers[i].answerReplica.replicaText = EditorGUI.TextField(bufer, choice.answers[i].answerReplica.replicaText);
                if (choice.answers[i].answerReplica.character != null)
                {
                    bufer = new Rect(bufer.x + nodeTransform.width - 70 + 1, bufer.y, 5, 20);
                    EditorGUI.DrawRect(bufer, choice.answers[i].answerReplica.character.color);
                }
                bufer = new Rect(nodeTransform.x + choice.exitPointOffsetList[i].x, nodeTransform.y +
                    choice.exitPointOffsetList[i].y, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = choice;
                    exitBufer = i;
                }
            }
            else
            {
                bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = choice;
                    exitBufer = i;
                }
                bufer = new Rect(bufer.x + 22, bufer.y, 5, 20);
                if (choice.answers[i].answerReplica.character != null)
                {
                    EditorGUI.DrawRect(bufer, choice.answers[i].answerReplica.character.color);
                }
                bufer = new Rect(bufer.x + 6, bufer.y, nodeTransform.width - 70, 20);
                choice.answers[i].answerReplica.replicaText = EditorGUI.TextField(bufer, choice.answers[i].answerReplica.replicaText);
                bufer = new Rect(nodeTransform.x + choice.exitPointOffsetList[i].x - 21, nodeTransform.y +
                    choice.exitPointOffsetList[i].y, 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    choice.RemoveAnsver(i);
                    if (choice.answers.Count < 2)
                    {
                        choice.transformRect = new Rect(choice.transformRect.x, choice.transformRect.y,
                            choice.transformRect.width, choice.transformRect.height - 22);
                    }
                    break;
                }
            }
        }
        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (choice.answers.Count + 1), nodeTransform.width - 42, 20);
        if (choice.answers.Count < choice.answerLimit)
        {
            if (GUI.Button(bufer, "+"))
            {
                choice.AddAnswer();
                if (choice.answers.Count < 3)
                {
                    choice.transformRect = new Rect(choice.transformRect.x, choice.transformRect.y, choice.transformRect.width,
                        choice.transformRect.height + 22);
                }
            }
        }
    }
    private void DrawCondition(ConditionNode conditionNode, Rect nodeTransform)
    {
        Rect bufer;
        if (conditionNode.leftToRight)
        {
            bufer = new Rect(nodeTransform.x + conditionNode.positiveExitPointOffset.x,
                nodeTransform.y + conditionNode.positiveExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "+>"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 0;
            }
            bufer = new Rect(nodeTransform.x + conditionNode.negativeExitPointOffset.x,
                nodeTransform.y + conditionNode.negativeExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "->"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 1;
            }
            bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21, 130, 20);
            conditionNode.parameter = (ParameterPack)EditorGUI.ObjectField(bufer,
                conditionNode.parameter, typeof(ParameterPack), allowSceneObjects: true);
            if (conditionNode.parameter != null)
            {
                bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 42, 82, 20);
                conditionNode.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionNumber,
                    conditionNode.parameter.GetCharacteristic());
                bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                if (conditionNode.parameter.parametres[conditionNode.conditionNumber].type == ParameterType.Bool)
                {
                    conditionNode.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.checkType, new string[2] { "==", "!=" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                    conditionNode.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.checkBoolValue);
                }
                else
                {
                    conditionNode.checkType = (CheckType)EditorGUI.Popup(bufer,
                        (int)conditionNode.checkType, new string[4] { "==", "!=", ">", "<" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                    conditionNode.checkIntValue = EditorGUI.IntField(bufer, conditionNode.checkIntValue);
                }
            }
        }
        else
        {
            bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + conditionNode.positiveExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "<+"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 0;
            }
            bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + conditionNode.negativeExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "<-"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 1;
            }
            bufer = new Rect(nodeTransform.x + 31, nodeTransform.y + 21, 130, 20);
            conditionNode.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, conditionNode.parameter,
                typeof(ParameterPack), allowSceneObjects: true);
            if (conditionNode.parameter != null)
            {
                bufer = new Rect(nodeTransform.x + 31, nodeTransform.y + 42, 82, 20);
                conditionNode.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionNumber,
                    conditionNode.parameter.GetCharacteristic());
                bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                if (conditionNode.parameter.parametres[conditionNode.conditionNumber].type == ParameterType.Bool)
                {
                    conditionNode.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.checkType, new string[2] { "==", "!=" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                    conditionNode.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.checkBoolValue);
                }
                else
                {
                    conditionNode.checkType = (CheckType)EditorGUI.Popup(bufer,
                        (int)conditionNode.checkType, new string[4] { "==", "!=", ">", "<" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                    conditionNode.checkIntValue = EditorGUI.IntField(bufer, conditionNode.checkIntValue);
                }
            }
        }
    }
    private void DrawEvent(EventNode eventNode, Rect nodeTransform)
    {
        Rect bufer;
        if (eventNode.leftToRight)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 63, nodeTransform.y + 1, 21, 21);
            if (GUI.Button(bufer, eventNode.finalNode ? "-|" : "->"))
            {
                eventNode.finalNode = !eventNode.finalNode;
                if (eventNode.finalNode)
                {
                    sceneKit.ClearNextRelations(eventNode);
                }
            }
            if (!eventNode.finalNode)
            {
                bufer = new Rect(nodeTransform.x + eventNode.exitPointOffset.x, nodeTransform.y + eventNode.exitPointOffset.y, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = eventNode;
                    exitBufer = 0;
                }
            }
        }
        else
        {
            bufer = new Rect(nodeTransform.x + 53, nodeTransform.y + 1, 21, 21);
            if (GUI.Button(bufer, eventNode.finalNode ? "|-" : "<-"))
            {
                eventNode.finalNode = !eventNode.finalNode;
                if (eventNode.finalNode)
                {
                    sceneKit.ClearNextRelations(eventNode);
                }
            }
            if (!eventNode.finalNode)
            {
                bufer = new Rect(nodeTransform.x + eventNode.enterPointOffset.x, nodeTransform.y + eventNode.enterPointOffset.y, 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = eventNode;
                    exitBufer = 0;
                }
            }
        }

        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21, 110, 20);
        eventNode.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, eventNode.parameter,
            typeof(ParameterPack), allowSceneObjects: true);
        if (eventNode.parameter != null)
        {
            if (eventNode.changeParameter)
            {
                bufer = new Rect(nodeTransform.x + 1, bufer.y + 21, 80, 20);
                eventNode.changeingParameterIndex = EditorGUI.Popup(bufer, eventNode.changeingParameterIndex,
                    eventNode.parameter.GetCharacteristic());

                if (eventNode.parameter.parametres[eventNode.changeingParameterIndex].type == ParameterType.Bool)
                {
                    bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
                    EditorGUI.LabelField(bufer, "=");
                    bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
                    eventNode.targetBoolValue = EditorGUI.Toggle(bufer, eventNode.targetBoolValue);
                }
                else
                {
                    bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
                    eventNode.operation = (OperationType)EditorGUI.Popup(bufer, (int)eventNode.operation, new string[2] { "==", "+=" });
                    bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
                    eventNode.changeIntValue = EditorGUI.IntField(bufer, eventNode.changeIntValue);
                }
            }
        }
        if (eventNode.useMessage)
        {
            bufer = new Rect(nodeTransform.x + 21, bufer.y + 21, 100, 20);
            eventNode.messageText = EditorGUI.TextArea(bufer, eventNode.messageText);
        }
        if (eventNode.inSceneInvoke)
        {
            bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 63, 20, 20);
            EditorGUI.LabelField(bufer, "#");
        }
    }
    private void DrawLink(LinkNode linkNode, Rect nodeTransform)
    {
        Rect bufer;
        if (linkNode.leftToRight)
        {
            bufer = new Rect(nodeTransform.x + linkNode.exitPointOffset.x, nodeTransform.y + linkNode.exitPointOffset.y, 20, 20);
            if (GUI.Button(bufer, ">"))
            {
                beginRelationNodeBufer = linkNode;
                exitBufer = 0;
            }
        }
        else
        {
            bufer = new Rect(nodeTransform.x + linkNode.enterPointOffset.x, nodeTransform.y + linkNode.enterPointOffset.y, 20, 20);
            if (GUI.Button(bufer, "<"))
            {
                beginRelationNodeBufer = linkNode;
                exitBufer = 0;
            }
        }
        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
        EditorGUI.DrawRect(bufer, Color.white);
        EditorGUI.LabelField(bufer, linkNode.NextNodeNumber == -1 ? "пусто" : linkNode.NextNodeNumber.ToString());
    }
    private void DrawRandomizer(RandomizerNode randomizer, Rect nodeTransform)
    {
        Rect bufer;
        if (randomizer.leftToRight)
        {
            bufer = new Rect(nodeTransform.x + randomizer.exitPointOffset.x, nodeTransform.y + randomizer.exitPointOffset.y, 20, 20);
            if (GUI.Button(bufer, ">"))
            {
                beginRelationNodeBufer = randomizer;
                exitBufer = -1;
            }
        }
        else
        {
            bufer = new Rect(nodeTransform.x + randomizer.enterPointOffset.x, nodeTransform.y + randomizer.enterPointOffset.y, 20, 20);
            if (GUI.Button(bufer, "<"))
            {
                beginRelationNodeBufer = randomizer;
                exitBufer = -1;
            }
        }
        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
        EditorGUI.DrawRect(bufer, Color.white);
        EditorGUI.LabelField(bufer, randomizer.defaultNextNodeNumber == -1 ? "пусто" : randomizer.defaultNextNodeNumber.ToString());
        for (int i = 1; i < randomizer.nextNodesNumbers.Count; i++)
        {
            if (randomizer.leftToRight)
            {
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    randomizer.RemoveLinkNumber(i);
                    randomizer.transformRect = new Rect(randomizer.transformRect.x, randomizer.transformRect.y,
                        randomizer.transformRect.width, randomizer.transformRect.height - 22);
                    break;
                }
                bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
                bufer = new Rect(nodeTransform.x + randomizer.exitPointsOffsetList[i - 1].x,
                    nodeTransform.y + randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = randomizer;
                    exitBufer = i;
                }
            }
            else
            {
                bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = randomizer;
                    exitBufer = i;
                }
                bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
                bufer = new Rect(nodeTransform.x + randomizer.exitPointsOffsetList[i - 1].x - 21,
                    nodeTransform.y + randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    randomizer.RemoveLinkNumber(i);
                    randomizer.transformRect = new Rect(randomizer.transformRect.x, randomizer.transformRect.y,
                        randomizer.transformRect.width, randomizer.transformRect.height - 22);
                    break;
                }
            }
        }
        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (randomizer.nextNodesNumbers.Count + 1),
            nodeTransform.width - 42, 20);
        if (GUI.Button(bufer, "+"))
        {
            randomizer.AddLinkNumber(-1);
            randomizer.transformRect = new Rect(randomizer.transformRect.x, randomizer.transformRect.y,
                randomizer.transformRect.width, randomizer.transformRect.height + 22);
        }
        bufer = new Rect(bufer.x, bufer.y +21, bufer.width, bufer.height);
        randomizer.withRemoving = EditorGUI.ToggleLeft(bufer, "Без повторов", randomizer.withRemoving);
    }

    private void DrawRelations()
    {
        for (int i = 0; i < sceneKit.Nodes.Count; i++)
        {
            int startMultiplicator, endMultiplicator;
            startMultiplicator = endMultiplicator = 1;

            DialogueNode node = sceneKit.Nodes[i];

            if (node is ReplicaNode replica)
            {
                if (replica.NextNodeNumber != -1)
                {
                    DialogueNode nextNode = sceneKit.Nodes[replica.NextNodeNumber];

                    Vector2 startPoint = new Vector2(replica.transformRect.x + replica.exitPointOffset.x,
                        replica.transformRect.y + replica.exitPointOffset.y);
                    if (!replica.leftToRight)
                    {
                        startPoint = new Vector2(replica.transformRect.x + replica.enterPointOffset.x,
                            replica.transformRect.y + replica.enterPointOffset.y);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y);
                        endMultiplicator = -1;
                    }

                    DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.white);
                }
            }
            else if (node is ChoiceNode choice)
            {
                for (int j = 0; j < choice.nextNodesNumbers.Count; j++)
                {
                    if (choice.nextNodesNumbers[j] != -1)
                    {
                        DialogueNode nextNode = sceneKit.Nodes[choice.nextNodesNumbers[j]];

                        Vector2 startPoint = new Vector2(choice.transformRect.x + choice.exitPointOffsetList[j].x,
                            choice.transformRect.y + choice.exitPointOffsetList[j].y);
                        if (!choice.leftToRight)
                        {
                            startPoint = new Vector2(choice.transformRect.x + choice.enterPointOffset.x,
                                choice.transformRect.y + choice.enterPointOffset.y + +(21 * j));
                            startMultiplicator = -1;
                        }

                        Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                            nextNode.transformRect.y + nextNode.enterPointOffset.y);
                        if (!nextNode.leftToRight)
                        {
                            endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                                nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y);
                            endMultiplicator = -1;
                        }

                        DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.white);
                    }
                }
            }
            else if (node is EventNode eventNode)
            {
                if (eventNode.NextNodeNumber != -1)
                {
                    Vector2 startPoint = new Vector2(eventNode.transformRect.x + eventNode.exitPointOffset.x,
                        sceneKit.Nodes[i].transformRect.y + eventNode.exitPointOffset.y);
                    if (!eventNode.leftToRight)
                    {
                        startPoint = new Vector2(eventNode.transformRect.x + eventNode.enterPointOffset.x, eventNode.transformRect.y +
                        eventNode.enterPointOffset.y);
                        startMultiplicator = -1;
                    }

                    DialogueNode nextNode = sceneKit.Nodes[eventNode.NextNodeNumber];

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y);
                        endMultiplicator = -1;
                    }

                    DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.yellow);
                }
            }
            else if (node is ConditionNode condition)
            {
                if (condition.PositiveNextNumber != -1)
                {
                    DialogueNode nextNode = sceneKit.Nodes[condition.PositiveNextNumber];
                    Vector2 startPoint = new Vector2(condition.transformRect.x + condition.positiveExitPointOffset.x,
                        condition.transformRect.y + condition.positiveExitPointOffset.y);
                    if (!condition.leftToRight)
                    {
                        startPoint = new Vector2(condition.transformRect.x - 1, condition.transformRect.y +
                            condition.positiveExitPointOffset.y);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y);
                        endMultiplicator = -1;
                    }

                    DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.green);
                }
                if (condition.NegativeNextNumber != -1)
                {
                    startMultiplicator = endMultiplicator = 1;
                    DialogueNode nextNode = sceneKit.Nodes[condition.NegativeNextNumber];
                    Vector2 startPoint = new Vector2(condition.transformRect.x + condition.negativeExitPointOffset.x,
                        condition.transformRect.y + condition.negativeExitPointOffset.y);
                    if (!sceneKit.Nodes[i].leftToRight)
                    {
                        startPoint = new Vector2(condition.transformRect.x - 1,
                            condition.transformRect.y + condition.negativeExitPointOffset.y);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y);
                        endMultiplicator = -1;
                    }

                    DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.red);
                }
            }
        }
    }
    private void DrawCurve(Vector2 start, Vector2 end, int startM, int endM, Color color)
    {
        Vector3 bufer1, bufer2;
        Vector2 startMultiplicator, endMultiplicator;
        startMultiplicator.x = startM * Mathf.Abs(end.x - start.x) / 2;
        endMultiplicator.x = endM * Mathf.Abs(end.x - start.x) / 2;
        startMultiplicator.y = endMultiplicator.y = 0;

        if (start.x * startM > end.x * endM)
        {
            if (start.y > end.y)
            {
                startMultiplicator.x = endMultiplicator.x = Mathf.Abs(end.x - start.x) / 2;
                startMultiplicator.x *= startM;
                endMultiplicator.x *= endM;
            }
            else
            {
                startMultiplicator.x = endMultiplicator.x = (Mathf.Abs(end.x - start.x) / 2) + 100;
                startMultiplicator.x *= startM;
                endMultiplicator.x *= endM;
            }
        }

        bufer1 = new Vector3(start.x + startMultiplicator.x, start.y + startMultiplicator.y + mainInfoYSize, 0);
        bufer2 = new Vector3(end.x - endMultiplicator.x, end.y - endMultiplicator.y + mainInfoYSize, 0);

        Handles.DrawBezier(new Vector3(start.x + 5, start.y + 10 + mainInfoYSize, 0), new Vector3(end.x, end.y + 10 + mainInfoYSize, 0),
            bufer1, bufer2, color, null, 3);
    }
    #endregion
    #region Служебные методы
    private void AddRelation(DialogueNode node)
    {
        if(beginRelationNodeBufer is ReplicaNode replica)
        {
            if (replica.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(replica);
            }
            replica.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(beginRelationNodeBufer, node);
        }
        else if(beginRelationNodeBufer is ChoiceNode choice)
        {
            if(choice.nextNodesNumbers[exitBufer] != -1)
            {
                sceneKit.ClearOneNextNumber(choice, exitBufer);
            }
            choice.nextNodesNumbers[exitBufer] = node.index;
            sceneKit.AddInPreviousRelations(choice, node);
        }
        else if(beginRelationNodeBufer is EventNode eventNode)
        {
            if (eventNode.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(eventNode);
            }
            eventNode.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(eventNode, sceneKit.Nodes[eventNode.NextNodeNumber]);
        }
        else if(beginRelationNodeBufer is ConditionNode condition)
        {
            if (exitBufer == 0)
            {
                if (condition.PositiveNextNumber != -1)
                {
                    sceneKit.ClearOneNextNumber(condition, exitBufer);
                }
                condition.PositiveNextNumber = node.index;
                sceneKit.AddInPreviousRelations(condition, sceneKit.Nodes[condition.PositiveNextNumber]);
            }
            else
            {
                if (condition.NegativeNextNumber != -1)
                {
                    sceneKit.ClearOneNextNumber(condition, exitBufer);
                }
                condition.NegativeNextNumber = node.index;
                sceneKit.AddInPreviousRelations(condition, sceneKit.Nodes[condition.NegativeNextNumber]);
            }
        }
        else if(beginRelationNodeBufer is LinkNode link)
        {
            if (link.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(link);
            }
            link.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(link, node);
        }
        else if(beginRelationNodeBufer is RandomizerNode randomizer)
        {
            if (exitBufer == -1)
            {
                if(randomizer.defaultNextNodeNumber != -1)
                {
                    sceneKit.ClearOneNextNumber(randomizer, 0);
                }
                randomizer.defaultNextNodeNumber = node.index;
            }
            else
            {
                if (randomizer.nextNodesNumbers[exitBufer] != -1)
                {
                    sceneKit.ClearOneNextNumber(randomizer, exitBufer);
                }
                randomizer.nextNodesNumbers[exitBufer] = node.index;
            }
            sceneKit.AddInPreviousRelations(randomizer, node);
        }
        beginRelationNodeBufer = null;
        exitBufer = 0;
    }
    private void DragNode()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (ClickInNode(Event.current.mousePosition, out dragNodeBufer))
            {
                dragNode = true;
            }
        }
        if (dragNode)
        {
            if (Event.current.type == EventType.MouseDrag && dragNodeBufer != null)
            {
                Vector2 offset = Event.current.delta;
                buferRect = new Rect(dragNodeBufer.transformRect.x + offset.x, dragNodeBufer.transformRect.y + offset.y, dragNodeBufer.transformRect.width,
                    dragNodeBufer.transformRect.height);
                dragNodeBufer.transformRect = buferRect;
                return;
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && dragNodeBufer != null)
            {
                dragNode = false;
            }
        }
    }
    private bool ClickInNode(Vector2 mousePos, out DialogueNode node)
    {
        node = null;
        for (int i = sceneKit.Nodes.Count - 1; i >= 0; i--)
        {
            if (mousePos.x > sceneKit.Nodes[i].transformRect.x - scrollPosition.x && mousePos.x < sceneKit.Nodes[i].transformRect.x
                - scrollPosition.x + sceneKit.Nodes[i].transformRect.width && mousePos.y > sceneKit.Nodes[i].transformRect.y -
                scrollPosition.y + mainInfoYSize && mousePos.y < sceneKit.Nodes[i].transformRect.y - scrollPosition.y + mainInfoYSize
                + sceneKit.Nodes[i].transformRect.height)
            {
                node = sceneKit.Nodes[i];
                clickPoint = mousePos;
                return true;
            }
        }
        return false;
    }
    private bool ClickInNode(Vector2 mousePos)
    {
        for (int i = sceneKit.Nodes.Count - 1; i >= 0; i--)
        {
            if (mousePos.x > sceneKit.Nodes[i].transformRect.x - scrollPosition.x && mousePos.x < sceneKit.Nodes[i].transformRect.x
                - scrollPosition.x + sceneKit.Nodes[i].transformRect.width && mousePos.y > sceneKit.Nodes[i].transformRect.y -
                scrollPosition.y + mainInfoYSize && mousePos.y < sceneKit.Nodes[i].transformRect.y - scrollPosition.y + mainInfoYSize
                + sceneKit.Nodes[i].transformRect.height)
            {
                clickPoint = mousePos;
                return true;
            }
        }
        return false;
    }
    private Rect GetScrollViewZone()
    {
        Rect rezult = new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, scrollViewRect.height);
        float maxX, maxY;
        maxX = maxY = 0;

        foreach (var item in sceneKit.Nodes)
        {
            if (maxX < item.transformRect.x + item.transformRect.width)
            {
                maxX = item.transformRect.x + item.transformRect.width;
            }
            if (maxY < item.transformRect.y + mainInfoYSize + item.transformRect.height)
            {
                maxY = item.transformRect.y + mainInfoYSize + item.transformRect.height;
            }
        }

        if (maxX > rezult.x + rezult.width)
        {
            rezult = new Rect(rezult.x, rezult.y, rezult.width + 200, rezult.height);
        }
        else if(maxX < rezult.x + rezult.width - 200)
        {
            rezult = new Rect(rezult.x, rezult.y, rezult.width - 200, rezult.height);
        }
        if (maxY > rezult.y + rezult.height)
        {
            rezult = new Rect(rezult.x, rezult.y, rezult.width, rezult.height + 200);
        }
        else if (maxY < rezult.y + rezult.height - 200)
        {
            rezult = new Rect(rezult.x, rezult.y, rezult.width, rezult.height - 200);
        }
        return rezult;
    }
    private void MouseScroll()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            if (!ClickInNode(Event.current.mousePosition))
            {
                mouseScroll = true;
                clickPoint = Event.current.mousePosition;
            }
        }
        if (mouseScroll)
        {
            Vector2 offset = Event.current.mousePosition - clickPoint;
            clickPoint = Event.current.mousePosition;
            scrollPosition += offset;
        }
        if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
        {
            mouseScroll = false;
        }
    }
    #endregion
}