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
        [SerializeField][Header("攻撃範囲")] private float attackInterval;

        [SerializeField] private MeshRenderer meshRenderer;

        private float revivalTimer, attackTimer;
        private Tweener tweener, deathTweener;

        private Vector3 startLocalScale;

        private EnemyModel enemyModel;
        private readonly IDisposable subscription;
        private GameObject player;
        private Rigidbody rb;

        void Start()
        {
            enemyModel = new EnemyModel();
            enemyModel.SetScale((int)startScale);
            startLocalScale = transform.localScale;
            player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody>();

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
                        meshRenderer.material.color = Color.red;
                        break;

                    case EnemyModel.EnemyState.Scalling:
                        OnScaleAnimation();
                        break;

                    case EnemyModel.EnemyState.Attack:
                        StartCoroutine(Attack());
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
                revivalTimer += Time.deltaTime;
            }

            attackTimer += Time.deltaTime;

            if (revivalTimer >= revivalTime)
            {
                enemyModel.SetState(EnemyModel.EnemyState.Idle);
                OnScale(startScale);
                revivalTimer = 0;
                attackTimer = 0;
            }

            if (attackTimer >= attackInterval && enemyModel.State.Value == EnemyModel.EnemyState.Idle)
            {
                enemyModel.SetState(EnemyModel.EnemyState.Attack);
                attackTimer = 0;
            }
        }

        IEnumerator Attack()
        {
            meshRenderer.material.color = Color.yellow;
            yield return new WaitForSeconds(0.3f);
            meshRenderer.material.color = Color.red;
            rb.velocity = (player.transform.position - transform.position).normalized * moveSpeed;
            yield return new WaitForSeconds(0.3f);
            rb.velocity = Vector3.zero;
            enemyModel.SetState(EnemyModel.EnemyState.Idle);
        }

        public void OnScale(float addScale)
        {
            enemyModel.SetScale((int)(enemyModel.Scale.Value + addScale));
        }

        private void OnScaleAnimation()
        {
            var toScale = startLocalScale * (enemyModel.Scale.Value / startScale);
            if (toScale.magnitude <= minimumScale) toScale = Vector3.one * minimumScale;

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
                    collision.gameObject.GetComponent<PlayerController>().Death();
                }
            }
        }
    }
}