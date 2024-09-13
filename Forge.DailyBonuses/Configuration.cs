using Rocket.API;

namespace Forge.DailyBonuses
{
    public class Configuration : IRocketPluginConfiguration
    {
        public ushort EffectID { get; internal set; }
        public short EffectKey { get; internal set; }

        public void LoadDefaults()
        {
            throw new System.NotImplementedException();
        }
    }
}