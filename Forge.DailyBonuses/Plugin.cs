using Forge.DailyBonuses.Modules;
using Rocket.API.Collections;
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
            DataManager.LoadData();

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

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "DailyBonus_Received", "You have received a daily bonus for day {0}!" },
            { "DailyBonus_AllReceived", "You have already received all bonuses for this cycle." },
            { "DailyBonus_AlreadyReceived", "You have already received a bonus today." },
            { "DailyBonus_ResetSuccess", "Daily bonus progress for player {0} has been reset." },
            { "DailyBonus_InvalidSteamID", "Invalid SteamID64 format." },
            { "DailyBonus_Usage", "Usage: /daily [reset <steamID64>]" },
            { "DailyBonus_ConfigError", "Configuration error: Please specify bonuses for exactly 7 days for proper functionality." }
        };
    }
}