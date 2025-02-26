using System;
using System.Collections;
using DG.Tweening;
using Module.Player;
using R3;
using UnityEngine;

namespace Module.ScalableObject.Enemy
{
    public class Enemy : MonoBehaviour, InterfaceScalable
    {
        [SerializeField][Header("初期サイズ(HP)")] private float startScale;
        [SerializeField][Header("最小サイズ")] private float minimumScale;
        [SerializeField][Header("初期サイズへ戻る時間")] private float revivalTime;
        [SerializeField][Header("移動速度")] private float moveSpeed;
        [SerializeField][Header("最短攻撃間隔")] private float minAttackInterval;
        [SerializeField] [Header("最長攻撃間隔")] private float maxAttackInterval;

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private EnemyAttack enemyAttack;

        private bool _isEncount;
        
        private float revivalTimer, attackTimer;
        private Tweener tweener, deathTweener;

        private Vector3 startLocalScale;

        private EnemyModel enemyModel;
        private readonly IDisposable subscription;
        private GameObject player;

        void Start()
        {
            enemyModel = new EnemyModel();
            enemyModel.SetScale((int)startScale);

            player = GameObject.FindGameObjectWithTag("Player");

            attackTimer = UnityEngine.Random.Range(minAttackInterval, maxAttackInterval);
            revivalTimer = revivalTime;
            startLocalScale = transform.localScale;

            enemyModel.Scale.Subscribe(scale =>
            {
                if (scale <= 1)
                {
                    enemyModel.SetState(EnemyModel.EnemyState.Minimizing);
                }
                else
                {
                    enemyModel.SetState(EnemyModel.EnemyState.Scalling);
                }
            });

            enemyModel.State.Subscribe(state =>
            {
                switch (state)
                {
                    case EnemyModel.EnemyState.Idle:
                        break;

                    case EnemyModel.EnemyState.Scalling:
                        OnScaleAnimation();
                        break;

                    case EnemyModel.EnemyState.Attack:
                        enemyAttack.Attack(player.transform,() => { enemyModel.SetState(EnemyModel.EnemyState.Idle); }, _isEncount);
                        break;

                    case EnemyModel.EnemyState.Minimizing:
                        meshRenderer.material.color = Color.cyan;
                        break;

                    case EnemyModel.EnemyState.Death:
                        Death();
                        break;
                }
            });
        }

        void Update()
        {
            if (enemyModel.State.Value == EnemyModel.EnemyState.Death) return;

            if (enemyModel.State.Value == EnemyModel.EnemyState.Minimizing)
            {
                UpdateRevivalTimer();
            }

            if (enemyModel.State.Value == EnemyModel.EnemyState.Idle) 
            {
                UpdateAttackTimer();
            }
        }

        void UpdateAttackTimer()
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                attackTimer = UnityEngine.Random.Range(minAttackInterval,maxAttackInterval);
                enemyModel.SetState(EnemyModel.EnemyState.Attack);
            }
        }

        void UpdateRevivalTimer()
        {
            revivalTimer -= Time.deltaTime;

            if (revivalTimer <= 0)
            {
                enemyModel.SetState(EnemyModel.EnemyState.Idle);
                meshRenderer.material.color = Color.red;
                OnScale(startScale);
                revivalTimer = revivalTime;
                attackTimer = maxAttackInterval;
            }
        }

        public void OnScale(float addScale)
        {
            enemyModel.SetScale((int)(enemyModel.Scale.Value + addScale));
        }

        private void OnScaleAnimation()
        {
            var toScale = startLocalScale * (enemyModel.Scale.Value / startScale);
            if (toScale.x <= minimumScale) toScale = Vector3.one * minimumScale;

            tweener = transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutBounce)
                .SetUpdate(true)
                .OnComplete(() => { if (enemyModel.State.Value == EnemyModel.EnemyState.Scalling) enemyModel.SetState(EnemyModel.EnemyState.Idle); });
        }

        private void Death()
        {
            tweener?.Kill();

            deathTweener = transform.DOScale(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .SetUpdate(true)
                    .OnComplete(() => Destroy(gameObject));
        }

        private void OnDisable()
        {
            KillTweeners();
        }

        private void KillTweeners()
        {
            if (DOTween.instance != null)
            {
                deathTweener?.Kill();
                tweener?.Kill();
            }
        }

        public bool isEncount
        {
            set {  _isEncount = value; }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (enemyModel.State.Value == EnemyModel.EnemyState.Death) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                //最小化してる場合プレイヤーに触れたら死亡
                if (enemyModel.State.Value == EnemyModel.EnemyState.Minimizing)
                {
                    enemyModel.SetState(EnemyModel.EnemyState.Death);
                }
                //それ以外はプレイヤーにダメージ
                else
                {
                    collision.gameObject.GetComponent<PlayerController>().Damage(1);
                }
            }
        }
    }
}