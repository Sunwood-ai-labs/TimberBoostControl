using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Mods.TimberBoostControl
{
    public sealed class TimberBoostControlBottomBarModuleProvider : IProvider<BottomBarModule>
    {
        private readonly TimberBoostControlBottomBarButton _button;

        public TimberBoostControlBottomBarModuleProvider(TimberBoostControlBottomBarButton button)
        {
            _button = button;
        }

        public BottomBarModule Get()
        {
            var builder = new BottomBarModule.Builder();
            builder.AddRightSectionElement(_button);
            return builder.Build();
        }
    }
}
