using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using Forge.DailyBonuses.Modules;

namespace Forge.DailyBonuses.Commands
{
    public class CommandDaily : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "daily";
        public string Help => "Manage daily bonuses.";
        public string Syntax => "/daily [reset <steamID64>]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "forge.daily" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is ConsolePlayer)
            {
                if (command.Length == 2 && command[0].ToLower() == "reset")
                {
                    if (ulong.TryParse(command[1], out ulong steamID64))
                    {
                        Plugin.Instance.DataManager.DeletePlayerData(steamID64);
                        UnturnedChat.Say(caller, Plugin.Instance.Translate("DailyBonus_ResetSuccess", steamID64));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Plugin.Instance.Translate("DailyBonus_InvalidSteamID"));
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, Plugin.Instance.Translate("DailyBonus_Usage"));
                }
            }
            else if (caller is UnturnedPlayer player)
            {
                Data playerData = Plugin.Instance.DataManager.GetPlayerData(player.CSteamID.m_SteamID);
                if (playerData == null)
                {
                    Plugin.Instance.DataManager.CreatePlayerData(player.CSteamID.m_SteamID);
                    playerData = Plugin.Instance.DataManager.GetPlayerData(player.CSteamID.m_SteamID);
                }

                int claimedDays = EffectHandler.CalculateClaimedDays(playerData);
                int nextBonusDay = claimedDays + 1;

                if (nextBonusDay > 7)
                {
                    UnturnedChat.Say(player, Plugin.Instance.Translate("DailyBonus_AllReceived"));
                }
                else if (claimedDays == 0 && playerData.LastBonusClaim.Date == DateTime.Today)
                {
                    UnturnedChat.Say(player, Plugin.Instance.Translate("DailyBonus_AlreadyReceived"));
                }
                else
                {
                    EffectHandler.GiveBonus(player, nextBonusDay);
                    Plugin.Instance.DataManager.UpdatePlayerData(playerData);
                    Plugin.Instance.DataManager.SaveData();
                    UnturnedChat.Say(player, Plugin.Instance.Translate("DailyBonus_Received", nextBonusDay));
                }
            }
        }
    }
}