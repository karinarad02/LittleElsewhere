using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace LittleElsewhere
{
    [Serializable]
    public sealed class SaveData
    {
        public int version = 2;
        public float x, y, elapsed;
        public double worldHours = 9;
        public int day = 1;
        public string location = "The Cabin";
        public List<string> collected = new List<string>();
        public List<string> discovered = new List<string>();
        public List<string> wildlife = new List<string>();
        public List<string> notes = new List<string>();
        public List<string> lanterns = new List<string>();
        public int logs, berries, fish;
        public bool craneMade, craneReleased, finished;
        public string savedAt;
        public bool Ready(GameConfig config)
        {
            return logs >= config.firewoodGoal && berries >= config.berryGoal &&
                wildlife.Count >= config.wildlifeGoal && notes.Count >= 3 && lanterns.Count >= 3 && fish >= config.fishGoal;
        }
    }
    [Serializable]
    public sealed class Preferences
    {
        public float music = .65f, sound = .8f, brightness = 1f;
        public bool footprints = true, snow = true, fullscreen = false;
    }
    public static class SaveSystem
    {
        static int currentSlot = 1;
        public static int CurrentSlot { get { return currentSlot; } }
        public static void SelectSlot(int slot) { currentSlot = Mathf.Clamp(slot, 1, 3); }
        public static string SlotPath(int slot) { return Path.Combine(Application.persistentDataPath, "little-elsewhere-slot-" + Mathf.Clamp(slot, 1, 3) + ".json"); }
        public static string FilePath { get { return SlotPath(currentSlot); } }
        public static string LastError { get; private set; }
        public static bool Exists { get { for(int i=1;i<=3;i++)if(ExistsSlot(i))return true;return false; } }
        public static bool ExistsSlot(int slot) { return File.Exists(SlotPath(slot)) || File.Exists(SlotPath(slot) + ".bak") || (slot==1 && File.Exists(LegacyPath)); }
        static string LegacyPath { get { return Path.Combine(Application.persistentDataPath, "little-elsewhere-save.json"); } }
        public static bool Write(SaveData data)
        {
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                data.version=2;data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var temp = FilePath + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(data, true));
                if (File.Exists(FilePath)) File.Copy(FilePath, FilePath + ".bak", true);
                File.Copy(temp, FilePath, true);File.Delete(temp);
                LastError = null;return true;
            }
            catch (Exception ex) { LastError = ex.Message; Debug.LogWarning("Save failed: " + ex.Message); return false; }
        }
        static SaveData TryRead(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                if (data == null || (data.version != 1 && data.version != 2) || data.collected == null || data.discovered == null || data.wildlife == null || data.notes == null || data.lanterns == null || float.IsNaN(data.x) || float.IsInfinity(data.x) || float.IsNaN(data.y) || float.IsInfinity(data.y) || double.IsNaN(data.worldHours) || double.IsInfinity(data.worldHours)) return null;
                if(data.version==1){data.worldHours=9;data.day=1;data.version=2;}
                data.worldHours=Math.Max(0,data.worldHours);data.day=Math.Max(1,data.day);
                return data;
            }
            catch (Exception) { return null; }
        }
        public static SaveData PreviewSlot(int slot)
        {
            var data=TryRead(SlotPath(slot)) ?? TryRead(SlotPath(slot)+".bak");
            if(data==null && slot==1 && !File.Exists(SlotPath(1)) && !File.Exists(SlotPath(1)+".bak"))data=TryRead(LegacyPath);
            return data;
        }
        public static SaveData Read()
        {
            var data = PreviewSlot(currentSlot);
            LastError = data == null ? "This slot has no readable save. Choose another slot or begin a new walk." : null;
            return data;
        }
        public static Preferences ReadPreferences()
        {
            try { return JsonUtility.FromJson<Preferences>(PlayerPrefs.GetString("LE.Preferences", "")) ?? new Preferences(); }
            catch (Exception) { return new Preferences(); }
        }
        public static void WritePreferences(Preferences preferences)
        { PlayerPrefs.SetString("LE.Preferences", JsonUtility.ToJson(preferences)); PlayerPrefs.Save(); }
    }
}
