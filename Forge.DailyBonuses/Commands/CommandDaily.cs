using Forge.DailyBonuses.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Forge.DailyBonuses.Commands
{
    public class CommandDaily : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "daily";

        public string Help => "Reset daily bonus progress for a player.";

        public string Syntax => "/daily reset <steamID64>";

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
                EffectHandler.sendEffectReward(player);
            }
        }
    }
}