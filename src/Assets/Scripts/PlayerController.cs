using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float stopForce = 5f;
    [SerializeField] private float acceleration = 10f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject miniBulletPrefab;
    [SerializeField] private Image bulletState;
    [SerializeField] private MeshRenderer aimObject;
    [SerializeField] private Transform aimDirection;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHoldForce = 5f;
    [SerializeField] private float jumpDuration = 0.2f;
    [SerializeField] private float jumpHoldDuration = 0.2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float fallHoldMultiplier = 2.5f;

    [SerializeField] private TextMeshProUGUI SpecialCountText;

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

    private bool isBigBullet = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        Renderer renderer = firePoint.GetComponent<Renderer>();
        renderer.material.renderQueue = 4000;

        InputBind();
    }

    private void InputBind()
    {
        // Bind Input Events
        playerInput.actions["Move"].performed += ctx => moveInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, 0);
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector3.zero;

        playerInput.actions["Aim"].performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Aim"].canceled += ctx => aimInput = Vector2.zero;

        playerInput.actions["Jump"].performed += ctx => StartJump();
        playerInput.actions["Jump"].canceled += ctx => StopJump();

        playerInput.actions["Fire"].performed += ctx => FireBullet();

        playerInput.actions["ChangeScale"].performed += ctx => ChangeScale(); ;
    }

    private void Update()
    {
        Aim();

        isGrounded = CheckGroundedByTag();

        if (transform.position.y <= -25)
        {
            Death();
        }
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Aim()
    {
        if (CheckJoinGameped())
        {
            if (aimInput.sqrMagnitude > 0.1f)
            {
                float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
                aimDirection.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z; // カメラとオブジェクト間の距離を設定
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = (mousePosition - aimDirection.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            aimDirection.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Move()
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
    }

    private void Jump()
    {
        //ジャンプ時にタイマー計測
        if (!isGrounded)
        {
            jumpTimeCounter += Time.fixedDeltaTime;
        }
        else
        {
            jumpTimeCounter = 0;
            return;
        }

        //落下時の重力
        if (jumpTimeCounter >= jumpDuration)
        {
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Impulse);
        }

        //長押しジャンプ時の追加ジャンプ
        if (isHoldingJump && jumpTimeCounter <= jumpHoldDuration && jumpTimeCounter >= jumpDuration)
        {
            rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Impulse);
        }

        //長押しジャンプ後の重力
        if (jumpTimeCounter >= jumpHoldDuration)
        {
            rb.AddForce(Vector3.down * fallHoldMultiplier, ForceMode.Impulse);
        }
    }

    private void StartJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isHoldingJump = true;
            jumpTimeCounter = 0;
        }
    }

    private void StopJump()
    {
        isHoldingJump = false;
    }

    //スケールオブジェクトの反動で受ける追加ジャンプ
    public void AddJump(Vector3 dir, float power)
    {
        rb.AddForce(dir * power, ForceMode.Impulse);
        isHoldingJump = false;
    }

    private void ChangeScale()
    {
        isBigBullet = !isBigBullet; bulletState.color = isBigBullet ? Color.red : Color.cyan;
        aimObject.material.color = isBigBullet ? Color.red : Color.cyan;
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(isBigBullet ? bulletPrefab : miniBulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = firePoint.right * bulletSpeed;
            }
        }
    }

    public void Death()
    {
        SceneManager.LoadScene("Main");
    }

    int specialCount = 0;
    int specialMaxCount = 3;
    public void AddSpecial()
    {
        specialCount++;
        SpecialCountText.text = $"{specialCount}/{specialMaxCount}";
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

    private bool CheckJoinGameped()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length > 0)
        {
            // 接続されているジョイスティックを確認
            bool isGamepadConnected = false;

            foreach (string name in joystickNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    isGamepadConnected = true;
                    break;
                }
            }

            if (isGamepadConnected)
            {
                Debug.Log("ゲームパッドが接続されています。");
            }
            else
            {
                Debug.Log("ジョイスティックは検出されましたが、名前が空です。");
            }

            return isGamepadConnected;
        }
        else
        {
            Debug.Log("ゲームパッドは接続されていません。");

            return false;
        }
    }
}
