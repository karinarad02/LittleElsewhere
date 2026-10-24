using System.IO;
using UnityEngine;
using LittleElsewhere.World;

namespace LittleElsewhere.Saving
{
    [System.Serializable]
    public class SaveData
    {
        public int seed;
        public float playerCanonicalX;
        public float playerCanonicalY;
        public float timeOfDay;
        public string saveTimestamp;
    }

    /// <summary>
    /// Reliable save/load manager. Persists seed, canonical player coordinates, and time of day to JSON.
    /// </summary>
    public class WorldSaveSystem : MonoBehaviour
    {
        [SerializeField] private WorldSeedData worldConfig;
        [SerializeField] private Transform playerTransform;

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, "world_save.json");

        public bool SaveGame(int seed, Vector2 canonicalPlayerPos, float timeOfDay)
        {
            try
            {
                SaveData data = new SaveData
                {
                    seed = seed,
                    playerCanonicalX = canonicalPlayerPos.x,
                    playerCanonicalY = canonicalPlayerPos.y,
                    timeOfDay = timeOfDay,
                    saveTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[WorldSaveSystem] Game saved successfully to {SaveFilePath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WorldSaveSystem] Save failed: {ex.Message}");
                return false;
            }
        }

        public SaveData LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning("[WorldSaveSystem] No save file found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[WorldSaveSystem] Game loaded successfully. Seed={data.seed}, Pos=({data.playerCanonicalX}, {data.playerCanonicalY})");
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WorldSaveSystem] Load failed: {ex.Message}");
                return null;
            }
        }
    }
}
