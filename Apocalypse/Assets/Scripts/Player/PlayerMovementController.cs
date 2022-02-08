using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Apocalypse.Player
{
    class PlayerMovementController : PlayerComponent
    {
        Vector3 velocity = Vector3.zero;

        float velocityY;
        [SerializeField] float gravity;
        [SerializeField] float velocityShift;

        [Space]
        [Header("MoveSpeed")]
        [SerializeField] private float walkMoveSpeed;
        [SerializeField] private float runMoveSpeed;
        [SerializeField] private float crouchMoveSpeed;

        private float applySpeed;

        [Space]
        [Header("Jump")]
        [SerializeField] private float jumpForce;
        [SerializeField] LayerMask GroundLayerMask;

        // 상태 변수
        private bool isRun = false;
        private bool isCrouch = false;
        private bool isGround = true;

        [Space]
        [Header("Crouch")]
        // 앉았을 때 얼마나 앉을지 결정하는 변수
        [SerializeField] float crouchSpeed;
        [SerializeField] private float crouchPosY;
        private float originPosY;
        private float applyCrouchPosY;

        // 민감도
        [SerializeField]
        private float lookSensitivity;

        // 카메라 한계
        [SerializeField]
        private float cameraRotationLimit;
        private float currentCameraRotationX = 0;

        // 필요한 컴포넌트
        [SerializeField]
        private Camera theCamera;
        private CharacterController characterController;

        void Start()
        {
            characterController = GetComponent<CharacterController>();

            //초기화
            applySpeed = walkMoveSpeed;
            originPosY = theCamera.transform.localPosition.y;
            applyCrouchPosY = originPosY;

            Player.PreMove.AddListner(Gravity);
            Player.PreMove.AddListner(IsGround);

            Player.Jump.AddListner(TryJump);
            Player.Crouch.AddListner(Crouch);
            Player.Move.AddListner(Move);
        }
        void Gravity()
        {
            velocityY += gravity * Time.deltaTime;
        }
        
        //앉기 동작
        private void Crouch()
        {
            isCrouch = !isCrouch;

            if (isCrouch)
            {
                applySpeed = crouchMoveSpeed;
                applyCrouchPosY = crouchPosY;
            }
            else
            {
                applySpeed = walkMoveSpeed;
                applyCrouchPosY = originPosY;
            }
            StartCoroutine(CrouchCoroutine());
        }

        // 부드러운 동작 실행
        IEnumerator CrouchCoroutine()
        {
            float _posY = theCamera.transform.localPosition.y;

            while (_posY != applyCrouchPosY)
            {
                _posY = Mathf.Lerp(_posY, applyCrouchPosY, crouchSpeed * Time.deltaTime);
                theCamera.transform.localPosition = new Vector3(0, _posY, 0);

                if (Mathf.Abs(_posY - applyCrouchPosY) < 0.01f)
                    break;
                yield return null;
            }
            theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
        }

        // 지면 체크
        private void IsGround()
        {
            isGround = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, GroundLayerMask);
            Debug.Log(isGround);
            if (!isGround) return;

            if (velocityY > 0)
            {
                isGround = false;
                return;
            }

            Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.red);
            velocityY = 0;
        }

        // 점프 시도
        private void TryJump()
        {
            if (!isGround) return;

            Jump();
        }

        // 점프
        private void Jump()
        {
            if (isCrouch)
            {
                Crouch();
            }

            velocityY = jumpForce;
        }

        // 달리기 시도
        private void TryRun(bool run)
        {
            if (run)
            {
                Running();
            }
            else
            {
                RunningCancel();
            }
        }

        // 달리기 실행
        private void Running()
        {
            if (isCrouch)
            {
                Crouch();
            }
            isRun = true;
            applySpeed = runMoveSpeed;
        }

        // 달리기 취소
        private void RunningCancel()
        {
            isRun = false;
            applySpeed = walkMoveSpeed;
        }

        // 움직임 실행
        private void Move()
        {
            Vector3 _velocity = Player.MoveInput.normalized * applySpeed;

            velocity = Vector3.Lerp(velocity, _velocity, velocityShift * Time.deltaTime);
            velocity = new Vector3(velocity.x, velocityY, velocity.z);
            characterController.Move(velocity * Time.deltaTime);
        }

        // 좌우 카메라 회전
        private void CharacterRotation(float _yRotation)
        {
            Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            characterController.transform.rotation = characterController.transform.rotation * Quaternion.Euler(_characterRotationY);
        }

        // 상하 카메라 회전
        private void CameraRotation(float _xRotation)
        {
            float _cameraRotationX = _xRotation * lookSensitivity;
            currentCameraRotationX -= _cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
