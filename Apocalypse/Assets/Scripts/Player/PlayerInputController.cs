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
        }

        #region Movement
        void Movement()
        {
            Move();
            Jump();
            Run();
            Crouch();
            Rotate();
        }
        void Rotate()
        {
            CharacterRotation();
            CameraRotation();
        }

        private void Jump()
        {
            if (!Input.GetKeyDown(KeyCodeMap.instance.TryGetKeyID(KeyID.Jump))) return;

            Player.Jump.Send();
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
            Player.CameraRotate.Send(Input.GetAxis("Mouse X"));
        }
        #endregion
    }
}
