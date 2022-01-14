using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using FrameWork.Monster;

namespace Apocalypse
{
    [System.Serializable]
    public abstract class State : ScriptableObject, IState
    {
        protected IBlackBoard blackBoard;
        protected IMonsterBehaviourHandler behaviourHandler;
        public abstract EventBase[] Events { get; }
        public abstract int ID { get; }
        public bool CanChange { get; set; }
        public virtual void Initialize(GameObject owner)
        {
            blackBoard = owner.GetComponent<IBlackBoard>();
            behaviourHandler = owner.GetComponent<IMonsterBehaviourHandler>();
            for (int i = 0; i < Events.Length; i++)
            {
                Events[i].Initialize(blackBoard);
            }
        }
        public virtual void Enter()
        {
            for (int i = 0; i < Events.Length; i++)
            {
                Events[i].Enter();
            }
        }
        public virtual bool Update()
        {
            int Target = 0;
            for (int i = 0; i < Events.Length; i++)
            {
                Target = Events[i].EventChack();
                if (Target != -1)
                {
                    behaviourHandler.DoChangeState(Target);
                    return true;
                }
            }
            return false;
        }
        public abstract void Exit();
        public abstract void Terminate();
    }
}