using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float scalePower;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ScaleObject"))
        {
            collision.gameObject.GetComponent<ScaleObject>().OnScale(scalePower);
            Destroy(gameObject);
        }
    }
}
