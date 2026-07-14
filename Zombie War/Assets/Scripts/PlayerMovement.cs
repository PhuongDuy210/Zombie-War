using Terresquall;   // Required for VirtualJoystick
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;   // Movement speed
    public float gravity = -9.81f;
    private Animator anim;
    private CharacterController controller;
    private Collider playerCollider;

    private Vector3 velocity;

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
        controller = GetComponent<CharacterController>();
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

    void Update()
    {
        // Read joystick input as a Vector2
        Vector2 joyInput = VirtualJoystick.GetAxis();

        // Convert joystick input into world-space movement
        Vector3 moveDirection = new Vector3(joyInput.x, 0, joyInput.y);

        // Apply movement
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Gravity handling
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to keep grounded
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Optional: Rotate player to face movement direction
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }

        // --- Animator Speed Scaling ---
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
