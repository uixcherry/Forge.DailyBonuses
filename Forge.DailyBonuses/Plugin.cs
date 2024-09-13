using Forge.DailyBonuses.Modules;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;

namespace Forge.DailyBonuses
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }
        public DataManager DataManager { get; private set; }

        protected override void Load()
        {
            Instance = this;
            DataManager = new DataManager();

            if (Configuration.Instance.DailyBonuses.Count < 7)
            {
                Rocket.Core.Logging.Logger.LogError("Configuration error: Please specify bonuses for exactly 7 days for proper functionality.");
                return;
            }

            U.Events.OnPlayerConnected += Modules.EventHandler.OnPlayerConnected;
            EffectManager.onEffectButtonClicked += EffectHandler.onEffectButtonClicked;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Modules.EventHandler.OnPlayerConnected;
            EffectManager.onEffectButtonClicked -= EffectHandler.onEffectButtonClicked;

            Instance = null;
        }
    }
}