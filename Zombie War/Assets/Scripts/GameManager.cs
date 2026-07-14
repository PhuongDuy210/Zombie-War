using UnityEngine;
using UnityEngine.Android;

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
    }
    private void Start()
    {
        GameEventHandler.StartGame();
    }

    private void OnEnable()
    {
        GameEventHandler.OnGameStart += LoadLevel;
        GameEventHandler.OnEnemyKilled += IncreaseKillCount;
        GameEventHandler.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameEventHandler.OnGameStart -= LoadLevel;
        GameEventHandler.OnEnemyKilled -= IncreaseKillCount;
        GameEventHandler.OnGameOver -= GameOver;
    }

    private void LoadLevel()
    {
        levelConfig = Resources.Load<LevelConfig>("Levels/" + level);
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<EnemySpawner>();
        }
        spawner.StartSpawner();
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
