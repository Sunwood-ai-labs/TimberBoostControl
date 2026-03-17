using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlPanel : ILoadableSingleton
    {
        private readonly TimberBoostControlGenerator _generator;
        private readonly QuickNotificationService _quickNotificationService;
        private readonly TimberBoostControlSettingsStore _settingsStore;
        private readonly UILayout _uiLayout;

        private Toggle _buildCostTenthToggle;
        private Toggle _carryTenXToggle;
        private Toggle _doubleFactoryWorkersToggle;
        private Toggle _freeScienceToggle;
        private Toggle _moveTwoXToggle;
        private Toggle _powerTenthToggle;
        private Label _statusLabel;
        private Toggle _storageTenXToggle;

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
            var root = BuildRoot(settings);
            _uiLayout.AddBottomRight(root, 150);
        }

        private VisualElement BuildRoot(TimberBoostControlSettings settings)
        {
            var root = new VisualElement();
            root.style.width = 340;
            root.style.marginRight = 12;
            root.style.marginBottom = 12;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
            root.style.backgroundColor = new Color(0.08f, 0.13f, 0.11f, 0.96f);
            root.style.borderTopLeftRadius = 8;
            root.style.borderTopRightRadius = 8;
            root.style.borderBottomLeftRadius = 8;
            root.style.borderBottomRightRadius = 8;

            var title = new Label("Timber Boost Control");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 16;
            title.style.color = Color.white;
            title.style.marginBottom = 6;
            root.Add(title);

            var description = new Label("Practice UI mod. Save writes blueprint tweaks into this mod folder. Restart the game after saving.");
            description.style.whiteSpace = WhiteSpace.Normal;
            description.style.color = new Color(0.83f, 0.9f, 0.86f);
            description.style.fontSize = 11;
            description.style.marginBottom = 8;
            root.Add(description);

            _carryTenXToggle = CreateToggle("10x carry capacity", settings.CarryTenX);
            _moveTwoXToggle = CreateToggle("2x move speed", settings.MoveTwoX);
            _storageTenXToggle = CreateToggle("10x storage capacity", settings.StorageTenX);
            _buildCostTenthToggle = CreateToggle("1/10 building cost", settings.BuildCostTenth);
            _freeScienceToggle = CreateToggle("0 science cost", settings.FreeScience);
            _doubleFactoryWorkersToggle = CreateToggle("2x factory workers", settings.DoubleFactoryWorkers);
            _powerTenthToggle = CreateToggle("1/10 power input", settings.PowerTenth);

            root.Add(_carryTenXToggle);
            root.Add(_moveTwoXToggle);
            root.Add(_storageTenXToggle);
            root.Add(_buildCostTenthToggle);
            root.Add(_freeScienceToggle);
            root.Add(_doubleFactoryWorkersToggle);
            root.Add(_powerTenthToggle);

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 10;
            buttons.style.marginBottom = 8;

            var saveButton = CreateButton("Save", SaveClicked);
            var reloadButton = CreateButton("Reload", ReloadClicked);
            var resetButton = CreateButton("Reset", ResetClicked);

            buttons.Add(saveButton);
            buttons.Add(reloadButton);
            buttons.Add(resetButton);
            root.Add(buttons);

            _statusLabel = new Label();
            _statusLabel.style.whiteSpace = WhiteSpace.Normal;
            _statusLabel.style.fontSize = 11;
            _statusLabel.style.color = new Color(0.86f, 0.91f, 1f);
            root.Add(_statusLabel);

            UpdateStatus(string.Format("Loaded settings. Enabled: {0} option(s).", CollectSettings().EnabledCount));
            return root;
        }

        private static Toggle CreateToggle(string text, bool value)
        {
            var toggle = new Toggle(text)
            {
                value = value
            };
            toggle.style.color = Color.white;
            toggle.style.marginTop = 2;
            toggle.style.marginBottom = 2;
            return toggle;
        }

        private static Button CreateButton(string text, System.Action clicked)
        {
            var button = new Button(clicked)
            {
                text = text
            };
            button.style.flexGrow = 1;
            button.style.height = 28;
            button.style.marginRight = 6;
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
            UpdateStatus(string.Format("Reloaded settings from disk. Enabled: {0} option(s).", settings.EnabledCount));
            _quickNotificationService.SendNotification("TimberBoostControl reloaded its saved settings.");
        }

        private void ResetClicked()
        {
            var settings = new TimberBoostControlSettings();
            ApplySettings(settings);
            UpdateStatus("Reset the UI to defaults. Click Save to rewrite generated files.");
            _quickNotificationService.SendNotification("TimberBoostControl reset its UI values.");
        }

        private TimberBoostControlSettings CollectSettings()
        {
            return new TimberBoostControlSettings
            {
                CarryTenX = _carryTenXToggle.value,
                MoveTwoX = _moveTwoXToggle.value,
                StorageTenX = _storageTenXToggle.value,
                BuildCostTenth = _buildCostTenthToggle.value,
                FreeScience = _freeScienceToggle.value,
                DoubleFactoryWorkers = _doubleFactoryWorkersToggle.value,
                PowerTenth = _powerTenthToggle.value
            };
        }

        private void ApplySettings(TimberBoostControlSettings settings)
        {
            _carryTenXToggle.value = settings.CarryTenX;
            _moveTwoXToggle.value = settings.MoveTwoX;
            _storageTenXToggle.value = settings.StorageTenX;
            _buildCostTenthToggle.value = settings.BuildCostTenth;
            _freeScienceToggle.value = settings.FreeScience;
            _doubleFactoryWorkersToggle.value = settings.DoubleFactoryWorkers;
            _powerTenthToggle.value = settings.PowerTenth;
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.text = message;
        }
    }
}
