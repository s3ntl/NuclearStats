using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Unix;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using UnityEngine;
using BepInEx.Logging;
using NuclearOption.Networking;
using NS.StateCopyClasses;
namespace NS
{
    public class GameResultReader
    {

        public void Awake()
        {
            HandleGameEnd.onGameEnded += EndReader;
        }

        public void EndReader(FactionHQ factionHQ, EndType endType)
        {
            try
            {
                SortiesReader();
                PlayersSavedDataManager.SaveAllPlayers();
                Plugin.playersSortieController.Reset();
                string factionName = factionHQ.faction.factionName;
                string type = endType.ToString();
                float time = Time.timeSinceLevelLoad;
                double minutes = Math.Truncate((time / 60));
                double seconds = Math.Truncate((time % 60));

                Plugin.logger.LogInfo(string.Format("Match time: {0} minutes, {1} seconds", minutes, seconds));
                LogWriter.WriteLog(string.Format("Match time: {0} minutes, {1} seconds", minutes, seconds));


                Plugin.logger.LogInfo(string.Format("factionName: {0} || endType: {1}", factionName, endType));
                LogWriter.WriteLog(string.Format("factionName: {0} || endType: {1}", factionName, endType));

                FactionHQ bdf = null;
                FactionHQ pala = null;
                foreach (Faction faction in FactionRegistry.factionLookup.Values)
                {

                    FactionHQ HQ = FactionRegistry.HQFromFaction(faction);
                    if (HQ != null)
                    {
                        string name = HQ.faction.factionName;
                        if (name == "Primeva")
                        {
                            pala = HQ;
                        }
                        else if (name == "Boscali")
                        {
                            bdf = HQ;
                        }
                        Plugin.logger.LogInfo(string.Format("Faction {0}, summary score: {1}", name, HQ.factionScore));
                        LogWriter.WriteLog(string.Format("Faction {0}, summary score: {1}", name, HQ.factionScore));

                        List<Player> players = HQ.GetPlayers(true);
                        List<PlayerSavedData> save = PlayersSavedDataManager.GetHQSavedPlayers(HQ);
                        foreach (PlayerSavedData player in save)
                        {
                            Plugin.logger.LogInfo(string.Format("PlayerName: {0} [steamID: {2} " +
                                "| fraction {3}], score {1}",
                                player.Name, player.Score, player.SteamID,
                                player.FactionHQ.faction.factionName));

                            LogWriter.WriteLog(string.Format("PlayerName: {0} [steamID: {2} " +
                                "| fraction {3}], score {1}",
                                player.Name, player.Score, player.SteamID,
                                player.FactionHQ.faction.factionName));
                        }
                    }
                }

                if (endType == EndType.Victory && factionHQ.faction.factionName == "Primeva")
                {
                    Plugin.logger.LogInfo(string.Format("Faction {0} won\nfaction {1} lose", pala, bdf));
                    LogWriter.WriteLog(string.Format("Faction {0} won\nfaction {1} lose", pala, bdf));
                }
                else if (endType == EndType.Victory && factionHQ.faction.factionName == "Boscali")
                {
                    Plugin.logger.LogInfo(string.Format("Faction {0} won\nfaction {1} lose", bdf, pala));
                    LogWriter.WriteLog(string.Format("Faction {0} won\nfaction {1} lose", bdf, pala));
                }
                else if (endType == EndType.Defeat && factionHQ.faction.factionName == "Boscali")
                {
                    Plugin.logger.LogInfo(string.Format("Faction {0} won\nfaction {1} lose", pala, bdf));
                    LogWriter.WriteLog(string.Format("Faction {0} won\nfaction {1} lose", pala, bdf));
                }
                else if (endType == EndType.Defeat && factionHQ.faction.factionName == "Primeva")
                {
                    Plugin.logger.LogInfo(string.Format("Faction {0} won\nfaction {1} lose", bdf, pala));
                    LogWriter.WriteLog(string.Format("Faction {0} won\nfaction {1} lose", bdf, pala));
                }

                Plugin.logger.LogInfo("Mission ended.");
                LogWriter.WriteLog("Mission ended.");
            }
            catch (Exception ex)
            {
                Plugin.DebugLog($"Exception in EndReader: {ex.Message}");
            }
        }

        public void SortiesReader()
        {
            Plugin.logger.LogDebug("Reading sorties...");
            Plugin.playersSortieController.ShowSorties();
        }
       
    }
}
