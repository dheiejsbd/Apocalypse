using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse
{
    public class Sound : MonoBehaviour
    {
        public SoundEvent SoundEvent;
        public float Range;
        public LayerMask Monster;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            { 
                SoundEvent.Pos = transform.position;
                Collider[] coll = Physics.OverlapSphere(transform.position, Range, Monster);
                for (int i = 0; i < coll.Length; i++)
                {
                    if (coll[i].TryGetComponent(out MonsterController m))
                    {
                        m.GetSound(SoundEvent);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Range);
        }
    }
}
