using UnityEngine;

public interface IHittable
{
    void Hit(float damage, Vector3 hitDirection, float knockback);
}
