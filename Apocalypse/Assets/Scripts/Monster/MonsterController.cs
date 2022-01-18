using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FrameWork.Monster;

namespace Apocalypse
{
    public partial class MonsterController : MonoBehaviour
    {
        public void Start()
        {
            StateMachine = new StateMachine();
            StateMachine.Initialize(owner);
            Seeker = GetComponent<Pathfinding.Seeker>();
            AIPath = GetComponent<Pathfinding.AIPath>();
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
