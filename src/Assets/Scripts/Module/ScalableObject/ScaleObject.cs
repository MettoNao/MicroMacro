using UnityEngine;
using DG.Tweening;
using Module.ScalableObject;
using Module.Player;

namespace Module.LevelItem
{
    public class ScaleObject : MonoBehaviour, InterfaceScalable
    {
        [SerializeField] private float maxScale, minScale;
        private bool isScaleNow;

        public void OnScale(float addSclae)
        {
            if (transform.localScale.magnitude < minScale) return;
            isScaleNow = true;
            transform.DOScale(transform.localScale + new Vector3(transform.localScale.x, transform.localScale.y, 0).normalized * addSclae, 0.8f)
                .SetEase(Ease.OutBounce)
                .SetUpdate(true)
                .OnComplete(() => isScaleNow = false);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!isScaleNow) return;
            if (collision.gameObject.CompareTag("Player"))
            {
                var dir = collision.transform.position - transform.position;
                collision.gameObject.GetComponent<PlayerController>().AddJump(SnapToClosestDirection(dir), 50);
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