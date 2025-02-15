using R3;
using System;
using UnityEngine;

namespace Module.ScalableObject.Enemy
{
    public class EnemyModel : IDisposable
    {
        private readonly ReactiveProperty<int> scale = new(100);
        public ReactiveProperty<int> Scale => scale;

        private readonly ReactiveProperty<EnemyState> state = new(EnemyState.Idle);
        public ReactiveProperty<EnemyState> State => state;
        public enum EnemyState
        {
            Idle,
            Scalling,
            Attack,
            Minimizing,
            Death
        }

        internal void SetScale(int newScale)
        {
            scale.Value = newScale <= 1 ? 1 : newScale;
        }

        internal void SetState(EnemyState state)
        {
            //死んでたらステートを変更しない
            if (this.state.Value == EnemyState.Death) return;

            this.state.Value = state;
        }

        public void Dispose()
        {
            scale.Dispose();
            state.Dispose();
        }
    }
}