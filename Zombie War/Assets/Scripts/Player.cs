using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;

    private float currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        GameEventHandler.UpdatePlayerHealth(1); // Max health 100%
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        float healthPercentage = currentHealth / maxHealth;
        GameEventHandler.UpdatePlayerHealth(healthPercentage);
        GameEventHandler.PlaySFX(SFXID.PlayerHurt);
        FlashRed(0.2f);
        if (currentHealth <= 0)
        {
            GameEventHandler.EndGame(GameState.Lose);
        }
    }

    public void FlashRed(float duration)
    {
        StopCoroutine(nameof(FlashCoroutine));
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetFloat("_UseColorMap", 0f);
            block.SetColor("_Color", Color.red);
            r.SetPropertyBlock(block);
        }

        yield return new WaitForSeconds(duration);

        // Reset 
        foreach (Renderer r in renderers)
        {
            r.SetPropertyBlock(null);
        }
    }

}
