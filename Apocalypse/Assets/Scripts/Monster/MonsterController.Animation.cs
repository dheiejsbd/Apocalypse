using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Apocalypse
{
    public partial class MonsterController
    {
        public void SetMoveSpeed(float speed)
        {
            animator.SetFloat("MoveSpeed", speed);
        }

        public void TryPlayAnim(string name)
        {
            animator.Play(name);
        }
    }
}
