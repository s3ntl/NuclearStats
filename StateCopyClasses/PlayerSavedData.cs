using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuclearOption.Networking;

namespace NS.StateCopyClasses
{

    public static class PlayersSavedDataManager
    {
        public static Dictionary<FactionHQ, List<PlayerSavedData>> savedData = new Dictionary<FactionHQ, List<PlayerSavedData>>();

        public static void Awake()
        {
            Subscribe();
        }

        public static void Subscribe()
        {
            Handlers.HandlePlayerDisconnected.OnPlayerDisconnected += Save;
            HandleMapPrefabName.onMapLoaded += Clear;
        }
        public static List<PlayerSavedData> GetHQSavedPlayers(FactionHQ HQ)
        {
            
            List<PlayerSavedData> data = savedData.TryGetValue(HQ, out var savedPlayers) ? savedPlayers.OrderByDescending(p => p.Score).ToList() : new List<PlayerSavedData>();
            return data;
        }
        public static void SaveAllPlayers()
        {
            foreach(Faction faction in FactionRegistry.factions)
            {
                FactionHQ HQ = FactionRegistry.HQFromFaction(faction);
                foreach(Player player in HQ.GetPlayers(false))
                {
                    Save(player);
                }
            }
        }

        public static void Clear()
        {
            savedData.Clear();
        }
        public static void Save(Player player)
        {
            try
            {
                PlayerSavedData data = new PlayerSavedData(
                    player.SteamID,
                    player.PlayerName,
                    player.PlayerScore,
                    player.HQ
                );


                int index = savedData.ContainsKey(player.HQ)
                           ? savedData[player.HQ].FindIndex(x => x.SteamID == player.SteamID)
                           : -1;

                if (index != -1 && player.HQ != null)
                {
                    savedData[player.HQ][index] = data;
                }
                else
                {
                    if (!savedData.ContainsKey(player.HQ))
                    {
                        savedData.Add(player.HQ, new List<PlayerSavedData>());
                    }
                    savedData[player.HQ].Add(data);
                }

                Plugin.DebugLog($"Player data saved successfully." +
                    $"\nData: Name {data.Name}\nSteamID {data.SteamID}\nScore {data.Score}\nFactionHQ {data.FactionHQ.name}");
                // Plugin.logger.LogInfo($"Player data saved successfully." +
                //    $"\nData: Name {data.Name}\nSteamID {data.SteamID}\nScore {data.Score}\nFactionHQ {data.FactionHQ.name}");
            }
            catch (Exception ex)
            {
                Plugin.DebugLog($"Exception while saving player data: {ex.Message}");
            }

        }
    }
    public class PlayerSavedData
    {
        public ulong SteamID { get; private set; }
        public string Name { get; private set; }
        public float Score { get; private set; }
        public FactionHQ FactionHQ { get; private set; }
        public PlayerSavedData(ulong steamID, string name, float score, FactionHQ factionHQ)
        {
            try
            {
                SteamID = steamID;
                Name = name;
                Score = score;
                FactionHQ = factionHQ;
            }
            catch (Exception ex)
            {
                Plugin.DebugLog($"Exception in PlayerSavedData .ctor: {ex.Message}");
            }
        }
    }


}
