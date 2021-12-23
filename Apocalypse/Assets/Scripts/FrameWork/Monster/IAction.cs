using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork.Monster
{
    public interface IAction
    {
        int ID { get; }
        int Priority { get; }
        void Enter();
        void Update();
        void Exit();
        void Terminate();
    }
}
