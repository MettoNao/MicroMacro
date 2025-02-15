using DG.Tweening;
using Module.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.ScalableObject
{
    public class EnemyBullet : MonoBehaviour,InterfaceScalable
    {
        [SerializeField] private int attackPower;
        [SerializeField] private float startScale;

        private Vector3 startLocalScale;
        private float scale;
        private Tweener tweener;
        // Start is called before the first frame update
        void Start()
        {
            startLocalScale = transform.localScale;
            scale = startScale;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Bullet")) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().Damage(attackPower);
            }

            Destroy(gameObject);
        }

        public void OnScale(float addSclae)
        {
            scale += addSclae;

            if (scale <= 0)
            {
                tweener.Kill();
                Destroy(gameObject);
                return;
            }

            var toScale = startLocalScale * (scale / startScale);

            tweener = transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutBounce)
                .SetUpdate(true);
        }
    }
}