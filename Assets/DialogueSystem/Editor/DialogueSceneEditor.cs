using UnityEditor;

public class DialogueSceneEditor : EditorWindow
{
    private DialogueSceneKit scene;

    public static DialogueSceneEditor GetEditor(DialogueSceneKit sceneKit)
    {
        DialogueSceneEditor window = GetWindow<DialogueSceneEditor>();
        window.scene = sceneKit;
        return window;
    }
}
