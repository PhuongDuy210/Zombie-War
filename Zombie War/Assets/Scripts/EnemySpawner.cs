using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private List<Transform> spawnLocations;

    private Dictionary<EnemyType, EnemyConfig> enemyConfigs = new Dictionary<EnemyType, EnemyConfig>();
    private int currentSpawned = 0;

    private void Awake()
    {
        LoadEnemyConfigs();
    }

    private void OnEnable()
    {
        GameEventHandler.OnEnemyKilled += DecreaseSpawnCount;
    }

    private void OnDisable()
    {
        GameEventHandler.OnEnemyKilled -= DecreaseSpawnCount;
    }

    public void StartSpawner()
    {
        StartCoroutine(SpawnRoutine());
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
        foreach (var entry in levelConfig.enemyEntries)
        {
            // Wait until the buffer time for this entry
            yield return new WaitForSeconds(entry.timeSpawnBuffer);

            for (int i = 0; i < entry.amount; i++)
            {
                if (currentSpawned >= levelConfig.maxSpawn)
                {
                    yield break;
                }

                if (enemyConfigs.TryGetValue(entry.type, out var config))
                {
                    SpawnEnemy(config);
                }

                // Small delay between spawns to avoid popping all at once
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
        enemyGO.transform.position = spawnPoint.position;

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
