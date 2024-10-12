using UnityEditor;
using UnityEngine;

public static class StylePack
{
    public static GUIStyle startNodeStyle;
    public static GUIStyle nodeStyleReplica_default;
    public static GUIStyle nodeStyleReplica_selected;
    public static GUIStyle inPointStyle;
    public static GUIStyle outPointStyle;
    public static GUIStyle blackBoxStyle;

    static StylePack()
    {
        startNodeStyle = new GUIStyle();
        startNodeStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
        startNodeStyle.border = new RectOffset(0, 0, 0, 0);
        startNodeStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node5 on.png") as Texture2D;
        startNodeStyle.border = new RectOffset(0, 0, 0, 0);

        nodeStyleReplica_default = new GUIStyle();
        nodeStyleReplica_default.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyleReplica_default.border = new RectOffset(12, 12, 12, 12);

        nodeStyleReplica_selected = new GUIStyle();
        nodeStyleReplica_selected.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D;
        nodeStyleReplica_selected.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 4, 4);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        blackBoxStyle = new GUIStyle();
        blackBoxStyle.normal.background =
    EditorGUIUtility.Load("builtin skins/darkskin/images/text area") as Texture2D;
        blackBoxStyle.active.background =
            EditorGUIUtility.Load("builtin skins/darkskin/images/text area") as Texture2D;
        blackBoxStyle.border = new RectOffset(4, 4, 12, 12);
    }
}
