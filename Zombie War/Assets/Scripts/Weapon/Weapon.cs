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
        for (int i = 0; i < config.bulletCount; i++)
        {
            // Random angle within spread range
            float angle = Random.Range(-config.spreadAngle, config.spreadAngle);

            // Rotate the forward vector around the Y axis (horizontal fan)
            Vector3 spreadDir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;

            RaycastHit hit;
            float finalRange = config.range;

            if (Physics.Raycast(transform.position, spreadDir, out hit, config.range))
            {
                // Shorten trail to hit point
                finalRange = hit.distance;

                // Trigger hit logic on target
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