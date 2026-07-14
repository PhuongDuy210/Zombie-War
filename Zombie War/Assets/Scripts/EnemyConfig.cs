using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Gameplay/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public EnemyType id = EnemyType.Zombie;
    public PrefabKey prefabKey;
    public float hp = 100;
    public float attack = 10;
}