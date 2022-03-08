using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DialogueSceneEditor : EditorWindow
{
    #region Поля
    public DialogueSceneKit sceneKit;

    private List<Rect> windows = new List<Rect>();
    private DialogueNodeType nodeType = DialogueNodeType.Replica;
    private DialogueNode beginRelationNodeBufer;
    private int exitBufer;

    private Vector2 scrollPosition = Vector2.zero;
    private Rect scrollViewRect;

    private int currentCamPos = 0;
    private int currentEvent = 0;

    private const float mainInfoYSize = 50;
    #endregion

    #region Основные методы отрисовки
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

    void OnGUI()
    {
        DrawMainInfo();
        if (sceneKit != null)
        {
            DrawOptions();


            scrollViewRect = GetScrollViewZone();
            scrollPosition = GUI.BeginScrollView(new Rect(0, mainInfoYSize, position.width, position.height - mainInfoYSize), scrollPosition,
                scrollViewRect, false, false);

            DrawRelations();

            BeginWindows();
            if (sceneKit != null)
            {
                for (int i = 0; i < windows.Count; i++)
                {
                    GUI.color = i == sceneKit.firstNodeIndex? Color.green : sceneKit.Nodes[i].СolorInEditor;
                    windows[i] = GUI.Window(i, windows[i], DrawNodeWindow, i.ToString());
                }
            }
            EndWindows();

            GUI.EndScrollView();
        }
    }   

    private void DrawMainInfo()
    {
        Rect buferRect = new Rect(0, 0, position.width, 20);
        GUILayout.BeginVertical();
        GUI.Box(buferRect, GUIContent.none);
        GUILayout.BeginHorizontal();
        sceneKit = (DialogueSceneKit)EditorGUILayout.ObjectField(sceneKit, typeof(DialogueSceneKit), false, GUILayout.MaxWidth(200));
        if (sceneKit != null)
        {
            GUILayout.Label("Сцена: " + sceneKit.sceneName);
            GUILayout.Label("Узлов: " + sceneKit.Nodes.Count);
            if (sceneKit.Nodes.Count != windows.Count)
            {
                for (int i = 0; i < sceneKit.Nodes.Count; i++)
                {
                    Rect bufer = sceneKit.Nodes[i].transformRect;
                    Vector2 pos = new Vector2(bufer.x, bufer.y);
                    windows.Add(new Rect(pos.x, pos.y, bufer.width, bufer.height));
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    private void DrawOptions()
    {
        Rect buferRect = new Rect(0, 20, position.width, 25);
        GUILayout.BeginVertical();
        GUI.Box(buferRect, GUIContent.none);
        GUILayout.BeginHorizontal();
        nodeType = (DialogueNodeType)EditorGUILayout.EnumPopup(nodeType, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
        if (GUILayout.Button("Создать узел", GUILayout.MaxWidth(100), GUILayout.MinWidth(100)))
        {
            Vector2 pos = new Vector2(position.width / 2 + scrollPosition.x, position.height / 2 + scrollPosition.y);
            sceneKit.CreateNode(nodeType, pos);
            Rect bufer = sceneKit.Nodes[sceneKit.Nodes.Count - 1].transformRect;
            windows.Add(new Rect(pos.x, pos.y, bufer.width, bufer.height));
        }
        DrawCameraPositionsMenu();
        DrawInSceneEventsMenu();
        if (GUILayout.Button("Удалить всё", GUILayout.MaxWidth(100), GUILayout.MinWidth(100)))
        {
            sceneKit.Clear();
            windows.Clear();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.EndVertical();
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
    #endregion

    #region Отрисовка узлов
    private void DrawNodeWindow(int id)
    {
        DialogueNode node = sceneKit.Nodes[id];

        if (windows[id].x < 0 || windows[id].y < 0)
        {
            windows[id] = new Rect(windows[id].x > 0? windows[id].x : 0,
                windows[id].y > 0 ? windows[id].y : 0,
                windows[id].width,
                windows[id].height);
        }

        Rect windowRect = windows[id];

        DrawNodeMainInfo(node, windowRect, id);

        GUI.DragWindow();
    }

    private void DrawNodeMainInfo(DialogueNode node, Rect windowRect, int id)
    {
        node.transformRect = windowRect;
        Rect bufer = new Rect(1, 1, 20, 20);
        if (GUI.Button(bufer, "1"))
        {
            sceneKit.SetAsFirst(node);
            return;
        }

        bufer = new Rect(windowRect.width - 21, 1, 20, 20);
        if (GUI.Button(bufer, "X"))
        {
            sceneKit.Remove(node);
            windows.RemoveAt(id);
            return;
        }

        bufer = new Rect(22, 1, 30, 20);
        if (node.leftToRight)
        {
            if (GUI.Button(bufer, ">>"))
            {
                node.leftToRight = !node.leftToRight;

            }
            bufer = new Rect(1, 21, 20, 20);
        }
        else
        {
            if (GUI.Button(bufer, "<<"))
            {
                node.leftToRight = !node.leftToRight;
            }
            bufer = new Rect(windowRect.width - 21, 21, 20, 20);
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
            DrawReplicInfo(windowRect, replicaNode);
        }
        else if (node is ChoiceNode choiceNode)
        {
            DrawChoice(choiceNode, windowRect);
        }
        else if (node is ConditionNode conditionNode)
        {
            DrawCondition(conditionNode, windowRect);
        }
        else if (node is EventNode eventNode)
        {
            DrawEvent(eventNode, windowRect);
        }
        else if (node is LinkNode linkNode)
        {
            DrawLink(linkNode, windowRect);
        }
        else if (node is RandomizerNode randomizer)
        {
            DrawRandomizer(randomizer, windowRect);
        }
    }

    private void DrawReplicInfo(Rect windowRect, ReplicaNode node)
    {
        ReplicInfo info = node.replicaInformation;
        Rect bufer = new Rect(windowRect.width - 42, 1, 24, 20);
        if (GUI.Button(bufer, "="))
        {
            DialogueReplicaEditorWindow.GetReplicaWindow(info, sceneKit).Show();
        }
        if (info.character != null)
        {
            bufer = new Rect(1, windowRect.height - 8, windowRect.width / 2, 7);
            EditorGUI.DrawRect(bufer, Color.grey);
            bufer = new Rect(2, windowRect.height - 7, windowRect.width / 2 - 2, 5);
            EditorGUI.DrawRect(bufer, info.character.color);
        }

        if (node.leftToRight)
        {
            bufer = new Rect(windowRect.width - 63, 1, 21, 21);
            if (GUI.Button(bufer, node.finalNode ? "-|" : "->"))
            {
                node.finalNode = !node.finalNode;
                if (node.finalNode)
                {
                    sceneKit.ClearNextRelations(node);
                }
            }
            if (!node.finalNode)
            {
                bufer = new Rect(node.exitPointOffset.x, node.exitPointOffset.y, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = node;
                    exitBufer = 0;
                }
            }
        }
        else
        {
            bufer = new Rect(windowRect.width - 63, 1, 21, 21);
            if (GUI.Button(bufer, node.finalNode ? "|-" : "<-"))
            {
                node.finalNode = !node.finalNode;
                if (node.finalNode)
                {
                    sceneKit.ClearNextRelations(node);
                }
            }
            if (!node.finalNode)
            {
                bufer = new Rect(node.enterPointOffset.x, node.enterPointOffset.y, 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = node;
                    exitBufer = 0;
                }
            }
        }
        bufer = new Rect(21, 22, windowRect.width - 39, 20);
        info.replicaText = EditorGUI.TextField(bufer, info.replicaText);
    }
    private void DrawChoice(ChoiceNode choiceNode, Rect windowRect)
    {
        Rect bufer = new Rect(windowRect.width - 42, 1, 24, 20);
        if (GUI.Button(bufer, "="))
        {
            DialogueChoiceEditorWindow.GetReplicaWindow(choiceNode, sceneKit).Show();
        }
        if (choiceNode.character != null)
        {
            bufer = new Rect(1, windowRect.height - 8, windowRect.width / 2, 7);
            EditorGUI.DrawRect(bufer, Color.grey);
            bufer = new Rect(2, windowRect.height - 7, windowRect.width / 2 - 2, 5);
            EditorGUI.DrawRect(bufer, choiceNode.character.color);
            for (int i = 0; i < choiceNode.answers.Count; i++)
            {
                if (choiceNode.leftToRight)
                {
                    bufer = new Rect(21, 21 * (i + 1), 20, 20);
                    if (GUI.Button(bufer, "x"))
                    {
                        choiceNode.RemoveAnsver(i);
                        if (choiceNode.answers.Count < choiceNode.answerLimit - 1)
                        {
                            windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 22);
                            choiceNode.transformRect = windows[choiceNode.index];
                        }
                        break;
                    }
                    bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
                    choiceNode.answers[i].answerTip = EditorGUI.TextField(bufer, choiceNode.answers[i].answerTip);
                    bufer = new Rect(choiceNode.exitPointOffsetList[i].x, choiceNode.exitPointOffsetList[i].y, 20, 20);
                    if (GUI.Button(bufer, ">"))
                    {
                        beginRelationNodeBufer = choiceNode;
                        exitBufer = i;
                    }
                }
                else
                {
                    bufer = new Rect(1, 21 * (i + 1), 20, 20);
                    if (GUI.Button(bufer, "<"))
                    {
                        beginRelationNodeBufer = choiceNode;
                        exitBufer = i;
                    }
                    bufer = new Rect(bufer.x + 22, bufer.y, 5, 20);
                    bufer = new Rect(bufer.x + 6, bufer.y, windowRect.width - 70, 20);
                    choiceNode.answers[i].answerTip = EditorGUI.TextField(bufer, choiceNode.answers[i].answerTip);
                    bufer = new Rect(choiceNode.exitPointOffsetList[i].x - 21, choiceNode.exitPointOffsetList[i].y, 20, 20);
                    if (GUI.Button(bufer, "x"))
                    {
                        choiceNode.RemoveAnsver(i);
                        if (choiceNode.answers.Count < choiceNode.answerLimit - 1)
                        {
                            windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 22);
                            choiceNode.transformRect = windows[choiceNode.index];
                        }
                        break;
                    }
                }
            }
            bufer = new Rect(21, 21 * (choiceNode.answers.Count + 1), windowRect.width - 42, 20);
            if (choiceNode.answers.Count < choiceNode.answerLimit)
            {
                if (GUI.Button(bufer, "+"))
                {
                    choiceNode.AddAnswer();
                    if (choiceNode.answers.Count < choiceNode.answerLimit)
                    {
                        windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height + 22);
                        choiceNode.transformRect = windows[choiceNode.index];
                    }
                }
            }

        }
    }
    private void DrawCondition(ConditionNode conditionNode, Rect nodeTransform)
    {
        Rect bufer;

        if (conditionNode.conditionItem == null)
        {
            conditionNode.conditionItem = new ConditionItem();
            return;
        }

        if (conditionNode.leftToRight)
        {
            bufer = new Rect(conditionNode.positiveExitPointOffset.x, conditionNode.positiveExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "+>"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 0;
            }
            bufer = new Rect(conditionNode.negativeExitPointOffset.x, conditionNode.negativeExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "->"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 1;
            }
            bufer = new Rect(21, 21, 130, 20);
            conditionNode.conditionItem.parameter = (ParameterPack)EditorGUI.ObjectField(bufer,
                conditionNode.conditionItem.parameter, typeof(ParameterPack), allowSceneObjects: true);
            if (conditionNode.conditionItem.parameter != null)
            {
                bufer = new Rect(1, 42, 82, 20);
                conditionNode.conditionItem.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionItem.conditionNumber,
                    conditionNode.conditionItem.parameter.GetCharacteristic());
                bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                if (conditionNode.conditionItem.parameter.parametres[conditionNode.conditionItem.conditionNumber].type == ParameterType.Bool)
                {
                    conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.conditionItem.checkType, new string[2] { "==", "!=" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                    conditionNode.conditionItem.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.conditionItem.checkBoolValue);
                }
                else
                {
                    conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer,
                        (int)conditionNode.conditionItem.checkType, new string[4] { "==", "!=", ">", "<" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                    conditionNode.conditionItem.checkIntValue = EditorGUI.IntField(bufer, conditionNode.conditionItem.checkIntValue);
                }
            }
        }
        else
        {
            bufer = new Rect(1, conditionNode.positiveExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "<+"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 0;
            }
            bufer = new Rect(1, conditionNode.negativeExitPointOffset.y, 30, 20);
            if (GUI.Button(bufer, "<-"))
            {
                beginRelationNodeBufer = conditionNode;
                exitBufer = 1;
            }
            bufer = new Rect(31, 21, 130, 20);
            conditionNode.conditionItem.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, conditionNode.conditionItem.parameter,
                typeof(ParameterPack), allowSceneObjects: true);
            if (conditionNode.conditionItem.parameter != null)
            {
                bufer = new Rect(31, 42, 82, 20);
                conditionNode.conditionItem.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionItem.conditionNumber,
                    conditionNode.conditionItem.parameter.GetCharacteristic());
                bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                if (conditionNode.conditionItem.parameter.parametres[conditionNode.conditionItem.conditionNumber].type == ParameterType.Bool)
                {
                    conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.conditionItem.checkType, new string[2] { "==", "!=" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                    conditionNode.conditionItem.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.conditionItem.checkBoolValue);
                }
                else
                {
                    conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer,
                        (int)conditionNode.conditionItem.checkType, new string[4] { "==", "!=", ">", "<" });
                    bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                    conditionNode.conditionItem.checkIntValue = EditorGUI.IntField(bufer, conditionNode.conditionItem.checkIntValue);
                }
            }
        }
    }
    private void DrawEvent(EventNode eventNode, Rect windowRect)
    {
        Rect bufer = new Rect(windowRect.width - 21, 1, 24, 20);
        bufer.x -= 21;
        if (GUI.Button(bufer, "="))
        {
            DialogueEventEditorWindow.GetEventWindow(eventNode, sceneKit).Show();
        }
        if (eventNode.leftToRight)
        {
            bufer = new Rect(windowRect.width - 63, 1, 21, 21);
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
                bufer = new Rect(eventNode.exitPointOffset.x, eventNode.exitPointOffset.y, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = eventNode;
                    exitBufer = 0;
                }
            }
        }
        else
        {
            bufer = new Rect(windowRect.width - 63, 1, 21, 21);
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
                bufer = new Rect(eventNode.enterPointOffset.x, eventNode.enterPointOffset.y, 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = eventNode;
                    exitBufer = 0;
                }
            }
        }

        bufer = new Rect(21, 21, 110, 20);
        eventNode.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, eventNode.parameter,
            typeof(ParameterPack), allowSceneObjects: true);

        if (eventNode.parameter != null)
        {
            if (eventNode.changeParameter)
            {
                bufer = new Rect(1, bufer.y + 21, 80, 20);
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
                    eventNode.parameterOperation = (OperationType)EditorGUI.Popup(bufer, (int)eventNode.parameterOperation, new string[2] { "==", "+=" });
                    bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
                    eventNode.changeIntValue = EditorGUI.IntField(bufer, eventNode.changeIntValue);
                }
            }
        }
        if (eventNode.useMessage)
        {
            bufer = new Rect(21, bufer.y + 21, 100, 20);
            eventNode.messageText = EditorGUI.TextArea(bufer, eventNode.messageText);
        }
        if (eventNode.inSceneInvoke)
        {
            bufer = new Rect(windowRect.width - 21, 63, 20, 20);
            EditorGUI.LabelField(bufer, "#");
        }
    }
    private void DrawLink(LinkNode linkNode, Rect nodeTransform)
    {
        Rect bufer;
        if (linkNode.leftToRight)
        {
            bufer = new Rect(linkNode.exitPointOffset.x, linkNode.exitPointOffset.y, 20, 20);
            if (GUI.Button(bufer, ">"))
            {
                beginRelationNodeBufer = linkNode;
                exitBufer = 0;
            }
        }
        else
        {
            bufer = new Rect(linkNode.enterPointOffset.x, linkNode.enterPointOffset.y, 20, 20);
            if (GUI.Button(bufer, "<"))
            {
                beginRelationNodeBufer = linkNode;
                exitBufer = 0;
            }
        }
        bufer = new Rect(21, 22, nodeTransform.width - 39, 20);
        GUI.color = Color.white;
        EditorGUI.LabelField(bufer, linkNode.NextNodeNumber == -1 ? "пусто" : linkNode.NextNodeNumber.ToString());
    }
    private void DrawRandomizer(RandomizerNode randomizer, Rect windowRect)
    {
        Rect bufer;
        if (randomizer.leftToRight)
        {
            bufer = new Rect(randomizer.exitPointOffset.x, randomizer.exitPointOffset.y, 20, 20);
            if (GUI.Button(bufer, ">"))
            {
                beginRelationNodeBufer = randomizer;
                exitBufer = -1;
            }
        }
        else
        {
            bufer = new Rect(randomizer.enterPointOffset.x, randomizer.enterPointOffset.y, 20, 20);
            if (GUI.Button(bufer, "<"))
            {
                beginRelationNodeBufer = randomizer;
                exitBufer = -1;
            }
        }
        bufer = new Rect(21, 22, windowRect.width - 39, 20);
        EditorGUI.DrawRect(bufer, Color.white);
        EditorGUI.LabelField(bufer, randomizer.defaultNextNodeNumber == -1 ? "пусто" : randomizer.defaultNextNodeNumber.ToString());
        for (int i = 1; i < randomizer.nextNodesNumbers.Count; i++)
        {
            if (randomizer.leftToRight)
            {
                bufer = new Rect(21, 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    randomizer.RemoveLinkNumber(i);
                    windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
                        windows[randomizer.index].width, windows[randomizer.index].height - 22);
                    randomizer.transformRect = windows[randomizer.index];
                    break;
                }
                bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
                EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
                bufer = new Rect(randomizer.exitPointsOffsetList[i - 1].x, randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
                if (GUI.Button(bufer, ">"))
                {
                    beginRelationNodeBufer = randomizer;
                    exitBufer = i;
                }
            }
            else
            {
                bufer = new Rect(1, 21 * (i + 1), 20, 20);
                if (GUI.Button(bufer, "<"))
                {
                    beginRelationNodeBufer = randomizer;
                    exitBufer = i;
                }
                bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
                EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
                bufer = new Rect(randomizer.exitPointsOffsetList[i - 1].x - 21, randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
                if (GUI.Button(bufer, "x"))
                {
                    randomizer.RemoveLinkNumber(i);
                    windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
                        windows[randomizer.index].width, windows[randomizer.index].height - 22);
                    randomizer.transformRect = windows[randomizer.index];
                    break;
                }
            }
        }
        bufer = new Rect(21, 21 * (randomizer.nextNodesNumbers.Count + 1), windowRect.width - 42, 20);
        if (GUI.Button(bufer, "+"))
        {
            randomizer.AddLinkNumber(-1);
            windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
                windows[randomizer.index].width, windows[randomizer.index].height + 22);
            randomizer.transformRect = windows[randomizer.index];
        }
        bufer = new Rect(bufer.x, bufer.y + 21, bufer.width, bufer.height);
        randomizer.withRemoving = EditorGUI.ToggleLeft(bufer, "Без повторов", randomizer.withRemoving);
    }
    #endregion

    #region Вспомогательные методы
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
                        replica.transformRect.y + replica.exitPointOffset.y) + new Vector2(20, 10);
                    if (!replica.leftToRight)
                    {
                        startPoint = new Vector2(replica.transformRect.x + replica.enterPointOffset.x,
                            replica.transformRect.y + replica.enterPointOffset.y) + new Vector2(0, 10);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y) + new Vector2(0, 10);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y) + new Vector2(20, 10);
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
                            choice.transformRect.y + choice.exitPointOffsetList[j].y) + new Vector2(20, 10);
                        if (!choice.leftToRight)
                        {
                            startPoint = new Vector2(choice.transformRect.x + choice.enterPointOffset.x,
                                choice.transformRect.y + choice.enterPointOffset.y + +(21 * j)) + new Vector2(0, 10);
                            startMultiplicator = -1;
                        }

                        Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                            nextNode.transformRect.y + nextNode.enterPointOffset.y) + new Vector2(0, 10);
                        if (!nextNode.leftToRight)
                        {
                            endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                                nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y) + new Vector2(20, 10);
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
                        sceneKit.Nodes[i].transformRect.y + eventNode.exitPointOffset.y) + new Vector2(20, 10);
                    if (!eventNode.leftToRight)
                    {
                        startPoint = new Vector2(eventNode.transformRect.x + eventNode.enterPointOffset.x, eventNode.transformRect.y +
                        eventNode.enterPointOffset.y) + new Vector2(0, 10);
                        startMultiplicator = -1;
                    }

                    DialogueNode nextNode = sceneKit.Nodes[eventNode.NextNodeNumber];

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y) + new Vector2(0, 10);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y) + new Vector2(20, 10);
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
                        condition.transformRect.y + condition.positiveExitPointOffset.y) + new Vector2(20, 10);
                    if (!condition.leftToRight)
                    {
                        startPoint = new Vector2(condition.transformRect.x - 1, condition.transformRect.y +
                            condition.positiveExitPointOffset.y) + new Vector2(0, 10);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y) + new Vector2(0, 10);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y) + new Vector2(20, 10);
                        endMultiplicator = -1;
                    }

                    DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.green);
                }
                if (condition.NegativeNextNumber != -1)
                {
                    startMultiplicator = endMultiplicator = 1;
                    DialogueNode nextNode = sceneKit.Nodes[condition.NegativeNextNumber];
                    Vector2 startPoint = new Vector2(condition.transformRect.x + condition.negativeExitPointOffset.x,
                        condition.transformRect.y + condition.negativeExitPointOffset.y) + new Vector2(20, 10);
                    if (!sceneKit.Nodes[i].leftToRight)
                    {
                        startPoint = new Vector2(condition.transformRect.x - 1,
                            condition.transformRect.y + condition.negativeExitPointOffset.y) + new Vector2(0, 10);
                        startMultiplicator = -1;
                    }

                    Vector2 endPoint = new Vector2(nextNode.transformRect.x + nextNode.enterPointOffset.x,
                        nextNode.transformRect.y + nextNode.enterPointOffset.y) + new Vector2(0, 10);
                    if (!nextNode.leftToRight)
                    {
                        endPoint = new Vector2(nextNode.transformRect.x + nextNode.InverseEnterPointOffset.x,
                            nextNode.transformRect.y + nextNode.InverseEnterPointOffset.y) + new Vector2(20, 10);
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

        bufer1 = new Vector3(start.x + startMultiplicator.x, start.y + startMultiplicator.y, 0);
        bufer2 = new Vector3(end.x - endMultiplicator.x, end.y - endMultiplicator.y, 0);

        Handles.DrawBezier(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0),
            bufer1, bufer2, color, null, 3);
    }

    private void AddRelation(DialogueNode node)
    {
        if (beginRelationNodeBufer is ReplicaNode replica)
        {
            if (replica.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(replica);
            }
            replica.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(beginRelationNodeBufer, node);
        }
        else if (beginRelationNodeBufer is ChoiceNode choice)
        {
            if (choice.nextNodesNumbers[exitBufer] != -1)
            {
                sceneKit.ClearOneNextNumber(choice, exitBufer);
            }
            choice.nextNodesNumbers[exitBufer] = node.index;
            sceneKit.AddInPreviousRelations(choice, node);
        }
        else if (beginRelationNodeBufer is EventNode eventNode)
        {
            if (eventNode.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(eventNode);
            }
            eventNode.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(eventNode, sceneKit.Nodes[eventNode.NextNodeNumber]);
        }
        else if (beginRelationNodeBufer is ConditionNode condition)
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
        else if (beginRelationNodeBufer is LinkNode link)
        {
            if (link.NextNodeNumber != -1)
            {
                sceneKit.ClearNextRelations(link);
            }
            link.NextNodeNumber = node.index;
            sceneKit.AddInPreviousRelations(link, node);
        }
        else if (beginRelationNodeBufer is RandomizerNode randomizer)
        {
            if (exitBufer == -1)
            {
                if (randomizer.defaultNextNodeNumber != -1)
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
        else if (maxX < rezult.x + rezult.width - 200)
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
    #endregion
}