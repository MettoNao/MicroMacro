using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Module.Player
{
    public class PlayerShooter : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject miniBulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 15f;

        public void FireBullet(bool isBigBullet)
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