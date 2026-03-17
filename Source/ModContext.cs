using System.IO;

namespace Mods.TimberBoostControl
{
    internal static class ModContext
    {
        static ModContext()
        {
            ModDirectoryPath = string.Empty;
        }

        public static string ModDirectoryPath { get; private set; }

        public static bool IsInitialized
        {
            get { return !string.IsNullOrWhiteSpace(ModDirectoryPath); }
        }

        public static string SettingsPath
        {
            get { return Path.Combine(ModDirectoryPath, "settings.json"); }
        }

        public static string GeneratedFilesPath
        {
            get { return Path.Combine(ModDirectoryPath, ".generated-files.txt"); }
        }

        public static void Initialize(DirectoryInfo modDirectory)
        {
            ModDirectoryPath = modDirectory.FullName;
        }

        public static string ResolveModFile(string relativePath)
        {
            return Path.Combine(ModDirectoryPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
