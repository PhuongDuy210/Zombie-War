using System.Collections;
using UnityEngine;
public class Grenade : MonoBehaviour
{
    [SerializeField]
    private float fuseTime = 5f;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private float baseKnockback;

    [SerializeField]
    private float radius;

    private ObjectPool explodeEffectPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        explodeEffectPool = PoolManager.Instance.Get(PrefabKey.ExplosionVFX);
    }

    void OnEnable()
    {
        StartCoroutine(DetonateAfterDelay());
    }

    private IEnumerator DetonateAfterDelay()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        Vector3 grenadePosition = transform.position;

        var explodeFXObject = explodeEffectPool.Pop();
        explodeFXObject.SetActive(true);
        explodeFXObject.transform.position = transform.position;

        var explodeEffect = explodeFXObject.GetComponent<ParticleSystem>();
        explodeEffect.Play();
        GameEventHandler.PlaySFX(SFXID.GrenadeExplode);
        Collider[] hits = Physics.OverlapSphere(grenadePosition, radius);
        foreach (Collider col in hits)
        {
            IHittable hitable = col.GetComponent<IHittable>();
            if (hitable != null)
            {
                Vector3 hitDirection = (col.transform.position - grenadePosition).normalized;
                float dist = Vector3.Distance(grenadePosition, col.transform.position);
                float factor = Mathf.Pow((1 - (dist / radius)),2);  // Quadratic falloff
                float damage = baseDamage * factor;
                float knockback = baseKnockback * factor;

                hitable.Hit(damage, hitDirection, knockback);
            }
        }

        gameObject.SetActive(false);
    }
}
