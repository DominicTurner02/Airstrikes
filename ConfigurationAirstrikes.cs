using Rocket.API;

namespace Airstrikes
{
    public class ConfigurationAirstrikes : IRocketPluginConfiguration
    {
        public float MaxAirstrikeDistance;
        public float MinAirstrikeDistance;
        public int StrikeCount;
        public float StartDelay;
        public float StrikeDelayMin;
        public float StrikeDelayMax;
        public float DamageIntensity;
        public float DamageRadius;
        public bool LogAirstrikes;
        public bool BroadcastAirstrikes;
        

        public void LoadDefaults()
        {
            MaxAirstrikeDistance = 250;
            MinAirstrikeDistance = 50;
            StrikeCount = 35;
            StartDelay = 5;
            StrikeDelayMin = 0.25f;
            StrikeDelayMax = 0.75f;
            DamageIntensity = 15;
            DamageRadius = 20;
            LogAirstrikes = false;
            BroadcastAirstrikes = true;
        }

    }
}
