using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;

namespace Forge.DailyBonuses.Modules
{
    public static class EffectHandler
    {
        public static void onEffectButtonClicked(Player player, string buttonName)
        {
            throw new NotImplementedException();
        }

        public static void sendEffectReward(UnturnedPlayer player)
        {
            ITransportConnection transportConnection = player.Player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffect(Plugin.Instance.Configuration.Instance.EffectID, Plugin.Instance.Configuration.Instance.EffectKey, transportConnection, true);
        }
    }
}
