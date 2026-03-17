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
            if (!settings.CarryTenX && !settings.MoveTwoX)
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

                if (settings.CarryTenX)
                {
                    var baseLiftingCapacity = GetIntValue(source, "GoodCarrierSpec.BaseLiftingCapacity");
                    if (baseLiftingCapacity.HasValue)
                    {
                        var goodCarrierSpec = new JObject();
                        goodCarrierSpec["BaseLiftingCapacity"] = baseLiftingCapacity.Value * 10;
                        output["GoodCarrierSpec"] = goodCarrierSpec;
                    }
                }

                if (settings.MoveTwoX)
                {
                    var walker = new JObject();
                    var baseWalkingSpeed = GetDoubleValue(source, "WalkerSpeedManagerSpec.BaseWalkingSpeed");
                    var baseSlowedSpeed = GetDoubleValue(source, "WalkerSpeedManagerSpec.BaseSlowedSpeed");

                    if (baseWalkingSpeed.HasValue)
                    {
                        walker["BaseWalkingSpeed"] = Math.Round(baseWalkingSpeed.Value * 2.0, 3);
                    }

                    if (baseSlowedSpeed.HasValue)
                    {
                        walker["BaseSlowedSpeed"] = Math.Round(baseSlowedSpeed.Value * 2.0, 3);
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
            var hasBuildingTweaks = settings.BuildCostTenth ||
                                    settings.FreeScience ||
                                    settings.DoubleFactoryWorkers ||
                                    settings.PowerTenth ||
                                    settings.StorageTenX;

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

                    if (settings.BuildCostTenth || settings.FreeScience)
                    {
                        var buildingSpec = new JObject();
                        var buildingSpecSource = source["BuildingSpec"] as JObject;

                        if (settings.BuildCostTenth && buildingSpecSource != null)
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
                                    var newAmount = amount <= 0 ? 0 : Math.Max(1, (int)Math.Ceiling(amount / 10.0));
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

                        if (settings.FreeScience && buildingSpecSource != null)
                        {
                            var scienceToken = buildingSpecSource["ScienceCost"];
                            var scienceCost = scienceToken != null ? (int?)scienceToken.Value<int>() : null;
                            if (scienceCost.HasValue && scienceCost.Value > 0)
                            {
                                buildingSpec["ScienceCost"] = 0;
                            }
                        }

                        if (buildingSpec.HasValues)
                        {
                            output["BuildingSpec"] = buildingSpec;
                        }
                    }

                    if (settings.DoubleFactoryWorkers &&
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

                        if (maxWorkers.HasValue)
                        {
                            workplaceSpec["MaxWorkers"] = maxWorkers.Value * 2;
                        }

                        if (defaultWorkers.HasValue)
                        {
                            workplaceSpec["DefaultWorkers"] = defaultWorkers.Value * 2;
                        }

                        if (workplaceSpec.HasValues)
                        {
                            output["WorkplaceSpec"] = workplaceSpec;
                        }
                    }

                    if (settings.PowerTenth)
                    {
                        var powerInput = GetDoubleValue(source, "MechanicalNodeSpec.PowerInput");
                        if (powerInput.HasValue && powerInput.Value > 0)
                        {
                            var mechanicalNodeSpec = new JObject();
                            mechanicalNodeSpec["PowerInput"] = Math.Max(1, (int)Math.Ceiling(powerInput.Value / 10.0));
                            output["MechanicalNodeSpec"] = mechanicalNodeSpec;
                        }
                    }

                    if (settings.StorageTenX)
                    {
                        var maxCapacity = GetIntValue(source, "StockpileSpec.MaxCapacity");
                        if (maxCapacity.HasValue)
                        {
                            var stockpileSpec = new JObject();
                            stockpileSpec["MaxCapacity"] = maxCapacity.Value * 10;
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
