using Newtonsoft.Json;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlSettings
    {
        public const int DefaultCarryMultiplier = 10;
        public const int DefaultMoveSpeedPercent = 200;
        public const int DefaultStorageMultiplier = 10;
        public const int DefaultBuildCostPercent = 10;
        public const int DefaultScienceCostPercent = 0;
        public const int DefaultFactoryWorkerMultiplier = 2;
        public const int DefaultPowerInputPercent = 10;

        public TimberBoostControlSettings()
        {
            CarryMultiplier = DefaultCarryMultiplier;
            MoveSpeedPercent = DefaultMoveSpeedPercent;
            StorageMultiplier = DefaultStorageMultiplier;
            BuildCostPercent = DefaultBuildCostPercent;
            ScienceCostPercent = DefaultScienceCostPercent;
            FactoryWorkerMultiplier = DefaultFactoryWorkerMultiplier;
            PowerInputPercent = DefaultPowerInputPercent;
        }

        public int CarryMultiplier { get; set; }

        public int MoveSpeedPercent { get; set; }

        public int StorageMultiplier { get; set; }

        public int BuildCostPercent { get; set; }

        public int ScienceCostPercent { get; set; }

        public int FactoryWorkerMultiplier { get; set; }

        public int PowerInputPercent { get; set; }

        [JsonIgnore]
        public int EnabledCount
        {
            get
            {
                var count = 0;
                count += CarryMultiplier > 1 ? 1 : 0;
                count += MoveSpeedPercent != 100 ? 1 : 0;
                count += StorageMultiplier > 1 ? 1 : 0;
                count += BuildCostPercent != 100 ? 1 : 0;
                count += ScienceCostPercent != 100 ? 1 : 0;
                count += FactoryWorkerMultiplier > 1 ? 1 : 0;
                count += PowerInputPercent != 100 ? 1 : 0;
                return count;
            }
        }

        public void Normalize()
        {
            CarryMultiplier = ClampAtLeast(CarryMultiplier, 1);
            MoveSpeedPercent = ClampAtLeast(MoveSpeedPercent, 1);
            StorageMultiplier = ClampAtLeast(StorageMultiplier, 1);
            BuildCostPercent = ClampAtLeast(BuildCostPercent, 0);
            ScienceCostPercent = ClampAtLeast(ScienceCostPercent, 0);
            FactoryWorkerMultiplier = ClampAtLeast(FactoryWorkerMultiplier, 1);
            PowerInputPercent = ClampAtLeast(PowerInputPercent, 1);
        }

        private static int ClampAtLeast(int value, int minimum)
        {
            return value < minimum ? minimum : value;
        }
    }
}
