using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FrameWork.Monster
{
    public abstract class EventBase : ScriptableObject, IEvent
    {
        protected IBlackBoard blackBoard;

        public abstract void Enter();

        public abstract int EventChack();

        public abstract void Exit();

        public virtual void Initialize(IBlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
        }
    }
}
