using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

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
        public bool AutoAirstrikes;
        public List<AutoAirstrike> AutomaticAirstrikes;
        public int AutoAirstrikeIntervalMinutes;
        public bool BroadcastAirstrikes;
        public bool LogAirstrikes;


        public class AutoAirstrike
        {
            public AutoAirstrike() { }

            internal AutoAirstrike(int strikeCount, float strikeSpeed, float damageIntensity, float damageRadius, string name, string position)
            {
                StrikeCount = strikeCount;
                StrikeSpeed = strikeSpeed;
                DamageIntensity = damageIntensity;
                DamageRadius = damageRadius;
                Name = name;
                Position = position;
            }


            [XmlAttribute]
            public int StrikeCount;
            public float StrikeSpeed;
            public float DamageIntensity;
            public float DamageRadius;
            public string Name;
            public string Position;           
        }


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
            AutoAirstrikes = false;
            AutoAirstrikeIntervalMinutes = 5;
            AutomaticAirstrikes = new List<AutoAirstrike>()
            {
                new AutoAirstrike(25, 0.5f, 15, 20, "Seattle", "(-334.8, 38.7, 129.0)"),
                new AutoAirstrike(25, 0.5f, 15, 20, "Scorpion-7", "(853.9, 43.5, 655.2)")
            };
            BroadcastAirstrikes = true;
            LogAirstrikes = false;
        }
    }
}
