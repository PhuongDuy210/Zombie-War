using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponConfig config;

    protected WeaponController controller;

    public GameObject barrel;

    private ParticleSystem muzzleFlash;
    public void Init(WeaponController controller)
    {
        this.controller = controller;
        var muzzleFlashGO = Instantiate(config.muzzleFlashFX, barrel.transform);
        muzzleFlash = muzzleFlashGO.GetComponent<ParticleSystem>();
    }

    public void Fire()
    {
        // Step 1: Find slope normal under player
        RaycastHit groundHit;
        Vector3 groundNormal = Vector3.up; // default flat ground
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 2f))
        {
            groundNormal = groundHit.normal;
        }

        for (int i = 0; i < config.bulletCount; i++)
        {
            // Step 2: Base aim direction (forward in top-down)
            Vector3 aimDir = transform.forward;

            // Project aim direction onto slope plane
            aimDir = Vector3.ProjectOnPlane(aimDir, groundNormal).normalized;

            // Step 3: Apply spread around the slope-aware direction
            float angle = Random.Range(-config.spreadAngle, config.spreadAngle);
            Vector3 spreadDir = Quaternion.AngleAxis(angle, groundNormal) * aimDir;

            RaycastHit hit;
            float finalRange = config.range;

            if (Physics.Raycast(transform.position, spreadDir, out hit, config.range))
            {
                finalRange = hit.distance;

                var target = hit.collider.GetComponent<IHittable>();
                if (target != null)
                {
                    target.Hit(config.damage, spreadDir, config.knockback);
                }
            }

            controller.SpawnBulletTrail(spreadDir, finalRange);
        }

        muzzleFlash.Play();
        GameEventHandler.PlaySFX(config.gunShotSFXID);
    }
}