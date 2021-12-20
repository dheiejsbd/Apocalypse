using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace FrameWork.Page
{
    public interface IPage
    {
        int ID { get; }
        void Initialize(BlackBoard blackBoard);
        void Prepare();
        void Enter();
        void Update();
        void LateUpdate();
        void Exit();
    }
}