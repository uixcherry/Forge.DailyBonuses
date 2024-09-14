﻿using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using Rocket.Core;
using System.Linq;
using System.Collections.Generic;

namespace Forge.DailyBonuses.Modules
{
    public static class EffectHandler
    {
        public static void onEffectButtonClicked(Player player, string buttonName)
        {
            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromPlayer(player);
            Data playerData = Plugin.Instance.DataManager.GetPlayerData(unturnedPlayer.CSteamID.m_SteamID);

            if (playerData == null)
            {
                Plugin.Instance.DataManager.CreatePlayerData(unturnedPlayer.CSteamID.m_SteamID);
                playerData = Plugin.Instance.DataManager.GetPlayerData(unturnedPlayer.CSteamID.m_SteamID);
            }

            if (buttonName == "forge.daily_close")
            {
                EffectManager.askEffectClearByID(Plugin.Instance.Configuration.Instance.EffectID, unturnedPlayer.Player.channel.owner.transportConnection);
            }

            int claimedDays = CalculateClaimedDays(playerData);

            Dictionary<string, int> buttonToDayMap = new Dictionary<string, int>
            {
                { "forge.daily_day_0", 1 },
                { "forge.daily_day_1", 2 },
                { "forge.daily_day_2", 3 },
                { "forge.daily_day_3", 4 },
                { "forge.daily_day_4", 5 },
                { "forge.daily_day_5", 6 },
                { "forge.daily_day_6", 7 }
            };

            if (buttonToDayMap.TryGetValue(buttonName, out int day) && claimedDays == day - 1)
            {
                GiveBonus(unturnedPlayer, day);
                Plugin.Instance.DataManager.UpdatePlayerData(playerData);
                Plugin.Instance.DataManager.SaveData();
            }
        }

        public static void sendEffectReward(UnturnedPlayer player)
        {
            ITransportConnection transportConnection = player.Player.channel.GetOwnerTransportConnection();

            Data playerData = Plugin.Instance.DataManager.GetPlayerData(player.CSteamID.m_SteamID);
            int claimedDays = CalculateClaimedDays(playerData);

            DateTime nextBonusTime = playerData.LastBonusClaim.Date.AddDays(1);
            TimeSpan timeUntilNextBonus = nextBonusTime - DateTime.Now;
            string formattedTime = timeUntilNextBonus.ToString(@"hh\:mm");

            EffectManager.sendUIEffect(Plugin.Instance.Configuration.Instance.EffectID, Plugin.Instance.Configuration.Instance.EffectKey, transportConnection, true);
            EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.EffectKey, transportConnection, true, "forge.daily_timer", formattedTime);

            for (int i = 0; i < 7; i++)
            {
                string buttonName = $"forge.daily_day_{i}_text";
                string buttonText = GetButtonText(i, claimedDays);
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.EffectKey, transportConnection, true,
                    buttonName, buttonText);
            }
        }

        private static string GetButtonText(int buttonIndex, int claimedDays)
        {
            if (claimedDays > buttonIndex)
                return Plugin.Instance.Translate("DailyBonus_Button_Claimed");
            else if (claimedDays == buttonIndex)
                return Plugin.Instance.Translate("DailyBonus_Button_Claim");
            else
                return Plugin.Instance.Translate("DailyBonus_Button_Unavailable");
        }

        public static int CalculateClaimedDays(Data playerData)
        {
            if (playerData.LastBonusClaim.Date == DateTime.Today)
            {
                return Plugin.Instance.Configuration.Instance.DailyBonuses.FindIndex(bonus => bonus.Day == (playerData.LastBonusClaim.DayOfYear % 7) + 1) + 1;
            }
            else
            {
                TimeSpan timeSinceLastClaim = DateTime.Today - playerData.LastBonusClaim.Date;

                if (timeSinceLastClaim.Days > 1 && !Plugin.Instance.Configuration.Instance.AllowClaimingMissedBonuses)
                {
                    return 0;
                }
                else
                {
                    return Math.Min(timeSinceLastClaim.Days, 7);
                }
            }
        }

        public static void GiveBonus(UnturnedPlayer player, int day)
        {
            foreach (var command in Plugin.Instance.Configuration.Instance.DailyBonuses.FirstOrDefault(bonus => bonus.Day == day).Commands)
            {
                R.Commands.Execute(player, command.Replace("{player}", player.CSteamID.m_SteamID.ToString()));
            }

            Data playerData = Plugin.Instance.DataManager.GetPlayerData(player.CSteamID.m_SteamID);
            playerData.LastBonusClaim = DateTime.Now;
            if (day == 7 && Plugin.Instance.Configuration.Instance.ResetProgressAfterAllBonuses)
            {
                playerData.LastBonusClaim = playerData.LastBonusClaim.AddDays(-6);
            }
        }
    }
}