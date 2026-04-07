using UnityEditor;

public class ExampleEditorScript
{
    [MenuItem("Gravinium/Jilwer/Test")]
    static void Test()
    {
        EditorUtility.DisplayDialog("Example Script", "Opened This Dialog", "OK");
    }
}
