using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Module.Player
{
    public class PlayerShooter : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject miniBulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float fireRate = 0.5f;

        private float timer = 0;
        private bool isTrigger;
        private bool isBigBullet;

        private void Update()
        {
            if (timer <= 0 && isTrigger)
            {
                Fire();
                timer = fireRate;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        public void FireBullet(bool isBigBullet)
        {
            isTrigger = true;
            this.isBigBullet = isBigBullet;
        }

        public void CancelFire(InputAction.CallbackContext context)
        {
            isTrigger = false;
        }

        private void Fire()
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
    }
}