using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlSettingsStore
    {
        public TimberBoostControlSettings Load()
        {
            if (!ModContext.IsInitialized || !File.Exists(ModContext.SettingsPath))
            {
                return new TimberBoostControlSettings();
            }

            try
            {
                var json = File.ReadAllText(ModContext.SettingsPath);
                var settings = JsonConvert.DeserializeObject<TimberBoostControlSettings>(json) ?? new TimberBoostControlSettings();
                settings.Normalize();

                var legacy = JObject.Parse(json);
                ApplyLegacyBooleanPresets(settings, legacy);
                return settings;
            }
            catch (JsonException)
            {
                return new TimberBoostControlSettings();
            }
            catch (IOException)
            {
                return new TimberBoostControlSettings();
            }
        }

        public void Save(TimberBoostControlSettings settings)
        {
            if (!ModContext.IsInitialized)
            {
                return;
            }

            if (settings == null)
            {
                settings = new TimberBoostControlSettings();
            }

            settings.Normalize();
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ModContext.SettingsPath, json + System.Environment.NewLine);
        }

        private static void ApplyLegacyBooleanPresets(TimberBoostControlSettings settings, JObject legacy)
        {
            if (legacy == null)
            {
                return;
            }

            var carryTenX = legacy["CarryTenX"];
            if (carryTenX != null && carryTenX.Type == JTokenType.Boolean)
            {
                settings.CarryMultiplier = carryTenX.Value<bool>()
                    ? TimberBoostControlSettings.DefaultCarryMultiplier
                    : 1;
            }

            var moveTwoX = legacy["MoveTwoX"];
            if (moveTwoX != null && moveTwoX.Type == JTokenType.Boolean)
            {
                settings.MoveSpeedPercent = moveTwoX.Value<bool>()
                    ? TimberBoostControlSettings.DefaultMoveSpeedPercent
                    : 100;
            }

            var storageTenX = legacy["StorageTenX"];
            if (storageTenX != null && storageTenX.Type == JTokenType.Boolean)
            {
                settings.StorageMultiplier = storageTenX.Value<bool>()
                    ? TimberBoostControlSettings.DefaultStorageMultiplier
                    : 1;
            }

            var buildCostTenth = legacy["BuildCostTenth"];
            if (buildCostTenth != null && buildCostTenth.Type == JTokenType.Boolean)
            {
                settings.BuildCostPercent = buildCostTenth.Value<bool>()
                    ? TimberBoostControlSettings.DefaultBuildCostPercent
                    : 100;
            }

            var freeScience = legacy["FreeScience"];
            if (freeScience != null && freeScience.Type == JTokenType.Boolean)
            {
                settings.ScienceCostPercent = freeScience.Value<bool>()
                    ? TimberBoostControlSettings.DefaultScienceCostPercent
                    : 100;
            }

            var doubleFactoryWorkers = legacy["DoubleFactoryWorkers"];
            if (doubleFactoryWorkers != null && doubleFactoryWorkers.Type == JTokenType.Boolean)
            {
                settings.FactoryWorkerMultiplier = doubleFactoryWorkers.Value<bool>()
                    ? TimberBoostControlSettings.DefaultFactoryWorkerMultiplier
                    : 1;
            }

            var powerTenth = legacy["PowerTenth"];
            if (powerTenth != null && powerTenth.Type == JTokenType.Boolean)
            {
                settings.PowerInputPercent = powerTenth.Value<bool>()
                    ? TimberBoostControlSettings.DefaultPowerInputPercent
                    : 100;
            }

            settings.Normalize();
        }
    }
}
