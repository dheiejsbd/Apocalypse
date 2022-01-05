using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;
namespace Apocalypse
{
    [CreateAssetMenu(menuName = "ScriptableObject/Monster/State/Patrol", fileName = "Patrol", order = int.MaxValue)]
    public class Patrol : State
    {
        public override int ID => (int)StateID.Patrol;
        [SerializeField] EventBase[] events;
        public override EventBase[] Events => events;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
        }
        public override void Enter()
        {
            base.Enter();
            blackBoard.owner.GetComponent<MeshRenderer>().material.color = Color.yellow;
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
