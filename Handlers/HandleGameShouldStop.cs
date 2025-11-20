using HarmonyLib;
using NuclearOption.DedicatedServer;

namespace NS.Handlers
{
    [HarmonyPatch(typeof(DedicatedServerManager), "GameShouldStop")]
    internal class HandleGameShouldStop
    {
        
    }
}
