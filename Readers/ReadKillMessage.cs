using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NS.Handlers;
using UnityEngine;

namespace NS
{
    public class ReadKillMessage
    {
        public static Dictionary<PersistentID, Dictionary<PersistentID, DateTime>> playerNukeInfluencedObjects = new Dictionary<PersistentID, Dictionary<PersistentID, DateTime>>();
        public void Awake()
        {
            Subscribe();
        }
        public void Update()
        {
            foreach (PersistentID key in playerNukeInfluencedObjects.Keys)
            {
                foreach (PersistentID key2 in playerNukeInfluencedObjects[key].Keys)
                {
                    TimeSpan elapsed =  DateTime.UtcNow - playerNukeInfluencedObjects[key][key2];

                    UnitRegistry.TryGetPersistentUnit(key2, out PersistentUnit influencedPersistentUnit);

                    if (influencedPersistentUnit == null)
                    {
                        continue;
                    }
                    
                    Unit influencedUnit = influencedPersistentUnit.unit;
                   
                    Dictionary<PersistentID, float> damageCredit = GetDamageCredit(influencedUnit);
                    
                    if (elapsed.TotalSeconds > 30)
                    {
                        
                        playerNukeInfluencedObjects[key].Remove(key2);
                        
                        Debug.Log(string.Format("Unit with id {0} removed from influencedObjects, damage count {1}", key2, damageCredit[key]));
                    }

                }
            }
        }
        private void Subscribe()
        {
            //HandleShockwaveCreation.shockWaveCanInfluenceObject += AddInfluencedObjects;
            HandleKillMessage.HandleKill += KillLog;
            HandleGameEnd.onGameEnded += ResetInfluencedObjects;
            HandleWarheadDetonation.shockwaveCanInfluenceObject += AddInfluencedObjects;
        }

        private void ResetInfluencedObjects(FactionHQ a, NuclearOption.SavedMission.ObjectiveV2.Outcomes.EndType b)
        {
            playerNukeInfluencedObjects.Clear();
        }
        private void AddInfluencedObjects(PersistentID ownerPersistentID, PersistentID persistentUnitID)
        {
            DateTime objectAddedTime = DateTime.UtcNow;
            if (playerNukeInfluencedObjects.ContainsKey(ownerPersistentID))
            {
                if (!playerNukeInfluencedObjects[ownerPersistentID].ContainsKey(persistentUnitID))
                {
                    playerNukeInfluencedObjects[ownerPersistentID].Add(persistentUnitID, objectAddedTime); 
                    Debug.Log(string.Format("ownerID: {0}, persistentUnitID: {1}", ownerPersistentID, persistentUnitID));

                    Debug.Log(string.Format("ownerID: {0}, unitsInfluenced: {1}", ownerPersistentID, playerNukeInfluencedObjects[ownerPersistentID].Count));
                }
            }
            else
            {
                playerNukeInfluencedObjects.Add(ownerPersistentID, new Dictionary<PersistentID, DateTime>());
                playerNukeInfluencedObjects[ownerPersistentID].Add(persistentUnitID, objectAddedTime);
            }
        }
        public static void KillLog(PersistentID killerID, PersistentID killedID)
        {
            string killType;
            
            PersistentUnit persistentUnitKiller;
            PersistentUnit persistentUnitKilled;
            bool killedExist = UnitRegistry.TryGetPersistentUnit(killedID, out persistentUnitKilled);
            bool killerExist = UnitRegistry.TryGetPersistentUnit(killerID, out persistentUnitKiller);
            
            
            if (killedExist && killerExist)
            {
                if (persistentUnitKiller.player != null)
                {
                    PersistentID persistentPlayerID = persistentUnitKiller.unit.persistentID;
                    if (playerNukeInfluencedObjects.ContainsKey(persistentPlayerID) && playerNukeInfluencedObjects[persistentPlayerID].ContainsKey(killedID))
                    {
                        killType = "[NukeKill]";
                        playerNukeInfluencedObjects[persistentPlayerID].Remove(killedID);
                        Debug.Log(string.Format("unit with id {0} was killed and removed from influenced objects", killedID));
                    }
                    else
                    {
                        if (playerNukeInfluencedObjects.ContainsKey(persistentPlayerID))
                        {
                           playerNukeInfluencedObjects[persistentPlayerID].Remove(killedID);
                            Debug.Log(string.Format("unit with id {0} was killed and removed from influenced objects", killedID));
                        }
                        killType = "[CommonKill]";
                    }


                    if (persistentUnitKilled.player != null && persistentUnitKilled.GetFaction() != persistentUnitKiller.GetFaction())
                    {
                        Plugin.playersSortieController.AddKill(killerID, persistentUnitKilled);
                        Plugin.logger.LogInfo(string.Format("{6} Player {0} [steamID: {2} | faction: {4} ] kills player {1} [steamID: {3} | faction {5}]",
                            persistentUnitKiller.player.PlayerName, persistentUnitKilled.player.PlayerName,
                            persistentUnitKiller.player.SteamID, persistentUnitKilled.player.SteamID,
                            persistentUnitKiller.player.HQ.faction.factionName,
                            persistentUnitKilled.player.HQ.faction.factionName, killType));

                       // LogWriter.WriteLog(string.Format("{6} Player {0} [steamID: {2} | faction: {4} ] kills player {1} [steamID: {3} | faction {5}]",
                       //     persistentUnitKiller.player.PlayerName, persistentUnitKilled.player.PlayerName,
                       //     persistentUnitKiller.player.SteamID, persistentUnitKilled.player.SteamID,
                       //     persistentUnitKiller.player.HQ.faction.factionName,
                        //    persistentUnitKilled.player.HQ.faction.factionName, killType));
                    }
                    else if (persistentUnitKilled.player == null && persistentUnitKilled.GetFaction() != persistentUnitKiller.GetFaction())
                    {
                        Plugin.playersSortieController.AddKill(killerID, persistentUnitKilled);
                        Plugin.logger.LogInfo(string.Format("{5} Player {0} [steamID: {3} | faction: {4}] kills bot {1} || Bot type: {2}",
                        persistentUnitKiller.player.PlayerName, persistentUnitKilled.unitName,
                        ReadUnitType(persistentUnitKilled), persistentUnitKiller.player.SteamID,
                        persistentUnitKiller.player.HQ.faction.factionName, killType));

                       // LogWriter.WriteLog(string.Format("{5} Player {0} [steamID: {3} | faction: {4}] kills bot {1} || Bot type: {2}",
                       // persistentUnitKiller.player.PlayerName, persistentUnitKilled.unitName,
                      //  ReadUnitType(persistentUnitKilled), persistentUnitKiller.player.SteamID,
                       // persistentUnitKiller.player.HQ.faction.factionName, killType));
                    }
                }
                else if (persistentUnitKiller.player == null && persistentUnitKilled.player != null)
                {
                    Plugin.playersSortieController.AddKill(killerID, persistentUnitKilled);
                    Plugin.logger.LogInfo(string.Format("Player {0} [steamID: {1} | faction: {2}] shot down by bot", persistentUnitKilled.player.PlayerName,
                        persistentUnitKilled.player.SteamID, persistentUnitKilled.player.HQ.faction.factionName));

                    //LogWriter.WriteLog(string.Format("Player {0} [steamID: {1} | faction: {2}] shot down by bot", persistentUnitKilled.player.PlayerName,
                    //    persistentUnitKilled.player.SteamID, persistentUnitKilled.player.HQ.faction.factionName));
                }
            }
            else if (!killerExist && killedExist)
            {
                if (persistentUnitKilled.player != null)
                {
                    Plugin.playersSortieController.AddKill(killerID, persistentUnitKilled);
                    Plugin.logger.LogInfo(string.Format("Player {0} [steamID: {1} | faction: {2} ] crashed", persistentUnitKilled.player.PlayerName,
                        persistentUnitKilled.player.SteamID, persistentUnitKilled.player.HQ.faction.factionName));

                   // LogWriter.WriteLog(string.Format("Player {0} [steamID: {1} | faction: {2} ] crashed", persistentUnitKilled.player.PlayerName,
                   //     persistentUnitKilled.player.SteamID, persistentUnitKilled.player.HQ.faction.factionName));
                }
            }
            
        }
        public Dictionary<PersistentID, float> GetDamageCredit(object instance)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField("damageCredit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);           
            var damageCredit = (Dictionary<PersistentID, float>)fieldInfo.GetValue(instance);
 
            return damageCredit; 
        }
        public static string ReadUnitType(PersistentUnit persistentUnit)
        {
            Unit unit = persistentUnit.unit;
            if (unit is Aircraft)
            {
                return "Aircraft";
            }
            else if (unit is GroundVehicle)
            {
                return "Vehicle";
            }
            else if (unit is Ship)
            {
                return "Ship";
            }
            else if (unit is Building)
            {
                return "Building";
            }
            else
            {
                return "unknown type";
            }
        }
    }
}
