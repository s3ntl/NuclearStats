using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(FactionHQ))]
    public class HandleGameEnd
    {
       
        public static Action<FactionHQ, EndType> onGameEnded;
        [HarmonyPatch("DeclareEndGame")]
        [HarmonyPrefix]
        public static bool Prefix(FactionHQ __instance, EndType endType)
        {
           
            if (onGameEnded != null)
            {
                onGameEnded.Invoke(__instance, endType);
            }
            return true;
        }
    }
}
