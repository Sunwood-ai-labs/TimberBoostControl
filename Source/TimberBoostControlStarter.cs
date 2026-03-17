using System.IO;
using Timberborn.ModManagerScene;
using UnityEngine;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlStarter : IModStarter
    {
        public void StartMod(IModEnvironment modEnvironment)
        {
            var modPath = modEnvironment.ModPath;
            if (string.IsNullOrWhiteSpace(modPath))
            {
                modPath = modEnvironment.OriginPath;
            }

            var modDirectory = new DirectoryInfo(string.IsNullOrWhiteSpace(modPath) ? "." : modPath);
            ModContext.Initialize(modDirectory);

            var settingsStore = new TimberBoostControlSettingsStore();
            if (!File.Exists(ModContext.SettingsPath))
            {
                settingsStore.Save(new TimberBoostControlSettings());
            }

            Debug.Log(string.Format("TimberBoostControl initialized in {0}", ModContext.ModDirectoryPath));
        }
    }
}
