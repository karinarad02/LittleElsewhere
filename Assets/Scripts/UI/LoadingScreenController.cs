using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LittleElsewhere.UI
{
    /// <summary>
    /// Handles asynchronous scene loading with progress bar updates, smooth fade transitions, and lore hints.
    /// Styled using the project's custom Menu UI components (PanelA, PanelB, ButtonNormal, ButtonPressed).
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
        private Texture2D panelATex, panelBTex, btnNormalTex, btnPressedTex;
        private GUIStyle progressStyle, barBgStyle, tipStyle, percentStyle, boxStyle;
        private bool guiStylesReady;

        private void Start()
        {
            panelATex = Resources.Load<Texture2D>("UI/PanelA");
            panelBTex = Resources.Load<Texture2D>("UI/PanelB");
            btnNormalTex = Resources.Load<Texture2D>("UI/ButtonNormal");
            btnPressedTex = Resources.Load<Texture2D>("UI/ButtonPressed");

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

            if (panelATex == null) panelATex = Resources.Load<Texture2D>("UI/PanelA");
            if (panelBTex == null) panelBTex = Resources.Load<Texture2D>("UI/PanelB");
            if (btnNormalTex == null) btnNormalTex = Resources.Load<Texture2D>("UI/ButtonNormal");
            if (btnPressedTex == null) btnPressedTex = Resources.Load<Texture2D>("UI/ButtonPressed");

            // Panel frame using PanelA texture
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(16, 16, 16, 16)
            };
            if (panelATex) boxStyle.normal.background = panelATex;

            // Bar background using PanelB frame
            barBgStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(8, 8, 8, 8)
            };
            if (panelBTex) barBgStyle.normal.background = panelBTex;

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

            // Progress bar fill using ButtonNormal component texture
            progressStyle = new GUIStyle()
            {
                border = new RectOffset(8, 8, 8, 8)
            };
            if (btnNormalTex) progressStyle.normal.background = btnNormalTex;
        }

        private void OnGUI()
        {
            if (progressBarFill != null && progressPercentText != null) return;

            SetupStyles();

            float scale = Mathf.Min(UnityEngine.Screen.width / 1280f, UnityEngine.Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(
                new Vector3((UnityEngine.Screen.width - 1280 * scale) / 2, (UnityEngine.Screen.height - 720 * scale) / 2, 0),
                Quaternion.identity,
                new Vector3(scale, scale, 1)
            );

            // Main Paper Container Panel (PanelA)
            GUI.Box(new Rect(320, 500, 640, 170), "", boxStyle);

            // Lore Tip Text
            GUI.Label(new Rect(340, 515, 600, 50), activeLoreTip, tipStyle);

            // Progress Bar Background Frame (PanelB)
            GUI.Box(new Rect(360, 575, 560, 32), "", barBgStyle);

            // Progress Bar Fill (ButtonNormal texture)
            float fillWidth = 552f * currentProgress;
            if (fillWidth > 4f)
            {
                GUI.Box(new Rect(364, 579, fillWidth, 24), "", progressStyle);
            }

            // Percentage Text
            GUI.Label(new Rect(360, 618, 560, 32), $"{Mathf.RoundToInt(currentProgress * 100)}%", percentStyle);
        }
    }
}
