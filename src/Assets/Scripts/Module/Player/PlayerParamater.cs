using System;
using UnityEngine;

namespace Module.Player
{
    [Serializable]
    public class PlayerParamater
    {
        [Header("Movement Settings")]
        [Header("最大速度"), SerializeField] internal float maxSpeed = 10f;
        [Header("加速度"), SerializeField] internal float acceleration = 10f;
        [Header("速度減衰"), SerializeField] internal float stopForce = 5f;
        [Header("Jump Settings")]
        [Header("ジャンプ力"), SerializeField] internal float jumpForce = 10f;
        [Header("長押しジャンプ力"), SerializeField] internal float jumpHoldForce = 5f;
        [Header("ジャンプ時間"), SerializeField] internal float jumpDuration = 0.2f;
        [Header("長押しジャンプ時間"), SerializeField] internal float jumpHoldDuration = 0.2f;
        [Header("落下時の重力"), SerializeField] internal float fallMultiplier = 2.5f;
        [Header("長押し時の重力"), SerializeField] internal float fallHoldMultiplier = 2.5f;
    }
}