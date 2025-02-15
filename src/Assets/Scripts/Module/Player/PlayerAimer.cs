using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Player
{
    public class PlayerAimer : MonoBehaviour
    {
        [Header("Aiming Settings")]
        [SerializeField] private Transform aimDirection;

        private readonly JoinGamepedChecker joinGamepedChecker = new JoinGamepedChecker();

        public void Aim(Vector2 aimInput)
        {
            //ゲームパッド時の処理
            if (joinGamepedChecker.CheckJoinGameped())
            {
                if (aimInput.sqrMagnitude > 0.1f)
                {
                    float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
                    aimDirection.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            //キーボードマウス時の処理
            else
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = -Camera.main.transform.position.z;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector2 direction = (mousePosition - aimDirection.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                aimDirection.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}