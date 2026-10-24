using System.Collections.Generic;
using UnityEngine;
using LittleElsewhere.Stories;

namespace LittleElsewhere.UI
{
    /// <summary>
    /// Minimal journal UI view. Renders collected personal memories without quest objective arrows or checklists.
    /// </summary>
    public class JournalUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private JournalManager journalManager;
        [SerializeField] private GameObject journalWindow;

        private bool isOpen = false;

        private void Start()
        {
            if (journalManager == null) journalManager = FindFirstObjectByType<JournalManager>();
            if (journalWindow != null) journalWindow.SetActive(false);
        }

        private void Update()
        {
            // Toggle journal with 'J' or 'Tab' key
            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleJournal();
            }
        }

        public void ToggleJournal()
        {
            isOpen = !isOpen;
            if (journalWindow != null)
            {
                journalWindow.SetActive(isOpen);
            }

            if (isOpen && journalManager != null)
            {
                List<JournalEntrySO> entries = journalManager.GetAllEntries();
                Debug.Log($"[JournalUIController] Displaying {entries.Count} recorded memories.");
            }
        }
    }
}
