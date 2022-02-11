using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Player
{
    public class PlayerInputController : PlayerComponent
    {
        private void Update()
        {
            Movement();
            Weapon();
        }

        #region Movement
        void Movement()
        {
            Player.PreMove.Send();
            Move();
            Jump();
            Run();
            Crouch();
            Rotate();
            Player.Move.Send();
        }
        void Rotate()
        {
            CharacterRotation();
            CameraRotation();
        }

        private void Jump()
        {
            if (!Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Jump))) return;

            Player.TryJump.Send();
        }
        private void Run()
        {
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Run)))
            {
                Player.Run.Send(true);
            }   
            else if (Input.GetKeyUp(KeyCodeMap.instance.TryGetKeyID(KeyID.Run)))
            {
                Player.Run.Send(false);
            }
        }
        private void Crouch()
        {
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Crouch)))
            {
                Player.Crouch.Send();
            }
        }
        private void Move()
        {
            Vector3 input = new Vector3();
            if(Input.GetKey(KeyCodeMap.instance.TryGetKeyID(KeyID.Forward)))
            {
                input += Vector3.forward;
            }
            if(Input.GetKey(KeyCodeMap.instance.TryGetKeyID(KeyID.Backward)))
            {
                input += Vector3.back;
            }
            if (Input.GetKey(KeyCodeMap.instance.TryGetKeyID(KeyID.Rightward)))
            {
                input += Vector3.right;
            }
            if (Input.GetKey(KeyCodeMap.instance.TryGetKeyID(KeyID.Leftward)))
            {
                input += Vector3.left;
            }
            Player.MoveInput = input;
        }

        private void CameraRotation()
        {
            Player.CameraRotate.Send(Input.GetAxis("Mouse Y"));
        }

        private void CharacterRotation()
        {
            Player.CharacterRotate.Send(Input.GetAxis("Mouse X"));
        }
        #endregion

        #region Weapon
        void Weapon()
        {
            ChangeWeapon();
            Reload();
            ChangeFireMode();
            StartAttack();
            EndAttack();
            Attack();
        }

        void ChangeWeapon()
        {
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Weapon1)))
            {
                Player.ChangeWeapon.Send(1);
                Player.EndAttack.Send();
                return;
            }
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Weapon2)))
            {
                Player.ChangeWeapon.Send(2);
                Player.EndAttack.Send();
                return;
            }
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Weapon3)))
            {
                Player.ChangeWeapon.Send(3);
                Player.EndAttack.Send();
                return;
            }
        }
        void Reload()
        {
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Reload)))
            {
                Player.EndAttack.Send();
                Player.Reload.Send();
            }
        }
        void ChangeFireMode()
        {
            if (Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.FireMode)))
            {
                Player.EndAttack.Send();
                Player.FireMode.Send();
            }
        }

        void StartAttack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Player.StartAttack.Send();
            }
        }
        void EndAttack()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Player.EndAttack.Send();
            }
        }
        void Attack()
        {
            if (Input.GetMouseButton(0))
            {
                Player.Attack.Send();
            }
        }




        #endregion
    }
}
