using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
namespace FrameWork.Monster
{
    public interface IBlackBoard
    {
        int HP { get; set; }
        bool Hit { get; set; }
        GameObject owner { get; }
        List<int> SoundEventLevel { get; set; }
        List<Vector3> SoundEventPos { get; set; }
        Transform Target { get; set; }
        Vector3 TargetPos { get ; set; }
        Transform FOV { get; }
        AIPath ai { get; set; }
    }
}
