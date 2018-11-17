using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Airstrikes
{
    class CommandBoom : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "Boom";
        public string Help => "Explodes either a User or what you are looking at.";
        public string Syntax => "/boom <User>";
        public List<string> Aliases => new List<string>() { "explode" };
        public List<string> Permissions => new List<string>() { "boom" };


        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;
            

            if (command.Length == 0) // No user selected
            {

                Vector3? eyePosition = GetEyePosition(Airstrikes.Instance.Configuration.Instance.MaxBoomDistance, uCaller);

                if (!eyePosition.HasValue)
                {
                    UnturnedChat.Say(caller, $"There is no where to boom (Max distance: {Airstrikes.Instance.Configuration.Instance.MaxBoomDistance})!", Color.red);
                    return;
                }


                List<EPlayerKill> boomList = new List<EPlayerKill>();
                EffectManager.sendEffect(20, EffectManager.INSANE, eyePosition.Value);
                DamageTool.explode(eyePosition.Value, 10f, EDeathCause.KILL, uCaller.CSteamID, 200, 200, 200, 200, 200, 200, 200, 200, out boomList, EExplosionDamageType.CONVENTIONAL, 32, true, false, EDamageOrigin.Unknown);
                boomList.Clear();
                UnturnedChat.Say(uCaller, $"Successfully exploded position: {eyePosition.Value} ({(int)Vector3.Distance(eyePosition.Value, uCaller.Position)} meters).");
                Logger.LogWarning($"{uCaller.DisplayName} has exploded position: {eyePosition.Value} ({(int)Vector3.Distance(eyePosition.Value, uCaller.Position)} meters).");


            }
            else // User selected
            {
                UnturnedPlayer uVictim;
                Vector3 victimPosition;
                try
                {
                    uVictim = UnturnedPlayer.FromName(command[0]);
                    victimPosition = uVictim.Position;

                } catch
                {
                    UnturnedChat.Say(uCaller, "User not found!", Color.red);
                    return;
                }

                
                List<EPlayerKill> boomList = new List<EPlayerKill>();
                EffectManager.sendEffect(20, EffectManager.INSANE, victimPosition);
                DamageTool.explode(victimPosition, 10f, EDeathCause.KILL, uCaller.CSteamID, 100, 100, 100, 100, 100, 100, 100, 100, out boomList, EExplosionDamageType.CONVENTIONAL, 32, true, false, EDamageOrigin.Unknown);
                boomList.Clear();

                UnturnedChat.Say(uCaller, $"Successfully exploded {uVictim.DisplayName}");
                Logger.LogWarning($"{uCaller.DisplayName} has exploded {uVictim.DisplayName} ({(int)Vector3.Distance(uCaller.Position, uVictim.Position)} meters from caller)!");



            }

        }

        private Vector3? GetEyePosition(float distance, UnturnedPlayer tempPlayer)
        {
            int Masks = RayMasks.BLOCK_COLLISION & ~(1 << 0x15);
            PlayerLook Look = tempPlayer.Player.look;

            Physics.Raycast(Look.aim.position, Look.aim.forward, out RaycastHit Raycast, distance, Masks);

            if (Raycast.transform == null)
                return null;

            return Raycast.point;
        }

    }
}
