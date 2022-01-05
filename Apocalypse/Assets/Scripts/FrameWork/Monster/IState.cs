using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork.Monster
{
    public interface IState
    {
        int ID { get; }
        bool CanChange { get; set; }
        void Initialize(GameObject owner);
        void Enter();
        void Update();
        void Exit();
        void Terminate();
    }
}
