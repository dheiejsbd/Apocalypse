using System;
using System.Collections.Generic;
using FrameWork.Monster;

namespace Apocalypse
{
    public partial class MonsterController : IMonsterBehaviourHandler
    {
        public void DoChangeState(int ID)
        {
            StateMachine.TryChangeState(ID);
        }
    }
}
