using Rocket.Unturned.Player;
using System;

namespace Forge.DailyBonuses.Modules
{
    public static class EventHandler
    {
        public static void OnPlayerConnected(UnturnedPlayer player)
        {
            ulong steamID = player.CSteamID.m_SteamID;
            Data playerData = Plugin.Instance.DataManager.GetPlayerData(steamID);

            if (playerData == null)
            {
                Plugin.Instance.DataManager.CreatePlayerData(steamID);
                playerData = Plugin.Instance.DataManager.GetPlayerData(steamID);
            }

            if (ShouldShowUI(playerData))
            {
                EffectHandler.sendEffectReward(player);
            }
        }

        private static bool ShouldShowUI(Data playerData)
        {
            bool hasActiveBonus = playerData.LastBonusClaim.Date < DateTime.Today;
            bool hasAllBonuses = playerData.LastBonusClaim.Date == DateTime.Today &&
                                 (playerData.LastBonusClaim.DayOfYear % 7) == 6 &&
                                 Plugin.Instance.Configuration.Instance.ResetProgressAfterAllBonuses;

            return hasActiveBonus || hasAllBonuses;
        }
    }
}