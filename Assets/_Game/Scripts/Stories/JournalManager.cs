using System.Collections.Generic;
using UnityEngine;

namespace LittleElsewhere.Stories
{
    /// <summary>
    /// Runtime journal manager. Records discovered memories and journal entries.
    /// Provides queries for unlocked memories without exposing explicit quest scores.
    /// </summary>
    public class JournalManager : MonoBehaviour
    {
        private HashSet<string> unlockedEntryIDs = new HashSet<string>();
        private List<JournalEntrySO> journalEntries = new List<JournalEntrySO>();

        public event System.Action<JournalEntrySO> OnJournalEntryAdded;

        public bool IsEntryUnlocked(string entryID)
        {
            return unlockedEntryIDs.Contains(entryID);
        }

        public bool AddJournalEntry(JournalEntrySO entry)
        {
            if (entry == null || unlockedEntryIDs.Contains(entry.entryID))
            {
                return false;
            }

            unlockedEntryIDs.Add(entry.entryID);
            journalEntries.Add(entry);
            Debug.Log($"[JournalManager] Recorded new memory: '{entry.title}'");
            OnJournalEntryAdded?.Invoke(entry);
            return true;
        }

        public List<JournalEntrySO> GetAllEntries()
        {
            return new List<JournalEntrySO>(journalEntries);
        }
    }
}
