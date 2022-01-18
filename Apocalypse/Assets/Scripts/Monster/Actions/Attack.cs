using UnityEditor;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/State/Attack", fileName = "Attack", order = int.MaxValue)]
    public class Attack : State
    {
        [SerializeField] EventBase[] events;
        public override EventBase[] Events => events;
        public override int ID => (int)StateID.Attack;
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
        }

        public override void Enter()
        {
            base.Enter();
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.red;
            blackBoard.AIPath.canMove = false;
        }

        public override void Exit()
        {
            blackBoard.AIPath.canMove = true;
        }

        public override void Terminate()
        {

        }

        public override bool Update()
        {
            if (base.Update()) return true;
            blackBoard.TargetPos = blackBoard.Target.position;
            return false;
        }
    }
}