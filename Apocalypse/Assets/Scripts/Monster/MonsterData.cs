using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Stat", fileName = "Stat", order = int.MaxValue)]
    public class MonsterData : ScriptableObject
    {
        public int MaxHp;
        public float MoveSpeed;
        public float RotateSpeed;
        public float AttackDamage;

        [Header("FOV")]
        [Range(0, 360)]
        public float Angle;
        public float Dist;
        public float Offset;
        public LayerMask TargetLayerMask;
        public LayerMask RaycastLayerMask;

        public State[] states;
    }
}
