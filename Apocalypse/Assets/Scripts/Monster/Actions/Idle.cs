using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using FrameWork.Monster;

namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/State/Idle", fileName = "Idle", order = int.MaxValue)]

    public class Idle : State
    {
        [SerializeField] EventBase[] events;
        public override EventBase[] Events => events;
        public override int ID => (int)StateID.Idle;
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
        }

        public override void Enter()
        {
            base.Enter();
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.green;
            blackBoard.TargetPos = blackBoard.owner.transform.position;
            blackBoard.agent.speed = 0;
        }

        public override void Exit()
        {
        }

        public override void Terminate()
        {
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
