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
        private const int ExpandedWidth = 400;

        private readonly TimberBoostControlGenerator _generator;
        private readonly QuickNotificationService _quickNotificationService;
        private readonly TimberBoostControlSettingsStore _settingsStore;
        private readonly UILayout _uiLayout;

        private Label _buildCostPercentValueLabel;
        private Label _carryMultiplierValueLabel;
        private TimberBoostControlSettings _currentSettings;
        private Label _factoryWorkerMultiplierValueLabel;
        private Label _moveSpeedPercentValueLabel;
        private Label _powerInputPercentValueLabel;
        private VisualElement _root;
        private Label _scienceCostPercentValueLabel;
        private Label _settingsPathLabel;
        private Label _statusLabel;
        private Label _storageMultiplierValueLabel;
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
            _currentSettings = _settingsStore.Load();
            _currentSettings.Normalize();

            var root = BuildRoot();
            _uiLayout.AddBottomRight(root, 150);
            SetVisible(false);
            RefreshDisplayedValues();
            UpdateStatus(CreateLoadedStatusMessage("Loaded values from settings.json."));
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

            var description = new Label(
                "This panel is read-only. Edit settings.json directly, then click Reload settings.json to refresh the values. Restart the game after reloading to apply regenerated blueprint overrides.");
            description.style.whiteSpace = WhiteSpace.Normal;
            description.style.color = new Color(0.83f, 0.9f, 0.86f);
            description.style.fontSize = 11;
            description.style.marginBottom = 10;
            _root.Add(description);

            _root.Add(CreateValueRow("Carry multiplier", out _carryMultiplierValueLabel));
            _root.Add(CreateValueRow("Move speed", out _moveSpeedPercentValueLabel));
            _root.Add(CreateValueRow("Storage multiplier", out _storageMultiplierValueLabel));
            _root.Add(CreateValueRow("Building cost", out _buildCostPercentValueLabel));
            _root.Add(CreateValueRow("Science cost", out _scienceCostPercentValueLabel));
            _root.Add(CreateValueRow("Workplace workers", out _factoryWorkerMultiplierValueLabel));
            _root.Add(CreateValueRow("Power input", out _powerInputPercentValueLabel));
            _root.Add(CreatePathCard("settings.json path", ModContext.SettingsPath, out _settingsPathLabel));

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 10;
            buttons.style.marginBottom = 8;

            var reloadButton = CreateActionButton(
                "Reload settings.json",
                ReloadClicked,
                new Color(0.24f, 0.33f, 0.42f, 1f));
            buttons.Add(reloadButton);
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

            return _root;
        }

        private static VisualElement CreateValueRow(string labelText, out Label valueLabel)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.alignItems = Align.Center;
            row.style.marginBottom = 6;
            row.style.paddingLeft = 10;
            row.style.paddingRight = 10;
            row.style.paddingTop = 7;
            row.style.paddingBottom = 7;
            row.style.backgroundColor = new Color(0.11f, 0.16f, 0.14f, 0.94f);
            row.style.borderTopLeftRadius = 6;
            row.style.borderTopRightRadius = 6;
            row.style.borderBottomLeftRadius = 6;
            row.style.borderBottomRightRadius = 6;
            row.style.borderTopWidth = 1;
            row.style.borderRightWidth = 1;
            row.style.borderBottomWidth = 1;
            row.style.borderLeftWidth = 1;
            row.style.borderTopColor = new Color(0.28f, 0.36f, 0.32f, 1f);
            row.style.borderRightColor = row.style.borderTopColor;
            row.style.borderBottomColor = row.style.borderTopColor;
            row.style.borderLeftColor = row.style.borderTopColor;

            var label = new Label(labelText);
            label.style.color = new Color(0.88f, 0.95f, 0.9f);
            label.style.fontSize = 13;
            label.style.flexGrow = 1;
            label.style.marginRight = 8;
            row.Add(label);

            valueLabel = new Label();
            valueLabel.style.color = Color.white;
            valueLabel.style.fontSize = 14;
            valueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            row.Add(valueLabel);

            return row;
        }

        private static VisualElement CreatePathCard(string titleText, string pathText, out Label pathLabel)
        {
            var card = new VisualElement();
            card.style.marginTop = 4;
            card.style.marginBottom = 2;
            card.style.paddingLeft = 10;
            card.style.paddingRight = 10;
            card.style.paddingTop = 8;
            card.style.paddingBottom = 8;
            card.style.backgroundColor = new Color(0.09f, 0.11f, 0.15f, 0.92f);
            card.style.borderTopLeftRadius = 6;
            card.style.borderTopRightRadius = 6;
            card.style.borderBottomLeftRadius = 6;
            card.style.borderBottomRightRadius = 6;
            card.style.borderTopWidth = 1;
            card.style.borderRightWidth = 1;
            card.style.borderBottomWidth = 1;
            card.style.borderLeftWidth = 1;
            card.style.borderTopColor = new Color(0.29f, 0.37f, 0.47f, 1f);
            card.style.borderRightColor = card.style.borderTopColor;
            card.style.borderBottomColor = card.style.borderTopColor;
            card.style.borderLeftColor = card.style.borderTopColor;

            var title = new Label(titleText);
            title.style.color = new Color(0.85f, 0.91f, 0.98f);
            title.style.fontSize = 12;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 4;
            card.Add(title);

            pathLabel = new Label(pathText);
            pathLabel.style.whiteSpace = WhiteSpace.Normal;
            pathLabel.style.color = Color.white;
            pathLabel.style.fontSize = 12;
            card.Add(pathLabel);

            return card;
        }

        private void ReloadClicked()
        {
            _currentSettings = _settingsStore.Load();
            _currentSettings.Normalize();
            RefreshDisplayedValues();

            var result = _generator.Generate(CloneSettings(_currentSettings));
            if (result.Success)
            {
                UpdateStatus(string.Format(
                    "Reloaded settings.json and regenerated {0} blueprint file(s). Restart the game to apply.",
                    result.FileCount));
                _quickNotificationService.SendNotification(
                    "TimberBoostControl reloaded settings.json. Restart the game to apply changes.");
                return;
            }

            UpdateStatus(result.Message);
            _quickNotificationService.SendNotification(
                "TimberBoostControl could not regenerate blueprint files from settings.json.");
        }

        private void SetVisible(bool visible)
        {
            _visible = visible;
            _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshDisplayedValues()
        {
            _carryMultiplierValueLabel.text = FormatMultiplier(_currentSettings.CarryMultiplier);
            _moveSpeedPercentValueLabel.text = FormatPercent(_currentSettings.MoveSpeedPercent);
            _storageMultiplierValueLabel.text = FormatMultiplier(_currentSettings.StorageMultiplier);
            _buildCostPercentValueLabel.text = FormatPercent(_currentSettings.BuildCostPercent);
            _scienceCostPercentValueLabel.text = FormatPercent(_currentSettings.ScienceCostPercent);
            _factoryWorkerMultiplierValueLabel.text = FormatMultiplier(_currentSettings.FactoryWorkerMultiplier);
            _powerInputPercentValueLabel.text = FormatPercent(_currentSettings.PowerInputPercent);
            _settingsPathLabel.text = ModContext.SettingsPath;
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.text = message;
        }

        private static string FormatMultiplier(int value)
        {
            return value == 1 ? "1x (vanilla)" : string.Format("{0}x", value);
        }

        private static string FormatPercent(int value)
        {
            return value == 100 ? "100% (vanilla)" : string.Format("{0}%", value);
        }

        private static Button CreateActionButton(string text, Action clicked, Color backgroundColor)
        {
            var button = new Button(clicked)
            {
                text = text
            };
            button.style.flexGrow = 1;
            button.style.height = 30;
            button.style.color = Color.white;
            button.style.backgroundColor = backgroundColor;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            return button;
        }

        private static Button CreateHeaderButton(string text, Action clicked)
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

        private static string CreateLoadedStatusMessage(string prefix)
        {
            return string.Format(
                "{0} Edit settings.json directly, then use Reload settings.json to refresh the panel.",
                prefix);
        }
    }
}
