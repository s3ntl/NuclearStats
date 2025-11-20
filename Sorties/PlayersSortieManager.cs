using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using NS.Handlers;
using UnityEngine;

namespace NS.Sorties
{
    public class PlayersSortieManager
    {
        private Dictionary<PersistentID, Sortie> playerAircraftsInfo = new Dictionary<PersistentID, Sortie>();

        public void Awake()
        {
            HandleUnitRegister.register += RegisterPlayerSortieStart;
            HandleUnitUnregister.unregister += RegisterPlayerSortieEnd;
            HandleMissionLoad.OnMissionLoad += Reset;
            //HandleShockwaveCreation.onNukeExploded += AddNuke;
            HandleWarheadDetonation.onWarheadDetonated += AddNuke;
            HandleTargetDetect.onTargetDetected += AddDetects;
            HandleJamming.OnFire += AddJammingAmmount;
        }

        public void FixedUpdate()
        {

        }
        public void Reset()
        {
            playerAircraftsInfo.Clear();
        }
        public void RegisterPlayerSortieStart(Unit unit, PersistentID id)
        {
            if (unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)unit;
                if (aircraft.Player != null)
                {
                    Sortie wn8 = new Sortie(aircraft, playerAircraftsInfo.Count() + 1);
                    playerAircraftsInfo.Add(id, wn8);
                    Plugin.logger.LogInfo($"Sortie started, player {aircraft.Player}, persistentId {id}");
                }
            }
        }



        public void RegisterPlayerSortieEnd(Unit unit)
        {
            //Plugin.logger.LogInfo($"Sortie ended, unit {unit}");
            if (unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)unit;

                PersistentID id = aircraft.persistentID;
                    Debug.Log($"aircraft {aircraft} ended sortie, id {id}");
                    if (playerAircraftsInfo.ContainsKey(id))
                    {
                        playerAircraftsInfo[id].RegisterSortieEnd(EndReasons.landed);
                        Debug.Log("id founded in dictionary");
                    }
                    else
                    {
                        Debug.Log($"ключ {id} не найден");

                    }
            }  
        }
        public void AddDetects(PersistentID detector)
        {
            if(playerAircraftsInfo.ContainsKey(detector))
            {
                playerAircraftsInfo[detector].AddDetectedTarget();
                Plugin.DebugLog("Player detected target", "PlayersSortieController");
            }
            
        }
        public void AddJammingAmmount(PersistentID unitJammer, Weapon weapon)
        {
            if (playerAircraftsInfo.ContainsKey(unitJammer) && weapon is JammingPod)
            {
                playerAircraftsInfo[unitJammer].AddJamming();
                //Plugin.DebugLog("Player is jamming", "PlayersSortieController");
            }
        }
        public void AddKill(PersistentID killerPersistentID, PersistentUnit persistentUnitKilled)
        {
            if (playerAircraftsInfo.ContainsKey(killerPersistentID))
            {
                playerAircraftsInfo[killerPersistentID].AddKill(persistentUnitKilled);
            }
            if (persistentUnitKilled.player != null && playerAircraftsInfo.ContainsKey(persistentUnitKilled.id))
            {
                playerAircraftsInfo[persistentUnitKilled.id].DetectKilled(killerPersistentID);
            }
        }
        public void AddNuke(PersistentID persistentID)
        {
            playerAircraftsInfo[persistentID].AddNuke();
        }
        public void ShowSorties()
        {
            Plugin.logger.LogInfo($"SORTIE COUNT {playerAircraftsInfo.Count()}");
            foreach(Sortie wn in playerAircraftsInfo.Values)
            {
                wn.RegisterSortieEnd(EndReasons.matchEnded);
                wn.ShowSortie();
            }
        }
    }
}
