using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private LevelConfig levelConfig;
    [SerializeField] private List<Transform> spawnLocations;

    private Dictionary<EnemyType, EnemyConfig> enemyConfigs = new Dictionary<EnemyType, EnemyConfig>();
    private Dictionary<EnemyEntry, int> entrySpawned = new Dictionary<EnemyEntry, int>();

    private int currentSpawned = 0;

    private void Awake()
    {
        LoadEnemyConfigs();
    }

    private void OnEnable()
    {
        GameEventHandler.OnEnemyKilled += DecreaseSpawnCount;
        GameEventHandler.OnGameOver += StopSpawner;
    }

    private void OnDisable()
    {
        GameEventHandler.OnEnemyKilled -= DecreaseSpawnCount;
        GameEventHandler.OnGameOver -= StopSpawner;
    }

    public void StartSpawner(LevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawner(GameState gameState)
    {
        StopAllCoroutines();
    }

    private void LoadEnemyConfigs()
    {
        // Load all EnemyConfig assets from Resources
        EnemyConfig[] configs = Resources.LoadAll<EnemyConfig>("EnemyConfigs");
        foreach (var config in configs)
        {
            if (!enemyConfigs.ContainsKey(config.id))
            {
                enemyConfigs.Add(config.id, config);
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        entrySpawned.Clear();

        foreach (var entry in levelConfig.enemyEntries)
        {
            entrySpawned[entry] = 0; // start at zero

            yield return new WaitForSeconds(entry.timeSpawnBuffer);

            while (entrySpawned[entry] < entry.amount)
            {
                while (currentSpawned >= levelConfig.maxSpawn)
                {
                    yield return new WaitForSeconds(0.5f); // keep waiting until DecreaseSpawnCount lowers it
                }

                if (enemyConfigs.TryGetValue(entry.type, out var config))
                {
                    SpawnEnemy(config);
                    entrySpawned[entry]++;   // track per-entry
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void SpawnEnemy(EnemyConfig config)
    {
        if (spawnLocations == null || spawnLocations.Count == 0) return;

        // Pick a random spawn location with equal chance
        int index = Random.Range(0, spawnLocations.Count);
        Transform spawnPoint = spawnLocations[index];

        // Resolve prefab from your prefab system
        ObjectPool pool = PoolManager.Instance.Get(config.prefabKey);
        if (pool == null) return;

        GameObject enemyGO = pool.Pop();
        enemyGO.SetActive(true);

        // Add random offset around the spawn point
        Vector3 offset = new Vector3(
            Random.Range(-2f, 2f),   // X variation
            0f,                      // keep Y fixed so they don’t float
            Random.Range(-2f, 2f)    // Z variation
        );
        enemyGO.transform.position = spawnPoint.position + offset;

        // Initialize stats if your Enemy script supports it
        Zombie enemy = enemyGO.GetComponent<Zombie>();
        if (enemy != null)
        {
            enemy.Init(config);
        }

        currentSpawned++;
    }

    private void DecreaseSpawnCount()
    {
        currentSpawned--;
    }
}
