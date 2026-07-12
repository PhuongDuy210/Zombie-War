using UnityEngine;
using Terresquall;   // Required for VirtualJoystick

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;   // Movement speed
    private Rigidbody rb;
    private Animator anim;
    private Collider playerCollider;

    private ObjectPool grenadePool;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    //private int animIDJump;
    //private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDThrow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();

        if (anim  != null )
        {
            AssignAnimationIDs();
            anim.SetBool(animIDGrounded, true);
            anim.SetFloat(animIDMotionSpeed, 1);
        }

        grenadePool = PoolManager.Instance.Get(PrefabKey.Grenade);
    }

    private void OnEnable()
    {
        GameEventHandler.OnGrenadeButtonDown += ThrowGrenade;
    }

    private void OnDisable()
    {
        GameEventHandler.OnGrenadeButtonDown -= ThrowGrenade;
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        //animIDJump = Animator.StringToHash("Jump");
        //animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDThrow = Animator.StringToHash("Throw");
    }

    void FixedUpdate()
    {
        // Read joystick input as a Vector2
        Vector2 joyInput = VirtualJoystick.GetAxis();

        // Convert joystick input into world-space movement
        Vector3 moveDirection = new Vector3(joyInput.x, 0, joyInput.y);

        // Apply movement
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Optional: Rotate player to face movement direction
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.2f);
        }

        // --- Animator Speed Scaling ---
        // Joystick magnitude ranges from 0 to 1.
        // Scale it to 0–6 for your blend tree.
        float speedValue = joyInput.magnitude * 6f;
        anim.SetFloat(animIDSpeed, speedValue);
    }

    private void ThrowGrenade()
    {
        anim.SetTrigger(animIDThrow);
    }

    public void GrenadeOff()
    {
        var grenadeGO = grenadePool.Pop();
        Collider grenadeCollider = grenadeGO.GetComponent<Collider>();
        Physics.IgnoreCollision(grenadeCollider, playerCollider);

        grenadeGO.SetActive(true);
        grenadeGO.transform.position = transform.position;
        
        Rigidbody grenadeRb = grenadeGO.GetComponent<Rigidbody>();
        if (grenadeRb != null)
        {

            float throwForce = 10f;
            float upwardForce = 5f;

            grenadeRb.linearVelocity = Vector3.zero;
            Vector3 forceDirection = transform.forward * throwForce + Vector3.up * upwardForce;
            grenadeRb.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
