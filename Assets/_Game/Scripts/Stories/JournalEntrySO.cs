using UnityEngine;

namespace LittleElsewhere.Stories
{
    public enum JournalCategory
    {
        Wildlife,
        Neighbours,
        Supernatural,
        PersonalObservations
    }

    /// <summary>
    /// Data definition for a memory recorded in the player's personal journal.
    /// </summary>
    [CreateAssetMenu(fileName = "NewJournalEntry", menuName = "LittleElsewhere/Story/Journal Entry")]
    public class JournalEntrySO : ScriptableObject
    {
        [Header("Journal Entry Metadata")]
        public string entryID;
        public string title;
        public JournalCategory category;

        [Header("Content")]
        [TextArea(4, 8)]
        public string memoryText;

        [Header("Visual Asset")]
        public Sprite sketchIllustration;
    }
}
