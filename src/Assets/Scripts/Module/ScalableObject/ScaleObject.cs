using UnityEngine;
using DG.Tweening;
using Module.ScalableObject;
using Module.Player;

namespace Module.LevelItem
{
    public class ScaleObject : MonoBehaviour, InterfaceScalable
    {
        [SerializeField] private float maxScale, minScale;
        [SerializeField] private float playerBouncePower = 50;
        private bool isScaleNow;

        private Tweener tweener;
        private Vector3 startLocalScale;
        private float scale;

        private void Start()
        {
            startLocalScale = transform.localScale;
            scale = maxScale;
        }

        public void OnScale(float addSclae)
        {
            if (transform.localScale.magnitude < minScale) return;

            if (addSclae > 0f)
            {
                isScaleNow = true;
            }

            scale += addSclae;

            var toScale = startLocalScale * (scale / maxScale);
            if (toScale.x <= minScale) toScale = Vector3.one * minScale;

            tweener = transform.DOScale(toScale, 0.5f)
                .SetEase(Ease.OutBounce, -0.3f)
                .SetUpdate(true)
                .OnComplete(() => { isScaleNow = false; });
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!isScaleNow) return;
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                playerController.AddJump(-collision.GetContact(0).normal, playerBouncePower);
                isScaleNow = false;
            }
        }

        //4方向に絞る(仮)
        public static Vector3 SnapToClosestDirection(Vector3 input)
        {
            // 定義される方向ベクトル
            Vector3[] directions = new Vector3[]
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

            // 最小距離とその方向を保持
            float minDistance = float.MaxValue;
            Vector3 closestDirection = Vector3.zero;

            // 入力ベクトルを正規化（方向だけを計算するため）
            Vector3 normalizedInput = input.normalized;

            // 各方向ベクトルとの距離を比較
            foreach (Vector3 direction in directions)
            {
                float distance = Vector3.Distance(normalizedInput, direction);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestDirection = direction;
                }
            }

            return closestDirection;
        }
    }
}