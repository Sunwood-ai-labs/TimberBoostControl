using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlGenerator
    {
        private static readonly string[] CharacterFiles =
        {
            "Characters/Beaver/BeaverAdult.blueprint.json",
            "Characters/Bot/Bot.Folktails.blueprint.json",
            "Characters/Bot/Bot.IronTeeth.blueprint.json"
        };

        public GenerationResult Generate(TimberBoostControlSettings settings)
        {
            if (!ModContext.IsInitialized)
            {
                return GenerationResult.Fail("The mod directory is not initialized yet.");
            }

            if (settings == null)
            {
                settings = new TimberBoostControlSettings();
            }

            settings.Normalize();

            var blueprintsZipPath = Path.Combine(Application.dataPath, "StreamingAssets", "Modding", "Blueprints.zip");
            if (!File.Exists(blueprintsZipPath))
            {
                return GenerationResult.Fail("Could not find the base Blueprints.zip file.");
            }

            var previousFiles = ReadTrackedFiles();
            DeleteTrackedFiles(previousFiles);

            var generatedFiles = new List<string>();

            using (var archive = ZipFile.OpenRead(blueprintsZipPath))
            {
                GenerateCharacterFiles(archive, settings, generatedFiles);
                GenerateBuildingFiles(archive, settings, generatedFiles);
            }

            WriteTrackedFiles(generatedFiles);
            return GenerationResult.Ok(generatedFiles.Count);
        }

        private static void GenerateCharacterFiles(ZipArchive archive, TimberBoostControlSettings settings, ICollection<string> generatedFiles)
        {
            if (settings.CarryMultiplier <= 1 && settings.MoveSpeedPercent == 100)
            {
                return;
            }

            foreach (var relativePath in CharacterFiles)
            {
                var source = ReadJson(archive, relativePath);
                if (source == null)
                {
                    continue;
                }

                var output = new JObject();

                if (settings.CarryMultiplier > 1)
                {
                    var baseLiftingCapacity = GetIntValue(source, "GoodCarrierSpec.BaseLiftingCapacity");
                    if (baseLiftingCapacity.HasValue)
                    {
                        var goodCarrierSpec = new JObject();
                        goodCarrierSpec["BaseLiftingCapacity"] = baseLiftingCapacity.Value * settings.CarryMultiplier;
                        output["GoodCarrierSpec"] = goodCarrierSpec;
                    }
                }

                if (settings.MoveSpeedPercent != 100)
                {
                    var walker = new JObject();
                    var baseWalkingSpeed = GetDoubleValue(source, "WalkerSpeedManagerSpec.BaseWalkingSpeed");
                    var baseSlowedSpeed = GetDoubleValue(source, "WalkerSpeedManagerSpec.BaseSlowedSpeed");

                    if (baseWalkingSpeed.HasValue)
                    {
                        walker["BaseWalkingSpeed"] = RoundWithTwoDecimalPlaces(baseWalkingSpeed.Value * settings.MoveSpeedPercent / 100.0);
                    }

                    if (baseSlowedSpeed.HasValue)
                    {
                        walker["BaseSlowedSpeed"] = RoundWithTwoDecimalPlaces(baseSlowedSpeed.Value * settings.MoveSpeedPercent / 100.0);
                    }

                    if (walker.HasValues)
                    {
                        output["WalkerSpeedManagerSpec"] = walker;
                    }
                }

                if (output.HasValues)
                {
                    WriteGeneratedJson(relativePath, output);
                    generatedFiles.Add(relativePath);
                }
            }
        }

        private static void GenerateBuildingFiles(ZipArchive archive, TimberBoostControlSettings settings, ICollection<string> generatedFiles)
        {
            var hasBuildingTweaks = settings.BuildCostPercent != 100 ||
                                    settings.ScienceCostPercent != 100 ||
                                    settings.FactoryWorkerMultiplier > 1 ||
                                    settings.PowerInputPercent != 100 ||
                                    settings.StorageMultiplier > 1;

            if (!hasBuildingTweaks)
            {
                return;
            }

            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(".blueprint.json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!entry.FullName.StartsWith("Buildings/", StringComparison.Ordinal))
                {
                    continue;
                }

                using (var reader = new StreamReader(entry.Open()))
                {
                    var text = reader.ReadToEnd();
                    JObject source;
                    try
                    {
                        source = JObject.Parse(text);
                    }
                    catch (JsonException)
                    {
                        continue;
                    }

                    var output = new JObject();

                    if (settings.BuildCostPercent != 100 || settings.ScienceCostPercent != 100)
                    {
                        var buildingSpec = new JObject();
                        var buildingSpecSource = source["BuildingSpec"] as JObject;

                        if (settings.BuildCostPercent != 100 && buildingSpecSource != null)
                        {
                            var buildingCost = buildingSpecSource["BuildingCost"] as JArray;
                            if (buildingCost != null && buildingCost.Count > 0)
                            {
                                var replacementCost = new JArray();
                                foreach (var cost in buildingCost)
                                {
                                    var amountToken = cost["Amount"];
                                    var amount = amountToken != null ? amountToken.Value<int>() : 0;
                                    var idToken = cost["Id"];
                                    var newAmount = amount <= 0 ? 0 : (int)Math.Ceiling(amount * (settings.BuildCostPercent / 100.0));
                                    if (settings.BuildCostPercent >= 100)
                                    {
                                        newAmount = amount;
                                    }

                                    if (settings.BuildCostPercent == 0 && amount > 0)
                                    {
                                        newAmount = 0;
                                    }
                                    var replacementEntry = new JObject();
                                    replacementEntry["Id"] = idToken != null ? idToken.Value<string>() : string.Empty;
                                    replacementEntry["Amount"] = newAmount;
                                    replacementCost.Add(replacementEntry);
                                }

                                if (replacementCost.Count > 0)
                                {
                                    buildingSpec["BuildingCost"] = replacementCost;
                                }
                            }
                        }

                        if (settings.ScienceCostPercent != 100 && buildingSpecSource != null)
                        {
                            var scienceToken = buildingSpecSource["ScienceCost"];
                            var scienceCost = scienceToken != null ? (int?)scienceToken.Value<int>() : null;
                            if (scienceCost.HasValue)
                            {
                                var newScienceCost = (int)Math.Round(scienceCost.Value * (settings.ScienceCostPercent / 100.0), MidpointRounding.AwayFromZero);
                                buildingSpec["ScienceCost"] = Math.Max(0, newScienceCost);
                            }
                        }

                        if (buildingSpec.HasValues)
                        {
                            output["BuildingSpec"] = buildingSpec;
                        }
                    }

                    if (settings.FactoryWorkerMultiplier > 1 &&
                        source["WorkplaceSpec"] != null &&
                        source["WorkplaceSpec"].Type == JTokenType.Object &&
                        (source["ManufactorySpec"] != null || source["WorkshopSpec"] != null))
                    {
                        var workplaceSpec = new JObject();
                        var workplaceSource = (JObject)source["WorkplaceSpec"];
                        var maxWorkersToken = workplaceSource["MaxWorkers"];
                        var defaultWorkersToken = workplaceSource["DefaultWorkers"];
                        var maxWorkers = maxWorkersToken != null ? (int?)maxWorkersToken.Value<int>() : null;
                        var defaultWorkers = defaultWorkersToken != null ? (int?)defaultWorkersToken.Value<int>() : null;
                        var targetMaxWorkers = GetExpandedWorkerCount(source, maxWorkers, settings.FactoryWorkerMultiplier);
                        var targetDefaultWorkers = GetExpandedWorkerCount(source, defaultWorkers, settings.FactoryWorkerMultiplier);

                        if (targetMaxWorkers.HasValue)
                        {
                            workplaceSpec["MaxWorkers"] = targetMaxWorkers.Value;
                        }

                        if (targetDefaultWorkers.HasValue)
                        {
                            workplaceSpec["DefaultWorkers"] = targetMaxWorkers.HasValue
                                ? Math.Min(targetDefaultWorkers.Value, targetMaxWorkers.Value)
                                : targetDefaultWorkers.Value;
                        }

                        if (workplaceSpec.HasValues)
                        {
                            output["WorkplaceSpec"] = workplaceSpec;
                        }
                    }

                    if (settings.PowerInputPercent != 100)
                    {
                        var powerInput = GetDoubleValue(source, "MechanicalNodeSpec.PowerInput");
                        if (powerInput.HasValue && powerInput.Value > 0)
                        {
                            var mechanicalNodeSpec = new JObject();
                            var newPowerInput = (int)Math.Ceiling(powerInput.Value * (settings.PowerInputPercent / 100.0));
                            if (newPowerInput > 0)
                            {
                                mechanicalNodeSpec["PowerInput"] = Math.Max(1, newPowerInput);
                            }

                            if (mechanicalNodeSpec.HasValues)
                            {
                                output["MechanicalNodeSpec"] = mechanicalNodeSpec;
                            }
                        }
                    }

                    if (settings.StorageMultiplier > 1)
                    {
                        var maxCapacity = GetIntValue(source, "StockpileSpec.MaxCapacity");
                        if (maxCapacity.HasValue)
                        {
                            var stockpileSpec = new JObject();
                            stockpileSpec["MaxCapacity"] = maxCapacity.Value * settings.StorageMultiplier;
                            output["StockpileSpec"] = stockpileSpec;
                        }
                    }

                    if (output.HasValues)
                    {
                        WriteGeneratedJson(entry.FullName, output);
                        generatedFiles.Add(entry.FullName);
                    }
                }
            }
        }

        private static JObject ReadJson(ZipArchive archive, string entryName)
        {
            var entry = archive.GetEntry(entryName);
            if (entry == null)
            {
                return null;
            }

            using (var reader = new StreamReader(entry.Open()))
            {
                return JObject.Parse(reader.ReadToEnd());
            }
        }

        private static void WriteGeneratedJson(string relativePath, JObject content)
        {
            var fullPath = ModContext.ResolveModFile(relativePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(fullPath, content.ToString(Formatting.Indented) + Environment.NewLine);
        }

        private static int? GetIntValue(JObject source, string path)
        {
            var token = source.SelectToken(path);
            if (token == null)
            {
                return null;
            }

            return token.Value<int>();
        }

        private static double? GetDoubleValue(JObject source, string path)
        {
            var token = source.SelectToken(path);
            if (token == null)
            {
                return null;
            }

            return token.Value<double>();
        }

        private static int? GetExpandedWorkerCount(JObject source, int? originalWorkers, int multiplier)
        {
            if (!originalWorkers.HasValue)
            {
                return null;
            }

            var targetWorkers = originalWorkers.Value * multiplier;
            var capacityLimit = GetFinishedCapacityLimit(source);
            if (capacityLimit.HasValue)
            {
                targetWorkers = Math.Min(targetWorkers, capacityLimit.Value);
            }

            return targetWorkers > originalWorkers.Value ? (int?)targetWorkers : null;
        }

        private static double RoundWithTwoDecimalPlaces(double value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }

        private static int? GetFinishedCapacityLimit(JObject source)
        {
            var enterableSpec = source["EnterableSpec"] as JObject;
            if (enterableSpec == null)
            {
                return null;
            }

            var limitedCapacityToken = enterableSpec["LimitedCapacityFinished"];
            var capacityToken = enterableSpec["CapacityFinished"];
            if (limitedCapacityToken == null || capacityToken == null || !limitedCapacityToken.Value<bool>())
            {
                return null;
            }

            return capacityToken.Value<int>();
        }

        private static List<string> ReadTrackedFiles()
        {
            if (!File.Exists(ModContext.GeneratedFilesPath))
            {
                return new List<string>();
            }

            return new List<string>(File.ReadAllLines(ModContext.GeneratedFilesPath));
        }

        private static void WriteTrackedFiles(IReadOnlyCollection<string> generatedFiles)
        {
            if (generatedFiles.Count == 0)
            {
                if (File.Exists(ModContext.GeneratedFilesPath))
                {
                    File.Delete(ModContext.GeneratedFilesPath);
                }

                return;
            }

            File.WriteAllLines(ModContext.GeneratedFilesPath, generatedFiles);
        }

        private static void DeleteTrackedFiles(IEnumerable<string> trackedFiles)
        {
            foreach (var relativePath in trackedFiles)
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    continue;
                }

                var fullPath = ModContext.ResolveModFile(relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    DeleteEmptyParentDirectories(Path.GetDirectoryName(fullPath));
                }
            }
        }

        private static void DeleteEmptyParentDirectories(string directoryPath)
        {
            var current = directoryPath;
            while (!string.IsNullOrEmpty(current) &&
                   !string.Equals(current, ModContext.ModDirectoryPath, StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists(current) && Directory.GetFileSystemEntries(current).Length == 0)
                {
                    Directory.Delete(current);
                    current = Path.GetDirectoryName(current);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public sealed class GenerationResult
    {
        private readonly bool _success;
        private readonly int _fileCount;
        private readonly string _message;

        private GenerationResult(bool success, int fileCount, string message)
        {
            _success = success;
            _fileCount = fileCount;
            _message = message;
        }

        public bool Success
        {
            get { return _success; }
        }

        public int FileCount
        {
            get { return _fileCount; }
        }

        public string Message
        {
            get { return _message; }
        }

        public static GenerationResult Ok(int fileCount)
        {
            return new GenerationResult(true, fileCount, string.Format("Generated {0} blueprint file(s).", fileCount));
        }

        public static GenerationResult Fail(string message)
        {
            return new GenerationResult(false, 0, message);
        }
    }
}
