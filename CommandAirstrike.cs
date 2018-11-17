using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Airstrikes
{
    class CommandAirstrike : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Airstrike";
        public string Help => "Calls in an Airstrike at your map Marker/Waypoint!.";
        public string Syntax => "";
        public List<string> Aliases => new List<string>() { "AStrike" };
        public List<string> Permissions => new List<string>() { "airstrike" };


        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;

            if (!uCaller.Player.quests.isMarkerPlaced)
            {
                UnturnedChat.Say(uCaller, "Please set a Marker on your map before using this command!", Color.red);
                return;
            } 

            if (Vector3.Distance(uCaller.Player.quests.markerPosition, uCaller.Position) > Airstrikes.Instance.Configuration.Instance.MaxAirstrikeDistance)
            {
                UnturnedChat.Say(caller, $"The Airstrike is too far away! It has to be less than {Airstrikes.Instance.Configuration.Instance.MaxAirstrikeDistance} meters away!", Color.red);
                return;
            }

            if (Vector3.Distance(uCaller.Player.quests.markerPosition, uCaller.Position) < Airstrikes.Instance.Configuration.Instance.MinAirstrikeDistance)
            {
                UnturnedChat.Say(caller, $"The Airstrike is too close! It has to be at least {Airstrikes.Instance.Configuration.Instance.MinAirstrikeDistance} meters away!", Color.red);
                return;
            }

            Logger.LogWarning($"{uCaller.DisplayName} has started an Airstrike!");
            Airstrikes.Instance.StartCoroutine(Airstrikes.SendAirstrike(uCaller, uCaller.Player.quests.markerPosition));
        }

    }
}
