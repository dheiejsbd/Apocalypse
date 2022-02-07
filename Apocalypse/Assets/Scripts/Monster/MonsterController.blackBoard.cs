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
        public bool TargetLost { get { return Target == null; }}
        public Vector3 TargetPos { get { return AIPath.target.position; } set { AIPath.target.position = value; } }

        public List<SoundEvent> SoundEvents { get; set; }
        public SoundEvent SoundEvent { get; set; }

        public Transform FOV => FrameWork.Math.FOV.GetTarget(transform.position, transform.forward, data.TargetLayerMask, data.Dist, data.Angle,data.Offset, data.RaycastLayerMask);

        public Seeker Seeker { get; set; }
        public AIPath AIPath { get; set; }


        public void ResetEvent()
        {
            ResetSound();
            ResetHit();
        }
        public void GetSound(SoundEvent sound)
        {
            SoundEvents.Add(sound);
        }
        private void ResetSound()
        {
            SoundEvents = new List<Apocalypse.SoundEvent>();
        }

        private void ResetHit()
        {
            Hit = false;
        }
    }
}
