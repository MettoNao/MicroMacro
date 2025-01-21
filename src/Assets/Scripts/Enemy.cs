using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxScale;
    private bool isScaleNow;

    public void OnScale(float addSclae)
    {
        int value = addSclae >= 0 ? 1 : -1;

        maxScale += value;

        if (maxScale <= 0)
        {
            Destroy(gameObject);
        }

        isScaleNow = true;
        transform.DOScale(transform.localScale + new Vector3(transform.localScale.x, transform.localScale.y, 0).normalized * value * 2, 0.8f)
            .SetEase(Ease.OutBounce)
            .SetUpdate(true)
            .OnComplete(() => isScaleNow = false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Death();
        }
    }
}
