using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int level;
    private LevelConfig levelConfig;

    private EnemySpawner spawner;

    public GameState gameState;

    private float totalKillRequire = 0;
    private float totalKill = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameEventHandler.StartGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventHandler.OnLevelRetry += ReloadLevel;
        GameEventHandler.OnLevelChange += NextLevel;
        GameEventHandler.OnGameStart += LoadLevel;
        GameEventHandler.OnEnemyKilled += IncreaseKillCount;
        GameEventHandler.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventHandler.OnLevelRetry -= ReloadLevel;
        GameEventHandler.OnLevelChange -= NextLevel;
        GameEventHandler.OnGameStart -= LoadLevel;
        GameEventHandler.OnEnemyKilled -= IncreaseKillCount;
        GameEventHandler.OnGameOver -= GameOver;
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(level - 1);
    }

    private void NextLevel()
    {
        level++;
        SceneManager.LoadScene(level -1);
    }

    private void LoadLevel()
    {
        levelConfig = Resources.Load<LevelConfig>("Levels/" + level);
        Debug.Log("Load Config " + level);
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<EnemySpawner>();
        }
        spawner.StartSpawner(levelConfig);
        gameState = GameState.Start;

        totalKillRequire = 0;
        totalKill = 0;
        foreach (var enemyEntry in levelConfig.enemyEntries)
        {
            totalKillRequire += enemyEntry.amount;
        }
    }

    private void IncreaseKillCount()
    {
        totalKill++;
        if (totalKill == totalKillRequire)
        {
            GameEventHandler.EndGame(GameState.Win);
        }
    }

    private void GameOver(GameState state)
    {
        if (state == GameState.Lose)
        {
            GameEventHandler.PlaySFX(SFXID.Lose);
            gameState = GameState.Lose;
        }

        if (state == GameState.Win)
        {
            GameEventHandler.PlaySFX(SFXID.Win);
            gameState = GameState.Win;
        }
    }
}
