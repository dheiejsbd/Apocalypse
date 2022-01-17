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
            ai = GetComponent<Pathfinding.AIPath>();
            ai.target = new GameObject(gameObject.name + " Target").transform;
            SoundEventLevel = new List<int>();
            SoundEventPos = new List<Vector3>();
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
            Destroy(ai.target.gameObject);
        }
    }
}
