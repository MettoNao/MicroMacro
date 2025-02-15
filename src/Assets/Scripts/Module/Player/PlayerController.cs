using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Module.Utile;

namespace Module.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerParamater playerParamater;
        [SerializeField] private Image bulletState;
        [SerializeField] private TextMeshProUGUI SpecialCountText;

        [SerializeField] private PlayerAimer playerAimer;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private GroundChecker groundChecker;

        private PlayerMover playerMover;
        private PlayerJumper playerJumper;

        private Rigidbody rb;
        private Vector3 moveInput;
        private PlayerInput playerInput;

        private bool isBigBullet = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            playerMover = new PlayerMover(rb, playerParamater);
            playerJumper = new PlayerJumper(rb, groundChecker, playerParamater);

            InputBind();
        }

        private void InputBind()
        {
            playerInput.actions["Move"].performed += ctx => moveInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, 0);
            playerInput.actions["Move"].canceled += ctx => moveInput = Vector3.zero;

            playerInput.actions["Aim"].performed += ctx => playerAimer.Aim(ctx.ReadValue<Vector2>());

            playerInput.actions["Jump"].performed += ctx => playerJumper.StartJump();
            playerInput.actions["Jump"].canceled += ctx => playerJumper.StopJump();

            playerInput.actions["Fire"].performed += ctx => playerShooter.FireBullet(isBigBullet);

            playerInput.actions["ChangeScale"].performed += ctx => ChangeScale();
        }

        private void Update()
        {
            if (transform.position.y <= -25)
            {
                Death();
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

        private void ChangeScale()
        {
            isBigBullet = !isBigBullet;
            bulletState.color = isBigBullet ? Color.red : Color.cyan;
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
    }
}