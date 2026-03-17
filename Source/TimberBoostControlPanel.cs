using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlPanel : ILoadableSingleton
    {
        private const int CollapsedWidth = 220;
        private const int ExpandedWidth = 360;

        private readonly TimberBoostControlGenerator _generator;
        private readonly QuickNotificationService _quickNotificationService;
        private readonly TimberBoostControlSettingsStore _settingsStore;
        private readonly UILayout _uiLayout;

        private Button _buildCostTenthButton;
        private Label _buildCostTenthStateLabel;
        private Button _carryTenXButton;
        private Label _carryTenXStateLabel;
        private Label _collapsedSummaryLabel;
        private Button _collapseButton;
        private VisualElement _content;
        private TimberBoostControlSettings _currentSettings;
        private Button _doubleFactoryWorkersButton;
        private Label _doubleFactoryWorkersStateLabel;
        private Button _freeScienceButton;
        private Label _freeScienceStateLabel;
        private Button _moveTwoXButton;
        private Label _moveTwoXStateLabel;
        private bool _panelCollapsed;
        private Button _powerTenthButton;
        private Label _powerTenthStateLabel;
        private VisualElement _root;
        private Label _statusLabel;
        private Button _storageTenXButton;
        private Label _storageTenXStateLabel;

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
            _currentSettings = CloneSettings(settings);
            var root = BuildRoot(settings);
            _uiLayout.AddBottomRight(root, 150);
        }

        private VisualElement BuildRoot(TimberBoostControlSettings settings)
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

            _collapseButton = CreateHeaderButton("Hide", ToggleCollapsedClicked);
            header.Add(_collapseButton);
            _root.Add(header);

            _collapsedSummaryLabel = new Label();
            _collapsedSummaryLabel.style.whiteSpace = WhiteSpace.Normal;
            _collapsedSummaryLabel.style.fontSize = 11;
            _collapsedSummaryLabel.style.color = new Color(0.87f, 0.94f, 0.9f);
            _collapsedSummaryLabel.style.marginBottom = 6;
            _root.Add(_collapsedSummaryLabel);

            _content = new VisualElement();

            var description = new Label("Click a row to toggle it. Save writes blueprint tweaks into this mod folder, and the game must be restarted after saving.");
            description.style.whiteSpace = WhiteSpace.Normal;
            description.style.color = new Color(0.83f, 0.9f, 0.86f);
            description.style.fontSize = 11;
            description.style.marginBottom = 8;
            _content.Add(description);

            _carryTenXButton = CreateOptionButton("10x carry capacity", ToggleCarryTenXClicked, out _carryTenXStateLabel);
            _moveTwoXButton = CreateOptionButton("2x move speed", ToggleMoveTwoXClicked, out _moveTwoXStateLabel);
            _storageTenXButton = CreateOptionButton("10x storage capacity", ToggleStorageTenXClicked, out _storageTenXStateLabel);
            _buildCostTenthButton = CreateOptionButton("1/10 building cost", ToggleBuildCostTenthClicked, out _buildCostTenthStateLabel);
            _freeScienceButton = CreateOptionButton("0 science cost", ToggleFreeScienceClicked, out _freeScienceStateLabel);
            _doubleFactoryWorkersButton = CreateOptionButton("2x factory workers", ToggleDoubleFactoryWorkersClicked, out _doubleFactoryWorkersStateLabel);
            _powerTenthButton = CreateOptionButton("1/10 power input", TogglePowerTenthClicked, out _powerTenthStateLabel);

            _content.Add(_carryTenXButton);
            _content.Add(_moveTwoXButton);
            _content.Add(_storageTenXButton);
            _content.Add(_buildCostTenthButton);
            _content.Add(_freeScienceButton);
            _content.Add(_doubleFactoryWorkersButton);
            _content.Add(_powerTenthButton);

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
            _content.Add(buttons);

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
            _content.Add(_statusLabel);

            _root.Add(_content);

            RefreshOptionRows();
            ApplyCollapsedState(settings.PanelCollapsed);
            UpdateStatus(string.Format("Loaded settings. Selected: {0} option(s).", CollectSettings().EnabledCount));
            return _root;
        }

        private static Button CreateOptionButton(string text, System.Action clicked, out Label stateLabel)
        {
            var button = new Button(clicked);
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.SpaceBetween;
            button.style.height = 32;
            button.style.marginTop = 3;
            button.style.marginBottom = 3;
            button.style.paddingLeft = 10;
            button.style.paddingRight = 10;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            button.style.borderTopWidth = 1;
            button.style.borderRightWidth = 1;
            button.style.borderBottomWidth = 1;
            button.style.borderLeftWidth = 1;
            button.style.borderTopColor = new Color(0.28f, 0.36f, 0.32f, 1f);
            button.style.borderRightColor = new Color(0.28f, 0.36f, 0.32f, 1f);
            button.style.borderBottomColor = new Color(0.28f, 0.36f, 0.32f, 1f);
            button.style.borderLeftColor = new Color(0.28f, 0.36f, 0.32f, 1f);

            var textLabel = new Label(text);
            textLabel.style.color = Color.white;
            textLabel.style.fontSize = 13;
            textLabel.style.flexGrow = 1;
            button.Add(textLabel);

            stateLabel = new Label();
            stateLabel.style.minWidth = 42;
            stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            stateLabel.style.color = Color.white;
            stateLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            stateLabel.style.fontSize = 11;
            stateLabel.style.paddingLeft = 8;
            stateLabel.style.paddingRight = 8;
            stateLabel.style.paddingTop = 3;
            stateLabel.style.paddingBottom = 3;
            stateLabel.style.borderTopLeftRadius = 999;
            stateLabel.style.borderTopRightRadius = 999;
            stateLabel.style.borderBottomLeftRadius = 999;
            stateLabel.style.borderBottomRightRadius = 999;
            button.Add(stateLabel);

            return button;
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
            button.style.minWidth = 62;
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
            ApplySettings(settings);
            ApplyCollapsedState(settings.PanelCollapsed);
            UpdateStatus(string.Format("Reloaded settings from disk. Selected: {0} option(s).", settings.EnabledCount));
            _quickNotificationService.SendNotification("TimberBoostControl reloaded its saved settings.");
        }

        private void ResetClicked()
        {
            var settings = new TimberBoostControlSettings
            {
                PanelCollapsed = _panelCollapsed
            };
            ApplySettings(settings);
            UpdateStatus("Reset the UI to defaults. Click Save to rewrite generated files.");
            _quickNotificationService.SendNotification("TimberBoostControl reset its UI values.");
        }

        private TimberBoostControlSettings CollectSettings()
        {
            var settings = CloneSettings(_currentSettings);
            settings.PanelCollapsed = _panelCollapsed;
            return settings;
        }

        private void ApplySettings(TimberBoostControlSettings settings)
        {
            _currentSettings = CloneSettings(settings);
            RefreshOptionRows();
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.text = message;
        }

        private void ApplyCollapsedState(bool collapsed)
        {
            _panelCollapsed = collapsed;
            _content.style.display = collapsed ? DisplayStyle.None : DisplayStyle.Flex;
            _collapseButton.text = collapsed ? "Show" : "Hide";
            _root.style.width = collapsed ? CollapsedWidth : ExpandedWidth;
            _collapsedSummaryLabel.style.display = collapsed ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateCollapsedSummary();
        }

        private void PersistPanelState()
        {
            var settings = _settingsStore.Load();
            settings.PanelCollapsed = _panelCollapsed;
            _settingsStore.Save(settings);
        }

        private void RefreshSelectionStatus()
        {
            UpdateStatus(string.Format("Selected: {0} option(s). Click Save to write blueprint files.", CollectSettings().EnabledCount));
        }

        private void ToggleCollapsedClicked()
        {
            ApplyCollapsedState(!_panelCollapsed);
            PersistPanelState();
        }

        private void ToggleCarryTenXClicked()
        {
            _currentSettings.CarryTenX = !_currentSettings.CarryTenX;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void ToggleMoveTwoXClicked()
        {
            _currentSettings.MoveTwoX = !_currentSettings.MoveTwoX;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void ToggleStorageTenXClicked()
        {
            _currentSettings.StorageTenX = !_currentSettings.StorageTenX;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void ToggleBuildCostTenthClicked()
        {
            _currentSettings.BuildCostTenth = !_currentSettings.BuildCostTenth;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void ToggleFreeScienceClicked()
        {
            _currentSettings.FreeScience = !_currentSettings.FreeScience;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void ToggleDoubleFactoryWorkersClicked()
        {
            _currentSettings.DoubleFactoryWorkers = !_currentSettings.DoubleFactoryWorkers;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void TogglePowerTenthClicked()
        {
            _currentSettings.PowerTenth = !_currentSettings.PowerTenth;
            RefreshOptionRows();
            RefreshSelectionStatus();
        }

        private void RefreshOptionRows()
        {
            UpdateOptionButton(_carryTenXButton, _carryTenXStateLabel, _currentSettings.CarryTenX);
            UpdateOptionButton(_moveTwoXButton, _moveTwoXStateLabel, _currentSettings.MoveTwoX);
            UpdateOptionButton(_storageTenXButton, _storageTenXStateLabel, _currentSettings.StorageTenX);
            UpdateOptionButton(_buildCostTenthButton, _buildCostTenthStateLabel, _currentSettings.BuildCostTenth);
            UpdateOptionButton(_freeScienceButton, _freeScienceStateLabel, _currentSettings.FreeScience);
            UpdateOptionButton(_doubleFactoryWorkersButton, _doubleFactoryWorkersStateLabel, _currentSettings.DoubleFactoryWorkers);
            UpdateOptionButton(_powerTenthButton, _powerTenthStateLabel, _currentSettings.PowerTenth);
            UpdateCollapsedSummary();
        }

        private void UpdateCollapsedSummary()
        {
            _collapsedSummaryLabel.text = string.Format("{0} option(s) selected. Click Show to reopen.", CollectSettings().EnabledCount);
        }

        private static TimberBoostControlSettings CloneSettings(TimberBoostControlSettings settings)
        {
            if (settings == null)
            {
                return new TimberBoostControlSettings();
            }

            return new TimberBoostControlSettings
            {
                CarryTenX = settings.CarryTenX,
                MoveTwoX = settings.MoveTwoX,
                StorageTenX = settings.StorageTenX,
                BuildCostTenth = settings.BuildCostTenth,
                FreeScience = settings.FreeScience,
                DoubleFactoryWorkers = settings.DoubleFactoryWorkers,
                PowerTenth = settings.PowerTenth,
                PanelCollapsed = settings.PanelCollapsed
            };
        }

        private static void UpdateOptionButton(Button button, Label stateLabel, bool enabled)
        {
            button.style.backgroundColor = enabled
                ? new Color(0.16f, 0.28f, 0.19f, 0.98f)
                : new Color(0.11f, 0.16f, 0.14f, 0.94f);
            stateLabel.text = enabled ? "ON" : "OFF";
            stateLabel.style.backgroundColor = enabled
                ? new Color(0.27f, 0.58f, 0.33f, 1f)
                : new Color(0.32f, 0.36f, 0.39f, 1f);
        }
    }
}
