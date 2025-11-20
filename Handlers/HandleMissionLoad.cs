using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using NuclearOption.SavedMission;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(Mission))]
    public static class HandleMissionLoad
    {
        public static Action OnMissionLoad;
        [HarmonyPatch("AfterLoad", typeof(MissionKey))]
        [HarmonyPrefix] 
        public static void Prefix(Mission __instance, MissionKey key)
        {
            Plugin.logger.LogInfo(string.Format("MISION NAME: {0} \nMISSIN KEY: {1}", key.Name, key.Key));  
            Plugin.DebugLog(string.Format("MISION NAME: {0} \nMISSIN KEY: {1}", key.Name, key.Key));
            LogWriter.WriteLog(string.Format("MISION NAME: {0} \nMISSIN KEY: {1}", key.Name, key.Key));
            if (OnMissionLoad != null)
            {
                OnMissionLoad.Invoke();
            }
        }
    }
    [HarmonyPatch(typeof(MapSettingsManager))]
    public static class HandleMapPrefabName
    {
        public static Action onMapLoaded;
        [HarmonyPatch("LoadMap")]
        [HarmonyPrefix]
        public static void Prefix(MapSettingsManager.Map mapPrefab)
        {
            string prefabName = mapPrefab.Details.PrefabName;
            
            if (prefabName == "Terrain1") prefabName = "Heartland";
            else if (prefabName == "Terrain_naval")  prefabName = "Ignus"; 

            Plugin.logger.LogInfo(string.Format("CURRENT MAP: {0}" , prefabName));
            LogWriter.WriteLog(string.Format("CURRENT MAP: {0}", prefabName));
            if (onMapLoaded != null)
            {
                onMapLoaded.Invoke();
            }
        }
    }
}
