using System;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlPanel : ILoadableSingleton
    {
        private const int ExpandedWidth = 360;
        private static readonly int[] CarryMultipliers = { 1, 2, 5, 10 };
        private static readonly int[] MoveSpeedPresets = { 50, 100, 150, 200, 300 };
        private static readonly int[] StorageMultipliers = { 1, 2, 5, 10 };
        private static readonly int[] BuildCostPresets = { 100, 50, 25, 10 };
        private static readonly int[] ScienceCostPresets = { 0, 25, 50, 100 };
        private static readonly int[] FactoryWorkerPresets = { 1, 2, 4 };
        private static readonly int[] PowerInputPresets = { 10, 50, 100 };

        private readonly TimberBoostControlGenerator _generator;
        private readonly QuickNotificationService _quickNotificationService;
        private readonly TimberBoostControlSettingsStore _settingsStore;
        private readonly UILayout _uiLayout;

        private Button[] _buildCostPercentButtons;
        private Button[] _carryMultiplierButtons;
        private TimberBoostControlSettings _currentSettings;
        private Button[] _factoryWorkerMultiplierButtons;
        private Button[] _moveSpeedPercentButtons;
        private Button[] _powerInputPercentButtons;
        private Button[] _scienceCostPercentButtons;
        private VisualElement _root;
        private Label _statusLabel;
        private Button[] _storageMultiplierButtons;
        private bool _visible;

        public TimberBoostControlPanel(
            UILayout uiLayout,
            QuickNotificationService quickNotificationService,
            TimberBoostControlSettingsStore settingsStore,
            TimberBoostControlGenerator generator)
        {
            _uiLayout = uiLayout;
            _quickNotificationService = quickNotificationService;
            _settingsStore = settingsStore;
            _generator = generator;
        }

        public void Load()
        {
            var settings = _settingsStore.Load();
            settings.Normalize();
            _currentSettings = CloneSettings(settings);
            var snappedOnLoad = SnapSettingsToPresets(_currentSettings);
            var root = BuildRoot();
            _uiLayout.AddBottomRight(root, 150);
            SetVisible(false);
            UpdateStatus(CreateLoadedStatusMessage("Loaded settings.", _currentSettings, snappedOnLoad));
        }

        public void ToggleVisibility()
        {
            SetVisible(!_visible);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private VisualElement BuildRoot()
        {
            _root = new VisualElement();
            _root.style.width = ExpandedWidth;
            _root.style.marginRight = 12;
            _root.style.marginBottom = 12;
            _root.style.paddingLeft = 10;
            _root.style.paddingRight = 10;
            _root.style.paddingTop = 10;
            _root.style.paddingBottom = 10;
            _root.style.backgroundColor = new Color(0.08f, 0.13f, 0.11f, 0.96f);
            _root.style.borderTopLeftRadius = 8;
            _root.style.borderTopRightRadius = 8;
            _root.style.borderBottomLeftRadius = 8;
            _root.style.borderBottomRightRadius = 8;

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("Timber Boost Control");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 16;
            title.style.color = Color.white;
            title.style.flexGrow = 1;
            header.Add(title);

            var closeButton = CreateHeaderButton("Close", Hide);
            header.Add(closeButton);
            _root.Add(header);

            var description = new Label("Open this panel from the bottom bar Boost button. Pick a preset for each value, then Save and restart the game.");
            description.style.whiteSpace = WhiteSpace.Normal;
            description.style.color = new Color(0.83f, 0.9f, 0.86f);
            description.style.fontSize = 11;
            description.style.marginBottom = 8;
            _root.Add(description);

            _root.Add(CreatePresetRow(
                "Carry multiplier",
                CarryMultipliers,
                FormatCarryMultiplier,
                SelectCarryMultiplier));
            _root.Add(CreatePresetRow(
                "Move speed",
                MoveSpeedPresets,
                FormatPercent,
                SelectMoveSpeedPercent));
            _root.Add(CreatePresetRow(
                "Storage multiplier",
                StorageMultipliers,
                FormatStorageMultiplier,
                SelectStorageMultiplier));
            _root.Add(CreatePresetRow(
                "Building cost",
                BuildCostPresets,
                FormatPercent,
                SelectBuildCostPercent));
            _root.Add(CreatePresetRow(
                "Science cost",
                ScienceCostPresets,
                FormatPercent,
                SelectScienceCostPercent));
            _root.Add(CreatePresetRow(
                "Factory workers",
                FactoryWorkerPresets,
                FormatFactoryWorkerMultiplier,
                SelectFactoryWorkerMultiplier));
            _root.Add(CreatePresetRow(
                "Power input",
                PowerInputPresets,
                FormatPercent,
                SelectPowerInputPercent));

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 10;
            buttons.style.marginBottom = 8;

            var saveButton = CreateActionButton("Save", SaveClicked, new Color(0.24f, 0.48f, 0.3f, 1f));
            var reloadButton = CreateActionButton("Reload", ReloadClicked, new Color(0.24f, 0.33f, 0.42f, 1f));
            var resetButton = CreateActionButton("Reset", ResetClicked, new Color(0.45f, 0.23f, 0.24f, 1f));
            resetButton.style.marginRight = 0;

            buttons.Add(saveButton);
            buttons.Add(reloadButton);
            buttons.Add(resetButton);
            _root.Add(buttons);

            _statusLabel = new Label();
            _statusLabel.style.whiteSpace = WhiteSpace.Normal;
            _statusLabel.style.fontSize = 11;
            _statusLabel.style.color = new Color(0.86f, 0.91f, 1f);
            _statusLabel.style.backgroundColor = new Color(0.12f, 0.18f, 0.2f, 0.7f);
            _statusLabel.style.paddingLeft = 8;
            _statusLabel.style.paddingRight = 8;
            _statusLabel.style.paddingTop = 6;
            _statusLabel.style.paddingBottom = 6;
            _statusLabel.style.borderTopLeftRadius = 6;
            _statusLabel.style.borderTopRightRadius = 6;
            _statusLabel.style.borderBottomLeftRadius = 6;
            _statusLabel.style.borderBottomRightRadius = 6;
            _root.Add(_statusLabel);

            RefreshOptionRows();
            return _root;
        }

        private VisualElement CreatePresetRow(
            string text,
            int[] values,
            Func<int, string> optionLabelFor,
            Action<int> selectedAction)
        {
            var row = new VisualElement();
            row.style.marginTop = 3;
            row.style.marginBottom = 3;

            var title = new Label(text);
            title.style.color = new Color(0.88f, 0.95f, 0.9f);
            title.style.fontSize = 13;
            title.style.marginBottom = 2;
            row.Add(title);

            var optionList = new VisualElement();
            optionList.style.flexDirection = FlexDirection.Row;
            optionList.style.flexWrap = Wrap.Wrap;
            row.Add(optionList);

            var optionButtons = new Button[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var presetValue = values[i];
                var optionButton = new Button(() => selectedAction(presetValue))
                {
                    text = optionLabelFor(presetValue)
                };
                optionButton.style.flexGrow = 1;
                optionButton.style.marginRight = 4;
                optionButton.style.marginBottom = 4;
                optionButton.style.height = 38;
                optionButton.style.minWidth = 72;
                optionButton.style.unityFontStyleAndWeight = FontStyle.Normal;
                optionButton.style.borderTopLeftRadius = 6;
                optionButton.style.borderTopRightRadius = 6;
                optionButton.style.borderBottomLeftRadius = 6;
                optionButton.style.borderBottomRightRadius = 6;
                optionButton.style.borderTopWidth = 1;
                optionButton.style.borderRightWidth = 1;
                optionButton.style.borderBottomWidth = 1;
                optionButton.style.borderLeftWidth = 1;
                optionButton.style.fontSize = 12;
                optionList.Add(optionButton);
                optionButtons[i] = optionButton;
            }

            if (optionButtons.Length > 0)
            {
                optionButtons[optionButtons.Length - 1].style.marginRight = 0;
            }

            switch (text)
            {
                case "Carry multiplier":
                    _carryMultiplierButtons = optionButtons;
                    break;
                case "Move speed":
                    _moveSpeedPercentButtons = optionButtons;
                    break;
                case "Storage multiplier":
                    _storageMultiplierButtons = optionButtons;
                    break;
                case "Building cost":
                    _buildCostPercentButtons = optionButtons;
                    break;
                case "Science cost":
                    _scienceCostPercentButtons = optionButtons;
                    break;
                case "Factory workers":
                    _factoryWorkerMultiplierButtons = optionButtons;
                    break;
                case "Power input":
                    _powerInputPercentButtons = optionButtons;
                    break;
            }

            ApplyOptionSelectionState(_carryMultiplierButtons, CarryMultipliers, _currentSettings.CarryMultiplier);
            ApplyOptionSelectionState(_moveSpeedPercentButtons, MoveSpeedPresets, _currentSettings.MoveSpeedPercent);
            ApplyOptionSelectionState(_storageMultiplierButtons, StorageMultipliers, _currentSettings.StorageMultiplier);
            ApplyOptionSelectionState(_buildCostPercentButtons, BuildCostPresets, _currentSettings.BuildCostPercent);
            ApplyOptionSelectionState(_scienceCostPercentButtons, ScienceCostPresets, _currentSettings.ScienceCostPercent);
            ApplyOptionSelectionState(_factoryWorkerMultiplierButtons, FactoryWorkerPresets, _currentSettings.FactoryWorkerMultiplier);
            ApplyOptionSelectionState(_powerInputPercentButtons, PowerInputPresets, _currentSettings.PowerInputPercent);

            RefreshPresetSelectionByText(text, GetCurrentValueForText(text));

            return row;
        }

        private void RefreshPresetSelectionByText(string text, int selectedValue)
        {
            switch (text)
            {
                case "Carry multiplier":
                    ApplyOptionSelectionState(_carryMultiplierButtons, CarryMultipliers, selectedValue);
                    break;
                case "Move speed":
                    ApplyOptionSelectionState(_moveSpeedPercentButtons, MoveSpeedPresets, selectedValue);
                    break;
                case "Storage multiplier":
                    ApplyOptionSelectionState(_storageMultiplierButtons, StorageMultipliers, selectedValue);
                    break;
                case "Building cost":
                    ApplyOptionSelectionState(_buildCostPercentButtons, BuildCostPresets, selectedValue);
                    break;
                case "Science cost":
                    ApplyOptionSelectionState(_scienceCostPercentButtons, ScienceCostPresets, selectedValue);
                    break;
                case "Factory workers":
                    ApplyOptionSelectionState(_factoryWorkerMultiplierButtons, FactoryWorkerPresets, selectedValue);
                    break;
                case "Power input":
                    ApplyOptionSelectionState(_powerInputPercentButtons, PowerInputPresets, selectedValue);
                    break;
            }
        }

        private int GetCurrentValueForText(string text)
        {
            switch (text)
            {
                case "Carry multiplier":
                    return _currentSettings.CarryMultiplier;
                case "Move speed":
                    return _currentSettings.MoveSpeedPercent;
                case "Storage multiplier":
                    return _currentSettings.StorageMultiplier;
                case "Building cost":
                    return _currentSettings.BuildCostPercent;
                case "Science cost":
                    return _currentSettings.ScienceCostPercent;
                case "Factory workers":
                    return _currentSettings.FactoryWorkerMultiplier;
                case "Power input":
                    return _currentSettings.PowerInputPercent;
                default:
                    return 0;
            }
        }

        private static string FormatCarryMultiplier(int value)
        {
            return value == 1 ? "1x (vanilla)" : string.Format("{0}x", value);
        }

        private static string FormatStorageMultiplier(int value)
        {
            return value == 1 ? "1x (vanilla)" : string.Format("{0}x", value);
        }

        private static string FormatFactoryWorkerMultiplier(int value)
        {
            return value == 1 ? "1x (vanilla)" : string.Format("{0}x", value);
        }

        private static string FormatPercent(int value)
        {
            return value == 100 ? "100% (vanilla)" : string.Format("{0}%", value);
        }

        private static Button CreateActionButton(string text, System.Action clicked, Color backgroundColor)
        {
            var button = new Button(clicked)
            {
                text = text
            };
            button.style.flexGrow = 1;
            button.style.height = 28;
            button.style.marginRight = 6;
            button.style.color = Color.white;
            button.style.backgroundColor = backgroundColor;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            return button;
        }

        private static Button CreateHeaderButton(string text, System.Action clicked)
        {
            var button = new Button(clicked)
            {
                text = text
            };
            button.style.height = 24;
            button.style.minWidth = 68;
            button.style.color = Color.white;
            button.style.backgroundColor = new Color(0.2f, 0.29f, 0.26f, 1f);
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            return button;
        }

        private void SaveClicked()
        {
            var settings = CollectSettings();
            _settingsStore.Save(settings);
            var result = _generator.Generate(settings);

            if (result.Success)
            {
                UpdateStatus(string.Format("Saved settings. Generated {0} file(s). Restart the game to apply.", result.FileCount));
                _quickNotificationService.SendNotification("TimberBoostControl saved. Restart the game to apply changes.");
            }
            else
            {
                UpdateStatus(result.Message);
                _quickNotificationService.SendNotification("TimberBoostControl could not generate its blueprint files.");
            }
        }

        private void ReloadClicked()
        {
            var settings = _settingsStore.Load();
            settings.Normalize();
            var snappedOnReload = ApplySettings(settings);
            UpdateStatus(CreateLoadedStatusMessage("Reloaded settings from disk.", _currentSettings, snappedOnReload));
            _quickNotificationService.SendNotification("TimberBoostControl reloaded its saved settings.");
        }

        private void ResetClicked()
        {
            ApplySettings(new TimberBoostControlSettings());
            UpdateStatus("Reset to defaults. Click Save to rewrite generated files.");
            _quickNotificationService.SendNotification("TimberBoostControl reset its UI values.");
        }

        private TimberBoostControlSettings CollectSettings()
        {
            return CloneSettings(_currentSettings);
        }

        private bool ApplySettings(TimberBoostControlSettings settings)
        {
            _currentSettings = CloneSettings(settings);
            var snapped = SnapSettingsToPresets(_currentSettings);
            RefreshOptionRows();
            return snapped;
        }

        private void SetVisible(bool visible)
        {
            _visible = visible;
            _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.text = message;
        }

        private void RefreshSelectionStatus()
        {
            UpdateStatus(string.Format("Selected: {0} preset(s). Click Save to write blueprint files.", CollectSettings().EnabledCount));
        }

        private void SelectCarryMultiplier(int value)
        {
            _currentSettings.CarryMultiplier = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectMoveSpeedPercent(int value)
        {
            _currentSettings.MoveSpeedPercent = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectStorageMultiplier(int value)
        {
            _currentSettings.StorageMultiplier = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectBuildCostPercent(int value)
        {
            _currentSettings.BuildCostPercent = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectScienceCostPercent(int value)
        {
            _currentSettings.ScienceCostPercent = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectFactoryWorkerMultiplier(int value)
        {
            _currentSettings.FactoryWorkerMultiplier = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void SelectPowerInputPercent(int value)
        {
            _currentSettings.PowerInputPercent = value;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void RefreshOptionRows()
        {
            RefreshPresetSelectionByText("Carry multiplier", _currentSettings.CarryMultiplier);
            RefreshPresetSelectionByText("Move speed", _currentSettings.MoveSpeedPercent);
            RefreshPresetSelectionByText("Storage multiplier", _currentSettings.StorageMultiplier);
            RefreshPresetSelectionByText("Building cost", _currentSettings.BuildCostPercent);
            RefreshPresetSelectionByText("Science cost", _currentSettings.ScienceCostPercent);
            RefreshPresetSelectionByText("Factory workers", _currentSettings.FactoryWorkerMultiplier);
            RefreshPresetSelectionByText("Power input", _currentSettings.PowerInputPercent);
        }

        private static TimberBoostControlSettings CloneSettings(TimberBoostControlSettings settings)
        {
            if (settings == null)
            {
                return new TimberBoostControlSettings();
            }

            return new TimberBoostControlSettings
            {
                CarryMultiplier = settings.CarryMultiplier,
                MoveSpeedPercent = settings.MoveSpeedPercent,
                StorageMultiplier = settings.StorageMultiplier,
                BuildCostPercent = settings.BuildCostPercent,
                ScienceCostPercent = settings.ScienceCostPercent,
                FactoryWorkerMultiplier = settings.FactoryWorkerMultiplier,
                PowerInputPercent = settings.PowerInputPercent
            };
        }

        private static string CreateLoadedStatusMessage(string prefix, TimberBoostControlSettings settings, bool snapped)
        {
            if (snapped)
            {
                return string.Format("{0} Some values were snapped to the nearest preset. Selected: {1} preset(s).", prefix, settings.EnabledCount);
            }

            return string.Format("{0} Selected: {1} preset(s).", prefix, settings.EnabledCount);
        }

        private static bool SnapSettingsToPresets(TimberBoostControlSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            var snapped = false;
            snapped |= UpdateSettingToNearestPreset(settings, CarryMultipliers, settings.CarryMultiplier, value => settings.CarryMultiplier = value);
            snapped |= UpdateSettingToNearestPreset(settings, MoveSpeedPresets, settings.MoveSpeedPercent, value => settings.MoveSpeedPercent = value);
            snapped |= UpdateSettingToNearestPreset(settings, StorageMultipliers, settings.StorageMultiplier, value => settings.StorageMultiplier = value);
            snapped |= UpdateSettingToNearestPreset(settings, BuildCostPresets, settings.BuildCostPercent, value => settings.BuildCostPercent = value);
            snapped |= UpdateSettingToNearestPreset(settings, ScienceCostPresets, settings.ScienceCostPercent, value => settings.ScienceCostPercent = value);
            snapped |= UpdateSettingToNearestPreset(settings, FactoryWorkerPresets, settings.FactoryWorkerMultiplier, value => settings.FactoryWorkerMultiplier = value);
            snapped |= UpdateSettingToNearestPreset(settings, PowerInputPresets, settings.PowerInputPercent, value => settings.PowerInputPercent = value);
            return snapped;
        }

        private static bool UpdateSettingToNearestPreset(
            TimberBoostControlSettings settings,
            int[] presets,
            int currentValue,
            Action<int> applyValue)
        {
            var snappedValue = SnapToNearestPreset(currentValue, presets);
            if (snappedValue == currentValue)
            {
                return false;
            }

            applyValue(snappedValue);
            return true;
        }

        private static int SnapToNearestPreset(int value, int[] presets)
        {
            if (presets == null || presets.Length == 0)
            {
                return value;
            }

            var closest = presets[0];
            var closestDistance = Math.Abs(value - closest);

            for (var i = 1; i < presets.Length; i++)
            {
                var candidate = presets[i];
                var distance = Math.Abs(value - candidate);
                if (distance < closestDistance)
                {
                    closest = candidate;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private static void ApplyOptionSelectionState(Button[] optionButtons, int[] optionValues, int selectedValue)
        {
            if (optionButtons == null || optionButtons.Length == 0)
            {
                return;
            }

            for (var i = 0; i < optionButtons.Length; i++)
            {
                var isSelected = optionValues[i] == selectedValue;
                optionButtons[i].style.backgroundColor = isSelected
                    ? new Color(0.16f, 0.28f, 0.19f, 0.98f)
                    : new Color(0.11f, 0.16f, 0.14f, 0.94f);
                optionButtons[i].style.borderTopColor = isSelected
                    ? new Color(0.6f, 0.82f, 0.65f, 1f)
                    : new Color(0.28f, 0.36f, 0.32f, 1f);
                optionButtons[i].style.borderRightColor = optionButtons[i].style.borderTopColor;
                optionButtons[i].style.borderBottomColor = optionButtons[i].style.borderTopColor;
                optionButtons[i].style.borderLeftColor = optionButtons[i].style.borderTopColor;
                optionButtons[i].style.borderTopWidth = isSelected ? 2 : 1;
                optionButtons[i].style.borderRightWidth = isSelected ? 2 : 1;
                optionButtons[i].style.borderBottomWidth = isSelected ? 2 : 1;
                optionButtons[i].style.borderLeftWidth = isSelected ? 2 : 1;
                optionButtons[i].style.unityFontStyleAndWeight = isSelected ? FontStyle.Bold : FontStyle.Normal;
                optionButtons[i].style.color = Color.white;
            }
        }
    }
}
