using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Equipment
{
	[RequireComponent(typeof(EquipmentHandler))]
	public class EquipmentPhysicsHandler : PlayerComponent
	{
		[SerializeField]
		[Group]
		private SpringControlInfo m_SpringController = null;

		private EquipmentHandler m_FPHandler;
		private EquipmentPhysicsInfo m_Physics;

		private Transform m_Pivot;
		private Vector3 m_ModelOffset;
		private Vector3 m_OriginalRootPosition;
		private Quaternion m_OriginalRootRotation;

		//Springs
		private Spring m_PositionSpring;
		private Spring m_RotationSpring;
		private Spring m_PosRecoilSpring;
		private Spring m_RotRecoilSpring;

		// Motion states
		private EquipmentMotionState m_CurrentState;

		private Vector3 m_StatePosition;
		private Vector3 m_StateRotation;

		//Entry Offset
		private float m_ChangeToDefaultOffestTime;
		private float m_LerpedOffset;

		// Bob
		private int m_LastFootDown;
		private float m_CurrentBobParam;

		// State visualization
		private EquipmentMotionState m_StateToVisualize = null;
		private float m_VisualizationSpeed = 4f;
		private bool m_FirstStepTriggered;

		private List<Transform> m_PivotChildren = new List<Transform>();


		#region Recoil
		public void AdjustRecoilSprings(SpringSettings springSettings)
		{
			m_PosRecoilSpring.Adjust(springSettings.Position);
			m_RotRecoilSpring.Adjust(springSettings.Rotation);
		}

		public void ApplyPositionRecoil(Vector3 force, int distribution = 1)
		{
			if (distribution <= 1)
				m_PosRecoilSpring.AddForce(force);
			else
				m_PosRecoilSpring.AddDistributedForce(force, distribution);
		}

		public void ApplyRotationRecoil(Vector3 force, int distribution = 1)
		{
			if (distribution <= 1)
				m_RotRecoilSpring.AddForce(force);
			else
				m_RotRecoilSpring.AddDistributedForce(force, distribution);
		}
		#endregion

		public void SetStateToVisualize(EquipmentMotionState state, float speed)
		{
			m_StateToVisualize = state;
			m_VisualizationSpeed = speed;
			m_CurrentBobParam = 0f;
		}

		public void ReadjustSprings() 
		{
			m_PositionSpring.Adjust(m_CurrentState.SpringSettings.Position);
			m_RotationSpring.Adjust(m_CurrentState.SpringSettings.Rotation);
		} 

		public void SetOffset()
		{
			transform.localPosition = m_OriginalRootPosition + m_Physics.GeneralSettings.BasePosOffset;
			transform.localRotation = Quaternion.Euler(m_Physics.GeneralSettings.BaseRotOffset + m_OriginalRootRotation.eulerAngles);
		}

		private void Start()
		{
			Player.FallImpact.AddListener(On_FallImpact);
			Player.MoveCycleEnded.AddListener(On_StepTaken);
			Player.Death.AddListener(ClearCurrentState);
			Player.Jump.AddStartListener(On_Jump);

			m_FPHandler = GetComponent<EquipmentHandler>();
			m_FPHandler.OnChangeItem.AddListener(On_ChangeItem);

			transform.ResetLocal();

			foreach (Transform child in transform)
			{
				if (child.parent = transform)
					m_PivotChildren.Add(child);
			}

			m_ModelOffset = m_PivotChildren[0].localPosition;

			//Create a 'pivot' for the equipment underneath this GameObject
			var pivot = new GameObject("Pivot");
			m_Pivot = pivot.transform;
			m_Pivot.SetParent(transform, true);
			m_Pivot.ResetLocal();

			m_PositionSpring = new Spring(Spring.Type.OverrideLocalPosition, m_Pivot, Vector3.zero, m_SpringController.SpringLerpSpeed);
			m_RotationSpring = new Spring(Spring.Type.OverrideLocalRotation, m_Pivot, Vector3.zero, m_SpringController.SpringLerpSpeed);
			m_PosRecoilSpring = new Spring(Spring.Type.AddToLocalPosition, m_Pivot, Vector3.zero);
			m_RotRecoilSpring = new Spring(Spring.Type.AddToLocalRotation, m_Pivot, Vector3.zero);
		}

		private void FixedUpdate()
		{
			if (m_Physics != null)
			{
				m_StatePosition = Vector3.zero;
				m_StateRotation = Vector3.zero;

				UpdateState();

				UpdateOffset();
				UpdateBob(Time.fixedDeltaTime);
				UpdateSway();

				UpdateNoise();

				m_StatePosition *= m_SpringController.SpringForceMultiplier;
				m_StateRotation *= m_SpringController.SpringForceMultiplier;

				m_PositionSpring.AddForce(m_StatePosition);
				m_RotationSpring.AddForce(m_StateRotation);
		
				m_RotationSpring.FixedUpdate();
				m_PositionSpring.FixedUpdate();
				m_PosRecoilSpring.FixedUpdate();
				m_RotRecoilSpring.FixedUpdate();
			}
		}

		private void Update()
		{
			if (m_Physics != null)
			{
				m_RotationSpring.Update();
				m_PositionSpring.Update();
				m_PosRecoilSpring.Update();
				m_RotRecoilSpring.Update();
			}
		}

		private void On_ChangeItem()
		{
			m_Physics = m_FPHandler.EquipmentItem.EPhysics;

			if (m_Physics != null)
				ClearCurrentState();

			foreach (Transform child in m_PivotChildren)
			{
				child.SetParent(transform.parent);
				child.localPosition = m_ModelOffset;
				child.localRotation = Quaternion.identity;
			}

			m_Pivot.SetParent(transform.parent);
			m_Pivot.position = m_FPHandler.EquipmentItem.PhysicsPivot != null ? m_FPHandler.EquipmentItem.PhysicsPivot.position : m_Pivot.position;
			m_Pivot.localRotation = Quaternion.identity;

			transform.position = m_Pivot.position;
			transform.rotation = m_Pivot.rotation;

			m_OriginalRootPosition = transform.localPosition;
			m_OriginalRootRotation = transform.localRotation;

			m_Pivot.SetParent(transform, true);

			foreach (Transform child in m_PivotChildren)
				child.SetParent(m_Pivot, true);

			SetOffset();
		}

		private void UpdateState()
		{
			if (m_StateToVisualize != null)
				TrySetState(m_StateToVisualize);
			else
			{
				if (Player.Run.Active && Player.Velocity.Val.sqrMagnitude > 0.2f && Player.UseItem.LastExecutionTime + 0.3f < Time.time)
					TrySetState(m_Physics.RunState);
				else if (Player.Aim.Active)
					TrySetState(m_Physics.AimState);
				else if (Player.Crouch.Active)
					TrySetState(m_Physics.CrouchState);
				else if (Player.Prone.Active)
					TrySetState(m_Physics.ProneState);
				else if (Player.Walk.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
					TrySetState(m_Physics.WalkState);
				else
					TrySetState(m_Physics.IdleState);
			}
		}

		private void TrySetState(EquipmentMotionState state)
		{
			if (m_CurrentState != state)
			{
				if (m_CurrentState != null)
				{
					if ((m_CurrentState.EntryOffset.Enabled && m_ChangeToDefaultOffestTime < Time.time) || !m_CurrentState.EntryOffset.Enabled)
					{
						if (!(m_CurrentState == m_Physics.CrouchState && state == m_Physics.AimState))
						{
							float exitForceMultiplier = (state == m_Physics.AimState) ? 0.15f : 1f;

							m_RotationSpring.AddForce(m_CurrentState.ExitForce * exitForceMultiplier);
							m_PositionSpring.AddForce(m_CurrentState.PosExitForce * exitForceMultiplier);
						}
					}
				}

				float enterForceMultiplier = (m_CurrentState == m_Physics.AimState) ? 0.15f : 1f;

				m_CurrentState = state;

				ReadjustSprings();

				if (m_CurrentState != null)
				{
					if (m_CurrentState.EntryOffset.Enabled)
						m_ChangeToDefaultOffestTime = Time.time + m_CurrentState.EntryOffset.EntryOffsetDuration;

					m_LerpedOffset = 0f;

					m_RotationSpring.AddForce(m_CurrentState.EnterForce * enterForceMultiplier);
					m_PositionSpring.AddForce(m_CurrentState.PosEnterForce * enterForceMultiplier);
				}
			}
		}

		private void ClearCurrentState()
		{
			m_PositionSpring.Reset();
			m_RotationSpring.Reset();
			m_Pivot.ResetLocal();

			m_StatePosition = m_StateRotation = Vector3.zero;

			m_CurrentState = null;
		}

		private void UpdateOffset()
		{
			if (!m_CurrentState.Offset.Enabled || Player.Reload.Active)
				return;

			if (m_CurrentState.EntryOffset.Enabled)
			{
				if (m_ChangeToDefaultOffestTime > Time.time)
				{
					m_StatePosition += m_CurrentState.EntryOffset.Offset.PositionOffset * 0.0001f;
					m_StateRotation += m_CurrentState.EntryOffset.Offset.RotationOffset * 0.02f;
				}
				else
				{
					m_LerpedOffset = Mathf.Lerp(m_LerpedOffset, 1, Time.deltaTime * m_CurrentState.EntryOffset.LerpToOffsetSpeed);

					m_StatePosition += m_CurrentState.Offset.PositionOffset * 0.0001f * m_LerpedOffset;
					m_StateRotation += m_CurrentState.Offset.RotationOffset * 0.02f * m_LerpedOffset;
				}
			}
			else
			{
				m_StatePosition += m_CurrentState.Offset.PositionOffset * 0.0001f;
				m_StateRotation += m_CurrentState.Offset.RotationOffset * 0.02f;
			}
		}

		private void UpdateBob(float deltaTime)
		{
			if (!m_CurrentState.Bob.Enabled || (Player.Velocity.Get().sqrMagnitude < 0.1f && Player.Aim.Active))
				return;

			if (m_StateToVisualize != null)
			{
				m_CurrentBobParam += deltaTime * m_VisualizationSpeed * 2;

				if (!m_FirstStepTriggered && m_CurrentBobParam >= Mathf.PI)
				{
					m_FirstStepTriggered = true;
					ApplyStepForce();
				}

				if (m_CurrentBobParam >= Mathf.PI * 2f)
				{
					m_CurrentBobParam -= Mathf.PI * 2f;
					m_FirstStepTriggered = false;
					ApplyStepForce();
				}
			}
			else
			{
				m_CurrentBobParam = Player.MoveCycle.Get() * Mathf.PI;

				if (m_LastFootDown != 0)
					m_CurrentBobParam += Mathf.PI;
			}

			// Update position bob
			Vector3 posBobAmplitude = Vector3.zero;

			posBobAmplitude.x = m_CurrentState.Bob.PositionAmplitude.x * -0.00001f;
			posBobAmplitude.y = m_CurrentState.Bob.PositionAmplitude.y * 0.00001f;
			posBobAmplitude.z = m_CurrentState.Bob.PositionAmplitude.z * 0.00001f;

			m_StatePosition.x += Mathf.Cos(m_CurrentBobParam + m_SpringController.PositionBobOffset) * posBobAmplitude.x;
			m_StatePosition.y += Mathf.Cos(m_CurrentBobParam * 2 + m_SpringController.PositionBobOffset) * posBobAmplitude.y;
			m_StatePosition.z += Mathf.Cos(m_CurrentBobParam + m_SpringController.PositionBobOffset) * posBobAmplitude.z;

			// Update rotation bob
			Vector3 rotBobAmplitude = m_CurrentState.Bob.RotationAmplitude * 0.001f;

			m_StateRotation.x += Mathf.Cos(m_CurrentBobParam * 2 + m_SpringController.RotationBobOffset) * rotBobAmplitude.x;
			m_StateRotation.y += Mathf.Cos(m_CurrentBobParam + m_SpringController.RotationBobOffset) * rotBobAmplitude.y;
			m_StateRotation.z += Mathf.Cos(m_CurrentBobParam + m_SpringController.RotationBobOffset) * rotBobAmplitude.z;
		}
		
		private void UpdateSway()
		{
			// Sway Multiplier
			float multiplier = Time.fixedDeltaTime;
			float aimMultiplier = multiplier * (Player.Aim.Active ? m_Physics.Sway.AimMultiplier : 1f);

			// Look Input
			Vector2 lookInput = Player.LookInput.Get();

			lookInput *= m_Physics.Sway.LookInputMultiplier;
			lookInput = Vector2.ClampMagnitude(lookInput, m_Physics.Sway.MaxLookInput);

			// Sway Velocity
			Vector3 swayVelocity = Player.Velocity.Get();

			Vector3 localVelocity = transform.InverseTransformVector(swayVelocity / 60);

			if (Mathf.Abs(swayVelocity.y) < 1.5f)
				swayVelocity.y = 0f;

			// Look position sway
			m_PositionSpring.AddForce(new Vector3(
				lookInput.x * m_Physics.Sway.LookPositionSway.x * 0.125f,
				lookInput.y * m_Physics.Sway.LookPositionSway.y * -0.125f,
				lookInput.y * m_Physics.Sway.LookPositionSway.z * -0.125f) * aimMultiplier);

			// Look rotation sway
			m_RotationSpring.AddForce(new Vector3(
				lookInput.y * m_Physics.Sway.LookRotationSway.x * 1.25f,
				lookInput.x * m_Physics.Sway.LookRotationSway.y * -1.25f,
				lookInput.x * m_Physics.Sway.LookRotationSway.z * -1.25f) * aimMultiplier);

			// Falling
			var fallSway = m_Physics.Sway.FallSway * swayVelocity.y * 0.2f * multiplier;
			if (Player.IsGrounded.Get())
				fallSway *= (30f * multiplier);

			fallSway.z = Mathf.Max(0f, m_Physics.Sway.FallSway.z);
			m_RotationSpring.AddForce(fallSway);

			// Strafe position sway
			m_PositionSpring.AddForce(new Vector3(
				localVelocity.x * m_Physics.Sway.StrafePositionSway.x * 0.08f,
				-Mathf.Abs(localVelocity.x * m_Physics.Sway.StrafePositionSway.y * 0.08f),
				-localVelocity.z * m_Physics.Sway.StrafePositionSway.z * 0.08f) * aimMultiplier);

			// Strafe rotation sway
			m_RotationSpring.AddForce(new Vector3(
				-Mathf.Abs(localVelocity.x * m_Physics.Sway.StrafeRotationSway.x * 8f),
				-localVelocity.x * m_Physics.Sway.StrafeRotationSway.y * 8f,
				localVelocity.x * m_Physics.Sway.StrafeRotationSway.z * 8f) * aimMultiplier);
		}

		private void UpdateNoise()
		{
			if (m_CurrentState.Noise.Enabled)
			{
				float jitter = Random.Range(0, m_CurrentState.Noise.MaxJitter);
				float timeScale = Time.time * m_CurrentState.Noise.NoiseSpeed;

				m_StatePosition.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.x / 1000;
				m_StatePosition.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.y / 1000;
				m_StatePosition.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * m_CurrentState.Noise.PosNoiseAmplitude.z / 1000;

				m_StateRotation.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.x / 10;
				m_StateRotation.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.y / 10;
				m_StateRotation.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * m_CurrentState.Noise.RotNoiseAmplitude.z / 10;
			}
		}

		private void On_StepTaken()
		{
			if (Player.Velocity.Val.sqrMagnitude > 0.2f && m_Physics != null)
				ApplyStepForce();

			m_LastFootDown = m_LastFootDown == 0 ? 1 : 0;
		}

		private void ApplyStepForce()
		{
			EquipmentPhysicsInfo.StepForceModule stepForce = null;

			if (Player.Walk.Active || m_StateToVisualize == m_Physics.ProneState)
				stepForce = m_Physics.WalkStepForce;
			else if (Player.Crouch.Active || m_StateToVisualize == m_Physics.ProneState)
				stepForce = m_Physics.CrouchStepForce;
			else if (Player.Run.Active || m_StateToVisualize == m_Physics.ProneState)
				stepForce = m_Physics.RunStepForce;

			if (stepForce != null && stepForce.Enabled && !Player.Aim.Active)
			{
				m_PositionSpring.AddForce(stepForce.PositionForce.Force * 0.0001f, stepForce.PositionForce.Distribution);
				m_RotationSpring.AddForce(stepForce.RotationForce.Force * 0.01f, stepForce.RotationForce.Distribution);
			}
		}

		private void On_Jump() 
		{
			if (m_Physics == null || !m_Physics.Jump.Enabled) return;

			m_PositionSpring.AddDistributedForce(m_Physics.Jump.PositionForce.Force / 100, m_Physics.Jump.PositionForce.Distribution);
			m_RotationSpring.AddDistributedForce(m_Physics.Jump.RotationForce.Force / 10, m_Physics.Jump.RotationForce.Distribution);
		}

		private void On_FallImpact(float impactSpeed)
		{
			if (m_Physics == null) return;

			//Decrease the fall impact force if the Player is aiming
			impactSpeed *= Player.Aim.Active ? 0.5f : 1f;

			m_PositionSpring.AddDistributedForce(m_Physics.FallImpact.PositionForce.Force * impactSpeed * 0.0001f, m_Physics.FallImpact.PositionForce.Distribution);
			m_RotationSpring.AddDistributedForce(m_Physics.FallImpact.RotationForce.Force * impactSpeed, m_Physics.FallImpact.RotationForce.Distribution);
		}
	}
}