using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Mods.TimberBoostControl
{
    [Context("Game")]
    public class TimberBoostControlConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<TimberBoostControlSettingsStore>().AsSingleton();
            Bind<TimberBoostControlGenerator>().AsSingleton();
            Bind<TimberBoostControlPanel>().AsSingleton();
            Bind<TimberBoostControlBottomBarButton>().AsSingleton();
            MultiBind<BottomBarModule>().ToProvider<TimberBoostControlBottomBarModuleProvider>().AsSingleton();
        }
    }
}
