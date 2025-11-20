using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using NuclearOption.Networking;
using UnityEngine;

namespace NS.Handlers
{
    [HarmonyPatch(typeof(Missile.Warhead), "Detonate")]
    public class HandleWarheadDetonation
    {
        public static Action<PersistentID> onWarheadDetonated;
        public static Action<PersistentID, PersistentID> shockwaveCanInfluenceObject;
        public static void Prefix(Missile.Warhead __instance, Rigidbody rb, PersistentID ownerID, Vector3 position, Vector3 normal, bool armed, float blastYield, bool hitArmor, bool hitTerrain)
        {
            //if (Plugin.LogEnabled.Value) Plugin.DebugLog($"warhead detonated, blastYiled {blastYield}, convert to kilotons: {blastYield / 1000000}");

            if (blastYield >= 1500000)
            {
                PersistentUnit unit;
                if (UnitRegistry.TryGetPersistentUnit(ownerID, out unit))
                {
                    float blastRadius = Mathf.Pow((blastYield / 1000000) * 1000000f, 0.3333f) * 13;
                    if (unit.player != null && onWarheadDetonated != null)
                    {
                        onWarheadDetonated.Invoke(ownerID);
                        Plugin.logger.LogInfo($"NUKE EXPLODED, OWNER {unit.player} [steamID: {unit.player.SteamID}]");
                        LogWriter.WriteLog($"NUKE EXPLODED, OWNER {unit.player} [steamID: {unit.player.SteamID}]");
                    }
                    foreach (Collider collider in Physics.OverlapSphere(position, blastRadius * 2))
                    {

                        UnitPart part = collider.GetComponent<UnitPart>();
                        if (part != null)
                        {
                            if (shockwaveCanInfluenceObject != null && unit != null)
                            {

                                shockwaveCanInfluenceObject.Invoke(unit.unit.persistentID, part.parentUnit.persistentID);
                            }
                            // Debug.Log(string.Format("Part name: {0}, Part parrent: {1}", part.name, part.parentUnit));
                        }

                    }
                }
                
            }
        }
    }
}
