using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FrameWork.Monster;
using Pathfinding;

namespace Apocalypse
{
    public partial class MonsterController : IBlackBoard
    {
        public int HP { get ; set ; }
        public bool Hit { get; set; }
        public GameObject owner => gameObject;
        public Transform Target { get ; set ; }
        public Vector3 TargetPos { get { return ai.target.position; } set { ai.target.position = value; } }

        public List<int> SoundEventLevel { get; set; }
        public List<Vector3> SoundEventPos { get; set; }

        public Transform FOV => FrameWork.Math.FOV.GetTarget(transform.position, transform.forward, data.TargetLayerMask, data.Dist, data.Angle,data.Offset, data.RaycastLayerMask);

        public AIPath ai { get; set; }


        public void ResetEvent()
        {
            ResetSound();
            ResetHit();
        }
        public void GetSound(SoundEvent sound)
        {
            if(SoundEventLevel == null)
            {
                SoundEventLevel = new List<int>();
                SoundEventPos = new List<Vector3>();
            }
            SoundEventLevel.Add(sound.Level);
            SoundEventPos.Add(sound.Pos);
        }
        private void ResetSound()
        {
            SoundEventLevel = new List<int>();
            SoundEventPos = new List<Vector3>();
        }

        private void ResetHit()
        {
            Hit = false;
        }
    }
}
