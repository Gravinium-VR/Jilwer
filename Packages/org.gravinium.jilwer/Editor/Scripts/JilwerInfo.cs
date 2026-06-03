using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Gravinium.Jilwer.Editor
{
    public static class JilwerInfo
    {
        private const string PackagePath = "Packages/org.gravinium.jilwer";
        
        public static string Version
        {
            get
            {
                var package = PackageInfo.FindForAssetPath(PackagePath);
                return package?.version ?? "Unknown";
            }
        }

        public static string FullName
        {
            get
            {
                var package = PackageInfo.FindForAssetPath(PackagePath);
                return package?.name ?? "Unknown";
            }
        }
    }
}