using System;
using UnityEngine;

namespace Module.ScalableObject.Enemy
{
    public class PlayerSensor : MonoBehaviour
    {
        [Header("入口かどうか")] 
        [SerializeField] private bool isEntrance;
        
        [SerializeField] private Enemy[] enemies;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isEntrance)
                {
                    SwitchingIsEncount(true);   // 接敵
                    Debug.Log("接敵");
                }
                else
                {
                    SwitchingIsEncount(false);  // 解除 ここいじって戻った時また襲われるようにしてもいいかも
                    Debug.Log("解除");
                }
            }
        }

        private void SwitchingIsEncount(bool value)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.isEncount = value;
            }
        }
    }
}