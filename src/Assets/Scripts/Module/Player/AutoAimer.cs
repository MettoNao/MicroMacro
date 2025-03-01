using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.ScalableObject
{
    public class AutoAimer : MonoBehaviour
    {
        List<GameObject> scalableObjects = new List<GameObject>();
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("ScaleObject"))
            {
                if (!scalableObjects.Any(s => s == other.gameObject))
                {
                    scalableObjects.Add(other.gameObject);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("ScaleObject"))
            {
                if (scalableObjects.Any(s => s == other.gameObject))
                {
                    scalableObjects.Remove(other.gameObject);
                }
            }
        }

        public GameObject GetNearestScalableObject()
        {
            GameObject nearestObj = null;
            float minDistance = float.MaxValue;

            foreach (var scalableObject in scalableObjects)
            {
                if (scalableObject == null) continue;

                float distanceSqr = (scalableObject.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < minDistance)
                {
                    minDistance = distanceSqr;
                    nearestObj = scalableObject;
                }
            }

            return nearestObj;
        }
    }
}