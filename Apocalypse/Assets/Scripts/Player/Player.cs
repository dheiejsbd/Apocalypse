using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Massage;

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
        public Vector3 MoveInput;

        #region MoveMassage
        public Massage PreMove = new Massage();
        public Massage Jump = new Massage();
        public Massage Crouch = new Massage();
        public Massage Move = new Massage();
        public Massage<bool> Run = new Massage<bool>();
        public Massage<float> CameraRotate = new Massage<float>();
        public Massage<float> CharacterRotate = new Massage<float>();
        #endregion

        public Massage Attack;

    }
}
