using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/Hit", fileName = "Hit", order = int.MaxValue)]
    public class Hit : EventBase
    {
        [SerializeField] StateID TargetID;
        [SerializeField] EventBase eventBase;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            return eventBase.EventChack() == -1 ? (int)TargetID : -1;
        }

        public override void Enter()
        {
            eventBase.Enter();
        }

        public override void Exit()
        {
            eventBase.Exit();
        }
    }
}
