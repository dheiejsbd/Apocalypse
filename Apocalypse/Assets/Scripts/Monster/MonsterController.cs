using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    [RequireComponent(typeof(Pathfinding.AIPath))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public partial class MonsterController : MonoBehaviour
    {
        public void Start()
        {
            StateMachine = new StateMachine();
            StateMachine.Initialize(owner);
            Seeker = GetComponent<Pathfinding.Seeker>();
            AIPath = GetComponent<Pathfinding.AIPath>();
            animator = GetComponent<Animator>();
            AIPath.target = new GameObject(gameObject.name + " Target").transform;
            SoundEvents = new List<SoundEvent>();
            for (int i = 0; i < data.states.Length; i++)
            {
                data.states[i].Initialize(owner);
                StateMachine.TryAddState(data.states[i]);
            }
        }

        public void Update()
        {
            StateMachine.Update();
            ResetSound();
            UpdateTarget();
        }

        void UpdateTarget()
        {
            if (TargetLost)
            {
                Target = FOV;
            }
            
            if(!TargetLost)
            {
                Vector3 Targetdir = (Target.position - transform.position).normalized;

                if (!Physics.Raycast(transform.position + Vector3.up * data.Offset, Targetdir, data.Dist, data.RaycastLayerMask))
                {
                    Target = null;
                }
            }
        }

        public void OnLateUpdate()
        {

        }
        public void Terminate()
        {
            Destroy(AIPath.target.gameObject);
        }
    }
}
