using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;

using NuclearOption.Networking;
using UnityEngine;

namespace NS.Sorties
{
    public class Sortie
    {
        public Aircraft aircraft {  get; private set; }
        public DateTime sortieStartTime { get; private set; }
        public DateTime sortieEndTime { get; private set; }
        public int sortieSerialNumber { get; private set; }

        public TimeSpan elapsed;

        private bool isKilled = false;

        private EndReasons sortieEndReason;

        private bool isSortieEnded = false;

        public List<UnitInfo> killedUnits = new List<UnitInfo>();

        public float explodedNukes { get; private set; }

        public PlayerAircraftInfo selfInfo;

        public SavedPlayerData savedPlayerData;

        public float jammingAmount = 0f;

        public int detectedTargets = 0;

        private GlobalPosition multipassStartingPoint;
        private bool isInMultipass = false;
        private float greatestDistanceInMultipass;

        public Sortie(Aircraft aircraft, int sortieSerialNumber) 
        {
            this.aircraft = aircraft;
            this.sortieSerialNumber = sortieSerialNumber;
            sortieStartTime = DateTime.UtcNow;
            SetSelfInfo();
        }


        public void AddJamming()
        {
            jammingAmount += Time.deltaTime;
        }

        public void AddDetectedTarget()
        {
            detectedTargets++;
        }

        public void AddNuke()
        {
            explodedNukes++;
        }

        public override string ToString()
        {
            return base.ToString();
        }


        public void RegisterSortieEnd(EndReasons sortieEndReason)
        {
            Plugin.DebugLog($"register sortie end called, end reason {sortieEndReason}, sortie already ended {isSortieEnded}", "WN8");
            if (!isSortieEnded) 
            {
                this.sortieEndReason = sortieEndReason;
                sortieEndTime = DateTime.UtcNow;
                isSortieEnded = true;
                elapsed = sortieEndTime - sortieStartTime;
                Plugin.logger.LogInfo(string.Format("sortie N{1} ended, aircraftInfo {0}", selfInfo.ToString(), sortieSerialNumber));
            }
 
        }

        public void AddKill(PersistentUnit unit)
        {
            if (aircraft.NetworkHQ == unit.unit.NetworkHQ) { return; }
            killedUnits.Add(CopyUnitInfo(unit.unit));
        }

        private void AddParachutedKill(PersistentUnit unit)
        {

        }

        public void FixedUpdate()
        {
            AnalyzeMultipass();
        }

        private void AnalyzeMultipass()
        {
            if(aircraft.transform.position.y <= 5f)
            {
                if (!isInMultipass)
                {
                    multipassStartingPoint = aircraft.transform.position.ToGlobalPosition();
                    isInMultipass = true;
                }
            }
            else
            {
                if (isInMultipass)
                {

                }
            }
        }

        public void DetectKilled(PersistentID killerPersistentID)
        {
            Plugin.logger.LogInfo($"detect killed, id {selfInfo.persistentID}, unit[{selfInfo.ToString()}]");
            PersistentUnit persistentUnit;
            if (UnitRegistry.TryGetPersistentUnit(killerPersistentID, out persistentUnit))
            {
                Unit unit = persistentUnit.unit;
                if (unit is Aircraft)
                {
                    Aircraft aircraft = (Aircraft)unit;
                    if (aircraft.Player != null)
                    {
                        RegisterSortieEnd(EndReasons.killedByPlayer);
                        return;
                    }
                }
                RegisterSortieEnd(EndReasons.killedByBot);
            }
            else
            {
                RegisterSortieEnd(EndReasons.crashed);
            }
            
        }

        

        public void ShowSortie()
        {
            if (isSortieEnded)
            {
                int divide = 1;
                foreach (var info in selfInfo.weaponsInfo)
                {
                    if (info.jammer) divide = info.count;
                }
                jammingAmount /= divide;
                //Plugin.logger.LogInfo(string.Format("Sortie N{3}: aircraftInfo [{0}], kills {1}, liveTime {2}, endReason {4}, nukesExploded {5}, jammingAmount {6}" +
                //    ", DetectedTargets {7}", selfInfo.ToString(),
                //    killedUnits.Count(), elapsed, sortieSerialNumber, sortieEndReason.ToString(), explodedNukes
                //    ,TimeSpan.FromSeconds(jammingAmount), detectedTargets));

                //LogWriter.WriteLog(string.Format("Sortie N{3}: aircraftInfo [{0}], kills {1}, liveTime {2}, endReason {4}, nukesExploded {5}", selfInfo.ToString(),
                //    killedUnits.Count(), elapsed, sortieSerialNumber, sortieEndReason.ToString(), explodedNukes));


                Plugin.logger.LogInfo($"Sortie N{sortieSerialNumber}: aircraftInfo[{selfInfo.ToString()}], kills {killedUnits.Count()}, " +
                    $"liveTime {elapsed}, endReason {sortieEndReason.ToString()}, nukesExploded {explodedNukes}, " +
                    $"jammingAmount {TimeSpan.FromSeconds(jammingAmount)}, DetectedTargets {detectedTargets}");

                LogWriter.WriteLog($"Sortie N{sortieSerialNumber}: aircraftInfo[{selfInfo.ToString()}], kills {killedUnits.Count()}, " +
                    $"liveTime {elapsed}, endReason {sortieEndReason.ToString()}, nukesExploded {explodedNukes}, " +
                    $"jammingAmount {TimeSpan.FromSeconds(jammingAmount)}, DetectedTargets {detectedTargets}");

                ShowKillInfo();
            }
        }

        private UnitInfo CopyUnitInfo(Unit unit)
        {
            if (unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)unit;
                PlayerAircraftInfo aircraftInfo = new PlayerAircraftInfo();
                if (aircraft.Player != null)
                {
                    aircraftInfo.CopyPlayerInfo(aircraft);
                }
                else
                {
                    aircraftInfo.CopyUnitInfo(unit);
                }
                return aircraftInfo;
            }
            else
            {
                UnitInfo unitInfo = new UnitInfo();
                unitInfo.CopyUnitInfo(unit);
                return unitInfo;
            }
        }

        private void SetSelfInfo()
        {
            selfInfo = (PlayerAircraftInfo)CopyUnitInfo(aircraft);    
        }
         
        private void ShowKillInfo()
        {
            foreach (UnitInfo unitInfo in killedUnits)
            {
                Plugin.logger.LogInfo(string.Format("Sortie N{0}: unit killed: [Victim: {1} | killerName: {2}  | KillerSteamID {3}]", sortieSerialNumber, unitInfo.ToString(), this.selfInfo.PlayerName, this.selfInfo.SteamID));
                LogWriter.WriteLog(string.Format("Sortie N{0}: unit killed: [Victim: {1} | KillerInfo ({2})]", sortieSerialNumber, unitInfo.ToString(), this.selfInfo));
            }
        }
    }
}
