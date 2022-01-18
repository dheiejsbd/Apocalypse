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
        GameObject owner { get; }
        
        int HP { get; set; }

        bool Hit { get; set; }
        
        Seeker Seeker { get; set; }
        AIPath AIPath { get; set; }
        
        List<Apocalypse.SoundEvent> SoundEvents { get; set; }
        Apocalypse.SoundEvent SoundEvent { get; set; }

        Transform Target { get; set; }
        Vector3 TargetPos { get ; set; }
        
        Transform FOV { get; }
    }
}
