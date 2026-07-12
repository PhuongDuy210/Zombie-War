using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/New Weapon")]
public class WeaponConfig : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float fireRate;
    public int bulletCount;     // e.g. shotgun pellets
    public float spreadAngle;   // bullet spray spread for rifle, pellets spread for shotgun
    public float range;
    public float knockback;
    public SFXID gunShotSFXID;
    public Sprite gunIcon;
    public GameObject muzzleFlashFX;
}