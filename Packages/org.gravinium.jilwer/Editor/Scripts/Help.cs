using UnityEditor;

namespace Org.Gravinium.Jilwer.Editor
{
    public static class Help
    {
        [MenuItem("Gravinium/Jilwer/Help")]
        static void ShowDialog()
        {
            EditorUtility.DisplayDialog("Jilwer Help", "Coming Soon!", "OK");
        }
    }
}