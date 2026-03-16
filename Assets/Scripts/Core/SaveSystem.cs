using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FactoryRush.Scripts.Core
{
    /// <summary>
    /// Lưu/Load high scores top 5 bằng JSON file (persistentDataPath).
    /// Có hỗ trợ migrate dữ liệu cũ từ PlayerPrefs.
    /// </summary>
    public static class SaveSystem
    {
        private const int TopLimit = 5;
        private const string SaveFileName = "highscores.json";
        private const string LegacyPlayerPrefsKey = "HighScores";

        [Serializable]
        private class HighScoreData
        {
            public List<int> scores = new List<int>();
        }

        private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        /// <summary>
        /// 1) Load danh sách hiện tại
        /// 2) Thêm score mới
        /// 3) Sort giảm dần
        /// 4) Giữ top 5
        /// 5) Save lại file JSON
        /// </summary>
        public static void SubmitScore(int gold)
        {
            if (gold < 0) gold = 0;

            var scores = LoadHighScores().ToList();
            scores.Add(gold);

            var normalized = NormalizeScores(scores);
            WriteScores(normalized);
        }

        /// <summary>
        /// Load high scores từ JSON file.
        /// Nếu chưa có file thì thử migrate từ PlayerPrefs cũ.
        /// </summary>
        public static int[] LoadHighScores()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    if (string.IsNullOrWhiteSpace(json))
                        return Array.Empty<int>();

                    var data = JsonUtility.FromJson<HighScoreData>(json);
                    if (data == null || data.scores == null)
                        return Array.Empty<int>();

                    return NormalizeScores(data.scores).ToArray();
                }

                // Fallback migrate từ PlayerPrefs cũ (nếu có)
                if (PlayerPrefs.HasKey(LegacyPlayerPrefsKey))
                {
                    string legacyJson = PlayerPrefs.GetString(LegacyPlayerPrefsKey, string.Empty);
                    if (!string.IsNullOrWhiteSpace(legacyJson))
                    {
                        var legacyData = JsonUtility.FromJson<HighScoreData>(legacyJson);
                        if (legacyData != null && legacyData.scores != null)
                        {
                            var migrated = NormalizeScores(legacyData.scores).ToArray();
                            WriteScores(migrated);
                            PlayerPrefs.DeleteKey(LegacyPlayerPrefsKey);
                            PlayerPrefs.Save();
                            return migrated;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem.LoadHighScores failed: {ex.Message}");
            }

            return Array.Empty<int>();
        }

        /// <summary>
        /// Xóa toàn bộ dữ liệu high scores (file JSON + key PlayerPrefs cũ).
        /// </summary>
        public static void ClearAll()
        {
            try
            {
                if (File.Exists(SavePath))
                    File.Delete(SavePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem.ClearAll file delete failed: {ex.Message}");
            }

            PlayerPrefs.DeleteKey(LegacyPlayerPrefsKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Trả về đường dẫn file save để debug.
        /// </summary>
        public static string GetSavePath()
        {
            return SavePath;
        }

        private static List<int> NormalizeScores(IEnumerable<int> scores)
        {
            return scores
                .Where(s => s >= 0)
                .OrderByDescending(s => s)
                .Take(TopLimit)
                .ToList();
        }

        private static void WriteScores(IEnumerable<int> scores)
        {
            try
            {
                var data = new HighScoreData { scores = scores.ToList() };
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem.WriteScores failed: {ex.Message}");
            }
        }
    }
}

