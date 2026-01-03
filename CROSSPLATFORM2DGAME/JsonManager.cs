using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class JsonManager {
        private static readonly string fileName = "gameSaveData.json";
        private static string filePath;

        static JsonManager() {
            filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        }

        public static async Task SaveGameDataAsync(gameSaveData data) {
            try {
                string jsonData = JsonSerializer.Serialize(data);
                await File.WriteAllTextAsync(filePath, jsonData);
            } catch (Exception ex) {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error saving game data: {ex.Message}");
            }
        }

        public static async Task<gameSaveData> LoadGameDataAsync() {
            try {
                if (File.Exists(filePath)) {
                    string jsonData = await File.ReadAllTextAsync(filePath);
                    gameSaveData data = JsonSerializer.Deserialize<gameSaveData>(jsonData);
                    return data ?? new gameSaveData();
                } else {
                    return new gameSaveData();
                }
            } catch (Exception ex) {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error loading game data: {ex.Message}");
                return new gameSaveData();
            }
        }

        public static async Task SaveHighScoreAsync(int highScore) {
            gameSaveData data = await LoadGameDataAsync();
            data.HighestScore = highScore;
            await SaveGameDataAsync(data);
        }

        public static async Task SaveAudioSettingsAsync(double musicVolume, double sfxVolume) {
            gameSaveData data = await LoadGameDataAsync();
            data.MusicVolume = musicVolume;
            data.sfxVolume = sfxVolume;
            await SaveGameDataAsync(data);
        }
    }
}
