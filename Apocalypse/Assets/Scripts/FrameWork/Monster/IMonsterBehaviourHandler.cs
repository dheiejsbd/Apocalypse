using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.Monster
{
    public interface IMonsterBehaviourHandler
    {
        void DoChangeState(int ID);
    }
}
