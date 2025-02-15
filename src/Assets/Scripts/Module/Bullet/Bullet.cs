using System.Collections;
using System.Collections.Generic;
using Module.ScalableObject;
using UnityEngine;

namespace Module.Bullet
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float scalePower;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<InterfaceScalable>(out InterfaceScalable interfaceScalable))
            {
                interfaceScalable.OnScale(scalePower);
            }

            Destroy(gameObject);
        }
    }
}