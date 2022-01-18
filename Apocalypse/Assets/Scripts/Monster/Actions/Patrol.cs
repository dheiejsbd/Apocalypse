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

        Transform Path;
        int PathID;
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            Path = PathManager.GetPath(blackBoard.owner.transform.position);
            PathID = 0;
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

        public override bool Update()
        {
            if(base.Update()) return true;

            if(blackBoard.Seeker.GetCurrentPath().GetTotalLength() < 2f && !blackBoard.AIPath.pathPending)
            {
                PathID++;
                PathID %= Path.childCount;
                blackBoard.TargetPos = Path.GetChild(PathID).position;
                blackBoard.AIPath.SearchPath();
            }


            return false;
        }
    }
}
