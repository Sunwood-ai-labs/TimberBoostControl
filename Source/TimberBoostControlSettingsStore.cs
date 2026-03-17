using System.IO;
using Newtonsoft.Json;

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
                return JsonConvert.DeserializeObject<TimberBoostControlSettings>(json) ?? new TimberBoostControlSettings();
            }
            catch (JsonException)
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

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ModContext.SettingsPath, json + System.Environment.NewLine);
        }
    }
}
