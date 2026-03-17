using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlBottomBarButton : IBottomBarElementsProvider
    {
        private readonly TimberBoostControlPanel _panel;

        private Button _button;

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
            var button = new Button(_panel.ToggleVisibility)
            {
                text = "Boost"
            };
            button.style.width = 60;
            button.style.height = 44;
            button.style.marginLeft = 4;
            button.style.marginRight = 4;
            button.style.marginTop = 3;
            button.style.marginBottom = 3;
            button.style.paddingLeft = 6;
            button.style.paddingRight = 6;
            button.style.color = Color.white;
            button.style.backgroundColor = new Color(0.18f, 0.3f, 0.23f, 0.98f);
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
            button.style.borderTopColor = new Color(0.36f, 0.5f, 0.41f, 1f);
            button.style.borderRightColor = new Color(0.36f, 0.5f, 0.41f, 1f);
            button.style.borderBottomColor = new Color(0.36f, 0.5f, 0.41f, 1f);
            button.style.borderLeftColor = new Color(0.36f, 0.5f, 0.41f, 1f);
            return button;
        }
    }
}
