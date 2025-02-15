using System;
using UnityEngine;
public abstract class EnemyAttack : MonoBehaviour
{
    public abstract void Attack(Transform target,Action attackEndEvent);
}