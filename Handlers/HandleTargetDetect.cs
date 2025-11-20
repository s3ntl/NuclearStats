using System;
using HarmonyLib;
using NuclearOption.Networking;
using UnityEngine;

namespace NS.Handlers
{
    [HarmonyPatch(typeof(TargetDetector))]
    public class HandleTargetDetect
    {
        public static Action<PersistentID> onTargetDetected;
        [HarmonyPatch("DetectTarget")]
        [HarmonyPrefix]
        public static void DetectTargetPrefix(TargetDetector __instance, Unit target)
        {
            
            Unit attachedUnit = ReflectionUtils.ReadPrivateField<Unit>("attachedUnit", __instance);
            if (attachedUnit is Aircraft)
            {
                Aircraft detector = (Aircraft)attachedUnit;
                if (detector.Player != null)
                {
                    if (!detector.NetworkHQ.trackingDatabase.ContainsKey(target.persistentID))
                    {
                       // Plugin.DebugLog($"Player {detector.Player} made first detect on {target}");
                        onTargetDetected?.Invoke(detector.persistentID);
                    }
                    else if (!detector.NetworkHQ.IsTargetPositionAccurate(target, 500))
                    {
                       // Plugin.DebugLog($"Player {detector.Player} spotted {target}");
                        onTargetDetected?.Invoke(detector.persistentID);
                    }
                }
            }
        }
    }
}
