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
    public class HandlePlayerDisconnected
    {
        public static Action<Player> OnPlayerDisconnected;
        
        [HarmonyPatch("DisconnectedMessage")]
        [HarmonyPrefix]
        public static void DisconnectedMessagePrefix(Player player)
        {
            Plugin.logger.LogInfo($"Player {player.PlayerName} disconnected.");
            if (OnPlayerDisconnected != null)
            {
                Plugin.DebugLog("onplayerdisconnected invoking");
                OnPlayerDisconnected.Invoke(player);
                Plugin.DebugLog("onplayerdisconnected invoked succesfully");
            }
            else
            {
                Plugin.DebugLog("OnPlayerDisconnected delegate is null");
            }
        }
    }
    [HarmonyPatch(typeof(NetworkManagerNuclearOption), "LogServerDisconnected")]
    public class HandleDisconnection
    {
        public static Action<INetworkPlayer> OnPlayerDisconnected;
        public static void Prefix(INetworkPlayer player)
        {
            if (OnPlayerDisconnected != null)
            {
                Plugin.DebugLog($"Invoking player component {player} disconnection", "HandleDisconnection");
                OnPlayerDisconnected.Invoke(player);
            }
            else
            {

            }
        }
    }
}
