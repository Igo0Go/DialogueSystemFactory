using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(DialogueScenePoint))]
public class DialogueScenePointEditor : Editor
{
    private DialogueScenePoint point;

    private Vector2 answersScroll = Vector2.zero;
    private Vector2 actorsPointScroll = Vector2.zero;
    private Vector2 actorsScroll = Vector2.zero;

    private bool MainMenu;
    private bool UIMenu;
    private bool SceneMenu;
    private bool ReactMenu;

    private void OnEnable()
    {
        point = (DialogueScenePoint)target;
    }

    public override void OnInspectorGUI()
    {
        MainMenu = EditorGUILayout.BeginFoldoutHeaderGroup(MainMenu, "Основное");
        if (MainMenu) DrawMainMenu();
        EditorGUILayout.EndFoldoutHeaderGroup();

        UIMenu = EditorGUILayout.BeginFoldoutHeaderGroup(UIMenu, "UI-элементы");
        if (UIMenu) DrawUIMenu();
        EditorGUILayout.EndFoldoutHeaderGroup();

        SceneMenu = EditorGUILayout.BeginFoldoutHeaderGroup(SceneMenu, "Постановка сцены");
        if (SceneMenu) DrawSceneMenu();
        EditorGUILayout.EndFoldoutHeaderGroup();

        ReactMenu = EditorGUILayout.BeginFoldoutHeaderGroup(ReactMenu, "Реакции в сцене");
        if (ReactMenu) DrawReactMenu();
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawFunctionButton();
    }

    private void DrawMainMenu()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Основное");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Контроллер:");
        EditorGUI.BeginChangeCheck();
        point.teamDirector = (DialogueTeamDirector)EditorGUILayout.ObjectField(point.teamDirector, typeof(DialogueTeamDirector),
            allowSceneObjects: true, GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён игровой контроллер для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Схема сцены:");

        EditorGUI.BeginChangeCheck();
        point.scene = (DialogueSceneKit)EditorGUILayout.ObjectField(point.scene, typeof(DialogueSceneKit),
            allowSceneObjects: true, GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменана схема сцены для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Текстовая подсказка:");
        EditorGUI.BeginChangeCheck();
        point.tipString = GUILayout.TextField(point.tipString, GUILayout.MinWidth(80), GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён текст подсказки для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Кнопка пропуска реплики:");
        EditorGUI.BeginChangeCheck();
        point.skipButton = (KeyCode)EditorGUILayout.EnumPopup(point.skipButton, GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменена кнопка пропуска реплики" + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
    private void DrawUIMenu()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("UI");
        point.dialogueUIController = (DialogueUIController)EditorGUILayout.ObjectField(point.dialogueUIController,
            typeof(DialogueUIController), allowSceneObjects: true, GUILayout.MaxWidth(120));
        GUILayout.EndVertical();
    }
    private void DrawSceneMenu()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Постановка сцены");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Игровая камера");
        EditorGUI.BeginChangeCheck();
        point.sceneCamera = (Transform)EditorGUILayout.ObjectField(point.sceneCamera, typeof(Transform),
          allowSceneObjects: true, GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменена камера для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Источник звука");
        EditorGUI.BeginChangeCheck();
        point.audioSource = (AudioSource)EditorGUILayout.ObjectField(point.audioSource, typeof(AudioSource),
           allowSceneObjects: true, GUILayout.MaxWidth(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён источник звука для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Диалоговые контроллеры персонажей:");
        if (GUILayout.Button("+", GUILayout.MinWidth(20), GUILayout.MaxWidth(20)))
        {
            if (point.actors == null)
                point.actors = new List<DialogueController>();
            point.actors.Add(null);
            Undo.RecordObject(point, "Добавлен контроллер персонажа для " + name);
            EditorUtility.SetDirty(point);
        }
        if (point.actors != null)
        {
            actorsScroll = GUILayout.BeginScrollView(actorsScroll, GUILayout.MaxHeight(point.actors.Count > 3 ? 60 : point.actors.Count * 20));

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < point.actors.Count; i++)
            {
                GUILayout.BeginHorizontal();
                point.actors[i] = (DialogueController)EditorGUILayout.ObjectField(point.actors[i], typeof(DialogueController),
                    allowSceneObjects: true);
                if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                {
                    point.actors.Remove(point.actors[i]);
                }
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(point, "Изменены контроллеры персонажей для" + name);
                EditorUtility.SetDirty(point);
            }


            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Точки персонажей:");
        if (GUILayout.Button("+", GUILayout.MinWidth(20), GUILayout.MaxWidth(20)))
        {
            if (point.actorsPoints == null)
                point.actorsPoints = new List<DialogueActorPointItem>();
            GameObject obj = new GameObject();
            obj.transform.position = point.transform.position;
            obj.transform.rotation = point.transform.rotation;
            obj.transform.parent = point.transform;
            obj.name = "ActorPont_" + (point.actorsPoints.Count + 1);
            point.actorsPoints.Add(new DialogueActorPointItem(obj.transform));
            Undo.RecordObject(point, "Добавлена точка персонажа");
            EditorUtility.SetDirty(point);
        }
        if (point.actorsPoints != null)
        {
            actorsPointScroll = GUILayout.BeginScrollView(actorsPointScroll,
                GUILayout.MaxHeight(point.actorsPoints.Count > 3 ? 60 : point.actorsPoints.Count * 20));
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < point.actorsPoints.Count; i++)
            {
                GUILayout.BeginHorizontal();
                point.actorsPoints[i].actorPoint = (Transform)EditorGUILayout.ObjectField(point.actorsPoints[i].actorPoint, typeof(Transform),
                    allowSceneObjects: true);
                point.actorsPoints[i].actorRole = (DialogueCharacter)EditorGUILayout.ObjectField(point.actorsPoints[i].actorRole, typeof(DialogueCharacter),
                    allowSceneObjects: true);

                if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                {
                    point.actorsPoints.Remove(point.actorsPoints[i]);
                }
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(point, "Изменены точки персонажей для" + name);
                EditorUtility.SetDirty(point);
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Точки камеры:");
        if (point.scene == null)
        {
            GUILayout.Label("Для подгрузки ракурсов требуется схема сцены");
            if (point.cameraPoints != null)
            {
                point.cameraPoints = null;
                Undo.RecordObject(point, "Изменены точки камеры для " + name);
                EditorUtility.SetDirty(point);
            }
        }
        else
        {
            if (point.cameraPoints == null)
            {
                EditorGUI.BeginChangeCheck();
                point.cameraPoints = new List<Transform>();
                for (int i = 0; i < point.scene.camerasPositions.Count; i++)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position = point.transform.position;
                    obj.transform.rotation = point.transform.rotation;
                    obj.transform.parent = point.transform;
                    obj.name = "CameraPosition" + (i + 1);
                    point.cameraPoints.Add(obj.transform);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(point, "Изменены точки камеры для" + name);
                    EditorUtility.SetDirty(point);
                }
            }

            if (GUILayout.Button("Создать точки для камеры в сцене", GUILayout.MinHeight(20), GUILayout.MaxHeight(20), GUILayout.MinWidth(80)))
            {
                point.cameraPoints = new List<Transform>();
                for (int i = 0; i < point.scene.camerasPositions.Count; i++)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position = point.transform.position;
                    obj.transform.rotation = point.transform.rotation;
                    obj.transform.parent = point.transform;
                    obj.name = "CameraPosition" + (i + 1);
                    point.cameraPoints.Add(obj.transform);
                }
                Undo.RecordObject(point, "Изменены точки камеры для " + name);
                EditorUtility.SetDirty(point);
            }
            GUILayout.Space(10);
            if (point.cameraPoints.Count < point.scene.camerasPositions.Count)
            {
                EditorGUI.BeginChangeCheck();
                point.cameraPoints = new List<Transform>();
                for (int i = 0; i < point.scene.camerasPositions.Count; i++)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position = point.transform.position;
                    obj.transform.rotation = point.transform.rotation;
                    obj.transform.parent = point.transform;
                    obj.name = "CameraPosition" + (i + 1);
                    point.cameraPoints.Add(obj.transform);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(point, "Изменены точки камеры для" + name);
                    EditorUtility.SetDirty(point);
                }
            }

            for (int i = 0; i < point.scene.camerasPositions.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(point.scene.camerasPositions[i], GUILayout.MinWidth(60), GUILayout.MaxWidth(200));
                point.cameraPoints[i] = (Transform)EditorGUILayout.ObjectField(point.cameraPoints[i], typeof(Transform),
                    allowSceneObjects: true);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }
    private void DrawReactMenu()
    {
        GUILayout.BeginVertical();

        if (point.scene == null)
        {
            GUILayout.Label("Для подгрузки событий требуется схема сцены");
        }
        else
        {
            if (point.reactors == null || point.reactors.Count == 0)
            {
                point.reactors = new List<DialogueEventReactor>();
                for (int i = 0; i < point.scene.inSceneInvokeObjects.Count; i++)
                {
                    point.reactors.Add(null);
                }
                Undo.RecordObject(point, "Изменены реакторы событий для " + name);
                EditorUtility.SetDirty(point);
            }
            else
            {
                if(point.scene.inSceneInvokeObjects.Count == 0)
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Диалог не содержит событий", GUILayout.MinWidth(60), GUILayout.MaxWidth(200));
                    GUILayout.Space(20);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    for (int i = 0; i < point.scene.inSceneInvokeObjects.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(point.scene.inSceneInvokeObjects[i], GUILayout.MinWidth(60), GUILayout.MaxWidth(200));
                        point.reactors[i] = (DialogueEventReactor)EditorGUILayout.ObjectField(point.reactors[i], typeof(DialogueEventReactor),
                            allowSceneObjects: true);
                        GUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(point, "Изменены реакторы событий для " + name);
                        EditorUtility.SetDirty(point);
                    }
                }
            }
        }
        GUILayout.EndVertical();
    }
    private void DrawFunctionButton()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Одноразовый диалог");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.once = EditorGUILayout.Toggle(point.once);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр одноразовый для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Режим дебага");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.debug = EditorGUILayout.Toggle(point.debug);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр дебаг для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Использовать озвучку в диалоге");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.useVoice = EditorGUILayout.Toggle(point.useVoice);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр озвучка для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Использовать анимации");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.useAnimations = EditorGUILayout.Toggle(point.useAnimations);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр использовать анимации для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Поддержка сети");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.useNetwork = EditorGUILayout.Toggle(point.useNetwork);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр использование сети для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Телепортировтаь игроков на позиции");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        point.teleportPlayersToPositions = EditorGUILayout.Toggle(point.teleportPlayersToPositions);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Изменён параметр телепортировтаь игроков на позиции для " + name);
            EditorUtility.SetDirty(point);
        }
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Вернуть доступ случайных узлов"))
        {
            point.ReturnAccessForAllRandomizerNodes();
            EditorUtility.SetDirty(point);
        }

        GUILayout.EndVertical();
    }
}
