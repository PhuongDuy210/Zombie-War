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
            controller.SpawnBulletTrail(spreadDir, config.range);
        }
        muzzleFlash.Play();
        GameEventHandler.PlaySFX(config.gunShotSFXID);
    }
}