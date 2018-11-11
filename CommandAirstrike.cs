using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Airstrikes
{
    class CommandAirstrike : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Airstrike";
        public string Help => "Calls in an Airstrike at the place you are looking at.";
        public string Syntax => "";
        public List<string> Aliases => new List<string>() { "AStrike" };
        public List<string> Permissions => new List<string>() { "airstrike" };


        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;
            Vector3? eyePosition = GetEyePosition(Airstrikes.Instance.Configuration.Instance.MaxAirstrikeDistance, uCaller);

            if (!eyePosition.HasValue)
            {
                UnturnedChat.Say(caller, $"There is no where to Airstrike!", Color.red);
                return;
            }

            if (Vector3.Distance(eyePosition.Value, uCaller.Position) > Airstrikes.Instance.Configuration.Instance.MaxAirstrikeDistance)
            {
                UnturnedChat.Say(caller, $"The Airstrike is too far! It has to be less than {Airstrikes.Instance.Configuration.Instance.MaxAirstrikeDistance} meters away!", Color.red);
                return;
            }

            if (Vector3.Distance(eyePosition.Value, uCaller.Position) < Airstrikes.Instance.Configuration.Instance.MinAirstrikeDistance)
            {
                UnturnedChat.Say(caller, $"The Airstrike is too close! It has to be at least {Airstrikes.Instance.Configuration.Instance.MinAirstrikeDistance} meters away!", Color.red);
                return;
            }

            Logger.LogWarning($"{uCaller.DisplayName} has started an Airstrike!");
            Airstrikes.Instance.StartCoroutine(Airstrikes.SendAirstrike(uCaller, eyePosition.Value));
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
