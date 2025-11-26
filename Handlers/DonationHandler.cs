using HarmonyLib;
namespace NS.Handlers
{
    [HarmonyPatch(typeof(NuclearOption.Networking.Player))]
    public class DonationHandler
    {
        [HarmonyPatch("UserCode_CmdDonateFactionUnit_1989758706")]
        [HarmonyPrefix]
        public static void Prefix(NuclearOption.Networking.Player __instance, UnitDefinition unitDefinition, int quantity)
        {
            Plugin.logger.LogInfo($"{__instance.PlayerName} donated {quantity} {unitDefinition.name}");
        }
    }
}
