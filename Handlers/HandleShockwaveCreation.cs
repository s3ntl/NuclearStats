using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(Shockwave))]
    [Obsolete("Use HandleWarheadDetonation instead")]
    public class HandleShockwaveCreation
    {
        public static Action<PersistentID, PersistentID> shockWaveCanInfluenceObject;
        public static Action<PersistentID> onNukeExploded;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void PostfixStart(Shockwave __instance)
        {
            
            FieldInfo kilotons = typeof(Shockwave).GetField("yieldKilotons", BindingFlags.Instance | BindingFlags.NonPublic);
            float yieldKilotons = (float)kilotons.GetValue(__instance);
            Plugin.logger.LogWarning($"Shockwave: yieldKilotons: {yieldKilotons}");
            if (yieldKilotons < 1) { return; }

            FieldInfo field = typeof(Shockwave).GetField("ownerID", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Shockwave).GetField("blastRadius", BindingFlags.Instance | BindingFlags.NonPublic);
            PersistentID ownerID = (PersistentID)field.GetValue(__instance);
            float blastRadius = (float)field2.GetValue(__instance);
             
            PersistentUnit persistentPlayerUnit;
            UnitRegistry.TryGetPersistentUnit(ownerID, out persistentPlayerUnit);
            List<int> persistendIDS = new List<int>();
            Plugin.logger.LogInfo(string.Format("NUKE EXPLODED, OWNER {0} [steamID: {1}]", persistentPlayerUnit.player.PlayerName,
                persistentPlayerUnit.player.SteamID));
            LogWriter.WriteLog(string.Format("NUKE EXPLODED, OWNER {0} [steamID: {1}]", persistentPlayerUnit.player.PlayerName,
                persistentPlayerUnit.player.SteamID));
            if (onNukeExploded != null)
            {
                onNukeExploded.Invoke(ownerID);
            }
            foreach (Collider collider in Physics.OverlapSphere(__instance.transform.position, blastRadius * 2))
            {
               
                UnitPart part = collider.GetComponent<UnitPart>();
                if (part != null)
                {
                    if (shockWaveCanInfluenceObject != null && persistentPlayerUnit != null) 
                    {
                        
                        shockWaveCanInfluenceObject.Invoke(persistentPlayerUnit.unit.persistentID, part.parentUnit.persistentID);
                    }
                  // Debug.Log(string.Format("Part name: {0}, Part parrent: {1}", part.name, part.parentUnit));
                }
                
            }

        }
    }
}
