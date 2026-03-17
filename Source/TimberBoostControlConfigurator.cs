using Bindito.Core;

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
        }
    }
}
