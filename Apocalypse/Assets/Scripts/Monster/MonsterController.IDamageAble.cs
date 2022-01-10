using UnityEditor;
using UnityEngine;
using FrameWork.Monster;

namespace Apocalypse
{
    public partial class MonsterController : IDamageAble
    {
        public void TakeHit(int Damage)
        {
            HP -= Damage;
        }
    }
}