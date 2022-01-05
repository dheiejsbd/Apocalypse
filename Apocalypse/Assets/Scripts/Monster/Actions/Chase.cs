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
            blackBoard.agent.speed = 1;
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public override void Update()
        {
            base.Update();
            blackBoard.Target = FrameWork.Math.FOV.GetTarget(blackBoard.owner.transform.position, blackBoard.owner.transform.forward, TargetLayerMask, Dist, Angle, RaycastLayerMask);
            if(blackBoard.Target) blackBoard.TargetPos = blackBoard.Target.position;
        }
        public override void Exit()
        {

        }

        public override void Terminate()
        {

        }
    }
}
