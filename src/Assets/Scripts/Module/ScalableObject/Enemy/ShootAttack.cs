using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.ScalableObject
{
    public class ShootAttack : EnemyAttack
    {
        [SerializeField] private float bulletSpeed;
        [SerializeField] private EnemyBullet bulletPrefab;
        [SerializeField] private Transform firePoint;

        public override void Attack(Transform target, Action attackEndEvent, bool isEncount)
        {
            if (!isEncount) // 接敵してなければ攻撃なし
            {
                attackEndEvent.Invoke();
                return;
            }
            
            if (bulletPrefab != null && firePoint != null)
            {
                var attackDirection = (target.transform.position - transform.position).normalized;
                GameObject bullet = Instantiate(bulletPrefab.gameObject, firePoint.position,firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = attackDirection * bulletSpeed;
                }

                attackEndEvent.Invoke();
            }
        }
    }
}