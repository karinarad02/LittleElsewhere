using UnityEngine;

namespace LittleElsewhere.Stories
{
    public enum ClueType
    {
        Footprints,
        WrittenLetter,
        EnvironmentalAnomaly,
        Dialogue,
        WildlifeObservation
    }

    /// <summary>
    /// Data definition for a single clue in a mystery chain.
    /// </summary>
    [CreateAssetMenu(fileName = "NewClue", menuName = "LittleElsewhere/Story/Clue")]
    public class ClueSO : ScriptableObject
    {
        [Header("Clue Identity")]
        public string clueID;
        public string clueName;
        public ClueType clueType;

        [Header("Narrative Description")]
        [TextArea(3, 6)]
        public string description;

        [Header("Associated Journal Entry")]
        public JournalEntrySO unlockedJournalEntry;
    }
}
