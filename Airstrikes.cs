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
        public List<Vector3> PlayerVectors;
        public static int AirstrikeCount = 0;

        protected override void Load()
        {
            base.Load();
            Instance = this;
            Vectors = new List<Vector3>();
            PlayerVectors = new List<Vector3>();

            Logger.LogWarning("\n Loading Airstrikes, made by Mr.Kwabs...");
            Logger.LogWarning("\n General Options:");
            Logger.LogWarning($" Max Boom Distance: {Instance.Configuration.Instance.MaxBoomDistance} meters.");
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
                System.IO.Directory.CreateDirectory("Plugins/Airstrikes/Logs");
                Logger.LogWarning(" Log Airstrikes: Enabled.");
            }
            else
            {
                Logger.LogError(" Log Airstrikes: Disabled.");
            }    
            Logger.LogWarning("\n General Airstrike Options:");
            Logger.LogWarning($" Max Airstrike Distance: {Instance.Configuration.Instance.MaxAirstrikeDistance} meters.");
            Logger.LogWarning($" Min Airstrike Distance: {Instance.Configuration.Instance.MinAirstrikeDistance} meters.");
            Logger.LogWarning($" Airstrike Strike Count: {Instance.Configuration.Instance.StrikeCount}.");
            Logger.LogWarning($" Airstrike Start Delay: {Instance.Configuration.Instance.StartDelay} seconds.");
            Logger.LogWarning($" Airstrike Location Effect: {Instance.Configuration.Instance.AirstrikeLocationEffectID}");
            Logger.LogWarning("\n Specific Airstrike Options:");
            Logger.LogWarning($" Strike Interval Max: {Instance.Configuration.Instance.StrikeDelayMax} seconds.");
            Logger.LogWarning($" Strike Interval Min: {Instance.Configuration.Instance.StrikeDelayMin} seconds.");
            Logger.LogWarning($" Strike Damange Intensity: {Instance.Configuration.Instance.DamageIntensity}.");
            Logger.LogWarning($" Strike Damange Radius: {Instance.Configuration.Instance.DamageRadius} meters.");
            Logger.LogWarning($" Strike Explosion Effect: {Instance.Configuration.Instance.StrikeExplosionEffectID}");
            Logger.LogWarning("\n Successfully loaded Airstrikes, made by Mr.Kwabs!");
        }

        protected override void Unload()
        {
            Instance = null;
            base.Unload();
        }

        private static void LogAirstrike(string AirstrikeLog, List<Vector3> StrikeLocations) 
        {
            using (StreamWriter LogSW = new StreamWriter(File.Open($"Plugins/Airstrikes/Logs/Airstrikes ({DateTime.Now.ToString("yyy-MM-dd HH.mm.ss")}).log", FileMode.Append)))
            {
                LogSW.WriteLine(AirstrikeLog);
                if (StrikeLocations.Count != 0)
                {
                    LogSW.WriteLine($"\n\n\nIndividual Strike Locations:\n");
                    int Count = 1;
                    foreach (Vector3 Location in StrikeLocations)
                    {
                        LogSW.WriteLine($"Strike {Count}: {Location}"); 

                        Count++;
                    }   
                }
            }
        }

        public static IEnumerator SendAirstrike(UnturnedPlayer uCaller, Vector3 Position)
        {
            Vector3 callerPosition = new Vector3();
            List<Vector3> StrikeList = new List<Vector3>();
            Instance.Vectors.Add(Position);

            EffectManager.sendEffect(Instance.Configuration.Instance.AirstrikeLocationEffectID, EffectManager.INSANE, GetGround(Position).Value);

            if (Instance.Configuration.Instance.LogAirstrikes)
            {
                callerPosition = uCaller.Position;  
                AirstrikeCount += 1;
            }
            if (Instance.Configuration.Instance.BroadcastAirstrikes)
            {
                UnturnedChat.Say($"Incomming Airstrike in {Instance.Configuration.Instance.StartDelay} Seconds! The Location is marked on your Map! Get into cover!", Color.yellow);
            }
            
            foreach (SteamPlayer sPlayer in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(sPlayer);
                uPlayer.Player.quests.askSetMarker(uPlayer.CSteamID, true, Position);
            }
            yield return new WaitForSeconds(Instance.Configuration.Instance.StartDelay);
            Instance.Vectors.Remove(Position);           

            DateTime beforeStrike = DateTime.Now;
            for (int i = 0; i < (Instance.Configuration.Instance.StrikeCount + 1); i++)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(Instance.Configuration.Instance.StrikeDelayMin, Instance.Configuration.Instance.StrikeDelayMax));

                Vector3 airstrikeLocation = new Vector3(UnityEngine.Random.Range(Position.x - Instance.Configuration.Instance.DamageIntensity, Position.x + Instance.Configuration.Instance.DamageRadius), Position.y + 100, UnityEngine.Random.Range(Position.z - Instance.Configuration.Instance.DamageRadius, Position.z + Instance.Configuration.Instance.DamageRadius));
                Ray airstrikeRay = new Ray(airstrikeLocation, Vector3.down);
                if (Instance.Configuration.Instance.LogAirstrikes)
                {
                    StrikeList.Add(airstrikeLocation);
                }

                if (Physics.Raycast(airstrikeRay, out RaycastHit Hit))
                {             
                    EffectManager.sendEffect(Instance.Configuration.Instance.StrikeExplosionEffectID, EffectManager.INSANE, Hit.point);
                    List<EPlayerKill> killList = new List<EPlayerKill>();
                    DamageTool.explode(Hit.point, Instance.Configuration.Instance.DamageIntensity, EDeathCause.MISSILE, uCaller.CSteamID, 200, 200, 200, 200, 200, 200, 200, 200, out killList, EExplosionDamageType.CONVENTIONAL, 32, true, false, EDamageOrigin.Unknown);
                    killList.Clear();
                }
            }
            DateTime afterStrike = DateTime.Now;

            if (Instance.Configuration.Instance.LogAirstrikes)
            {
                LogAirstrike($"[{DateTime.Now.ToString("yyy/MM/dd h:mm:ss tt")}]\n\nUser:\n{uCaller.DisplayName} [{uCaller.Id}].\n\nAirstrike Start Delay:\n{Instance.Configuration.Instance.StartDelay} seconds.\n\nStrikes:\n{Instance.Configuration.Instance.StrikeCount + 1}.\n\nAverage Strike Time:\n{(Instance.Configuration.Instance.StrikeDelayMin + Instance.Configuration.Instance.StrikeDelayMax) / 2} seconds.\n\nCaller Position:\n{callerPosition}.\n\nAirstrike Location:\n{Position}.\n\nDistance from Caller:\n{(int)Vector3.Distance(callerPosition, Position)}M.\n\nDamage Intensity:\n{Instance.Configuration.Instance.DamageIntensity}.\n\nStrike Radius:\n{Instance.Configuration.Instance.DamageRadius}M.\n\nAirstrikes this Uptime: {AirstrikeCount}.\n\nThis Airstrike lasted {Math.Round((afterStrike - beforeStrike).TotalSeconds, 2)} Seconds.", StrikeList);
                StrikeList.Clear();
            }
            
            if (Instance.Configuration.Instance.BroadcastAirstrikes)
            {
                UnturnedChat.Say("The Airstrike is over!", Color.yellow);
            }

            foreach (SteamPlayer sPlayer in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(sPlayer);
                uPlayer.Player.quests.askSetMarker(uPlayer.CSteamID, false, Position);
                
            }
            yield return new WaitForSeconds(Instance.Configuration.Instance.LocationFadeTime);
            EffectManager.askEffectClearByID(Instance.Configuration.Instance.AirstrikeLocationEffectID, uCaller.CSteamID);
        }

        private static Vector3? GetGround(Vector3 Position)
        {
            int layerMasks = (RayMasks.BARRICADE | RayMasks.BARRICADE_INTERACT | RayMasks.ENEMY | RayMasks.ENTITY | RayMasks.ENVIRONMENT | RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.ITEM | RayMasks.RESOURCE | RayMasks.STRUCTURE | RayMasks.STRUCTURE_INTERACT);
            if (Physics.Raycast(new Vector3(Position.x, Position.y + 200, Position.z), Vector3.down, out RaycastHit Hit, 250, layerMasks))
            {
                return Hit.point;
            } else
            {
                return null;
            }
        }
    }
}
