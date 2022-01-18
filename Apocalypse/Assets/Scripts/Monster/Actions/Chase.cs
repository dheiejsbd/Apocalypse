using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/State/Chase", fileName = "Chase", order = int.MaxValue)]

    public class Chase : State
    {
        [SerializeField] EventBase[] events;
        public override EventBase[] Events => events;

        [SerializeField] float Angle;
        [SerializeField] float Dist;
        [SerializeField] LayerMask TargetLayerMask;
        [SerializeField] LayerMask RaycastLayerMask;



        public override int ID => (int)StateID.Chase;

        public override void Enter()
        {
            base.Enter();
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        public override bool Update()
        {
            if (base.Update()) return true;
            Transform Target = blackBoard.FOV;
            if(Target != null) blackBoard.Target = Target;
            if(blackBoard.Target != null) blackBoard.TargetPos = blackBoard.Target.position;
            return false;
        }
        public override void Exit()
        {

        }

        public override void Terminate()
        {

        }
    }
}
