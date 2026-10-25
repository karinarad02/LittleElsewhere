using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LittleElsewhere.UI
{
    /// <summary>
    /// Handles asynchronous scene loading with progress bar updates, smooth fade transitions, and lore hints.
    /// </summary>
    public class LoadingScreenController : MonoBehaviour
    {
        public static string TargetSceneName = "MainGameScene";

        [Header("UI Elements")]
        [SerializeField] private Image progressBarFill;
        [SerializeField] private Text progressPercentText;
        [SerializeField] private Text tipMessageText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Loading Lore Cues")]
        [SerializeField] private string[] loreTips = new string[]
        {
            "In winter, soft blue shadows cast deep across the snow.",
            "Footprints fade gently over time. Memory guides your return.",
            "Listen carefully to the wind to stay oriented in the dense forest.",
            "Resting at campsites advances time and restores quiet focus.",
            "Neighbours share stories with those who take time to listen."
        };

        [Header("Timing")]
        [SerializeField] private float minimumLoadingTime = 1.5f;

        private void Start()
        {
            if (loreTips != null && loreTips.Length > 0 && tipMessageText != null)
            {
                int randomIndex = Random.Range(0, loreTips.Length);
                tipMessageText.text = loreTips[randomIndex];
            }

            StartCoroutine(LoadTargetSceneAsync());
        }

        private IEnumerator LoadTargetSceneAsync()
        {
            float elapsedTime = 0f;
            string sceneToLoad = string.IsNullOrEmpty(TargetSceneName) ? "MainGameScene" : TargetSceneName;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
            if (operation == null)
            {
                Debug.LogError($"LoadingScreenController: Failed to load scene '{sceneToLoad}'.");
                yield break;
            }

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                elapsedTime += Time.deltaTime;

                // Unity async progress stops at 0.9 until allowSceneActivation = true
                float asyncProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timedProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);
                float displayProgress = Mathf.Min(asyncProgress, timedProgress);

                if (progressBarFill != null)
                {
                    progressBarFill.fillAmount = displayProgress;
                }

                if (progressPercentText != null)
                {
                    progressPercentText.text = $"{Mathf.RoundToInt(displayProgress * 100)}%";
                }

                // Ready when both async load is complete (0.9) and minimum duration has elapsed
                if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
                {
                    if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                    if (progressPercentText != null) progressPercentText.text = "100%";

                    yield return new WaitForSeconds(0.3f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
