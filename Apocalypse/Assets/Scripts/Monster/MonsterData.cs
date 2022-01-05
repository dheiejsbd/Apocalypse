using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Stat", fileName = "Stat", order = int.MaxValue)]
    public class MonsterData : ScriptableObject
    {
        public int MaxHp;
        public float MoveSpeed;
        public float RotateSpeed;
        public float AttackDamage;
        public State[] states;
    }
}
