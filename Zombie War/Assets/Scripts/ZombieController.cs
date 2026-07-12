using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
    public float attackRange = 2f;      // Distance at which enemy attacks
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

        player = FindFirstObjectByType<PlayerMovement>().gameObject.transform;
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

        if (!agent.isStopped)
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
        Debug.Log("Enemy attacks the player!");
        anim.SetTrigger(animIDAttack);
        // Add animation trigger or damage logic here
    }

    public void TakeDamage(Vector3 hitDirection, float knockback)
    {
        anim.SetTrigger(animIDDamaged);
        GameEventHandler.PlaySFX(SFXID.ZombieDamaged);
        TriggerFlash();
        StartCoroutine(TakeDamageRoutine());

        ApplyKnockback(hitDirection, knockback);
    }

    private IEnumerator TakeDamageRoutine()
    {
        agent.isStopped = true; // stop moving
        yield return new WaitForSeconds(1f); // stunned for 1 second
        agent.isStopped = false; // resume movement
    }

    private void ApplyKnockback(Vector3 hitDirection, float knockback)
    {
        StopCoroutine(nameof(KnockbackCoroutine));
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
