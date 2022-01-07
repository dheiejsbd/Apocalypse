using UnityEditor;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/Gate/NOT", fileName = "NOT", order = int.MaxValue)]
    public class Not : EventBase
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