using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mirage;
using NuclearOption.Networking;
namespace NS.Handlers
{
    [HarmonyPatch(typeof(MessageManager))]
    public class HandlePlayerConnected
    {
        public static Action<Player> OnPlayerConnected;
        
        [HarmonyPatch("JoinMessage")]
        [HarmonyPrefix]
        public static void JoinMessagePrefix(Player joinedPlayer)
        {
            Plugin.logger.LogInfo($"Player {joinedPlayer.PlayerName} connected.");
            if (OnPlayerConnected != null)
            {
               
                Plugin.DebugLog("onplayerconnected invoking");
                OnPlayerConnected.Invoke(joinedPlayer);
                Plugin.DebugLog("onplayerconnected invoked succesfully");
            }
            else
            {
                Plugin.DebugLog("onplayer connected delegate is null");
            }
        }
    }

    [HarmonyPatch(typeof(NetworkManagerNuclearOption), "LogServerConnected")]
    public class HandleConnection
    {
        public static Action<INetworkPlayer> OnPlayerConnected;
        public static void Prefix(INetworkPlayer player)
        {
            if (OnPlayerConnected != null)
            {
                Plugin.DebugLog($"Invoking player component {player} connection", "HandleConnection");
                OnPlayerConnected.Invoke(player);
            }
            else
            {

            }
        }
    }
}
