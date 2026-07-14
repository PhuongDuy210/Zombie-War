using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class ZombieController : MonoBehaviour
{
    private Transform player;            // Reference to the player
    private Animator anim;

    private SkinnedMeshRenderer skinRenderer;
    private MaterialPropertyBlock block;

    private float flashDuration = 0.2f;
    private float dissolveSpeed = 1;

    [SerializeField]
    private float chaseRange = 10f;      // Distance at which enemy starts chasing

    [SerializeField]
    private Collider hitbox;
    [SerializeField]
    public float attackRange = 1f;      // Distance at which enemy attacks
    [SerializeField]
    public float attackCooldown = 1.5f; // Time between attacks

    private NavMeshAgent agent;
    private float lastAttackTime;

    // animation IDs
    private int animIDSpeed;
    private int animIDAttack;
    private int animIDDead;
    private int animIDDamaged;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        AssignAnimationIDs();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        if (skinRenderer == null)
        {
            skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            block = new MaterialPropertyBlock();
        }
        skinRenderer.GetPropertyBlock(block);
        block.SetFloat("_DissolveAmount", 0);
        skinRenderer.SetPropertyBlock(block);

        hitbox.enabled = false;
        StartCoroutine(PlaySFXRoutine());
    }

    private void OnDisable()
    {
        
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDAttack = Animator.StringToHash("Attack");
        animIDDead = Animator.StringToHash("Dead");
        animIDDamaged = Animator.StringToHash("Damaged");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (!agent.isStopped && GameManager.Instance.gameState == GameState.Start)
        {
            if (distanceToPlayer <= chaseRange)
            {
                // Chase the player
                agent.SetDestination(player.position);

                anim.SetFloat(animIDSpeed, agent.velocity.magnitude);

                if (distanceToPlayer <= attackRange)
                {
                    // Stop moving when close enough
                    agent.ResetPath();

                    // Rotate toward player
                    Vector3 toPlayer = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(toPlayer.x, 0, toPlayer.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                    // Attack if cooldown passed
                    if (Time.time > lastAttackTime + attackCooldown)
                    {
                        Attack();
                        lastAttackTime = Time.time;
                    }
                }
            }
            else
            {
                // Idle or patrol logic can go here
                agent.ResetPath();
            }
        }
    }

    private void Attack()
    {
        hitbox.enabled = true;
        anim.SetTrigger(animIDAttack);
        StartCoroutine(TemporaryStopRoutine());
        // Add animation trigger or damage logic here
    }

    private IEnumerator PlaySFXRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            TryPlayZombieSFX();
        }
    }

    private void TryPlayZombieSFX()
    {
        if (Random.value <= 0.25f)
        {
            GameEventHandler.PlaySFX(SFXID.ZombieGroan);
        }
    }

    public void TakeDamage(Vector3 hitDirection, float knockback)
    {
        anim.SetTrigger(animIDDamaged);
        GameEventHandler.PlaySFX(SFXID.ZombieDamaged);
        TriggerFlash();
        // Only play SFX 30% of the time to avoid clutter
        if (Random.value <= 0.3f)
        {
            GameEventHandler.PlaySFX(SFXID.ZombieDamaged);
        }
        StartCoroutine(TemporaryStopRoutine());

        ApplyKnockback(hitDirection, knockback);
    }

    private IEnumerator TemporaryStopRoutine()
    {
        agent.isStopped = true; // stop moving
        yield return new WaitForSeconds(1f); // stunned for 1 second
        agent.isStopped = false; // resume movement
    }

    private void ApplyKnockback(Vector3 hitDirection, float knockback)
    {
        //StopCoroutine(nameof(KnockbackCoroutine));
        StartCoroutine(KnockbackCoroutine(hitDirection, knockback));
    }

    private IEnumerator KnockbackCoroutine(Vector3 hitDirection, float knockback)
    {
        // Face the attacker (opposite of hitDirection)
        Vector3 lookDir = -hitDirection;
        lookDir.y = 0; // keep rotation flat on ground plane
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDir);

        // Knockback movement
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + hitDirection.normalized * knockback;

        float elapsed = 0f;
        float duration = 0.2f; // fixed duration, distance scales with knockback

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }

    public void Dead()
    {
        agent.isStopped = true;
        anim.SetTrigger(animIDDead);
        GameEventHandler.PlaySFX(SFXID.ZombieDead);
        GameEventHandler.EnemyKilled();
        StopAllCoroutines();
        StartDissolve();
    }

    public void TriggerFlash()
    {
        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Apply hurt color
        skinRenderer.GetPropertyBlock(block);
        block.SetFloat("_FlashIntensity", 0.5f);
        skinRenderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(flashDuration);

        // Reset back to default (clear block)
        skinRenderer.GetPropertyBlock(block);
        block.Clear();
        skinRenderer.SetPropertyBlock(block);
    }

    private void StartDissolve()
    {
        StopCoroutine(nameof(DissolveCoroutine));
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(2f);
        float dissolveAmount = 0f;
        skinRenderer.GetPropertyBlock(block);
        while (dissolveAmount < 1f)
        {
            dissolveAmount += dissolveSpeed * Time.deltaTime;
            block.SetFloat("_DissolveAmount", dissolveAmount);
            skinRenderer.SetPropertyBlock(block);

            yield return null;
        }

        // Fully dissolved → disable object
        gameObject.SetActive(false);
    }
}
