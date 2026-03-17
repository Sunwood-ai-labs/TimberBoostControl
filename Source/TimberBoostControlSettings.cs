using Newtonsoft.Json;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlSettings
    {
        public bool CarryTenX { get; set; }

        public bool MoveTwoX { get; set; }

        public bool StorageTenX { get; set; }

        public bool BuildCostTenth { get; set; }

        public bool FreeScience { get; set; }

        public bool DoubleFactoryWorkers { get; set; }

        public bool PowerTenth { get; set; }

        public bool PanelCollapsed { get; set; }

        [JsonIgnore]
        public int EnabledCount
        {
            get
            {
                var count = 0;
                count += CarryTenX ? 1 : 0;
                count += MoveTwoX ? 1 : 0;
                count += StorageTenX ? 1 : 0;
                count += BuildCostTenth ? 1 : 0;
                count += FreeScience ? 1 : 0;
                count += DoubleFactoryWorkers ? 1 : 0;
                count += PowerTenth ? 1 : 0;
                return count;
            }
        }
    }
}
