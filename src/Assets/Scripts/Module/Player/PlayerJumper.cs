using System.Collections;
using System.Collections.Generic;
using Module.Utile;
using UnityEngine;

namespace Module.Player
{
    public class PlayerJumper
    {
        private Rigidbody rb;
        private float jumpTimeCounter = 0f;
        private bool isHoldingJump = false;

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
            //着地時にタイマーリセット
            if (groundChecker.CheckGroundedByTag())
            {
                jumpTimeCounter = 0;
                rb.AddForce(new Vector3(0, -rb.velocity.y, 0));
                return;
            }
            //ジャンプ時にタイマー計測
            else
            {
                jumpTimeCounter += Time.fixedDeltaTime;
            }

            //落下時の重力
            if (!isHoldingJump && jumpTimeCounter >= playerParamater.jumpDuration)
            {
                rb.AddForce(Vector3.down * playerParamater.fallMultiplier, ForceMode.Impulse);
                return;
            }

            //長押しジャンプ時の追加ジャンプ
            if (isHoldingJump && jumpTimeCounter <= playerParamater.jumpHoldDuration && jumpTimeCounter >= playerParamater.jumpDuration)
            {
                rb.AddForce(Vector3.up * playerParamater.jumpHoldForce, ForceMode.Impulse);
                return;
            }

            //長押しジャンプ後の重力
            if (jumpTimeCounter >= playerParamater.jumpHoldDuration)
            {
                rb.AddForce(Vector3.down * playerParamater.fallHoldMultiplier, ForceMode.Impulse);
            }
        }

        public void StartJump()
        {
            if (groundChecker.CheckGroundedByTag())
            {
                rb.AddForce(Vector3.up * playerParamater.jumpForce, ForceMode.Impulse);
                isHoldingJump = true;
                jumpTimeCounter = 0;
            }
        }

        public void StopJump()
        {
            isHoldingJump = false;
        }

        //スケールオブジェクトの反動で受ける追加ジャンプ
        public void AddJump(Vector3 dir, float power)
        {
            rb.AddForce(dir * power, ForceMode.Impulse);
            isHoldingJump = false;
        }
    }
}