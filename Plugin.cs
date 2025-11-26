using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using NS.Handlers;
using BepInEx.Configuration;
using System;
using System.IO;
using NS.Sorties;
using NS.StateCopyClasses;




/* 
Игрок сбил игрока (на чем игроки были) +
Игрок уничтожил ботика (какого) +

Игрок сдох/был сбит +



Какая карта сейчас играется, сколько прошло времени со старта (это в целом и так есть, но можно и чаще выводить, хотя я хз)
В конце боя после результатов матча:

Сколько у кого очков +
Сколько очков у фракции +
Кто победил по итогу +



-----------------------------
Игрок заебашил ядеркой (кого) +
-----------------------------
*/

namespace NS
{
    
    [BepInPlugin("NuclearStats", "NS", "1.0.2.4")]
    
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        ReadKillMessage readKillMessage;
        public static PlayersSortieManager playersSortieController = new PlayersSortieManager();
        GameResultReader resultReader = new GameResultReader();
        //GameController gameController = new GameController();

        public static ConfigEntry<string> LogPath { get; private set; }
        public static ConfigEntry<bool> LogEnabled { get; private set; }

        private void InitConfig()
        {
            string home = AppDomain.CurrentDomain.BaseDirectory;
            LogPath = Config.Bind<string>("Logging", "Path", $"{home}\\NuclearStats", "Путь к файлу с логами");
            LogEnabled = Config.Bind<bool>("ConsoleLogs", "Logging Enabled", true, "Вывод дебаг инфы");
        }
      

        public void Awake()
        {
            Plugin.logger = base.Logger;
            Harmony harmony = new Harmony("NS");
            readKillMessage = new ReadKillMessage();
            InitConfig(); 

            harmony.PatchAll(typeof(HandleGameEnd));
            harmony.PatchAll(typeof(HandleKillMessage));
            harmony.PatchAll(typeof(HandleFactionJoin));
            //harmony.PatchAll(typeof(HandleShockwaveCreation));
            harmony.PatchAll(typeof(HandleMissionLoad));
            harmony.PatchAll(typeof(HandleMapPrefabName));
            harmony.PatchAll(typeof(HandleUnitRegister));
            harmony.PatchAll(typeof(HandleUnitUnregister));
            harmony.PatchAll(typeof(HandlePlayerConnected));
            harmony.PatchAll(typeof(HandlePlayerDisconnected));
            harmony.PatchAll(typeof(HandleWarheadDetonation));
            harmony.PatchAll(typeof(HandleConnection));
            harmony.PatchAll(typeof(HandleDisconnection));
            harmony.PatchAll(typeof(HandleJamming));
            harmony.PatchAll(typeof(HandleTargetDetect));
            harmony.PatchAll(typeof(CargoDeploymentSystemHandler));

            harmony.PatchAll(typeof(DonationHandler));

            resultReader.Awake();
            readKillMessage.Awake();
            playersSortieController.Awake();
            PlayersSavedDataManager.Awake();
            //gameController.Awake();
            logger.LogInfo("Plugin loaded");
        }



        public void Update() 
        { 
            readKillMessage.Update();
            //gameController.Update();
            if (Input.GetKeyDown(KeyCode.F5))
            {
                playersSortieController.ShowSorties();
            }
           
        }

        public void FixedUpdate()
        {
            playersSortieController?.FixedUpdate();
        }

        public static void ShowSorties()
        {
            playersSortieController.ShowSorties();
        }
        public static void DebugLog(string msg, string sender = null)
        {
            if (Plugin.LogEnabled.Value)
            {
                if (sender != null) { msg = $"[{sender}]: {msg}"; }
                Plugin.logger.LogWarning($"[DEBUG INFO]{msg}");
            }
        }
       

    }
}
