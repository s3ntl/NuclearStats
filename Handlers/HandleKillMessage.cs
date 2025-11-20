using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(MessageManager))]
    public class HandleKillMessage
    {
        public static Action<PersistentID, PersistentID> HandleKill;
        [HarmonyPatch("RpcKillMessage")]
        [HarmonyPrefix]
        public static void Prefix(PersistentID killerID, PersistentID killedID)
        {
            if (HandleKill != null)
            {
                HandleKill.Invoke(killerID, killedID);
            }
        }
    }
}
