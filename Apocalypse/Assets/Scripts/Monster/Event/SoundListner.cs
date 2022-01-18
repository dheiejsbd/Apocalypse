using UnityEditor;
using UnityEngine;
using FrameWork.Monster;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/SoundListner", fileName ="SoundListner", order =int.MaxValue)]
    public class SoundListner : EventBase
    {
        [SerializeField] StateID TargetID;
        [SerializeField] int SoundLevel;
        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            if (blackBoard.SoundEvents.Count == 0) return -1;

            float dist = float.MaxValue;
            for (int i = 0; i < blackBoard.SoundEvents.Count; i++)
            {
                if (blackBoard.SoundEvents[i].Level >= SoundLevel)
                {
                    if (dist > Vector3.Distance(blackBoard.owner.transform.position, blackBoard.SoundEvents[i].Pos))
                    {
                        dist = Vector3.Distance(blackBoard.owner.transform.position, blackBoard.SoundEvents[i].Pos);
                        blackBoard.SoundEvent = blackBoard.SoundEvents[i];
                    }
                }
            }

            return dist == float.MaxValue ? -1 : (int)TargetID;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}