using UnityEditor;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    public static class Help
    {
        [MenuItem("Gravinium/Jilwer/Help/About", priority = 0)]
        private static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "About Jilwer",
                $"Jilwer ({JilwerInfo.FullName})"+
                $"\nVersion {JilwerInfo.Version}"+
                $"\n\nCreated by Gravinium",
                "OK");
        }
        
        [MenuItem("Gravinium/Jilwer/Help/Documentation", priority = 10)]
        private static void OpenDocumentation()
        {
            Application.OpenURL("https://docs.gravinium.org/projects/jilwer");
        }

        [MenuItem("Gravinium/Jilwer/Help/GitHub Repository", priority = 20)]
        private static void OpenGitHub()
        {
            Application.OpenURL("https://github.com/Gravinium-VR/Jilwer");
        }

        [MenuItem("Gravinium/Jilwer/Help/Report a Bug", priority = 30)]
        private static void ReportBug()
        {
            Application.OpenURL("https://github.com/Gravinium-VR/Jilwer/issues/new");
        }

        [MenuItem("Gravinium/Jilwer/Help/Discord", priority = 40)]
        private static void OpenDiscord()
        {
            Application.OpenURL("https://gravinium.org/discord");
        }
    }
}