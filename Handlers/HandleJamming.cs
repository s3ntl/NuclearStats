using System;
using HarmonyLib;
using UnityEngine;
namespace NS.Handlers
{
    [HarmonyPatch(typeof(JammingPod), "Fire")]
    public class HandleJamming
    {
        public static Action<PersistentID, Weapon> OnFire;
        [HarmonyPostfix]
        public static void Fire(JammingPod __instance, Unit owner, Unit target, Vector3 inheritedVelocity, WeaponStation weaponStation, GlobalPosition aimpoint)
        {
            //Plugin.DebugLog($"fire method, instance type {__instance.GetType()}" ,"HandleFire");
            OnFire?.Invoke(owner.persistentID, __instance);
        }
    }
}
