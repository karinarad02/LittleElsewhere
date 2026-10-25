using UnityEngine;
using UnityEngine.SceneManagement;
using LittleElsewhere.UI;

public class LEGame : MonoBehaviour {
    public static bool Paused;
    public static string Message = "";
    
    public enum GameScreen { Main, Playing, Pause, Settings, Save, Dialogue }
    public static GameScreen Current = GameScreen.Main;
    
    static float music = .7f, sound = .8f;
    Texture2D bg, btnNormalTex, btnHoverTex, btnPressedTex, panelATex, panelBTex;
    GUIStyle title, button, body, boxStyle;
    bool ready;

    void Start() {
        bg = Resources.Load<Texture2D>("UI/MainMenu");
        btnNormalTex = Resources.Load<Texture2D>("UI/ButtonNormal");
        btnHoverTex = Resources.Load<Texture2D>("UI/ButtonHover");
        btnPressedTex = Resources.Load<Texture2D>("UI/ButtonPressed");
        panelATex = Resources.Load<Texture2D>("UI/PanelA");
        panelBTex = Resources.Load<Texture2D>("UI/PanelB");

        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene != "MainMenuScene" && activeScene != "LoadingScene") {
            Current = GameScreen.Playing;
            Paused = false;
        } else if (activeScene == "MainMenuScene") {
            Current = GameScreen.Main;
            Paused = true;
        }
    }

    void Setup() {
        if (ready) return;
        ready = true;

        if (btnNormalTex == null) btnNormalTex = Resources.Load<Texture2D>("UI/ButtonNormal");
        if (btnHoverTex == null) btnHoverTex = Resources.Load<Texture2D>("UI/ButtonHover");
        if (btnPressedTex == null) btnPressedTex = Resources.Load<Texture2D>("UI/ButtonPressed");
        if (panelATex == null) panelATex = Resources.Load<Texture2D>("UI/PanelA");
        if (panelBTex == null) panelBTex = Resources.Load<Texture2D>("UI/PanelB");

        title = new GUIStyle(GUI.skin.label) { fontSize = 38, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        title.normal.textColor = new Color(.12f, .24f, .33f);
        
        body = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        body.normal.textColor = title.normal.textColor;

        // Custom styled button using UI components
        button = new GUIStyle(GUI.skin.button) {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            border = new RectOffset(12, 12, 12, 12)
        };
        if (btnNormalTex) button.normal.background = btnNormalTex;
        if (btnHoverTex) button.hover.background = btnHoverTex;
        if (btnPressedTex) button.active.background = btnPressedTex;
        button.normal.textColor = new Color(.12f, .24f, .33f);
        button.hover.textColor = new Color(.08f, .16f, .22f);
        button.active.textColor = new Color(.05f, .10f, .15f);

        // Custom panel box using paper panel UI component
        boxStyle = new GUIStyle(GUI.skin.box) {
            border = new RectOffset(16, 16, 16, 16)
        };
        if (panelATex) boxStyle.normal.background = panelATex;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Current == GameScreen.Playing) {
                Current = GameScreen.Pause;
                Paused = true;
            } else if (Current == GameScreen.Pause || Current == GameScreen.Settings) {
                Current = GameScreen.Playing;
                Paused = false;
            }
        }
    }

    void OnGUI() {
        Setup();
        float scale = Mathf.Min(UnityEngine.Screen.width / 1280f, UnityEngine.Screen.height / 720f);
        GUI.matrix = Matrix4x4.TRS(
            new Vector3((UnityEngine.Screen.width - 1280 * scale) / 2, (UnityEngine.Screen.height - 720 * scale) / 2, 0),
            Quaternion.identity,
            new Vector3(scale, scale, 1)
        );

        if (Current == GameScreen.Playing) {
            if (!string.IsNullOrEmpty(Message)) {
                GUI.Box(new Rect(380, 620, 520, 55), Message, boxStyle);
                if (GUI.Button(new Rect(900, 625, 80, 40), "Close", button)) Message = "";
            }
            return;
        }

        if (Current == GameScreen.Main) {
            if (bg) GUI.DrawTexture(new Rect(0, 0, 1280, 720), bg, ScaleMode.ScaleAndCrop);
            GUI.Box(new Rect(435, 95, 410, 510), "", boxStyle);
            GUI.Label(new Rect(440, 135, 400, 70), "Little Elsewhere", title);
            
            if (Btn("New Game", 235)) {
                StartNewGame();
            }
            if (Btn("Continue", 305)) {
                ContinueGame();
            }
            if (Btn("Settings", 375)) {
                Current = GameScreen.Settings;
            }
            if (Btn("Quit", 445)) {
                Application.Quit();
            }
        } else {
            GUI.Box(new Rect(390, 105, 500, 510), "", boxStyle);
            GUI.Label(new Rect(425, 135, 430, 65), Current.ToString(), title);
            
            if (Current == GameScreen.Pause) {
                if (Btn("Resume", 240)) { Current = GameScreen.Playing; Paused = false; }
                if (Btn("Save Game", 295)) Save();
                if (Btn("Settings", 350)) Current = GameScreen.Settings;
                if (Btn("Main Menu", 405)) {
                    ReturnToMainMenu();
                }
            }
            if (Current == GameScreen.Settings) {
                GUI.Label(new Rect(480, 230, 320, 30), "Music Volume", body);
                music = GUI.HorizontalSlider(new Rect(510, 270, 260, 25), music, 0, 1);
                GUI.Label(new Rect(480, 305, 320, 30), "Sound Volume", body);
                sound = GUI.HorizontalSlider(new Rect(510, 345, 260, 25), sound, 0, 1);
                if (Btn("Back", 420)) Current = Paused ? GameScreen.Pause : GameScreen.Main;
            }
        }
    }

    bool Btn(string s, float y) {
        return GUI.Button(new Rect(490, y, 300, 48), s, button);
    }

    void StartNewGame() {
        string targetScene = Application.CanStreamedLevelBeLoaded("LittleElsewhere") ? "LittleElsewhere" : "MainGameScene";
        if (Application.CanStreamedLevelBeLoaded("LoadingScene")) {
            LoadingScreenController.TargetSceneName = targetScene;
            SceneManager.LoadScene("LoadingScene");
        } else {
            SceneManager.LoadScene(targetScene);
        }
        Current = GameScreen.Playing;
        Paused = false;
    }

    void ContinueGame() {
        string targetScene = Application.CanStreamedLevelBeLoaded("LittleElsewhere") ? "LittleElsewhere" : "MainGameScene";
        if (Application.CanStreamedLevelBeLoaded("LoadingScene")) {
            LoadingScreenController.TargetSceneName = targetScene;
            SceneManager.LoadScene("LoadingScene");
        } else {
            SceneManager.LoadScene(targetScene);
        }
        Current = GameScreen.Playing;
        Paused = false;
    }

    void ReturnToMainMenu() {
        if (Application.CanStreamedLevelBeLoaded("MainMenuScene")) {
            SceneManager.LoadScene("MainMenuScene");
        }
        Current = GameScreen.Main;
        Paused = true;
    }

    void Save() {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) {
            PlayerPrefs.SetFloat("LE_X", p.transform.position.x);
            PlayerPrefs.SetFloat("LE_Y", p.transform.position.y);
            PlayerPrefs.SetInt("LE_SAVED", 1);
            PlayerPrefs.Save();
            Message = "Game saved.";
        }
        Current = GameScreen.Playing;
        Paused = false;
    }

    void Load() {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p && PlayerPrefs.GetInt("LE_SAVED", 0) == 1) {
            p.transform.position = new Vector3(PlayerPrefs.GetFloat("LE_X"), PlayerPrefs.GetFloat("LE_Y"), 0);
        }
        Current = GameScreen.Playing;
        Paused = false;
    }
}
