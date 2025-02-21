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
        [SerializeField] private TextMeshProUGUI SpecialCountText;
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
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            playerMover = new PlayerMover(rb, groundChecker, playerParamater);
            playerJumper = new PlayerJumper(rb, groundChecker, playerParamater);

            InputBind();
        }

        private void InputBind()
        {
            map = playerInput.currentActionMap;
            map["Move"].performed += ctx => moveInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, 0);
            map["Move"].canceled += ctx => moveInput = Vector3.zero;

            map["Aim"].performed += Aim;

            map["Jump"].performed += playerJumper.StartJump;
            map["Jump"].canceled += playerJumper.StopJump;

            map["Fire"].performed += ctx => playerShooter.FireBullet(isBigBullet);

            map["ChangeScale"].performed += ChangeScale;
        }

        void Aim(InputAction.CallbackContext context)
        {
            playerAimer.Aim(context.ReadValue<Vector2>());
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

        private void ChangeScale(InputAction.CallbackContext context)
        {
            isBigBullet = !isBigBullet;
            bulletState.color = isBigBullet ? Color.red : Color.cyan;
            gunColor.material.color = bulletState.color;
            gunColor2.material.color = bulletState.color;
        }

        int hp = 10;
        float flashTime = 0.3f;
        public void Death()
        {
            SceneManager.LoadScene("Main");
        }

        public void Damage(int power)
        {
            hp -= power;
            hp = hp <= 0 ? 0 : hp;

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
            map["Aim"].performed -= Aim;
            map["Jump"].performed -= playerJumper.StartJump;
            map["Jump"].canceled -= playerJumper.StopJump;
            map["ChangeScale"].performed -= ChangeScale;
            map?.Dispose();
        }
    }
}