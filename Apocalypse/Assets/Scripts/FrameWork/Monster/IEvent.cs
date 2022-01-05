using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Monster
{
    public interface IEvent
    {
        void Initialize(IBlackBoard blackBoard);
        void Enter();
        int EventChack();
        void Exit();
    }
}