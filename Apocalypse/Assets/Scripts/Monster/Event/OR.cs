using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/Gate/OR", fileName = "OR", order = int.MaxValue)]
    public class OR : EventBase
    {
        [SerializeField] StateID TargetID;
        [SerializeField] EventBase[] eventBases;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
            for (int i = 0; i < eventBases.Length; i++)
            {
                eventBases[i].Initialize(blackBoard);
            }
        }

        public override int EventChack()
        {
            for (int i = 0; i < eventBases.Length; i++)
            {
                if (eventBases[i].EventChack() != -1) return (int)TargetID;
            }
            return -1;
        }

        public override void Enter()
        {
            for (int i = 0; i < eventBases.Length; i++)
            {
                eventBases[i].Enter();
            }
        }

        public override void Exit()
        {
            for (int i = 0; i < eventBases.Length; i++)
            {
                eventBases[i].Exit();
            }
        }
    }
}
