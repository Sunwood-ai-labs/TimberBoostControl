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

            var settings = settingsStore.Load();
            var generator = new TimberBoostControlGenerator();
            var generationResult = generator.Generate(settings);
            if (generationResult.Success)
            {
                Debug.Log(string.Format("TimberBoostControl regenerated {0} blueprint file(s) from {1}.", generationResult.FileCount, ModContext.SettingsPath));
            }
            else
            {
                Debug.LogError(string.Format("TimberBoostControl failed to regenerate blueprints: {0}", generationResult.Message));
            }

            Debug.Log(string.Format("TimberBoostControl initialized in {0}", ModContext.ModDirectoryPath));
        }
    }
}
