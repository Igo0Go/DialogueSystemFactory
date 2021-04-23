using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EventSystems;

public class DialogueSceneEditor : EditorWindow
{
    #region Поля

    /// <summary>
    /// ссылка на диалоговую схему
    /// </summary>
    public DialogueSceneKit sceneKit;

    private DialogueNodeType nodeType;
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
    {                                     //вызов окна
        DialogueSceneEditor sceneEditor = GetWindow<DialogueSceneEditor>();
        sceneEditor.Show();
    }
    public static DialogueSceneEditor GetEditor()
    {
        return GetWindow<DialogueSceneEditor>();
    }

    private void OnEnable() //при активации окна
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
            GUILayout.Label("Узлов: " + sceneKit.nodes.Count);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    private void DrawOptions()
    {
        buferRect = new Rect(0, 20, position.width, 25);
        GUILayout.BeginVertical();
        GUI.Box(buferRect, GUIContent.none);
        GUILayout.BeginHorizontal();        //поле с выпадающим списком (варианты - значения enum)
        nodeType = (DialogueNodeType)EditorGUILayout.EnumPopup(nodeType, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
        if (GUILayout.Button("Создать узел", GUILayout.MaxWidth(100), GUILayout.MinWidth(100)))
        {
            CreateNode();
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
                                //начало области прокрутки(размеры поля просмотра, смещение поля просмотра, фактические размеры рабочего поля)
        scrollPosition = GUI.BeginScrollView(new Rect(0, mainInfoYSize, position.width, position.height - mainInfoYSize), scrollPosition,
            scrollViewRect, false, false);

        DrawRelations(); //отрисовка связей

        for (int i = 0; i < sceneKit.nodes.Count; i++)
        {
            if(!sceneKit.nodes[i].Hide)
            {
                DrawNode(sceneKit.nodes[i]); // отрисовка конкретного узла
            }
        }
        GUI.EndScrollView();
    }
    #endregion
    #region Служебные методы
    private void DrawCameraPositionsMenu()
    {
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(320), GUILayout.MinWidth(200));
        GUILayout.Label("Ракурсы камеры:");
        if (GUILayout.Button("+", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
        {
            sceneKit.camerasPositions.Add("Новый ракурс " + (sceneKit.camerasPositions.Count + 1));
            currentCamPos = sceneKit.camerasPositions.Count-1;
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
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(sceneKit.inSceneInvokeObjects.Count>0? 360 : 100), GUILayout.MinWidth(100));
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
        if (node.transformRect.x < 0) //выравниевание, чтобы узел не вышел за пределы рабочего поля слева и сверху
        {
            node.transformRect = new Rect(0, node.transformRect.y, node.transformRect.width, node.transformRect.height);
        }
        if (node.transformRect.y < 0)
        {
            node.transformRect = new Rect(node.transformRect.x, 0, node.transformRect.width, node.transformRect.height);
        }

        Rect nodeTransform = new Rect(node.transformRect.x + 5, node.transformRect.y+33, 30, 25);

        EditorGUI.DrawRect(nodeTransform, node.colorInEditor);
        EditorGUI.LabelField(nodeTransform, node.index.ToString()); ;

        nodeTransform = new Rect(node.transformRect.x, node.transformRect.y + mainInfoYSize, node.transformRect.width, node.transformRect.height);

        if (node.index == sceneKit.firstNodeIndex)
        {
            EditorGUI.DrawRect(nodeTransform, Color.green);
        }
        else
        {
            EditorGUI.DrawRect(nodeTransform, node.colorInEditor); //рисуем прямоугольник-подложку
        }
        Rect bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 1, 20, 20);
        if (GUI.Button(bufer, "1")) //Назначить первым
        {
            sceneKit.SetAsFirst(node);
            return;
        }
        bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 1, 24, 20);
        if (GUI.Button(bufer, "X")) //кнопка удаления
        {
            sceneKit.Remove(node);
            return;
        }
        if (node.Type != NodeType.Condition && node.Type != NodeType.Link && node.Type != NodeType.RandomLink)
        {
            bufer.x -= 21;
            if (GUI.Button(bufer, "="))//кнопка расширенных настроек
            {
                DialogueNodeEditorWindow nodeEditorWindow = DialogueNodeEditorWindow.GetNodeEditor();
                nodeEditorWindow.kit = sceneKit;
                nodeEditorWindow.node = node;

                switch (node.Type)
                {
                    case NodeType.Replica:
                        nodeEditorWindow.minSize = nodeEditorWindow.maxSize = new Vector2(400, 150);
                        break;
                    case NodeType.Choice:
                        nodeEditorWindow.minSize = nodeEditorWindow.maxSize = new Vector2(400, 150);
                        break;
                    case NodeType.Event:
                        nodeEditorWindow.minSize = nodeEditorWindow.maxSize = new Vector2(400, 250);
                        break;
                }
                nodeEditorWindow.Show();
            }
        }

        bufer = new Rect(nodeTransform.x + 22, nodeTransform.y + 1, 30, 20);
        if (node.leftToRight)
        {
            if (GUI.Button(bufer, ">>")) // кнопка инверсии
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
            bufer = new Rect(nodeTransform.x + node.RightPointOffset.x, nodeTransform.y + node.RightPointOffset.y, 20, 20);
        }
        if (GUI.Button(bufer, "O")) // кнопка левый разъём
        {
            if (beginRelationNodeBufer != null)
            {
                AddRelation(node);
            }
        }

        switch (node.Type)
        {
            case NodeType.Replica:

                if (node.Character != null)
                {
                    bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + nodeTransform.height - 8, nodeTransform.width / 2, 7);
                    EditorGUI.DrawRect(bufer, Color.grey);
                    bufer = new Rect(nodeTransform.x + 2, nodeTransform.y + nodeTransform.height - 7, nodeTransform.width/2 - 2, 5);
                    EditorGUI.DrawRect(bufer, node.Character.color); //отрисовка цвета говорящего позволяет быстро понимать,
                }                                                   //кому принадлежит реплика

                if (node.leftToRight)
                {
                    bufer = new Rect(nodeTransform.x + nodeTransform.width - 63, nodeTransform.y + 1, 21, 21);
                    if (GUI.Button(bufer, node.finalNode ? "-|" : "->")) //кнопка финальный узел
                    {
                        node.finalNode = !node.finalNode;
                        if (node.finalNode)
                        {
                            sceneKit.ClearNextRelations(node);
                        }
                    }
                    if (!node.finalNode)
                    {
                        bufer = new Rect(nodeTransform.x + node.RightPointOffset.x, nodeTransform.y + node.RightPointOffset.y, 20, 20);
                        if (GUI.Button(bufer, ">")) //кнопка правый раъём
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = 0;
                        }
                    }
                    bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
                    node.ReplicText = EditorGUI.TextField(bufer, node.ReplicText);
                }
                else
                {
                    bufer = new Rect(nodeTransform.x + 53, nodeTransform.y + 1, 21, 21);
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
                        bufer = new Rect(nodeTransform.x + node.enterPointOffset.x, nodeTransform.y + node.enterPointOffset.y, 20, 20);
                        if (GUI.Button(bufer, "<"))
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = 0;
                        }
                    }
                    bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
                    node.ReplicText = EditorGUI.TextField(bufer, node.ReplicText);
                }
                break;
            case NodeType.Choice:
                if (node.Character != null)
                {
                    bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + nodeTransform.height - 8, nodeTransform.width / 2, 7);
                    EditorGUI.DrawRect(bufer, Color.grey);
                    bufer = new Rect(nodeTransform.x + 2, nodeTransform.y + nodeTransform.height - 7, nodeTransform.width / 2 - 2, 5);
                    EditorGUI.DrawRect(bufer, node.Character.color);
                }
                for (int i = 0; i < node.AnswerChoice.Count; i++)
                {
                    if (node.leftToRight)
                    {
                        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (i + 1), 20, 20);
                        if (GUI.Button(bufer, "x")) //можно удалять отдельные варианты ответов
                        {
                            sceneKit.Remove(sceneKit.nodes[node.AnswerChoice[i]]);
                            node.RemoveAnsver(i);
                            if (node.AnswerChoice.Count < 2)
                            {
                                node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height - 22);
                            }
                            break;
                        }
                        bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                        sceneKit.nodes[node.AnswerChoice[i]].ReplicText = EditorGUI.TextField(bufer, sceneKit.nodes[node.AnswerChoice[i]].ReplicText);
                        if(sceneKit.nodes[node.AnswerChoice[i]].Character != null)
                        {
                            bufer = new Rect(bufer.x + nodeTransform.width - 70 + 1, bufer.y, 5, 20);
                            EditorGUI.DrawRect(bufer, sceneKit.nodes[node.AnswerChoice[i]].Character.color);
                        }
                        bufer = new Rect(nodeTransform.x + node.ExitPointsOffset[i].x, nodeTransform.y + node.ExitPointsOffset[i].y, 20, 20);
                        if (GUI.Button(bufer, ">")) // также у каждого варианта ответа есть свой выходной разъём
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = i;
                        }
                    }
                    else
                    {
                        bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 21 * (i + 1), 20, 20);
                        if (GUI.Button(bufer, "<"))
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = i;
                        }
                        bufer = new Rect(bufer.x + 22, bufer.y, 5, 20);
                        if (sceneKit.nodes[node.AnswerChoice[i]].Character != null)
                        {
                            EditorGUI.DrawRect(bufer, sceneKit.nodes[node.AnswerChoice[i]].Character.color);
                        }
                        bufer = new Rect(bufer.x + 6, bufer.y, nodeTransform.width - 70, 20);
                        sceneKit.nodes[node.AnswerChoice[i]].ReplicText = EditorGUI.TextField(bufer, sceneKit.nodes[node.AnswerChoice[i]].ReplicText);
                        bufer = new Rect(nodeTransform.x + node.ExitPointsOffset[i].x - 21, nodeTransform.y + node.ExitPointsOffset[i].y, 20, 20);
                        if (GUI.Button(bufer, "x"))
                        {
                            sceneKit.Remove(sceneKit.nodes[node.AnswerChoice[i]]);
                            node.RemoveAnsver(i);
                            if (node.AnswerChoice.Count < 2)
                            {
                                node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height - 22);
                            }
                            break;
                        }
                    }
                }
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 * (node.AnswerChoice.Count + 1), nodeTransform.width - 42, 20);
                if (node.AnswerChoice.Count < limitAnswerCount)
                {
                    if (GUI.Button(bufer, "+")) // кнопка для добавления варианта ответа
                    {
                        var rep = new DialogueNode(NodeType.Replica, sceneKit.nodes.Count, true) { finalNode = false };
                        sceneKit.nodes.Add(rep);
                        node.AnswerChoice.Add(rep.index);
                        if (node.AnswerChoice.Count < 3)
                        {
                            node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height + 22);
                        }
                        node.CheckExitOffset();
                    }
                }
                break;
            case NodeType.Event:
                bufer = new Rect(nodeTransform.x + nodeTransform.width - 63, nodeTransform.y + 1, 21, 21);
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
                    if (node.leftToRight)
                    {
                        bufer = new Rect(nodeTransform.x + node.RightPointOffset.x, nodeTransform.y + node.RightPointOffset.y, 20, 20);
                        if (GUI.Button(bufer, ">"))
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = 0;
                        }
                    }
                    else
                    {
                        bufer = new Rect(nodeTransform.x + node.enterPointOffset.x, nodeTransform.y + node.enterPointOffset.y, 20, 20);
                        if (GUI.Button(bufer, "<"))
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = 0;
                        }
                    }

                }
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21, 110, 20);
                node.Parameter = (ParameterPack)EditorGUI.ObjectField(bufer, node.Parameter, typeof(ParameterPack), allowSceneObjects: true);
                if (node.Parameter != null)
                {
                    if (node.ChangeCondition)
                    {
                        bufer = new Rect(nodeTransform.x + 1, bufer.y + 21, 80, 20);
                        node.ConditionNumber = EditorGUI.Popup(bufer, node.ConditionNumber, node.Parameter.GetCharacteristic());
                        // string[] GetCharacteristic() - возвращаетс строки-названия параметров

                        if (node.Parameter.parametres[node.ConditionNumber].type == ParameterType.Bool)
                        {
                            bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
                            EditorGUI.LabelField(bufer, "=");
                            bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
                            node.ChangeBoolValue = EditorGUI.Toggle(bufer, node.ChangeBoolValue);
                        }
                        else
                        {
                            bufer = new Rect(bufer.x + 81, bufer.y, 40, 20);
                            node.Operation = (OperationType)EditorGUI.Popup(bufer, (int)node.CheckType, new string[2] { "==", "+=" });
                            bufer = new Rect(bufer.x + 41, bufer.y, 20, 20);
                            node.ChangeIntValue = EditorGUI.IntField(bufer, node.ChangeIntValue);
                        }
                    }
                }
                if (node.IsMessage)
                {
                    bufer = new Rect(nodeTransform.x + 21, bufer.y + 21, 100, 20);
                    node.MessageText = EditorGUI.TextArea(bufer, node.MessageText); //текстовое поле
                }
                if (node.InSceneInvoke)
                {
                    bufer = new Rect(nodeTransform.x + nodeTransform.width - 21, nodeTransform.y + 63, 20, 20);
                    EditorGUI.LabelField(bufer, "#");
                }
                break;
            case NodeType.Condition:
                if (node.leftToRight)
                {
                    bufer = new Rect(nodeTransform.x + node.PositiveExitPointOffset.x, nodeTransform.y + node.PositiveExitPointOffset.y, 30, 20);
                    if (GUI.Button(bufer, "+>")) //кнопка положительный разъём
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 0;
                    }
                    bufer = new Rect(nodeTransform.x + node.NegativeExitPointOffset.x, nodeTransform.y + node.NegativeExitPointOffset.y, 30, 20);
                    if (GUI.Button(bufer, "->")) //кнопка отрицательный разъём
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 1;
                    }
                    bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21, 130, 20);
                    node.Parameter = (ParameterPack)EditorGUI.ObjectField(bufer, node.Parameter, typeof(ParameterPack), allowSceneObjects: true);
                    if (node.Parameter != null)
                    {
                        bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 42, 82, 20);
                        node.ConditionNumber = EditorGUI.Popup(bufer, node.ConditionNumber, node.Parameter.GetCharacteristic());
                        bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                        if (node.Parameter.parametres[node.ConditionNumber].type == ParameterType.Bool)
                        {
                            node.CheckType = (CheckType)EditorGUI.Popup(bufer, (int)node.CheckType, new string[2] { "==", "!=" });
                            bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                            node.CheckBoolValue = EditorGUI.Toggle(bufer, node.CheckBoolValue);
                        }
                        else
                        {
                            node.CheckType = (CheckType)EditorGUI.Popup(bufer, (int)node.CheckType, new string[4] { "==", "!=", ">", "<" });
                            bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                            node.CheckIntValue = EditorGUI.IntField(bufer, node.CheckIntValue);
                        }
                    }
                }
                else
                {
                    bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + node.PositiveExitPointOffset.y, 30, 20);
                    if (GUI.Button(bufer, "<+"))
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 0;
                    }
                    bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + node.NegativeExitPointOffset.y, 30, 20);
                    if (GUI.Button(bufer, "<-"))
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 1;
                    }
                    bufer = new Rect(nodeTransform.x + 31, nodeTransform.y + 21, 130, 20);
                    node.Parameter = (ParameterPack)EditorGUI.ObjectField(bufer, node.Parameter, typeof(ParameterPack), allowSceneObjects: true);
                    if (node.Parameter != null)
                    {
                        bufer = new Rect(nodeTransform.x + 31, nodeTransform.y + 42, 82, 20);
                        node.ConditionNumber = EditorGUI.Popup(bufer, node.ConditionNumber, node.Parameter.GetCharacteristic());
                        bufer = new Rect(bufer.x + 81, bufer.y, 35, 20);
                        if (node.Parameter.parametres[node.ConditionNumber].type == ParameterType.Bool)
                        {
                            node.CheckType = (CheckType)EditorGUI.Popup(bufer, (int)node.CheckType, new string[2] { "==", "!=" });
                            bufer = new Rect(bufer.x + 36, bufer.y, 20, 20);
                            node.CheckBoolValue = EditorGUI.Toggle(bufer, node.CheckBoolValue);
                        }
                        else
                        {
                            node.CheckType = (CheckType)EditorGUI.Popup(bufer, (int)node.CheckType, new string[4] { "==", "!=", ">", "<" });
                            bufer = new Rect(bufer.x + 36, bufer.y, 30, 15);
                            node.CheckIntValue = EditorGUI.IntField(bufer, node.CheckIntValue);
                        }
                    }
                }
                break;
            case NodeType.Link:
                if (node.leftToRight)
                {
                    bufer = new Rect(nodeTransform.x + node.RightPointOffset.x, nodeTransform.y + node.RightPointOffset.y, 20, 20);
                    if (GUI.Button(bufer, ">")) //кнопка правый раъём
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 0;
                    }
                }
                else
                {
                    bufer = new Rect(nodeTransform.x + node.enterPointOffset.x, nodeTransform.y + node.enterPointOffset.y, 20, 20);
                    if (GUI.Button(bufer, "<"))
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = 0;
                    }
                }
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
                EditorGUI.DrawRect(bufer, Color.white);
                EditorGUI.LabelField(bufer, node.NextNodeNumber == -1? "пусто" : node.NextNodeNumber.ToString());
                break;
            case NodeType.RandomLink:
                if (node.leftToRight)
                {
                    bufer = new Rect(nodeTransform.x + node.RightPointOffset.x, nodeTransform.y + node.RightPointOffset.y, 20, 20);
                    if (GUI.Button(bufer, ">")) //кнопка правый раъём
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = -1;
                    }
                }
                else
                {
                    bufer = new Rect(nodeTransform.x + node.enterPointOffset.x, nodeTransform.y + node.enterPointOffset.y, 20, 20);
                    if (GUI.Button(bufer, "<"))
                    {
                        beginRelationNodeBufer = node;
                        exitBufer = -1;
                    }
                }
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 22, nodeTransform.width - 39, 20);
                EditorGUI.DrawRect(bufer, Color.white);
                EditorGUI.LabelField(bufer, node.NextNodeNumber == -1 ? "пусто" : node.NextNodeNumber.ToString());
                for (int i = 0; i < node.linkNextNodeNumbers.Count; i++)
                {
                    if (node.leftToRight)
                    {
                        bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 + 21 * (i + 1), 20, 20);
                        if (GUI.Button(bufer, "x")) //можно удалять отдельные варианты ответов
                        {
                            node.RemoveLinkNumber(i);
                            node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height - 22);
                            break;
                        }
                        bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                        EditorGUI.LabelField(bufer, node.linkNextNodeNumbers[i].link == -1 ? "пусто" : node.linkNextNodeNumbers[i].link.ToString());
                        bufer = new Rect(nodeTransform.x + node.ExitPointsOffset[i].x, nodeTransform.y + node.ExitPointsOffset[i].y, 20, 20);
                        if (GUI.Button(bufer, ">")) // также у каждого варианта ответа есть свой выходной разъём
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = i;
                        }
                    }
                    else
                    {
                        bufer = new Rect(nodeTransform.x + 1, nodeTransform.y + 21 + 21 * (i + 1), 20, 20);
                        if (GUI.Button(bufer, "<"))
                        {
                            beginRelationNodeBufer = node;
                            exitBufer = i;
                        }
                        bufer = new Rect(bufer.x + 21, bufer.y, nodeTransform.width - 70, 20);
                        EditorGUI.LabelField(bufer, node.linkNextNodeNumbers[i].link == -1 ? "пусто" : node.linkNextNodeNumbers[i].link.ToString());
                        bufer = new Rect(nodeTransform.x + node.ExitPointsOffset[i].x - 21, nodeTransform.y + node.ExitPointsOffset[i].y, 20, 20);
                        if (GUI.Button(bufer, "x"))
                        {
                            node.RemoveLinkNumber(i);
                            node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height - 22);
                            break;
                        }
                    }
                }
                bufer = new Rect(nodeTransform.x + 21, nodeTransform.y + 21 + 21 * (node.linkNextNodeNumbers.Count + 1), nodeTransform.width - 42, 20);
                if (GUI.Button(bufer, "+")) // кнопка для добавления варианта ответа
                {
                    node.linkNextNodeNumbers.Add(new DialogueNode.RandomLinlkPair(-1));
                    node.transformRect = new Rect(node.transformRect.x, node.transformRect.y, node.transformRect.width, node.transformRect.height + 22);
                    node.CheckExitOffsetForRandomLink();
                }
                break;
        }
    }
    
    private void AddRelation(DialogueNode node)
    {
        switch (beginRelationNodeBufer.Type)
        {
            case NodeType.Replica:
                if (beginRelationNodeBufer.NextNodeNumber != -1)
                {
                    sceneKit.RemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.NextNodeNumber]);
                }
                beginRelationNodeBufer.NextNodeNumber = node.index;
                sceneKit.AddInPreviousRelations(sceneKit.nodes[beginRelationNodeBufer.NextNodeNumber], beginRelationNodeBufer);
                break;
            case NodeType.Choice:
                if (sceneKit.nodes[beginRelationNodeBufer.AnswerChoice[exitBufer]].NextNodeNumber != -1)
                {
                    sceneKit.RemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.AnswerChoice[exitBufer]]);
                }
                sceneKit.nodes[beginRelationNodeBufer.AnswerChoice[exitBufer]].NextNodeNumber = node.index;
                sceneKit.AddInPreviousRelations(sceneKit.nodes[node.index], sceneKit.nodes[beginRelationNodeBufer.AnswerChoice[exitBufer]]);
                break;
            case NodeType.Event:
                if (beginRelationNodeBufer.NextNodeNumber != -1)
                {
                    sceneKit.RemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.NextNodeNumber]);
                }
                beginRelationNodeBufer.NextNodeNumber = node.index;
                sceneKit.AddInPreviousRelations(sceneKit.nodes[beginRelationNodeBufer.NextNodeNumber], beginRelationNodeBufer);
                break;
            case NodeType.Condition:
                if (exitBufer == 0)
                {
                    if (beginRelationNodeBufer.PositiveNextNumber != -1)
                    {
                        sceneKit.ConditionRemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.PositiveNextNumber], 0);
                    }
                    beginRelationNodeBufer.PositiveNextNumber = node.index;
                    sceneKit.AddInPreviousRelations(sceneKit.nodes[beginRelationNodeBufer.PositiveNextNumber], beginRelationNodeBufer);
                }
                else
                {
                    if (beginRelationNodeBufer.NegativeNextNumber != -1)
                    {
                        sceneKit.ConditionRemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.NegativeNextNumber], 1);
                    }
                    beginRelationNodeBufer.NegativeNextNumber = node.index;
                    sceneKit.AddInPreviousRelations(sceneKit.nodes[beginRelationNodeBufer.NegativeNextNumber], beginRelationNodeBufer);
                }
                break;
            case NodeType.Link:
                if (beginRelationNodeBufer.NextNodeNumber != -1)
                {
                    sceneKit.RemoveFromNext(beginRelationNodeBufer, sceneKit.nodes[beginRelationNodeBufer.NextNodeNumber]);
                }
                beginRelationNodeBufer.NextNodeNumber = node.index;
                sceneKit.AddInPreviousRelations(node, beginRelationNodeBufer);
                break;
            case NodeType.RandomLink:
                if(exitBufer == -1)
                {
                    beginRelationNodeBufer.NextNodeNumber = node.index;
                }
                else
                {
                    beginRelationNodeBufer.linkNextNodeNumbers[exitBufer].link = node.index;
                }
                sceneKit.AddInPreviousRelations(node, beginRelationNodeBufer);
                break;
        }
        beginRelationNodeBufer = null;
        exitBufer = 0;
    }
    private void DrawRelations()
    {
        for (int i = 0; i < sceneKit.nodes.Count; i++)
        {
            if(!sceneKit.nodes[i].Hide)
            {
                int startMultiplicator, endMultiplicator;
                startMultiplicator = endMultiplicator = 1;
                switch (sceneKit.nodes[i].Type)
                {
                    case NodeType.Replica:
                        if (sceneKit.nodes[i].NextNodeNumber != -1)
                        {
                            DialogueNode node = sceneKit.nodes[sceneKit.nodes[i].NextNodeNumber];

                            Vector2 startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].RightPointOffset.x, sceneKit.nodes[i].transformRect.y +
                                sceneKit.nodes[i].RightPointOffset.y);
                            if (!sceneKit.nodes[i].leftToRight)
                            {
                                startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].enterPointOffset.x, sceneKit.nodes[i].transformRect.y +
                                sceneKit.nodes[i].enterPointOffset.y);
                                startMultiplicator = -1;
                            }

                            Vector2 endPoint = new Vector2(node.transformRect.x + node.enterPointOffset.x, node.transformRect.y + node.enterPointOffset.y);
                            if (!node.leftToRight)
                            {
                                endPoint = new Vector2(node.transformRect.x + node.RightPointOffset.x, node.transformRect.y + node.RightPointOffset.y);
                                endMultiplicator = -1;
                            }

                            DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.white);
                        }
                        break;
                    case NodeType.Choice:
                        for (int j = 0; j < sceneKit.nodes[i].AnswerChoice.Count; j++)
                        {
                            if (sceneKit.nodes[sceneKit.nodes[i].AnswerChoice[j]].NextNodeNumber != -1)
                            {
                                DialogueNode node = sceneKit.nodes[sceneKit.nodes[sceneKit.nodes[i].AnswerChoice[j]].NextNodeNumber];
                                

                                Vector2 startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].ExitPointsOffset[j].x, sceneKit.nodes[i].transformRect.y +
                                    sceneKit.nodes[i].ExitPointsOffset[j].y);

                                if (!sceneKit.nodes[i].leftToRight)
                                {
                                    startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + 1, sceneKit.nodes[i].transformRect.y +
                                    sceneKit.nodes[i].ExitPointsOffset[j].y);
                                    startMultiplicator = -1;
                                }

                                Vector2 endPoint;

                                if (node.leftToRight)
                                {
                                    endPoint = new Vector2(node.transformRect.x - node.enterPointOffset.x, node.transformRect.y + node.enterPointOffset.y);
                                    endMultiplicator = 1;
                                }
                                else
                                {
                                    endPoint = new Vector2(node.transformRect.x + node.RightPointOffset.x, node.transformRect.y + node.RightPointOffset.y);
                                    endMultiplicator = -1;
                                }

                                DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.white);
                            }
                        }
                        break;
                    case NodeType.Event:
                        if (sceneKit.nodes[i].NextNodeNumber != -1)
                        {
                            DialogueNode node = sceneKit.nodes[sceneKit.nodes[i].NextNodeNumber];

                            Vector2 startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].RightPointOffset.x, sceneKit.nodes[i].transformRect.y +
                                sceneKit.nodes[i].RightPointOffset.y);
                            if (!sceneKit.nodes[i].leftToRight)
                            {
                                startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].enterPointOffset.x, sceneKit.nodes[i].transformRect.y +
                                sceneKit.nodes[i].enterPointOffset.y);
                                startMultiplicator = -1;
                            }

                            Vector2 endPoint = new Vector2(node.transformRect.x + node.enterPointOffset.x, node.transformRect.y + node.enterPointOffset.y);
                            if (!node.leftToRight)
                            {
                                endPoint = new Vector2(node.transformRect.x + node.RightPointOffset.x, node.transformRect.y + node.RightPointOffset.y);
                                endMultiplicator = -1;
                            }

                            DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.yellow);
                        }
                        break;
                    case NodeType.Condition:
                        if (sceneKit.nodes[i].PositiveNextNumber != -1)
                        {
                            DialogueNode node = sceneKit.nodes[sceneKit.nodes[i].PositiveNextNumber];

                            Vector2 startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].PositiveExitPointOffset.x,
                                sceneKit.nodes[i].transformRect.y + sceneKit.nodes[i].PositiveExitPointOffset.y);
                            if (!sceneKit.nodes[i].leftToRight)
                            {
                                startPoint = new Vector2(sceneKit.nodes[i].transformRect.x -1, sceneKit.nodes[i].transformRect.y +
                                    sceneKit.nodes[i].PositiveExitPointOffset.y);
                                startMultiplicator = -1;
                            }

                            Vector2 endPoint = new Vector2(node.transformRect.x + node.enterPointOffset.x, node.transformRect.y + node.enterPointOffset.y);
                            if (!node.leftToRight)
                            {
                                endPoint = new Vector2(node.transformRect.x + node.RightPointOffset.x, node.transformRect.y + node.RightPointOffset.y);
                                endMultiplicator = -1;
                            }

                            DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.green);
                        }
                        if (sceneKit.nodes[i].NegativeNextNumber != -1)
                        {
                            DialogueNode node = sceneKit.nodes[sceneKit.nodes[i].NegativeNextNumber];

                            Vector2 startPoint = new Vector2(sceneKit.nodes[i].transformRect.x + sceneKit.nodes[i].NegativeExitPointOffset.x,
                                sceneKit.nodes[i].transformRect.y + sceneKit.nodes[i].NegativeExitPointOffset.y);
                            if (!sceneKit.nodes[i].leftToRight)
                            {
                                startPoint = new Vector2(sceneKit.nodes[i].transformRect.x -1, sceneKit.nodes[i].transformRect.y +
                                    sceneKit.nodes[i].NegativeExitPointOffset.y);
                                startMultiplicator = -1;
                            }

                            Vector2 endPoint = new Vector2(node.transformRect.x + node.enterPointOffset.x, node.transformRect.y + node.enterPointOffset.y);
                            if (!node.leftToRight)
                            {
                                endPoint = new Vector2(node.transformRect.x + node.RightPointOffset.x, node.transformRect.y + node.RightPointOffset.y);
                                endMultiplicator = -1;
                            }

                            DrawCurve(startPoint, endPoint, startMultiplicator, endMultiplicator, Color.red);
                        }
                        break;
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
                startMultiplicator.x = endMultiplicator.x = Mathf.Abs(end.x - start.x)/2;
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

    private void CreateNode()
    {
        Vector2 pos = new Vector2(position.width / 2 + scrollPosition.x, position.height / 2 + scrollPosition.y);
        int index = sceneKit.nodes.Count;
        switch (nodeType)
        {
            case DialogueNodeType.Replica:
                sceneKit.nodes.Add(new ReplicaNode(pos, index));
                break;
            case DialogueNodeType.Choice:
                sceneKit.nodes.Add(new ChoiceNode(pos, index));
                break;
            case DialogueNodeType.Condition:
                sceneKit.nodes.Add(new ConditionNode(pos, index));
                break;
            case DialogueNodeType.Event:
                sceneKit.nodes.Add(new EventNode(pos, index));
                break;
            case DialogueNodeType.Link:
                sceneKit.nodes.Add(new LinkNode(pos, index));
                break;
            case DialogueNodeType.Randomizer:
                sceneKit.nodes.Add(new RandomizerNode(pos, index));
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Позволяет двигать узел
    /// </summary>
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
        for (int i = sceneKit.nodes.Count - 1; i >= 0; i--)
        {
            if(!sceneKit.nodes[i].Hide)
            {
                if (mousePos.x > sceneKit.nodes[i].transformRect.x - scrollPosition.x && mousePos.x < sceneKit.nodes[i].transformRect.x - scrollPosition.x
                + sceneKit.nodes[i].transformRect.width &&
                mousePos.y > sceneKit.nodes[i].transformRect.y - scrollPosition.y + mainInfoYSize && mousePos.y < sceneKit.nodes[i].transformRect.y - scrollPosition.y
                + mainInfoYSize + sceneKit.nodes[i].transformRect.height)
                {
                    node = sceneKit.nodes[i];
                    clickPoint = mousePos;
                    return true;
                }
            }
        }
        return false;
    }
    private bool ClickInNode(Vector2 mousePos)
    {
        for (int i = sceneKit.nodes.Count - 1; i >= 0; i--)
        {
            if(!sceneKit.nodes[i].Hide)
            {
                if (mousePos.x > sceneKit.nodes[i].transformRect.x - scrollPosition.x && mousePos.x < sceneKit.nodes[i].transformRect.x - scrollPosition.x
               + sceneKit.nodes[i].transformRect.width &&
               mousePos.y > sceneKit.nodes[i].transformRect.y - scrollPosition.y + mainInfoYSize && mousePos.y < sceneKit.nodes[i].transformRect.y - scrollPosition.y
               + mainInfoYSize + sceneKit.nodes[i].transformRect.height)
                {
                    clickPoint = mousePos;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Получает фактические размеры рабочего поля, исходя из расположения узлов
    /// </summary>
    /// <returns></returns>
    private Rect GetScrollViewZone()
    {
        Rect rezult = new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, scrollViewRect.height);
        float maxX, maxY;
        maxX = maxY = 0;

        foreach (var item in sceneKit.nodes)
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
    /// <summary>
    /// осуществляет прокрутку
    /// </summary>
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