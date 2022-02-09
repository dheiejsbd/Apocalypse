using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Player
{
    public class PlayerAnimationController : PlayerComponent
    {
        Animator Animator;
        private void Start()
        {
            Animator = GetComponent<Animator>();
            AddListener();
        }

        private void AddListener()
        {
            Player.Run.AddListener(Run);
            Player.IsGround.AddListener(IsGround);
            Player.Jump.AddListener(Jump);
        }

        void Run(bool isRun)
        {
            Animator.SetBool("Run", isRun);
        }

        void Jump()
        {
            Animator.SetTrigger("Jump");
        }

        void IsGround(bool isGround)
        {
            Animator.SetBool("IsGround", isGround);
        }
    }
}
