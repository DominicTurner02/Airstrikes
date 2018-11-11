using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Airstrikes
{
    public class Airstrikes : RocketPlugin<ConfigurationAirstrikes>
    {
        public static Airstrikes Instance { get; private set; }
        public List<Vector3> Vectors;

        protected override void Load()
        {
            base.Load();
            Instance = this;
            Vectors = new List<Vector3>();

            Logger.LogWarning("\n Loading Airstrikes, made by Mr.Kwabs...");
            Logger.LogWarning("\n General Options:");
            if (Instance.Configuration.Instance.BroadcastAirstrikes)
            {
                Logger.LogWarning(" Broadcasts Airstrikes: Enabled.");
            }
            else
            {
                Logger.LogError(" Broadcasts Airstrikes: Disabled.");
            }
            if (Instance.Configuration.Instance.LogAirstrikes)
            {
                Logger.LogWarning(" Log Airstrikes: Enabled.");
            }
            else
            {
                Logger.LogError(" Log Airstrikes: Disabled.");
            }
            Logger.LogWarning("\n Successfully loaded Airstrikes, made by Mr.Kwabs!");
            Logger.LogWarning("\n General Airstrike Options:");
            Logger.LogWarning($" Max Airstrike Distance: {Instance.Configuration.Instance.MaxAirstrikeDistance} meters.");
            Logger.LogWarning($" Min Airstrike Distance: {Instance.Configuration.Instance.MinAirstrikeDistance} meters.");
            Logger.LogWarning($" Airstrike Strike Count: {Instance.Configuration.Instance.StrikeCount}.");
            Logger.LogWarning($" Airstrike Start Delay: {Instance.Configuration.Instance.StartDelay} seconds.");
            Logger.LogWarning("\n Specific Airstrike Options:");
            Logger.LogWarning($" Strike Interval Max: {Instance.Configuration.Instance.StrikeDelayMax} seconds.");
            Logger.LogWarning($" Strike Interval Min: {Instance.Configuration.Instance.StrikeDelayMin} seconds.");
            Logger.LogWarning($" Strike Damange Intensity: {Instance.Configuration.Instance.DamageIntensity}.");
            Logger.LogWarning($" Strike Damange Radius: {Instance.Configuration.Instance.DamageRadius} meters.");

        }
        protected override void Unload()
        {
            Instance = null;
            base.Unload();
        }

        public static IEnumerator SendAirstrike(UnturnedPlayer uCaller, Vector3 Position)
        {
            Instance.Vectors.Add(Position);

            if (Instance.Configuration.Instance.BroadcastAirstrikes)
            {
                UnturnedChat.Say($"Incomming Airstrike in {Instance.Configuration.Instance.StartDelay} Seconds! Get into cover!", Color.yellow);
            }
            
            yield return new WaitForSeconds(Instance.Configuration.Instance.StartDelay);

            Instance.Vectors.Remove(Position);

            if (Instance.Configuration.Instance.LogAirstrikes)
            {
                using (StreamWriter LogSW = new StreamWriter(File.Open("Plugins/Airstrikes/Airstrikes.log", FileMode.Append))) { LogSW.WriteLine($"[{DateTime.Now.ToString("yyy/MM/dd h:mm:ss tt")}] {uCaller.DisplayName} has called an Airstrike at Location {Position}."); }
            }
            for (int i = 0; i < (Instance.Configuration.Instance.StrikeCount + 1); i++)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(Instance.Configuration.Instance.StrikeDelayMin, Instance.Configuration.Instance.StrikeDelayMax));

                Ray airstrikeRay = new Ray(new Vector3(UnityEngine.Random.Range(Position.x - Instance.Configuration.Instance.DamageIntensity, Position.x + Instance.Configuration.Instance.DamageRadius), Position.y + 50, UnityEngine.Random.Range(Position.z - Instance.Configuration.Instance.DamageRadius, Position.z + Instance.Configuration.Instance.DamageRadius)), Vector3.down);

                if (Physics.Raycast(airstrikeRay, out RaycastHit Hit))
                {                
                    List<EPlayerKill> killList = new List<EPlayerKill>();
                    EffectManager.sendEffect(20, EffectManager.INSANE, Hit.point);
                    DamageTool.explode(Hit.point, Instance.Configuration.Instance.DamageIntensity, EDeathCause.MISSILE, uCaller.CSteamID, 200, 200, 200, 200, 200, 200, 200, 200, out killList, EExplosionDamageType.CONVENTIONAL, 32, true, false, EDamageOrigin.Unknown);       
                    killList.Clear();
                }
            }

            if (Instance.Configuration.Instance.BroadcastAirstrikes)
            {
                UnturnedChat.Say("The Airstrike is over!", Color.yellow);
            }
            
        }
    }
}
