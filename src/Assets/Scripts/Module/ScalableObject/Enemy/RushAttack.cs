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

        public override void Attack(Transform target, Action attackEndEvent)
        {
            this.attackEndEvent = attackEndEvent;

            attackDirection = (target.transform.position - transform.position).normalized;
            OnRushAttack(destroyCancellationToken).Forget();
        }

        private async UniTask OnRushAttack(CancellationToken cancellation)
        {
            //�U���\������
            await attackEffectSetter.OnAttackEffect(destroyCancellationToken);

            //�^�[�Q�b�g�Ɍ������ēːi
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