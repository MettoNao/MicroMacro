using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Module.Utile;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Module.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerParamater playerParamater;
        [SerializeField] private Image bulletState;
        [SerializeField] private TextMeshProUGUI SpecialCountText, HpText;
        [SerializeField] private Image redFlashImage;

        [SerializeField] private PlayerAimer playerAimer;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private GroundChecker groundChecker;

        [SerializeField] private MeshRenderer gunColor, gunColor2;

        private PlayerMover playerMover;
        private PlayerJumper playerJumper;

        private Rigidbody rb;
        private Vector3 moveInput;
        private PlayerInput playerInput;

        private bool isBigBullet = true;
        InputActionMap map;
        public Vector2 Direction { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            playerMover = new PlayerMover(rb, groundChecker, playerParamater);
            playerJumper = new PlayerJumper(rb, groundChecker, playerParamater);

            HpText.text = $"hp:{hp}";

            InputBind();
        }

        private void InputBind()
        {
            map = playerInput.currentActionMap;

            map["Move"].performed += MoveInput;
            map["Move"].canceled += MoveInputZero;

            map["Aim"].performed += Aim;

            map["Jump"].performed += playerJumper.StartJump;
            map["Jump"].canceled += playerJumper.StopJump;

            map["LergeFire"].performed += LergeFire;
            map["LergeFire"].canceled += playerShooter.CancelFire;

            map["MinimalizeFire"].performed += MinimalizeFire;
            map["MinimalizeFire"].canceled += playerShooter.CancelFire;
        }

        void MoveInput(InputAction.CallbackContext ctx)
        {
            moveInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, 0);
        }

        void MoveInputZero(InputAction.CallbackContext callbackContext)
        {
            moveInput = Vector3.zero;
        }

        void Aim(InputAction.CallbackContext context)
        {
            Direction = context.ReadValue<Vector2>().normalized;
            playerAimer.Aim(Direction);
        }

        private void LergeFire(InputAction.CallbackContext context)
        {
            ChangeScale(true);
            playerShooter.FireBullet(isBigBullet);
        }

        private void MinimalizeFire(InputAction.CallbackContext context)
        {
            ChangeScale(false);
            playerShooter.FireBullet(isBigBullet);
        }

        private void Update()
        {
            if (transform.position.y <= -25)
            {
                Death();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                map?.Dispose();
                SceneManager.LoadScene("Title");
            }
        }

        private void FixedUpdate()
        {
            playerJumper.Jump();
            playerMover.Move(moveInput);
        }

        public void AddJump(Vector3 dir, float power)
        {
            playerJumper.AddJump(dir, power);
        }

        private void ChangeScale(bool isBigBullet)
        {
            this.isBigBullet = isBigBullet;
            bulletState.color = isBigBullet ? Color.red : Color.cyan;
            gunColor.material.color = bulletState.color;
            gunColor2.material.color = bulletState.color;
        }

        int hp = 5;
        float flashTime = 0.3f;
        public void Death()
        {
            SceneManager.LoadScene("Main");
        }

        public void Damage(int power)
        {
            hp -= power;
            hp = hp <= 0 ? 0 : hp;

            HpText.text = $"hp:{hp}";

            if (hp <= 0)
            {
                Death();
            }

            DamageFlash(destroyCancellationToken).Forget();
        }

        async UniTask DamageFlash(CancellationToken cancellation)
        {
            redFlashImage.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(flashTime), cancellationToken: cancellation);
            redFlashImage.gameObject.SetActive(false);
        }

        int specialCount = 0;
        int specialMaxCount = 3;
        public void AddSpecial()
        {
            specialCount++;
            SpecialCountText.text = $"{specialCount}/{specialMaxCount}";
        }

        private void OnDestroy()
        {
            map["Move"].performed -= MoveInput;
            map["Move"].canceled -= MoveInputZero;
            map["Aim"].performed -= Aim;
            map["Jump"].performed -= playerJumper.StartJump;
            map["Jump"].canceled -= playerJumper.StopJump;
            map["LergeFire"].performed -= LergeFire;
            map["MinimalizeFire"].performed -= MinimalizeFire;
            map?.Dispose();
        }
    }
}