using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.BottomBarSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlBottomBarButton : IBottomBarElementsProvider
    {
        private const int ButtonSize = 44;
        private const string ButtonText = "Boost";
        private const string IconRelativePath = "Assets/UI/boost-icon.png";

        private readonly TimberBoostControlPanel _panel;

        private Button _button;
        private Texture2D _iconTexture;

        public TimberBoostControlBottomBarButton(TimberBoostControlPanel panel)
        {
            _panel = panel;
        }

        public IEnumerable<BottomBarElement> GetElements()
        {
            if (_button == null)
            {
                _button = CreateButton();
            }

            yield return BottomBarElement.CreateSingleLevel(_button);
        }

        private Button CreateButton()
        {
            var button = new Button(_panel.ToggleVisibility);
            button.style.width = ButtonSize;
            button.style.height = ButtonSize;
            button.style.marginLeft = 4;
            button.style.marginRight = 4;
            button.style.marginTop = 3;
            button.style.marginBottom = 3;
            button.style.paddingLeft = 0;
            button.style.paddingRight = 0;
            button.style.paddingTop = 0;
            button.style.paddingBottom = 0;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.Center;
            button.style.color = Color.white;
            button.style.backgroundColor = new Color(0.18f, 0.3f, 0.23f, 0.98f);
            button.style.overflow = Overflow.Hidden;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.fontSize = 11;
            button.style.borderTopLeftRadius = 6;
            button.style.borderTopRightRadius = 6;
            button.style.borderBottomLeftRadius = 6;
            button.style.borderBottomRightRadius = 6;
            button.style.borderTopWidth = 1;
            button.style.borderRightWidth = 1;
            button.style.borderBottomWidth = 1;
            button.style.borderLeftWidth = 1;
            button.style.borderTopColor = new Color(0.32f, 0.43f, 0.36f, 0.55f);
            button.style.borderRightColor = new Color(0.32f, 0.43f, 0.36f, 0.55f);
            button.style.borderBottomColor = new Color(0.32f, 0.43f, 0.36f, 0.55f);
            button.style.borderLeftColor = new Color(0.32f, 0.43f, 0.36f, 0.55f);

            var iconTexture = LoadIconTexture();
            if (iconTexture != null)
            {
                button.text = string.Empty;
                button.tooltip = "Timber Boost Control";

                var icon = new Image
                {
                    image = iconTexture,
                    scaleMode = ScaleMode.ScaleToFit,
                    pickingMode = PickingMode.Ignore
                };
                icon.style.position = Position.Absolute;
                icon.style.left = 0;
                icon.style.top = 0;
                icon.style.right = 0;
                icon.style.bottom = 0;
                button.Add(icon);
            }
            else
            {
                button.text = ButtonText;
                button.tooltip = "Timber Boost Control";
            }

            return button;
        }

        private Texture2D LoadIconTexture()
        {
            if (_iconTexture != null)
            {
                return _iconTexture;
            }

            try
            {
                var iconPath = ModContext.ResolveModFile(IconRelativePath);
                if (!File.Exists(iconPath))
                {
                    Debug.LogWarning(string.Format(
                        "TimberBoostControl icon was not found at '{0}'. Falling back to text button.",
                        iconPath));
                    return null;
                }

                var bytes = File.ReadAllBytes(iconPath);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!ImageConversion.LoadImage(texture, bytes, false))
                {
                    UnityEngine.Object.Destroy(texture);
                    return null;
                }

                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                _iconTexture = texture;
                return _iconTexture;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(string.Format(
                    "TimberBoostControl could not load icon '{0}': {1}",
                    IconRelativePath,
                    exception.Message));
                return null;
            }
        }
    }
}
