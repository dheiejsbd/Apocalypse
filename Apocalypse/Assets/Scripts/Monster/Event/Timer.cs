using System;
using System.Collections.Generic;
using FrameWork.Monster;
using UnityEngine;
using UnityEditor;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/Timer", fileName = "Timer", order = int.MaxValue)]
    public class Timer : EventBase
    {
        [SerializeField] float EndTime;
        [SerializeField] StateID TargetID;

        float lastTime;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }
        public override void Enter()
        {
            lastTime = EndTime;
        }
        public override int EventChack()
        {
            lastTime -= Time.deltaTime;
            if (lastTime <= 0) return (int)TargetID;
            return -1;
        }
        public override void Exit()
        {
        }
    }
}
