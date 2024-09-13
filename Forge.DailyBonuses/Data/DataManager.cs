using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Forge.DailyBonuses
{
    public class DataManager
    {
        private string DataFilePath => Path.Combine(Directory, "DailyBonusData.json");
        private string Directory => Plugin.Instance.Directory;

        private List<Data> playerData = new List<Data>();

        public void LoadData()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    string jsonData = File.ReadAllText(DataFilePath);
                    playerData = JsonConvert.DeserializeObject<List<Data>>(jsonData);
                    Rocket.Core.Logging.Logger.Log("DailyBonus data loaded successfully!");
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log("DailyBonus data file not found. Creating a new one.");
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"Error loading DailyBonus data: {ex.Message}");
            }
        }

        public void SaveData()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);
                File.WriteAllText(DataFilePath, jsonData);
                Rocket.Core.Logging.Logger.Log("DailyBonus data saved successfully!");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"Error saving DailyBonus data: {ex.Message}");
            }
        }

        public Data GetPlayerData(ulong steamID64)
        {
            return playerData.FirstOrDefault(data => data.SteamID64 == steamID64);
        }

        public void CreatePlayerData(ulong steamID64)
        {
            if (GetPlayerData(steamID64) == null)
            {
                playerData.Add(new Data { SteamID64 = steamID64 });
                SaveData();
                Rocket.Core.Logging.Logger.Log($"Created new DailyBonus data for player {steamID64}");
            }
        }

        public void UpdatePlayerData(Data newData)
        {
            Data existingData = GetPlayerData(newData.SteamID64);
            if (existingData != null)
            {
                playerData.Remove(existingData);
                playerData.Add(newData);
                SaveData();
                Rocket.Core.Logging.Logger.Log($"Updated DailyBonus data for player {newData.SteamID64}");
            }
        }

        public void DeletePlayerData(ulong steamID64)
        {
            Data dataToRemove = GetPlayerData(steamID64);
            if (dataToRemove != null)
            {
                playerData.Remove(dataToRemove);
                SaveData();
                Rocket.Core.Logging.Logger.Log($"Deleted DailyBonus data for player {steamID64}");
            }
        }
    }
}