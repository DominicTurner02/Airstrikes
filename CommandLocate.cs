using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Airstrikes
{
    class CommandLocate : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Locate";
        public string Help => "Gives you the Coordinates of your Marker/Location.";
        public string Syntax => "/locate <WP>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "locate" };


        public void Execute(IRocketPlayer rCaller, params string[] Command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)rCaller;

            if (Command.Length == 0) // Players wants own position
            {
                UnturnedChat.Say(uCaller, $"Your Location - X: {uCaller.Position.x} | Y: {uCaller.Position.y} | Z: {uCaller.Position.z}", Color.yellow);
                Logger.Log($"{uCaller.DisplayName}'s Location - X: { uCaller.Player.quests.markerPosition.x} | Y: { uCaller.Player.quests.markerPosition.y} | Z: { uCaller.Player.quests.markerPosition.z}", ConsoleColor.Cyan);
                return;

            } else if (Command.Length == 1) // Players wants waypoint location
            {
                if (Command[0].ToLower() == "wp")
                {
                    if (uCaller.Player.quests.isMarkerPlaced)
                    {
                        Vector3? groundLocation = Airstrikes.GetGround(uCaller.Player.quests.markerPosition);
                        if (groundLocation == null)
                        {
                            UnturnedChat.Say(uCaller, "There has been an Error getting your Marker location, please check the Console.", Color.red);
                            Logger.LogError("[Error] Encountered error whilst trying to get Marker location - returned null. Please contact the plugin author.");
                            return;
                        }
                        UnturnedChat.Say(uCaller, $"Marker Location - X: {groundLocation.Value.x} | Y: {groundLocation.Value.y} | Z: {groundLocation.Value.z}", Color.yellow);
                        Logger.Log($"{uCaller.DisplayName}'s Marker Location - X: {groundLocation.Value.x} | Y: {groundLocation.Value.y} | Z: {groundLocation.Value.z}", ConsoleColor.Cyan);
                        return;
                    } else
                    {
                        UnturnedChat.Say(uCaller, "You need to place a Marker before you use this command!", Color.red);
                        return;
                    }
                } else
                {
                    UnturnedChat.Say(uCaller, Syntax, Color.red);
                    return;
                }
            } else
            {
                UnturnedChat.Say(uCaller, Syntax, Color.red);
                return;
            }
        }



    }
}
