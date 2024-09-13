using Rocket.API;
using System.Collections.Generic;

namespace Forge.DailyBonuses
{
    public class Configuration : IRocketPluginConfiguration
    {
        public ushort EffectID { get; set; }
        public short EffectKey { get; set; }
        public bool ResetProgressAfterAllBonuses { get; set; }
        public bool AllowClaimingMissedBonuses { get; set; }

        public List<DailyBonus> DailyBonuses { get; set; }

        public void LoadDefaults()
        {
            EffectID = 30;
            EffectKey = 0;
            ResetProgressAfterAllBonuses = true;
            AllowClaimingMissedBonuses = true;

            DailyBonuses = new List<DailyBonus>
            {
               new DailyBonus {
                    Day = 1,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 2,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 3,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 4,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 5,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 6,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
               new DailyBonus {
                    Day = 7,
                    Commands = new List<string>
                    {
                        "experience {player} 5000",
                        "give {player} 54 5"
                    }
               },
            };
        }
    }

    public class DailyBonus
    {
        public int Day { get; set; }
        public List<string> Commands { get; set; }
    }
}