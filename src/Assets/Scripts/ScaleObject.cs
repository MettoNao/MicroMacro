using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleObject : MonoBehaviour
{
    [SerializeField] private float maxScale,minScale;
    private bool isScaleNow;

    public void OnScale(float addSclae)
    {
        isScaleNow = true;
        transform.DOScale(transform.localScale + new Vector3(transform.localScale.x, transform.localScale.y, 0).normalized * addSclae,0.8f)
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
            collision.gameObject.GetComponent<PlayerController>().AddJump(SnapToClosestDirection(dir), 40);
            isScaleNow = false;
        }
    }

    public static Vector3 SnapToClosestDirection(Vector3 input)
    {
        // ��`���������x�N�g��
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        // �ŏ������Ƃ��̕�����ێ�
        float minDistance = float.MaxValue;
        Vector3 closestDirection = Vector3.zero;

        // ���̓x�N�g���𐳋K���i�����������v�Z���邽�߁j
        Vector3 normalizedInput = input.normalized;

        // �e�����x�N�g���Ƃ̋������r
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