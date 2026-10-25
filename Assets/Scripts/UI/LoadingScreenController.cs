using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LittleElsewhere.UI
{
    /// <summary>
    /// Handles asynchronous scene loading with progress bar updates, smooth fade transitions, and lore hints.
    /// Supports both uGUI and IMGUI rendering so it is fully functional out-of-the-box in any scene setup.
    /// </summary>
    public class LoadingScreenController : MonoBehaviour
    {
        public static string TargetSceneName = "MainGameScene";

        [Header("UI Elements (Optional uGUI)")]
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

        private float currentProgress = 0f;
        private string activeLoreTip = "";
        private GUIStyle progressStyle, tipStyle, percentStyle, boxStyle;
        private bool guiStylesReady;

        private void Start()
        {
            if (loreTips != null && loreTips.Length > 0)
            {
                int randomIndex = Random.Range(0, loreTips.Length);
                activeLoreTip = loreTips[randomIndex];
                if (tipMessageText != null) tipMessageText.text = activeLoreTip;
            }

            StartCoroutine(LoadTargetSceneAsync());
        }

        private IEnumerator LoadTargetSceneAsync()
        {
            float elapsedTime = 0f;
            string sceneToLoad = string.IsNullOrEmpty(TargetSceneName) ? "MainGameScene" : TargetSceneName;

            // Fallback if TargetSceneName is not in Build Settings
            if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
            {
                if (Application.CanStreamedLevelBeLoaded("LittleElsewhere")) sceneToLoad = "LittleElsewhere";
                else if (Application.CanStreamedLevelBeLoaded("MainGameScene")) sceneToLoad = "MainGameScene";
            }

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

                float asyncProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timedProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);
                currentProgress = Mathf.Min(asyncProgress, timedProgress);

                if (progressBarFill != null) progressBarFill.fillAmount = currentProgress;
                if (progressPercentText != null) progressPercentText.text = $"{Mathf.RoundToInt(currentProgress * 100)}%";

                if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
                {
                    currentProgress = 1f;
                    if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                    if (progressPercentText != null) progressPercentText.text = "100%";

                    yield return new WaitForSeconds(0.3f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        private void SetupStyles()
        {
            if (guiStylesReady) return;
            guiStylesReady = true;

            boxStyle = new GUIStyle(GUI.skin.box);
            
            percentStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            percentStyle.normal.textColor = new Color(0.12f, 0.24f, 0.33f);

            tipStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };
            tipStyle.normal.textColor = new Color(0.18f, 0.28f, 0.36f);

            progressStyle = new GUIStyle();
            Texture2D fillTex = new Texture2D(1, 1);
            fillTex.SetPixel(0, 0, new Color(0.25f, 0.55f, 0.70f, 0.9f));
            fillTex.Apply();
            progressStyle.normal.background = fillTex;
        }

        private void OnGUI()
        {
            // If uGUI is already connected, avoid drawing duplicate IMGUI
            if (progressBarFill != null && progressPercentText != null) return;

            SetupStyles();

            float scale = Mathf.Min(UnityEngine.Screen.width / 1280f, UnityEngine.Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(
                new Vector3((UnityEngine.Screen.width - 1280 * scale) / 2, (UnityEngine.Screen.height - 720 * scale) / 2, 0),
                Quaternion.identity,
                new Vector3(scale, scale, 1)
            );

            // Container Panel
            GUI.Box(new Rect(340, 520, 600, 150), "", boxStyle);

            // Lore Tip
            GUI.Label(new Rect(360, 535, 560, 50), activeLoreTip, tipStyle);

            // Progress Bar Background
            GUI.Box(new Rect(380, 595, 520, 24), "");

            // Progress Bar Fill
            float barWidth = 516f * currentProgress;
            if (barWidth > 0)
            {
                GUI.Box(new Rect(382, 597, barWidth, 20), "", progressStyle);
            }

            // Percentage Text
            GUI.Label(new Rect(380, 625, 520, 30), $"{Mathf.RoundToInt(currentProgress * 100)}%", percentStyle);
        }
    }
}
