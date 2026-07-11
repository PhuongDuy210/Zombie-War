using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private ObjectPool bulletTrailPool;

    [SerializeField]
    private Image gunIcon;

    private List<Weapon> weapons = new List<Weapon>();
    private int currentIndex = 0;

    private Weapon currentGun;

    private Animator anim;
    private int animIDShootingSpeed;
    private int animIDIsShooting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapons = GetComponentsInChildren<Weapon>().ToList();
        foreach (var w in weapons)
        {
            w.Init(this);
        }
        
        anim = GetComponent<Animator>();
        animIDShootingSpeed = Animator.StringToHash("ShootingSpeed");
        animIDIsShooting = Animator.StringToHash("IsShooting");

        ChangeWeapon(currentIndex);
    }

    private void OnEnable()
    {
        GameEventHandler.OnShootButtonDown += Shoot;
        GameEventHandler.OnShootButtonUp += StopShooting;
        GameEventHandler.OnSwitchButtonDown += CycleWeapon;
    }

    private void OnDisable()
    {
        GameEventHandler.OnShootButtonDown -= Shoot;
        GameEventHandler.OnShootButtonUp -= StopShooting;
        GameEventHandler.OnSwitchButtonDown -= CycleWeapon;
    }

    private void Shoot()
    {
        anim.SetBool(animIDIsShooting, true);
    }

    private void StopShooting()
    {
        anim.SetBool(animIDIsShooting, false);
    }

    public void CycleWeapon()
    {
        currentIndex = (currentIndex + 1) % weapons.Count;
        ChangeWeapon(currentIndex);
    }

    public void ChangeWeapon(int index)
    {
        // Deactivate all weapons
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        currentGun = weapons[index];
        currentGun.gameObject.SetActive(true);
        anim.SetFloat(animIDShootingSpeed, currentGun.config.fireRate);

        gunIcon.sprite = currentGun.config.gunIcon;

        GameEventHandler.PlaySFX(SFXID.WeaponSwitch);
    }

    public void AnimShoot()
    {
        currentGun.Fire();
    }

    public void SpawnBulletTrail(Vector3 dir, float range)
    {
        var bulletTrail = bulletTrailPool.Pop();
        if (bulletTrail != null)
        {
            bulletTrail.transform.position = Vector3.zero;

            var trailRenderer = bulletTrail.GetComponent<LineRenderer>();
            trailRenderer.SetPosition(0, currentGun.barrel.transform.position);
            Vector3 endPoint = currentGun.barrel.transform.position + dir * range;
            trailRenderer.SetPosition(1, endPoint);

            bulletTrail.SetActive(true);
            StartCoroutine(DisableTrail(bulletTrail));
        }
    }

    private IEnumerator DisableTrail(GameObject bulletTrail)
    {
        yield return new WaitForSeconds(0.2f);
        bulletTrail.SetActive(false);
    }
}
