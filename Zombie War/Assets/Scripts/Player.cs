using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;

    [SerializeField] 
    private float hurtCooldown = 1f;

    private float currentHealth;
    private bool canBeHurt = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        GameEventHandler.UpdatePlayerHealth(1); // Max health 100%
    }

    private void OnEnable()
    {
        GameEventHandler.OnGameOver += Suspend;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        GameEventHandler.OnGameOver -= Suspend;
    }

    private void Suspend(GameState gameState)
    {
        StopAllCoroutines();
        canBeHurt = false;
    }

    public void TakeDamage(float damage)
    {
        if (!canBeHurt || currentHealth <= 0) return;

        currentHealth -= damage;
        float healthPercentage = currentHealth / maxHealth;
        GameEventHandler.UpdatePlayerHealth(healthPercentage);
        GameEventHandler.PlaySFX(SFXID.PlayerHurt);
    
        if (currentHealth <= 0)
        {
            GameEventHandler.EndGame(GameState.Lose);
        }

        StartCoroutine(HurtCooldownRoutine());
    }

    private IEnumerator HurtCooldownRoutine()
    {
        canBeHurt = false;

        float elapsed = 0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < hurtCooldown)
        {
            // Toggle red on
            foreach (Renderer r in renderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetFloat("_UseColorMap", 0f);
                block.SetColor("_Color", Color.red);
                r.SetPropertyBlock(block);
            }

            yield return new WaitForSeconds(0.1f);

            // Toggle back to normal
            foreach (Renderer r in renderers)
            {
                r.SetPropertyBlock(null);
            }

            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f; // total cycle time
        }

        canBeHurt = true;
    }
}
