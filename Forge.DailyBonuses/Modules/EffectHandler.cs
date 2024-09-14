using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using Rocket.Core;
using System.Linq;

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
                CloseUI(unturnedPlayer);
                return;
            }

            int claimedDays = CalculateClaimedDays(playerData);

            if (TryGetDayFromButton(buttonName, out int day) && claimedDays == day - 1)
            {
                GiveBonus(unturnedPlayer, day);
                playerData.LastBonusClaim = DateTime.Now;

                if (day == 7 && Plugin.Instance.Configuration.Instance.ResetProgressAfterAllBonuses)
                {
                    playerData.LastBonusClaim = playerData.LastBonusClaim.AddDays(-6);
                }

                Plugin.Instance.DataManager.UpdatePlayerData(playerData);
                Plugin.Instance.DataManager.SaveData();

                sendEffectReward(unturnedPlayer);
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
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.EffectKey, transportConnection, true, buttonName, buttonText);
            }

            player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
        }

        private static bool TryGetDayFromButton(string buttonName, out int day)
        {
            day = 0;
            return buttonName.StartsWith("forge.daily_day_") &&
                   int.TryParse(buttonName.Substring("forge.daily_day_".Length), out day);
        }

        private static string GetButtonText(int buttonIndex, int claimedDays)
        {
            if (claimedDays > buttonIndex)
                return Plugin.Instance.Translate("DailyBonus_Button_Claimed");
            else if (claimedDays == buttonIndex)
                return Plugin.Instance.Translate("DailyBonus_Button_Claim");
            else
            {
                int remainingDays = (buttonIndex - claimedDays + 7) % 7;
                if (remainingDays == 0)
                    remainingDays = 7;

                return string.Format(Plugin.Instance.Translate("DailyBonus_Button_Unavailable"), buttonIndex + 1, remainingDays);
            }
        }

        public static int CalculateClaimedDays(Data playerData)
        {
            if (playerData.LastBonusClaim == DateTime.MinValue)
            {
                return 0;
            }
            else
            {
                TimeSpan timeSinceLastClaim = DateTime.Today - playerData.LastBonusClaim.Date;

                if (timeSinceLastClaim.Days == 0)
                {
                    return Plugin.Instance.Configuration.Instance.DailyBonuses
                        .FindIndex(bonus => bonus.Day == (playerData.LastBonusClaim.DayOfYear % 7) + 1) + 1;
                }
                else if (timeSinceLastClaim.Days == 1)
                {
                    return Plugin.Instance.Configuration.Instance.DailyBonuses
                        .FindIndex(bonus => bonus.Day == (playerData.LastBonusClaim.DayOfYear % 7) + 1) + 2;
                }
                else if (timeSinceLastClaim.Days > 1 && !Plugin.Instance.Configuration.Instance.AllowClaimingMissedBonuses)
                {
                    return 0;
                }
                else
                {
                    int daysSinceLastClaim = (int)timeSinceLastClaim.TotalDays;
                    int lastClaimedDay = Plugin.Instance.Configuration.Instance.DailyBonuses
                        .FindIndex(bonus => bonus.Day == (playerData.LastBonusClaim.DayOfYear % 7) + 1) + 1;

                    int nextDay = (lastClaimedDay % 7) + 1;
                    int missedDays = (daysSinceLastClaim - 1) % 7;

                    if (missedDays >= (7 - lastClaimedDay + 1))
                    {
                        return 0;
                    }
                    else
                    {
                        return nextDay + missedDays;
                    }
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

        private static void CloseUI(UnturnedPlayer player)
        {
            EffectManager.askEffectClearByID(
                Plugin.Instance.Configuration.Instance.EffectID,
                player.Player.channel.owner.transportConnection
            );
            player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
        }
    }
}