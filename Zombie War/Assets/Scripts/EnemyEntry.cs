using UnityEngine;

[System.Serializable]
public struct EnemyEntry
{
    public EnemyType type;
    [Tooltip("Amount to spawn")]
    public int amount;
    [Tooltip("Amount of time since the level start before starting spawning enemies")]
    public float timeSpawnBuffer;
}