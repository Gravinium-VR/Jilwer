using UnityEditor;

public class ExampleEditorScript
{
    [MenuItem("Gravinium/Jilwer/Help")]
    static void Test()
    {
        EditorUtility.DisplayDialog("Jilwer Help", "Good luck...", "OK");
    }
}
