using Rocket.API;

namespace Airstrikes
{
    public class ConfigurationAirstrikes : IRocketPluginConfiguration
    {
        public float MaxBoomDistance;
        public float MaxAirstrikeDistance;
        public float MinAirstrikeDistance;
        public int StrikeCount;
        public float StartDelay;
        public float StrikeDelayMin;
        public float StrikeDelayMax;
        public float DamageIntensity;
        public float DamageRadius;
        public ushort AirstrikeLocationEffectID;
        public ushort StrikeExplosionEffectID;
        public float LocationFadeTime;
        public bool LogAirstrikes;
        public bool BroadcastAirstrikes;
        

        public void LoadDefaults()
        {
            MaxBoomDistance = 250;
            MaxAirstrikeDistance = 250;
            MinAirstrikeDistance = 50;
            StrikeCount = 35;
            StartDelay = 5;
            StrikeDelayMin = 0.25f;
            StrikeDelayMax = 0.75f;
            DamageIntensity = 15;
            DamageRadius = 20;
            AirstrikeLocationEffectID = 120;
            StrikeExplosionEffectID = 45;
            LocationFadeTime = 5f;
            LogAirstrikes = false;
            BroadcastAirstrikes = true;
        }

    }
}
