using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/Distance", fileName = "Distance", order = int.MaxValue)]
    public class Distance : EventBase
    {
        [SerializeField] StateID TargetID;
        [SerializeField] float Dist = 0.2f;
        [SerializeField] bool In;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            if (blackBoard.Seeker.GetCurrentPath().GetTotalLength() < Dist && !blackBoard.AIPath.pathPending) return In ? (int)TargetID : -1;
            return In ? -1 : (int)TargetID;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}
