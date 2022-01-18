using UnityEditor;
using UnityEngine;

namespace FrameWork.Monster
{
    public interface IDamageAble
    {
        void TakeHit(int Damage, bool HitMotion);
    }
}