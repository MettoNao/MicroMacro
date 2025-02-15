using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class AttackEffectSetter : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float flashTime;
    public async UniTask OnAttackEffect(CancellationToken cancellation)
    {
        await meshRenderer.material.DOColor(Color.yellow, 0.1f);

        await UniTask.Delay(TimeSpan.FromSeconds(flashTime), cancellationToken: cancellation);

        await meshRenderer.material.DOColor(Color.red, 0.1f).WithCancellation(cancellation);
    }
}
