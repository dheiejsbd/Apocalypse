using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Message;

namespace Apocalypse.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        public float hp;
        public float stamina;
        public float hunger;
        public float thirst;
        [HideInInspector] public Vector3 MoveInput;

        #region MoveMessage
        public readonly Message PreMove = new Message();
        public readonly Message TryJump = new Message();
        public readonly Message Crouch = new Message();
        public readonly Message Move = new Message();
        public readonly Message<bool> Run = new Message<bool>();
        public readonly Message<float> CameraRotate = new Message<float>();
        public readonly Message<float> CharacterRotate = new Message<float>();
        #endregion

        #region ActionMessage
        public readonly Message Jump = new Message();
        public readonly Message<bool> IsGround = new Message<bool>();
        #endregion


        #region AnimationKeyFrameEvent
        public readonly Message EnterMagazine = new Message();
        public readonly Message EndReload = new Message();
        #endregion

        #region WeaponMessage
        public readonly Message<int> ChangeWeapon = new Message<int>();
        public readonly Message Reload = new Message();
        public readonly Message FireMode = new Message();
        public readonly Message StartAttack = new Message();
        public readonly Message Attack = new Message();
        public readonly Message EndAttack = new Message();
        public readonly Message Aiming = new Message();
        #endregion


    }
}
