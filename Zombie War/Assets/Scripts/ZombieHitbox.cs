using UnityEngine;

public class ZombieHitbox : MonoBehaviour
{
    private Zombie zombie;

    private void Awake()
    {
        zombie = GetComponentInParent<Zombie>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombie.Attack();
        }
    }
}
