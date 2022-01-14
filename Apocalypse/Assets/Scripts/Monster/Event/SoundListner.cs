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
            for (int i = 0; i < blackBoard.SoundEventLevel.Count; i++)
            {
                if(blackBoard.SoundEventLevel[i] >= SoundLevel)
                {
                    return (int)TargetID;
                }
            }
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