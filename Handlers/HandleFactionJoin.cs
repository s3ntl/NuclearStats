using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using NuclearOption.Networking;
namespace NS
{
    [HarmonyPatch(typeof(MessageManager))]
    internal class HandleFactionJoin
    {
        [HarmonyPatch("UserCode_RpcPlayerJoinFactionMessage_156835807")]
        [HarmonyPrefix]
        public static void Prefix(Player player, FactionHQ hq)
        {
            Plugin.logger.LogInfo(string.Format("Player {0} [steamdID: {2}] joined faction {1}", player.PlayerName,
                hq.faction.factionName, player.SteamID));
            LogWriter.WriteLog(string.Format("Player {0} [steamdID: {2}] joined faction {1}", player.PlayerName,
                hq.faction.factionName, player.SteamID));
        }
    }
}
