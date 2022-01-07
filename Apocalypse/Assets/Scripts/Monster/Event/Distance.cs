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
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            return -1;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}
