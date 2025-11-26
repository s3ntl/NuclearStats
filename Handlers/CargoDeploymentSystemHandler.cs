using System;
using HarmonyLib;

namespace NS.Handlers
{
    [HarmonyPatch(typeof(CargoDeploymentSystem))]
    public class CargoDeploymentSystemHandler
    {
        public static Action<Unit> onUnitDisabled;
        public static Action<Unit> onUnitLanded;

        [HarmonyPatch("CargoDeploymentSystem_OnUnitDisabled")]
        [HarmonyPrefix]
        public static void OnUnitDisabled(CargoDeploymentSystem __instance, Unit unit)
        {
            Plugin.logger.LogInfo($"Parachuting unit disabled: {unit}");
            onUnitDisabled?.Invoke(unit);
        }

        [HarmonyPatch("CargoDeploymentSystem_OnUnitLanded")]
        [HarmonyPrefix]
        public static void OnUnitLanded(CargoDeploymentSystem __instance)
        {
            Unit unit = ReflectionUtils.ReadPrivateField<Unit>("attachedUnit", __instance);
            Plugin.logger.LogInfo($"Parachuting unit landed: {unit}");
            onUnitLanded?.Invoke(unit);
        }
    }
}
