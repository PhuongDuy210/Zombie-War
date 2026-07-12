using UnityEngine;

public class Zombie : MonoBehaviour, IHittable
{
    public float health = 100;
    private ZombieController controller;
    private BoxCollider boxCollider;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<ZombieController>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }
        boxCollider.enabled = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        else
        {
            // Resume after short delay
            //StartCoroutine(ResumeAfterDelay(0.5f));
        }
    }

    private void Die()
    {
        controller.Dead();
        boxCollider.enabled = false;
    }
}
