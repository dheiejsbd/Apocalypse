using UnityEngine;
using System;

namespace HQFPSTemplate
{
    public class PlayerMovement : PlayerComponent
    {
        #region Internal
        [Serializable]
        public class MovementStateModule
        {
            public bool Enabled = true;

            [ShowIf("Enabled", true)]
            [Range(1f, 10f)]
            public float SpeedMultiplier = 4.5f;

            [ShowIf("Enabled", true)]
            [Range(0f, 3f)]
            public float StepLength = 1.9f;
        }

        [Serializable]
        public class CoreMovementModule
        {
            [Range(0f, 20f)]
            public float Acceleration = 5f;

            [Range(0f, 20f)]
            public float Damping = 8f;

            [Range(0f, 1f)]
            public float AirborneControl = 0.15f;

            [Range(0f, 3f)]
            public float StepLength = 1.2f;

            [Range(0f, 10f)]
            public float ForwardSpeed = 2.5f;

            [Range(0f, 10f)]
            public float BackSpeed = 2.5f;

            [Range(0f, 10f)]
            public float SideSpeed = 2.5f;

            public AnimationCurve SlopeSpeedMult = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

            public float AntiBumpFactor = 1f;

            [Range(0f, 1f)]
            public float HeadBounceFactor = 0.65f;
        }

        [Serializable]
        public class JumpStateModule
        {
            public bool Enabled = true;

            [ShowIf("Enabled", true)]
            [Range(0f, 3f)]
            public float JumpHeight = 1f;

            [ShowIf("Enabled", true)]
            [Range(0f, 1.5f)]
            public float JumpTimer = 0.3f;
        }

        [Serializable]
        public class LowerHeightStateModule : MovementStateModule
        {
            [ShowIf("Enabled", true)]
            [Range(0f, 2f)]
            public float ControllerHeight = 1f;

            [ShowIf("Enabled", true)]
            [Range(0f, 1f)]
            public float TransitionDuration = 0.3f;
        }

        [Serializable]
        public class SlidingStateModule
        {
            public bool Enabled = false;

            [ShowIf("Enabled", true)]
            [Range(20f, 90f)]
            public float SlideTreeshold = 32f;

            [ShowIf("Enabled", true)]
            [Range(0f, 50f)]
            public float SlideSpeed = 15f;
        }
        #endregion

        public bool IsGrounded { get => m_Controller.isGrounded; }
        public Vector3 Velocity { get => m_Controller.velocity; }
        public Vector3 SurfaceNormal { get; private set; }
        public float SlopeLimit { get => m_Controller.slopeLimit; }
        public float DefaultHeight { get; private set; }

        [SerializeField]
        private CharacterController m_Controller = null;

        [SerializeField]
        private LayerMask m_ObstacleCheckMask = ~0;

        [SerializeField]
        private float m_Gravity = 20f;

        [Space]

        [SerializeField]
        [Group]
        private CoreMovementModule m_CoreMovement;

        [SerializeField]
        [Group]
        private MovementStateModule m_RunState;

        [SerializeField]
        [Group]
        private LowerHeightStateModule m_CrouchState;

        [SerializeField]
        [Group]
        private LowerHeightStateModule m_ProneState;

        [SerializeField]
        [Group]
        private JumpStateModule m_JumpState;

        [SerializeField]
        [Group]
        private SlidingStateModule m_SlidingState;

        private MovementStateModule m_CurrentMovementState;

        private Vector3 m_DesiredVelocityLocal;
        private Vector3 m_SlideVelocity;

        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private float m_LastLandTime;

        private float m_NextTimeCanChangeHeight;

        private float m_DistMovedSinceLastCycleEnded;
        private float m_CurrentStepLength;


        private void Awake()
        {
            if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hitInfo, 3f, ~0, QueryTriggerInteraction.Ignore))
                transform.position = hitInfo.point + Vector3.up * 0.08f;
        }

        private void Start()
        {
            DefaultHeight = m_Controller.height;

            Player.IsGrounded.AddChangeListener(OnGroundingStateChanged);

            Player.Run.SetStartTryer(Try_Run);
            Player.Run.AddStopListener(StopRun);
            Player.Jump.SetStartTryer(Try_Jump);

            Player.Crouch.SetStartTryer(() => { return Try_ToggleCrouch(m_CrouchState); });
            Player.Crouch.SetStopTryer(() => { return Try_ToggleCrouch(null); });

            Player.Prone.SetStartTryer(() => { return Try_ToggleProne(m_ProneState); });
            Player.Prone.SetStopTryer(() => { return Try_ToggleProne(null); });

            Player.Death.AddListener(OnDeath);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            Vector3 translation;

            if (IsGrounded)
            {
                translation = transform.TransformVector(m_DesiredVelocityLocal) * deltaTime;

                if (!Player.Jump.Active)
                    translation.y = -m_CoreMovement.AntiBumpFactor;
            }
            else
                translation = transform.TransformVector(m_DesiredVelocityLocal * deltaTime);

            m_CollisionFlags = m_Controller.Move(translation);

            if ((m_CollisionFlags & CollisionFlags.Below) == CollisionFlags.Below && !m_PreviouslyGrounded)
            {
                bool wasJumping = Player.Jump.Active;

                if (Player.Jump.Active)
                    Player.Jump.ForceStop();

                Player.FallImpact.Send(Mathf.Abs(m_DesiredVelocityLocal.y));

                m_LastLandTime = Time.time;

                if (wasJumping)
                    m_DesiredVelocityLocal = Vector3.ClampMagnitude(m_DesiredVelocityLocal, 1f);
            }

            // Check if the top of the controller collided with anything,
            // If it did then add a counter force
            if (((m_CollisionFlags & CollisionFlags.Above) == CollisionFlags.Above && !m_Controller.isGrounded) && m_DesiredVelocityLocal.y > 0)
                m_DesiredVelocityLocal.y *= -m_CoreMovement.HeadBounceFactor;

            Vector3 targetVelocity = CalcTargetVelocity(Player.MoveInput.Get());

            if (!IsGrounded)
                UpdateAirborneMovement(deltaTime, targetVelocity, ref m_DesiredVelocityLocal);
            else if (!Player.Jump.Active)
                UpdateGroundedMovement(deltaTime, targetVelocity, ref m_DesiredVelocityLocal);

            Player.IsGrounded.Set(IsGrounded);
            Player.Velocity.Set(Velocity);

            m_PreviouslyGrounded = IsGrounded;
        }

        private void UpdateGroundedMovement(float deltaTime, Vector3 targetVelocity, ref Vector3 velocity)
        {
            // Make sure to lower the speed when moving on steep surfaces.
            float surfaceAngle = Vector3.Angle(Vector3.up, SurfaceNormal);
            targetVelocity *= m_CoreMovement.SlopeSpeedMult.Evaluate(surfaceAngle / SlopeLimit);

            // Calculate the rate at which the current speed should increase / decrease. 
            // If the player doesn't press any movement button, use the "m_Damping" value, otherwise use "m_Acceleration".
            float targetAccel = targetVelocity.sqrMagnitude > 0f ? m_CoreMovement.Acceleration : m_CoreMovement.Damping;

            velocity = Vector3.Lerp(velocity, targetVelocity, targetAccel * deltaTime);

            // If we're moving and not running, start the "Walk" activity.
            if (!Player.Walk.Active && targetVelocity.sqrMagnitude > 0.05f && !Player.Run.Active && !Player.Crouch.Active)
                Player.Walk.ForceStart();
            // If we're running, or not moving, stop the "Walk" activity.
            else if (Player.Walk.Active && (targetVelocity.sqrMagnitude < 0.05f || Player.Run.Active || Player.Crouch.Active || Player.Prone.Active))
                Player.Walk.ForceStop();

            if (Player.Run.Active)
            {
                bool wantsToMoveBackwards = Player.MoveInput.Get().y < 0f;
                bool runShouldStop = wantsToMoveBackwards || targetVelocity.sqrMagnitude == 0f || Player.Stamina.Is(0f);

                if (runShouldStop)
                    Player.Run.ForceStop();
            }

            if (m_SlidingState.Enabled)
            {
                // Sliding...
                if (surfaceAngle > m_SlidingState.SlideTreeshold && Player.MoveInput.Get().sqrMagnitude == 0f)
                {
                    Vector3 slideDirection = (SurfaceNormal + Vector3.down);
                    m_SlideVelocity += slideDirection * m_SlidingState.SlideSpeed * deltaTime;
                }
                else
                    m_SlideVelocity = Vector3.Lerp(m_SlideVelocity, Vector3.zero, deltaTime * 10f);

                velocity += transform.InverseTransformVector(m_SlideVelocity);
            }

            // Advance step
            m_DistMovedSinceLastCycleEnded += m_DesiredVelocityLocal.magnitude * deltaTime;

            // Which step length should be used?
            float targetStepLength = m_CoreMovement.StepLength;

            if (m_CurrentMovementState != null)
                targetStepLength = m_CurrentMovementState.StepLength;

            m_CurrentStepLength = Mathf.MoveTowards(m_CurrentStepLength, targetStepLength, deltaTime);

            // If the step cycle is complete, reset it, and send a notification.
            if (m_DistMovedSinceLastCycleEnded > m_CurrentStepLength)
            {
                m_DistMovedSinceLastCycleEnded -= m_CurrentStepLength;
                Player.MoveCycleEnded.Send();
            }

            Player.MoveCycle.Set(m_DistMovedSinceLastCycleEnded / m_CurrentStepLength);
        }

        private void UpdateAirborneMovement(float deltaTime, Vector3 targetVelocity, ref Vector3 velocity)
        {
            if (m_PreviouslyGrounded && !Player.Jump.Active)
                velocity.y = 0f;

            // Modify the current velocity by taking into account how well we can change direction when not grounded (see "m_AirControl" tooltip).
            velocity += targetVelocity * m_CoreMovement.Acceleration * m_CoreMovement.AirborneControl * deltaTime;

            // Apply gravity.
            velocity.y -= m_Gravity * deltaTime;
        }

        private bool Try_Run()
        {
            if (!m_RunState.Enabled || Player.Stamina.Get() < 15f)
                return false;

            bool wantsToMoveBack = Player.MoveInput.Get().y < 0f;
            bool canChangeState = Player.IsGrounded.Get() && !wantsToMoveBack && !Player.Crouch.Active && !Player.Aim.Active && !Player.Prone.Active;

            if (canChangeState)
                m_CurrentMovementState = m_RunState;

            return canChangeState;
        }

        private bool Try_Jump()
        {
            // If crouched, stop crouching first
            if (Player.Crouch.Active)
            {
                Player.Crouch.TryStop();
                return false;
            }
            else if (Player.Prone.Active)
            {
                if (!Player.Prone.TryStop())
                    Player.Crouch.TryStart();

                return false;
            }

            bool canJump = m_JumpState.Enabled &&
                IsGrounded &&
                !Player.Crouch.Active &&
                Time.time > m_LastLandTime + m_JumpState.JumpTimer;

            if (!canJump)
                return false;

            float jumpSpeed = Mathf.Sqrt(2 * m_Gravity * m_JumpState.JumpHeight);
            m_DesiredVelocityLocal = new Vector3(m_DesiredVelocityLocal.x, jumpSpeed, m_DesiredVelocityLocal.z);

            return true;
        }

        private bool Try_ToggleCrouch(LowerHeightStateModule lowerHeightState) 
        {
            if (!m_CrouchState.Enabled)
                return false;

            bool toggledSuccesfully;

            if (!Player.Crouch.Active)
                toggledSuccesfully = Try_ChangeControllerHeight(lowerHeightState);
            else
                toggledSuccesfully = Try_ChangeControllerHeight(null);

            //Stop the prone state if the crouch state is enabled
            if (toggledSuccesfully && Player.Prone.Active)
                Player.Prone.ForceStop();

            return toggledSuccesfully;
        }

        private bool Try_ToggleProne(LowerHeightStateModule lowerHeightState)
        {
            if (!m_ProneState.Enabled)
                return false;

            bool toggledSuccesfully;

            if (!Player.Prone.Active)
                toggledSuccesfully = Try_ChangeControllerHeight(lowerHeightState);
            else
                toggledSuccesfully = Try_ChangeControllerHeight(null);

            //Stop the crouch state if the prone state is enabled
            if (toggledSuccesfully && Player.Crouch.Active)
                Player.Crouch.ForceStop();

            return toggledSuccesfully;
        }

        private bool Try_ChangeControllerHeight(LowerHeightStateModule lowerHeightState)
        {
            bool canChangeHeight =
                (Time.time > m_NextTimeCanChangeHeight || m_NextTimeCanChangeHeight == 0f) &&
                Player.IsGrounded.Get() &&
                !Player.Run.Active;

            if (canChangeHeight)
            {
                float height = (lowerHeightState == null) ? DefaultHeight : lowerHeightState.ControllerHeight;

                //If the "lowerHeightState" height is bigger than the current one check if there's anything over the Player's head
                if (height > m_Controller.height)
                {
                    if (DoCollisionCheck(true, Mathf.Abs(height - m_Controller.height)))
                        return false;
                }

                if(lowerHeightState != null)
                    m_NextTimeCanChangeHeight = Time.time + lowerHeightState.TransitionDuration;

                SetHeight(height);

                m_CurrentMovementState = lowerHeightState;
            }

            return canChangeHeight;
        }

        private void StopRun() 
        {
            m_CurrentMovementState = null;
        }

        private void OnGroundingStateChanged(bool isGrounded)
        {
            if (!isGrounded)
            {
                Player.Walk.ForceStop();
                Player.Run.ForceStop();
            }
        }

        private Vector3 CalcTargetVelocity(Vector2 moveInput)
        {
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);

            bool wantsToMove = moveInput.sqrMagnitude > 0f;

            // Calculate the direction (relative to the us), in which the player wants to move.
            Vector3 targetDirection = (wantsToMove ? new Vector3(moveInput.x, 0f, moveInput.y) : m_DesiredVelocityLocal.normalized);

            float desiredSpeed = 0f;

            if (wantsToMove)
            {
                // Set the default speed.
                desiredSpeed = m_CoreMovement.ForwardSpeed;

                // If the player wants to move sideways...
                if (Mathf.Abs(moveInput.x) > 0f)
                    desiredSpeed = m_CoreMovement.SideSpeed;

                // If the player wants to move backwards...
                if (moveInput.y < 0f)
                    desiredSpeed = m_CoreMovement.BackSpeed;

                // If we're currently running...
                if (Player.Run.Active)
                {
                    // If the player wants to move forward or sideways, apply the run speed multiplier.
                    if (desiredSpeed == m_CoreMovement.ForwardSpeed || desiredSpeed == m_CoreMovement.SideSpeed)
                        desiredSpeed = m_CurrentMovementState.SpeedMultiplier;
                }
                else
                {
                    // If we're crouching/pronning...
                    if (m_CurrentMovementState != null)
                        desiredSpeed *= m_CurrentMovementState.SpeedMultiplier;
                }
            }

            return targetDirection * (desiredSpeed * Player.MovementSpeedFactor.Val);
        }

        private bool DoCollisionCheck(bool checkAbove, float maxDistance)
        {
            Vector3 rayOrigin = transform.position + (checkAbove ? Vector3.up * m_Controller.height : Vector3.zero);
            Vector3 rayDirection = checkAbove ? Vector3.up : Vector3.down;

            return Physics.Raycast(rayOrigin, rayDirection, maxDistance, m_ObstacleCheckMask, QueryTriggerInteraction.Ignore);
        }

        private void SetHeight(float height)
        {
            m_Controller.height = height;
            m_Controller.center = Vector3.up * height * 0.5f;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            SurfaceNormal = hit.normal;
        }

        private void OnDeath()
        {
            m_DesiredVelocityLocal = Vector3.zero;
        }
    }
}