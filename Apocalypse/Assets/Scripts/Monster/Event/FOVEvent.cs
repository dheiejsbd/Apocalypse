﻿using System;
using System.Collections.Generic;
using FrameWork.Monster;
using UnityEngine;
using UnityEditor;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/FOV", fileName ="FOV", order =int.MaxValue)]
    public class FOVEvent : EventBase
    {
        [SerializeField] StateID TargetID;
        [SerializeField] bool InSide;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            if (blackBoard.FOV == null) return InSide ? -1 : (int)TargetID;
            return InSide ? (int) TargetID : -1;
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {
        }
    }
}
