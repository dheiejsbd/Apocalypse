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
        public GameObject owner { get => gameObject; }
        public Transform Target { get ; set ; }
        public Vector3 TargetPos { get { return Vector3.zero; } set { TargetPos = value; } }

        public List<int> SoundEventLevel { get; set; }
        public List<Vector3> SoundEventPos { get; set; }


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
            SoundEventLevel = null;
            SoundEventPos = null;
        }
    }
}
