using UnityEngine;
using Module.Utile;

namespace Module.Player
{
    public class PlayerMover
    {
        private Rigidbody rb;
        private PlayerParamater playerParamater;
        private GroundChecker groundChecker;

        public PlayerMover(Rigidbody rb, GroundChecker groundChecker, PlayerParamater playerParamater)
        {
            this.rb = rb;
            this.groundChecker = groundChecker;
            this.playerParamater = playerParamater;
        }

        public void Move(Vector3 moveInput)
        {
            if (moveInput != Vector3.zero)
            {
                if (Mathf.Abs(rb.velocity.x) < playerParamater.maxSpeed)
                {
                    if (groundChecker.CheckGroundedByTag())
                    {
                        rb.AddForce(new Vector3(moveInput.x * playerParamater.acceleration, 0, 0), ForceMode.Force);
                    }
                    else
                    {
                        // 空中では加速を弱める
                        rb.AddForce(new Vector3(moveInput.x * playerParamater.airAcceleration, 0, 0), ForceMode.Force);
                    }
                }
            }
            else
            {
                // 空中でスティック離したとき慣性乗せる
                if (!groundChecker.CheckGroundedByTag())    
                    return;
                
                Vector3 velocity = rb.velocity;
                if (Mathf.Abs(velocity.x) > 0.1f)
                {
                     rb.AddForce(new Vector3(-velocity.x * playerParamater.stopForce, 0, 0), ForceMode.Force);
                }
            }
        }
    }
}