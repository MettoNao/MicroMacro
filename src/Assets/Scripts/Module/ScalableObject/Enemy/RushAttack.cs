using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Module.ScalableObject
{
    public class RushAttack:EnemyAttack
    {
        [SerializeField] private AttackEffectSetter attackEffectSetter;
        [SerializeField] private Rigidbody rigbody;
        [SerializeField] private float rushPower,rushTime;

        private Action attackEndEvent;
        private bool isAttack;
        private Vector3 attackDirection;
        private bool isEncount; 

        public override void Attack(Transform target, Action attackEndEvent, bool isEncount)
        {
            this.isEncount = isEncount; // 不本意
            
            this.attackEndEvent = attackEndEvent;

            attackDirection = (target.transform.position - transform.position).normalized;
          
            OnRushAttack(destroyCancellationToken).Forget();
        }

        private async UniTask OnRushAttack(CancellationToken cancellation)
        {
            if (!isEncount) // 不本意
            {
                attackEndEvent.Invoke();
                return;
            }
               
            //攻撃予備動作
            await attackEffectSetter.OnAttackEffect(destroyCancellationToken);

            //ターゲットに向かって突進
            isAttack = true;

            await UniTask.WaitForSeconds(rushTime, cancellationToken: cancellation);

            isAttack = false;

            rigbody.velocity = Vector3.zero;
            attackEndEvent.Invoke();
        }

        private void FixedUpdate()
        {
            if (isAttack)
            {
                rigbody.velocity = (attackDirection * rushPower);
            }
        }
    }
}