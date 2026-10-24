using UnityEngine;
using LittleElsewhere.World;
using LittleElsewhere.Player;
using LittleElsewhere.Lighting;
using LittleElsewhere.TimeSystem;
using LittleElsewhere.Environment;
using LittleElsewhere.Stories;
using LittleElsewhere.Navigation;
using LittleElsewhere.Saving;

namespace LittleElsewhere.Core
{
    /// <summary>
    /// Master Game Orchestrator. Coordinates world generation, player movement, camera follow,
    /// lighting, calendar, weather, story management, journal, and save/load persistence.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Primary Configuration")]
        [SerializeField] private WorldSeedData worldConfig;

        [Header("Sub-System References")]
        [SerializeField] private ChunkManager chunkManager;
        [SerializeField] private WrappedPlayerController playerController;
        [SerializeField] private WrappedCameraController cameraController;
        [SerializeField] private AtmosphericLightingController lightingController;
        [SerializeField] private CalendarManager calendarManager;
        [SerializeField] private WeatherController weatherController;
        [SerializeField] private SeasonalPaletteController paletteController;
        [SerializeField] private StoryManager storyManager;
        [SerializeField] private JournalManager journalManager;
        [SerializeField] private AchievementManager achievementManager;
        [SerializeField] private EndingManager endingManager;
        [SerializeField] private NavigationManager navigationManager;
        [SerializeField] private WorldSaveSystem saveSystem;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponentsIfMissing();
        }

        private void Start()
        {
            NewGame(1337);
        }

        public void NewGame(int seed)
        {
            if (worldConfig != null)
            {
                worldConfig.worldSeed = seed;
            }

            if (storyManager != null)
            {
                storyManager.InitializeStoryPlacement();
            }

            Debug.Log($"[GameManager] Initialized New Game with seed {seed}.");
        }

        public void SaveGame()
        {
            if (saveSystem != null && playerTransformPos() is Vector2 pos)
            {
                float hour = calendarManager != null ? calendarManager.CurrentHour : 12f;
                int seed = worldConfig != null ? worldConfig.worldSeed : 1337;
                saveSystem.SaveGame(seed, pos, hour);
            }
        }

        public void LoadGame()
        {
            if (saveSystem == null) return;

            SaveData data = saveSystem.LoadGame();
            if (data != null)
            {
                NewGame(data.seed);
                if (playerController != null)
                {
                    playerController.transform.position = new Vector2(data.playerCanonicalX, data.playerCanonicalY);
                }
                if (calendarManager != null)
                {
                    calendarManager.SetCalendarState(data.timeOfDay, 1, Season.Winter, 1);
                }
                Debug.Log($"[GameManager] Loaded game state from save file.");
            }
        }

        private Vector2 playerTransformPos()
        {
            return playerController != null ? (Vector2)playerController.transform.position : Vector2.zero;
        }

        private void InitializeComponentsIfMissing()
        {
            if (chunkManager == null) chunkManager = FindFirstObjectByType<ChunkManager>();
            if (playerController == null) playerController = FindFirstObjectByType<WrappedPlayerController>();
            if (cameraController == null) cameraController = FindFirstObjectByType<WrappedCameraController>();
            if (lightingController == null) lightingController = FindFirstObjectByType<AtmosphericLightingController>();
            if (calendarManager == null) calendarManager = FindFirstObjectByType<CalendarManager>();
            if (weatherController == null) weatherController = FindFirstObjectByType<WeatherController>();
            if (paletteController == null) paletteController = FindFirstObjectByType<SeasonalPaletteController>();
            if (storyManager == null) storyManager = FindFirstObjectByType<StoryManager>();
            if (journalManager == null) journalManager = FindFirstObjectByType<JournalManager>();
            if (achievementManager == null) achievementManager = FindFirstObjectByType<AchievementManager>();
            if (endingManager == null) endingManager = FindFirstObjectByType<EndingManager>();
            if (navigationManager == null) navigationManager = FindFirstObjectByType<NavigationManager>();
            if (saveSystem == null) saveSystem = FindFirstObjectByType<WorldSaveSystem>();
        }
    }
}
