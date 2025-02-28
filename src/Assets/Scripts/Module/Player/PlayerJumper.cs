using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Utile;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Module.Player
{
    public class PlayerJumper
    {
        private Rigidbody rb;
        private float jumpTimeCounter = 0f;
        private bool isHoldingJump = false;
        private bool isAddingJump = false;

        private GroundChecker groundChecker;
        private PlayerParamater playerParamater;

        public PlayerJumper(Rigidbody rb, GroundChecker groundChecker, PlayerParamater playerParamater)
        {
            this.rb = rb;
            this.groundChecker = groundChecker;
            this.playerParamater = playerParamater;
        }

        public void Jump()
        {
            Debug.Log($"isAddingJump{isAddingJump}");

            //着地時にタイマーリセット
            if (groundChecker.CheckGroundedByTag())
            {
                if (isHoldingJump)  // ジャンプボタン押したままならリセットしない
                    return;

                jumpTimeCounter = 0;
                rb.AddForce(new Vector3(0, -rb.velocity.y, 0));
                isAddingJump = false;
                return;
            }
            //ジャンプ時にタイマー計測
            else
            {
                jumpTimeCounter += Time.fixedDeltaTime;
            }

            //長押しジャンプ時の追加ジャンプ
            if (isHoldingJump && jumpTimeCounter <= playerParamater.jumpHoldDuration && jumpTimeCounter >= playerParamater.jumpDuration)
            {
                rb.AddForce(Vector3.up * playerParamater.jumpHoldForce, ForceMode.Impulse);
            }

            Gravity();
        }

        private void Gravity()
        {
            if (!groundChecker.CheckGroundedByTag())
            {
                //追加ジャンプ時の重力
                if (isAddingJump)
                {
                    rb.AddForce(Vector3.down * playerParamater.addingJumpMultiplier, ForceMode.Impulse);
                    return;
                }

                //長押しジャンプ後の重力
                if (jumpTimeCounter >= playerParamater.jumpHoldDuration)
                {
                    rb.AddForce(Vector3.down * playerParamater.fallHoldMultiplier, ForceMode.Impulse);
                    return;
                }

                //落下時の重力
                if (!isHoldingJump)
                {
                    rb.AddForce(Vector3.down * playerParamater.fallMultiplier, ForceMode.Impulse);
                }
            }
        }

        public void StartJump(InputAction.CallbackContext context)
        {
            if (groundChecker.CheckGroundedByTag())
            {
                rb.AddForce(Vector3.up * playerParamater.jumpForce, ForceMode.Impulse);
                isHoldingJump = true;
                jumpTimeCounter = 0;
            }
        }

        public void StopJump(InputAction.CallbackContext context)
        {
            isHoldingJump = false;
        }

        //スケールオブジェクトの反動で受ける追加ジャンプ
        public void AddJump(Vector3 dir, float power)
        {
            rb.AddForce(dir * power, ForceMode.Impulse);
            isHoldingJump = false;
            SetIsAddingJumpFlg(new CancellationTokenSource().Token).Forget();
        }

        private async UniTask SetIsAddingJumpFlg(CancellationToken cancellation)
        {
            await UniTask.WaitForSeconds(0.1f);
            isAddingJump = true;
        }
    }
}