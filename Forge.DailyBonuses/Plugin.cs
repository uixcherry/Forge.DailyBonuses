using Forge.DailyBonuses.Modules;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using System;

namespace Forge.DailyBonuses
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;

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