using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
namespace FrameWork.Monster
{
    public interface IBlackBoard
    {
        int HP { get; set; }
        GameObject owner { get; }
        NavMeshAgent agent { get; }
        Transform Target { get; set; }
        Vector3 TargetPos { get ; set; }
    }
}
