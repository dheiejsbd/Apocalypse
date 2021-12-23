using UnityEditor;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    public abstract class Action : IAction
    {
        public abstract int ID { get; }
        private int priority;
        public int Priority => priority;
        MonsterBlackBoard blackBoard;

        public virtual void Initialize(MonsterBlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
        }
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
        public abstract void Terminate();
    }
}