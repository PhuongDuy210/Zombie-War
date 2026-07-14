using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public static PoolManager Instance { get; private set; }

	[System.Serializable]
	public struct PoolEntry
	{
		public PrefabKey key;
		public ObjectPool pool;
	}

	[SerializeField] private PoolEntry[] pools;

	private Dictionary<PrefabKey, ObjectPool> lookup;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		lookup = new Dictionary<PrefabKey, ObjectPool>();
		foreach (var entry in pools)
		{
			lookup[entry.key] = entry.pool;
		}
	}

	public ObjectPool Get(PrefabKey key)
	{
		if (lookup.TryGetValue(key, out var pool))
		{
			return pool;
		}

		Debug.LogWarning($"No pool found for {key}");
		return null;
	}

    private void OnEnable()
    {
		GameEventHandler.OnGameStart += ResetAllPool;
    }

    private void OnDisable()
    {
        GameEventHandler.OnGameStart -= ResetAllPool;
    }

	private void ResetAllPool()
	{
		foreach (var (entry, pool) in lookup)
		{
			pool.RecallAllObjects();
        }
	}
}