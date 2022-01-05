using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FrameWork.Monster;

namespace Apocalypse
{
    public partial class MonsterController : IBlackBoard
    {
        public int HP { get ; set ; }
        public NavMeshAgent agent { get; private set; }
        public GameObject owner { get => gameObject; }
        public Transform Target { get ; set ; }
        public Vector3 TargetPos { get { return agent.destination; } set { agent.destination = value; } }
    }
}
