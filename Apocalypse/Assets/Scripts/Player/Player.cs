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

        #region MoveMassage
        public readonly Message PreMove = new Message();
        public readonly Message TryJump = new Message();
        public readonly Message Crouch = new Message();
        public readonly Message Move = new Message();
        public readonly Message<bool> Run = new Message<bool>();
        public readonly Message<float> CameraRotate = new Message<float>();
        public readonly Message<float> CharacterRotate = new Message<float>();
        #endregion

        #region AnimationMassage
        public readonly Message Jump = new Message();
        public readonly Message<bool> IsGround = new Message<bool>();
        #endregion

        public Message Attack = new Message();

    }
}
