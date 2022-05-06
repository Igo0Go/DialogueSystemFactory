using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DialogueSceneEditor : EditorWindow
{
    #region Поля
    public DialogueSceneKit sceneKit;

    private Vector2 drag;
    private Vector2 offset;
    private List<EditorNode> nodes;

    private GUIStyle nodeStyleReplica;
    private GUIStyle nodeStyleCondition;
    private GUIStyle nodeStyleEvent;
    private GUIStyle nodeStyleLink;
    private GUIStyle nodeStyleRandomizer;

    //private List<Rect> windows = new List<Rect>();
    //private DialogueNodeType nodeType = DialogueNodeType.Replica;
    //private DialogueNode beginRelationNodeBufer;
    //private int exitBufer;

    //private Vector2 scrollPosition = Vector2.zero;
    //private Rect scrollViewRect;

    //private int focusedNode = -1;
    //private int currentCamPos = 0;
    //private int currentEvent = 0;

    //private const float mainInfoYSize = 50;
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

    private void OnEnable()
    {
        nodeStyleReplica = new GUIStyle();
        nodeStyleReplica.normal.background = 
            EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyleReplica.border = new RectOffset(12, 12, 12, 12);

        nodeStyleCondition = new GUIStyle();
        nodeStyleCondition.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        nodeStyleCondition.border = new RectOffset(12, 12, 12, 12);

        nodeStyleLink = new GUIStyle();
        nodeStyleLink.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyleLink.border = new RectOffset(12, 12, 12, 12);

        nodeStyleEvent = new GUIStyle();
        nodeStyleEvent.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png") as Texture2D;
        nodeStyleEvent.border = new RectOffset(12, 12, 12, 12);

        nodeStyleRandomizer = new GUIStyle();
        nodeStyleRandomizer.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        nodeStyleRandomizer.border = new RectOffset(12, 12, 12, 12);
    }

    void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }
    #endregion

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, 
                new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, 
                new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        GUI.changed = true;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Replica"), false, 
            () => OnClickAddNode(mousePosition, DialogueNodeType.Replica));
        genericMenu.AddItem(new GUIContent("Choice"), false,
            () => OnClickAddNode(mousePosition, DialogueNodeType.Choice));
        genericMenu.AddItem(new GUIContent("Event"), false,
            () => OnClickAddNode(mousePosition, DialogueNodeType.Event));
        genericMenu.AddItem(new GUIContent("Condition"), false,
            () => OnClickAddNode(mousePosition, DialogueNodeType.Condition));
        genericMenu.AddItem(new GUIContent("Link"), false,
            () => OnClickAddNode(mousePosition, DialogueNodeType.Link));
        genericMenu.AddItem(new GUIContent("Randomizer"), false,
            () => OnClickAddNode(mousePosition, DialogueNodeType.Randomizer));
        genericMenu.ShowAsContext();
    }
    private void OnClickAddNode(Vector2 mousePosition, DialogueNodeType type)
    {
        if (nodes == null)
        {
            nodes = new List<EditorNode>();
        }

        switch (type)
        {
            case DialogueNodeType.Replica:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleReplica));
                break;
            case DialogueNodeType.Choice:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleReplica));
                break;
            case DialogueNodeType.Condition:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleCondition));
                break;
            case DialogueNodeType.Event:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleEvent));
                break;
            case DialogueNodeType.Link:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleLink));
                break;
            case DialogueNodeType.Randomizer:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleRandomizer));
                break;
            default:
                nodes.Add(new EditorNode(mousePosition, 200, 50, nodeStyleReplica));
                break;
        }


    }

    #region Отрисовка узлов
    //private void DrawNodeWindow(int id)
    //{
    //    DialogueNode node = sceneKit.Nodes[id];

    //    if (windows[id].x < 0 || windows[id].y < 0)
    //    {
    //        windows[id] = new Rect(windows[id].x > 0? windows[id].x : 0,
    //            windows[id].y > 0 ? windows[id].y : 0,
    //            windows[id].width,
    //            windows[id].height);
    //    }

    //    Rect windowRect = windows[id];

    //    DrawNodeMainInfo(node, windowRect, id);

    //    GUI.DragWindow();
    //}

    //private void DrawNodeMainInfo(DialogueNode node, Rect windowRect, int id)
    //{
    //    node.transformRect = windowRect;
    //    Rect bufer = new Rect(1, 1, 20, 20);
    //    if (GUI.Button(bufer, "1"))
    //    {
    //        sceneKit.SetAsFirst(node);
    //        return;
    //    }

    //    bufer = new Rect(windowRect.width - 21, 1, 20, 20);
    //    if (GUI.Button(bufer, "X"))
    //    {
    //        sceneKit.Remove(node);
    //        windows.RemoveAt(id);
    //        return;
    //    }

    //    bufer = new Rect(22, 1, 30, 20);
    //    if (node.leftToRight)
    //    {
    //        if (GUI.Button(bufer, ">>"))
    //        {
    //            node.leftToRight = !node.leftToRight;

    //        }
    //        bufer = new Rect(1, 21, 20, 20);
    //    }
    //    else
    //    {
    //        if (GUI.Button(bufer, "<<"))
    //        {
    //            node.leftToRight = !node.leftToRight;
    //        }
    //        bufer = new Rect(windowRect.width - 21, 21, 20, 20);
    //    }
    //    if (GUI.Button(bufer, "O"))
    //    {
    //        if (beginRelationNodeBufer != null)
    //        {
    //            AddRelation(node);
    //        }
    //    }
    //    if (node is ReplicaNode replicaNode)
    //    {
    //        DrawReplicInfo(windowRect, replicaNode);
    //    }
    //    else if (node is ChoiceNode choiceNode)
    //    {
    //        DrawChoice(choiceNode, windowRect);
    //    }
    //    else if (node is ConditionNode conditionNode)
    //    {
    //        DrawCondition(conditionNode, windowRect);
    //    }
    //    else if (node is EventNode eventNode)
    //    {
    //        DrawEvent(eventNode, windowRect);
    //    }
    //    else if (node is LinkNode linkNode)
    //    {
    //        DrawLink(linkNode, windowRect);
    //    }
    //    else if (node is RandomizerNode randomizer)
    //    {
    //        DrawRandomizer(randomizer, windowRect);
    //    }
    //}

    //private void DrawReplicInfo(Rect windowRect, ReplicaNode node)
    //{
    //    ReplicInfo info = node.replicaInformation;
    //    Rect bufer = new Rect(windowRect.width - 42, 1, 24, 20);
    //    if (GUI.Button(bufer, "="))
    //    {
    //        DialogueReplicaEditorWindow.GetReplicaWindow(info, sceneKit).Show();
    //    }
    //    if (info.character != null)
    //    {
    //        bufer = new Rect(1, windowRect.height - 8, windowRect.width / 2, 7);
    //        EditorGUI.DrawRect(bufer, Color.grey);
    //        bufer = new Rect(2, windowRect.height - 7, windowRect.width / 2 - 2, 5);
    //        EditorGUI.DrawRect(bufer, info.character.color);
    //    }

    //    if (node.leftToRight)
    //    {
    //        bufer = new Rect(windowRect.width - 63, 1, 21, 21);
    //        if (GUI.Button(bufer, node.finalNode ? "-|" : "->"))
    //        {
    //            node.finalNode = !node.finalNode;
    //            if (node.finalNode)
    //            {
    //                sceneKit.ClearNextRelations(node);
    //            }
    //        }
    //        if (!node.finalNode)
    //        {
    //            bufer = new Rect(node.exitPointOffset.x, node.exitPointOffset.y, 20, 20);
    //            if (GUI.Button(bufer, ">"))
    //            {
    //                beginRelationNodeBufer = node;
    //                exitBufer = 0;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        bufer = new Rect(windowRect.width - 63, 1, 21, 21);
    //        if (GUI.Button(bufer, node.finalNode ? "|-" : "<-"))
    //        {
    //            node.finalNode = !node.finalNode;
    //            if (node.finalNode)
    //            {
    //                sceneKit.ClearNextRelations(node);
    //            }
    //        }
    //        if (!node.finalNode)
    //        {
    //            bufer = new Rect(node.enterPointOffset.x, node.enterPointOffset.y, 20, 20);
    //            if (GUI.Button(bufer, "<"))
    //            {
    //                beginRelationNodeBufer = node;
    //                exitBufer = 0;
    //            }
    //        }
    //    }
    //    bufer = new Rect(21, 22, windowRect.width - 39, 20);
    //    info.replicaText = EditorGUI.TextField(bufer, info.replicaText);
    //}
    //private void DrawChoice(ChoiceNode choiceNode, Rect windowRect)
    //{
    //    Rect bufer = new Rect(windowRect.width - 42, 1, 24, 20);
    //    if (GUI.Button(bufer, "="))
    //    {
    //        DialogueChoiceEditorWindow.GetReplicaWindow(choiceNode, sceneKit).Show();
    //    }
    //    if (choiceNode.character != null)
    //    {
    //        bufer = new Rect(1, windowRect.height - 8, windowRect.width / 2, 7);
    //        EditorGUI.DrawRect(bufer, Color.grey);
    //        bufer = new Rect(2, windowRect.height - 7, windowRect.width / 2 - 2, 5);
    //        EditorGUI.DrawRect(bufer, choiceNode.character.color);
    //        for (int i = 0; i < choiceNode.answers.Count; i++)
    //        {
    //            if (choiceNode.leftToRight)
    //            {
    //                bufer = new Rect(21, 21 * (i + 1), 20, 20);
    //                if (GUI.Button(bufer, "x"))
    //                {
    //                    choiceNode.RemoveAnsver(i);
    //                    if (choiceNode.answers.Count < choiceNode.answerLimit - 1)
    //                    {
    //                        windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 22);
    //                        choiceNode.transformRect = windows[choiceNode.index];
    //                    }
    //                    break;
    //                }
    //                bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
    //                choiceNode.answers[i].answerTip = EditorGUI.TextField(bufer, choiceNode.answers[i].answerTip);
    //                bufer = new Rect(choiceNode.exitPointOffsetList[i].x, choiceNode.exitPointOffsetList[i].y, 20, 20);
    //                if (GUI.Button(bufer, ">"))
    //                {
    //                    beginRelationNodeBufer = choiceNode;
    //                    exitBufer = i;
    //                }
    //            }
    //            else
    //            {
    //                bufer = new Rect(1, 21 * (i + 1), 20, 20);
    //                if (GUI.Button(bufer, "<"))
    //                {
    //                    beginRelationNodeBufer = choiceNode;
    //                    exitBufer = i;
    //                }
    //                bufer = new Rect(bufer.x + 22, bufer.y, 5, 20);
    //                bufer = new Rect(bufer.x + 6, bufer.y, windowRect.width - 70, 20);
    //                choiceNode.answers[i].answerTip = EditorGUI.TextField(bufer, choiceNode.answers[i].answerTip);
    //                bufer = new Rect(choiceNode.exitPointOffsetList[i].x - 21, choiceNode.exitPointOffsetList[i].y, 20, 20);
    //                if (GUI.Button(bufer, "x"))
    //                {
    //                    choiceNode.RemoveAnsver(i);
    //                    if (choiceNode.answers.Count < choiceNode.answerLimit - 1)
    //                    {
    //                        windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 22);
    //                        choiceNode.transformRect = windows[choiceNode.index];
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //        bufer = new Rect(21, 21 * (choiceNode.answers.Count + 1), windowRect.width - 42, 20);
    //        if (choiceNode.answers.Count < choiceNode.answerLimit)
    //        {
    //            if (GUI.Button(bufer, "+"))
    //            {
    //                choiceNode.AddAnswer();
    //                if (choiceNode.answers.Count < choiceNode.answerLimit)
    //                {
    //                    windows[choiceNode.index] = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height + 22);
    //                    choiceNode.transformRect = windows[choiceNode.index];
    //                }
    //            }
    //        }

    //    }
    //}
    //private void DrawCondition(ConditionNode conditionNode, Rect nodeTransform)
    //{
    //    Rect bufer;

    //    if (conditionNode.conditionItem == null)
    //    {
    //        conditionNode.conditionItem = new ConditionItem();
    //        return;
    //    }

    //    if (conditionNode.leftToRight)
    //    {
    //        bufer = new Rect(conditionNode.positiveExitPointOffset.x, conditionNode.positiveExitPointOffset.y, 30, 20);
    //        if (GUI.Button(bufer, "+>"))
    //        {
    //            beginRelationNodeBufer = conditionNode;
    //            exitBufer = 0;
    //        }
    //        bufer = new Rect(conditionNode.negativeExitPointOffset.x, conditionNode.negativeExitPointOffset.y, 30, 20);
    //        if (GUI.Button(bufer, "->"))
    //        {
    //            beginRelationNodeBufer = conditionNode;
    //            exitBufer = 1;
    //        }
    //        bufer = new Rect(21, 21, 130, 20);
    //        conditionNode.conditionItem.parameter = (ParameterPack)EditorGUI.ObjectField(bufer,
    //            conditionNode.conditionItem.parameter, typeof(ParameterPack), allowSceneObjects: true);
    //        if (conditionNode.conditionItem.parameter != null)
    //        {
    //            bufer = new Rect(1, 42, 82, 20);
    //            conditionNode.conditionItem.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionItem.conditionNumber,
    //                conditionNode.conditionItem.parameter.GetCharacteristic());
    //            bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
    //            if (conditionNode.conditionItem.parameter.parametres[conditionNode.conditionItem.conditionNumber].type == ParameterType.Bool)
    //            {
    //                conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.conditionItem.checkType, new string[2] { "==", "!=" });
    //                bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
    //                conditionNode.conditionItem.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.conditionItem.checkBoolValue);
    //            }
    //            else
    //            {
    //                conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer,
    //                    (int)conditionNode.conditionItem.checkType, new string[4] { "==", "!=", ">", "<" });
    //                bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
    //                conditionNode.conditionItem.checkIntValue = EditorGUI.IntField(bufer, conditionNode.conditionItem.checkIntValue);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        bufer = new Rect(1, conditionNode.positiveExitPointOffset.y, 30, 20);
    //        if (GUI.Button(bufer, "<+"))
    //        {
    //            beginRelationNodeBufer = conditionNode;
    //            exitBufer = 0;
    //        }
    //        bufer = new Rect(1, conditionNode.negativeExitPointOffset.y, 30, 20);
    //        if (GUI.Button(bufer, "<-"))
    //        {
    //            beginRelationNodeBufer = conditionNode;
    //            exitBufer = 1;
    //        }
    //        bufer = new Rect(31, 21, 130, 20);
    //        conditionNode.conditionItem.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, conditionNode.conditionItem.parameter,
    //            typeof(ParameterPack), allowSceneObjects: true);
    //        if (conditionNode.conditionItem.parameter != null)
    //        {
    //            bufer = new Rect(31, 42, 82, 20);
    //            conditionNode.conditionItem.conditionNumber = EditorGUI.Popup(bufer, conditionNode.conditionItem.conditionNumber,
    //                conditionNode.conditionItem.parameter.GetCharacteristic());
    //            bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
    //            if (conditionNode.conditionItem.parameter.parametres[conditionNode.conditionItem.conditionNumber].type == ParameterType.Bool)
    //            {
    //                conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer, (int)conditionNode.conditionItem.checkType, new string[2] { "==", "!=" });
    //                bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
    //                conditionNode.conditionItem.checkBoolValue = EditorGUI.Toggle(bufer, conditionNode.conditionItem.checkBoolValue);
    //            }
    //            else
    //            {
    //                conditionNode.conditionItem.checkType = (CheckType)EditorGUI.Popup(bufer,
    //                    (int)conditionNode.conditionItem.checkType, new string[4] { "==", "!=", ">", "<" });
    //                bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
    //                conditionNode.conditionItem.checkIntValue = EditorGUI.IntField(bufer, conditionNode.conditionItem.checkIntValue);
    //            }
    //        }
    //    }
    //}
    //private void DrawEvent(EventNode eventNode, Rect windowRect)
    //{
    //    Rect bufer = new Rect(windowRect.width - 21, 1, 24, 20);
    //    bufer.x -= 21;
    //    if (GUI.Button(bufer, "="))
    //    {
    //        DialogueEventEditorWindow.GetEventWindow(eventNode, sceneKit).Show();
    //    }
    //    if (eventNode.leftToRight)
    //    {
    //        bufer = new Rect(windowRect.width - 63, 1, 21, 21);
    //        if (GUI.Button(bufer, eventNode.finalNode ? "-|" : "->"))
    //        {
    //            eventNode.finalNode = !eventNode.finalNode;
    //            if (eventNode.finalNode)
    //            {
    //                sceneKit.ClearNextRelations(eventNode);
    //            }
    //        }
    //        if (!eventNode.finalNode)
    //        {
    //            bufer = new Rect(eventNode.exitPointOffset.x, eventNode.exitPointOffset.y, 20, 20);
    //            if (GUI.Button(bufer, ">"))
    //            {
    //                beginRelationNodeBufer = eventNode;
    //                exitBufer = 0;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        bufer = new Rect(windowRect.width - 63, 1, 21, 21);
    //        if (GUI.Button(bufer, eventNode.finalNode ? "|-" : "<-"))
    //        {
    //            eventNode.finalNode = !eventNode.finalNode;
    //            if (eventNode.finalNode)
    //            {
    //                sceneKit.ClearNextRelations(eventNode);
    //            }
    //        }
    //        if (!eventNode.finalNode)
    //        {
    //            bufer = new Rect(eventNode.enterPointOffset.x, eventNode.enterPointOffset.y, 20, 20);
    //            if (GUI.Button(bufer, "<"))
    //            {
    //                beginRelationNodeBufer = eventNode;
    //                exitBufer = 0;
    //            }
    //        }
    //    }

    //    bufer = new Rect(21, 21, 110, 20);
    //    eventNode.parameter = (ParameterPack)EditorGUI.ObjectField(bufer, eventNode.parameter,
    //        typeof(ParameterPack), allowSceneObjects: true);

    //    if (eventNode.parameter != null)
    //    {
    //        if (eventNode.changeParameter)
    //        {
    //            bufer = new Rect(1, bufer.y + 21, 80, 20);
    //            eventNode.changeingParameterIndex = EditorGUI.Popup(bufer, eventNode.changeingParameterIndex,
    //                eventNode.parameter.GetCharacteristic());

    //            if (eventNode.parameter.parametres[eventNode.changeingParameterIndex].type == ParameterType.Bool)
    //            {
    //                bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
    //                EditorGUI.LabelField(bufer, "=");
    //                bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
    //                eventNode.targetBoolValue = EditorGUI.Toggle(bufer, eventNode.targetBoolValue);
    //            }
    //            else
    //            {
    //                bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
    //                eventNode.parameterOperation = (OperationType)EditorGUI.Popup(bufer, (int)eventNode.parameterOperation, new string[2] { "==", "+=" });
    //                bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
    //                eventNode.changeIntValue = EditorGUI.IntField(bufer, eventNode.changeIntValue);
    //            }
    //        }
    //    }
    //    if (eventNode.useMessage)
    //    {
    //        bufer = new Rect(21, bufer.y + 21, 100, 20);
    //        eventNode.messageText = EditorGUI.TextArea(bufer, eventNode.messageText);
    //    }
    //    if (eventNode.inSceneInvoke)
    //    {
    //        bufer = new Rect(windowRect.width - 21, 63, 20, 20);
    //        EditorGUI.LabelField(bufer, "#");
    //    }
    //}
    //private void DrawLink(LinkNode linkNode, Rect nodeTransform)
    //{
    //    Rect bufer;
    //    if (linkNode.leftToRight)
    //    {
    //        bufer = new Rect(linkNode.exitPointOffset.x, linkNode.exitPointOffset.y, 20, 20);
    //        if (GUI.Button(bufer, ">"))
    //        {
    //            beginRelationNodeBufer = linkNode;
    //            exitBufer = 0;
    //        }
    //    }
    //    else
    //    {
    //        bufer = new Rect(linkNode.enterPointOffset.x, linkNode.enterPointOffset.y, 20, 20);
    //        if (GUI.Button(bufer, "<"))
    //        {
    //            beginRelationNodeBufer = linkNode;
    //            exitBufer = 0;
    //        }
    //    }
    //    if(linkNode.nextNodesNumbers[0] >= 0)
    //    {
    //        bufer = new Rect(21, 22, nodeTransform.width - 39, 20);
    //        GUI.color = Color.white;
    //        if(GUI.Button(bufer, linkNode.NextNodeNumber.ToString()))
    //        {
    //            focusedNode = linkNode.nextNodesNumbers[0];
    //            GUI.FocusWindow(linkNode.nextNodesNumbers[0]);
    //            GUI.ScrollTo(windows[linkNode.nextNodesNumbers[0]]);
    //        }
    //    }
    //}
    //private void DrawRandomizer(RandomizerNode randomizer, Rect windowRect)
    //{
    //    Rect bufer;
    //    if (randomizer.leftToRight)
    //    {
    //        bufer = new Rect(randomizer.exitPointOffset.x, randomizer.exitPointOffset.y, 20, 20);
    //        if (GUI.Button(bufer, ">"))
    //        {
    //            beginRelationNodeBufer = randomizer;
    //            exitBufer = -1;
    //        }
    //    }
    //    else
    //    {
    //        bufer = new Rect(randomizer.enterPointOffset.x, randomizer.enterPointOffset.y, 20, 20);
    //        if (GUI.Button(bufer, "<"))
    //        {
    //            beginRelationNodeBufer = randomizer;
    //            exitBufer = -1;
    //        }
    //    }
    //    bufer = new Rect(21, 22, windowRect.width - 39, 20);
    //    EditorGUI.DrawRect(bufer, Color.white);
    //    EditorGUI.LabelField(bufer, randomizer.defaultNextNodeNumber == -1 ? "пусто" : randomizer.defaultNextNodeNumber.ToString());
    //    for (int i = 1; i < randomizer.nextNodesNumbers.Count; i++)
    //    {
    //        if (randomizer.leftToRight)
    //        {
    //            bufer = new Rect(21, 21 * (i + 1), 20, 20);
    //            if (GUI.Button(bufer, "x"))
    //            {
    //                randomizer.RemoveLinkNumber(i);
    //                windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
    //                    windows[randomizer.index].width, windows[randomizer.index].height - 22);
    //                randomizer.transformRect = windows[randomizer.index];
    //                break;
    //            }
    //            bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
    //            EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
    //            bufer = new Rect(randomizer.exitPointsOffsetList[i - 1].x, randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
    //            if (GUI.Button(bufer, ">"))
    //            {
    //                beginRelationNodeBufer = randomizer;
    //                exitBufer = i;
    //            }
    //        }
    //        else
    //        {
    //            bufer = new Rect(1, 21 * (i + 1), 20, 20);
    //            if (GUI.Button(bufer, "<"))
    //            {
    //                beginRelationNodeBufer = randomizer;
    //                exitBufer = i;
    //            }
    //            bufer = new Rect(bufer.x + 21, bufer.y, windowRect.width - 70, 20);
    //            EditorGUI.LabelField(bufer, randomizer.nextNodesNumbers[i] == -1 ? "пусто" : randomizer.nextNodesNumbers[i].ToString());
    //            bufer = new Rect(randomizer.exitPointsOffsetList[i - 1].x - 21, randomizer.exitPointsOffsetList[i - 1].y + 21, 20, 20);
    //            if (GUI.Button(bufer, "x"))
    //            {
    //                randomizer.RemoveLinkNumber(i);
    //                windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
    //                    windows[randomizer.index].width, windows[randomizer.index].height - 22);
    //                randomizer.transformRect = windows[randomizer.index];
    //                break;
    //            }
    //        }
    //    }
    //    bufer = new Rect(21, 21 * (randomizer.nextNodesNumbers.Count + 1), windowRect.width - 42, 20);
    //    if (GUI.Button(bufer, "+"))
    //    {
    //        randomizer.AddLinkNumber(-1);
    //        windows[randomizer.index] = new Rect(windows[randomizer.index].x, windows[randomizer.index].y,
    //            windows[randomizer.index].width, windows[randomizer.index].height + 22);
    //        randomizer.transformRect = windows[randomizer.index];
    //    }
    //    bufer = new Rect(bufer.x, bufer.y + 21, bufer.width, bufer.height);
    //    randomizer.withRemoving = EditorGUI.ToggleLeft(bufer, "Без повторов", randomizer.withRemoving);
    //}
    #endregion

    #region Вспомогательные методы

    #endregion
}