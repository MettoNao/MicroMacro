using System.Collections;
using System.Collections.Generic;
using Module.Player;
using UnityEngine;

namespace Module.LevelItem
{
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().Death();
            }
        }
    }
}