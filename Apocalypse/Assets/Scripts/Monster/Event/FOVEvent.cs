using System;
using System.Collections.Generic;
using FrameWork.Monster;
using UnityEngine;
using UnityEditor;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/Event/FOV", fileName ="FOV", order =int.MaxValue)]
    public class FOVEvent : EventBase
    {
        [SerializeField] StateID TargetID;

        [Space]

        [Range(0,360)]
        public float Angle;
        public float Dist;
        [SerializeField] bool InSide;

        [SerializeField] LayerMask TargetLayerMask;
        [SerializeField] LayerMask RaycastLayerMask;

        public override void Initialize(IBlackBoard blackBoard)
        {
            base.Initialize(blackBoard);
        }

        public override int EventChack()
        {
            Vector3 Target = FrameWork.Math.FOV.GetTargetVector(blackBoard.owner.transform.position, blackBoard.owner.transform.forward, TargetLayerMask, Dist, Angle, RaycastLayerMask);

            if (Target == Vector3.zero) return InSide ? -1 : (int)TargetID;
            return InSide ? (int) TargetID : -1;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}
