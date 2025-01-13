using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float stopForce = 5f;
    public float acceleration = 10f;
    public float maxSpeed = 10f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform aimDirection;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float jumpHoldForce = 5f; // Additional force when jump is held
    public float jumpHoldDuration = 0.2f; // Duration for which jump can be held
    public float fallMultiplier = 2.5f; // Multiplier for falling speed
    public float lowJumpMultiplier = 2f; // Multiplier for low jump speed

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector2 aimInput;
    private bool isGrounded = false;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private PlayerInput playerInput;

    private float jumpTimeCounter = 0f;
    private bool isHoldingJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Bind Input Events
        playerInput.actions["Move"].performed += ctx => moveInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, 0);
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector3.zero;

        playerInput.actions["Aim"].performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Aim"].canceled += ctx => aimInput = Vector2.zero;

        playerInput.actions["Jump"].performed += ctx => StartJump();
        playerInput.actions["Jump"].canceled += ctx => StopJump();

        playerInput.actions["Fire"].canceled += ctx => FireBullet();
        playerInput.actions["Fire"].performed += ctx => Aim();

        Renderer renderer = firePoint.GetComponent<Renderer>();
        renderer.material.renderQueue = 4000;
    }

    private void Update()
    {
        // Update aim direction
        if (aimInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
            aimDirection.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Update grounded status
        isGrounded = CheckGroundedByTag();
        // Reset jump time counter when grounded
        if (isGrounded)
        {
            jumpTimeCounter = jumpHoldDuration;
        }

        Death();
    }

    private void FixedUpdate()
    {
        if (moveInput != Vector3.zero)
        {
            if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.AddForce(new Vector3(moveInput.x * acceleration, 0, 0), ForceMode.Force);
            }
        }
        else
        {
            Vector3 velocity = rb.velocity;
            if (Mathf.Abs(velocity.x) > 0.1f)
            {
                rb.AddForce(new Vector3(-velocity.x * stopForce, 0, 0), ForceMode.Force);
            }
        }

        // Apply jump hold force
        if (isHoldingJump && jumpTimeCounter > 0)
        {
            rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Force);
            jumpTimeCounter -= Time.fixedDeltaTime;
        }

        // Modify falling and low jump speeds
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * (fallMultiplier - 1) * Physics.gravity.y, ForceMode.Acceleration);
        }
        else if (rb.velocity.y > 0 && !isHoldingJump)
        {
            rb.AddForce(Vector3.down * (lowJumpMultiplier - 1) * Physics.gravity.y, ForceMode.Acceleration);
        }
    }

    private void StartJump()
    {
        if (isGrounded)
        {
            // Initial jump force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isHoldingJump = true;
            jumpTimeCounter = jumpHoldDuration;
        }
    }

    private void StopJump()
    {
        isHoldingJump = false;
    }

    private bool CheckGroundedByTag()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ground") || collider.CompareTag("ScaleObject"))
            {
                return true;
            }
        }
        return false;
    }

    public void AddJump(Vector3 dir, float power)
    {
        if (isHoldingJump) return;
        rb.AddForce(dir * power, ForceMode.Impulse);
        isHoldingJump = false;
    }

    private void FireBullet()
    {
        Time.timeScale = 1.0f;

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = firePoint.right * bulletSpeed;
            }
        }
    }

    private void Aim()
    {
        Time.timeScale = 0.3f;
    }

    private void Death()
    {
        if (transform.position.y <= -25)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
