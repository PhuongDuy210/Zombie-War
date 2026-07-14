using UnityEngine;

public class Zombie : MonoBehaviour, IHittable
{
    private Player player;

    private float health;
    private float attack;
    private ZombieController controller;
    private Collider capCollider;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        controller = GetComponent<ZombieController>();
        capCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (capCollider == null)
        {
            capCollider = GetComponent<Collider>();
        }
        capCollider.enabled = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(EnemyConfig config)
    {
        health = config.hp;
        attack = config.attack;
    }

    public void Hit(float damage, Vector3 hitDirection, float knockback)
    {
        if (isDead) return;

        health -= damage;

        // Stop movement briefly
        controller.TakeDamage(hitDirection, knockback);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Attack()
    {
        if (isDead) return;
        player.TakeDamage(attack);
    }

    private void Die()
    {
        controller.Dead();
        capCollider.enabled = false;
    }
}
