using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LittleElsewhere.UI
{
    /// <summary>
    /// Controls Main Menu interaction, settings modal toggle, and asynchronous transition to Loading/Game Scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Transition")]
        [SerializeField] private string loadingSceneName = "LoadingScene";
        [SerializeField] private string mainGameSceneName = "MainGameScene";

        [Header("UI Panels")]
        [SerializeField] private GameObject mainButtonsPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject saveSlotsPanel;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource buttonClickAudio;

        private void Start()
        {
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        }

        public void OnStartGameClicked()
        {
            PlayButtonClickSound();
            
            // Set static or PlayerPrefs target scene if LoadingScene is used
            LoadingScreenController.TargetSceneName = mainGameSceneName;

            if (Application.CanStreamedLevelBeLoaded(loadingSceneName))
            {
                SceneManager.LoadScene(loadingSceneName);
            }
            else
            {
                // Fallback direct load if LoadingScene isn't built separately
                SceneManager.LoadScene(mainGameSceneName);
            }
        }

        public void OnOpenSaveSlotsClicked()
        {
            PlayButtonClickSound();
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
            if (saveSlotsPanel != null) saveSlotsPanel.SetActive(true);
        }

        public void OnCloseSaveSlotsClicked()
        {
            PlayButtonClickSound();
            if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        }

        public void OnOpenSettingsClicked()
        {
            PlayButtonClickSound();
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void OnCloseSettingsClicked()
        {
            PlayButtonClickSound();
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        }

        public void OnQuitClicked()
        {
            PlayButtonClickSound();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void PlayButtonClickSound()
        {
            if (buttonClickAudio != null)
            {
                buttonClickAudio.Play();
            }
        }
    }
}
