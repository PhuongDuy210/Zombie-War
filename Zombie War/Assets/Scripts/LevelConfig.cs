using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Gameplay/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int level;
    public EnemyEntry[] enemyEntries;
    [Tooltip("Amount of spawned enemies at one time")]
    public int maxSpawn = 50;
}
