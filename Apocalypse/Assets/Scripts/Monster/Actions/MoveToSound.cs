using UnityEditor;
using UnityEngine;
using FrameWork.Monster;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/State/MoveToSound", fileName = "MoveToSound", order = int.MaxValue)]
    public class MoveToSound : State
    {
        public override int ID => (int)StateID.SoundChack;
        [SerializeField] EventBase[] events;
        public override EventBase[] Events => events;
        [SerializeField] int SoundLevel;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
        }
        public override void Enter()
        {
            base.Enter();
            blackBoard.TargetPos = blackBoard.SoundEvent.Pos;
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.gray;
        }

        public override void Exit()
        {
        }

        public override void Terminate()
        {
        }

        public override bool Update()
        {
            if (base.Update()) return true;


            return false;
        }
    }
}