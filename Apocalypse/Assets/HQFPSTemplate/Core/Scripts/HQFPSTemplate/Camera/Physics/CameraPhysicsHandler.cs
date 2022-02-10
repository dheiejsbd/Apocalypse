using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate
{
	public class CameraPhysicsHandler : PlayerComponent
	{
		public float AimHeadbobMod { get; set; }
		public bool PhysicsEnabled { get; private set; }

		[SerializeField, Group(order = 3)]
		private SpringControlInfo m_SpringController = null;

		[SerializeField]
		private CameraPhysicsPreset m_CameraPhysicsPreset;

		// Springs
		private Spring m_PositionSpring;
		private Spring m_RotationSpring;
		private Spring m_PositionSpring_Force;
		private Spring m_RotationSpring_Force;
		private Spring m_PositionSpring_Recoil;
		private Spring m_RotationSpring_Recoil;
		private Spring m_PositionShakeSpring;
		private Spring m_RotationShakeSpring;

		// Motion states
		private CameraMotionState m_CurrentState;

		private Vector3 m_StatePosition;
		private Vector3 m_StateRotation;

		// Bob
		private int m_LastFootDown;
		private float m_CurrentMovingBobParam;
		private float m_CurrentStaticBobParam;

		// Shakes
		private List<CameraShake> m_Shakes = new List<CameraShake>();

		// State visualization
		private CameraMotionState m_StateToVisualize = null;
		private float m_VisualizationSpeed = 4f;
		private bool m_FirstStepTriggered;

		private List<QueuedCameraForce> m_QueuedCamForces = new List<QueuedCameraForce>();


		#region DelayedCamForces
		public void PlayDelayedCameraForces(DelayedCameraForce[] delayedCamForces)
		{
			for (int i = 0; i < delayedCamForces.Length; i++)
				PlayDelayedCameraForce(delayedCamForces[i]);
		}

		public void PlayDelayedCameraForce(DelayedCameraForce delayedCamForces)
		{
			m_QueuedCamForces.Add(new QueuedCameraForce(delayedCamForces, Time.time + delayedCamForces.Delay));
		}

		public void ClearQueuedCamForces() { m_QueuedCamForces.Clear(); }
		#endregion

		#region Camera Shakes
		public void DoShake(CameraShakeSettings shake, float scale)
		{
			m_Shakes.Add(new CameraShake(shake, m_PositionShakeSpring, m_RotationShakeSpring, scale));
		}

		public void AddExplosionShake(float scale)
		{
			m_Shakes.Add(new CameraShake(m_CameraPhysicsPreset.CameraShakes.ExplosionShake, m_PositionShakeSpring, m_RotationShakeSpring, scale));
		}

		private void OnShakeEvent(ShakeEventData shake)
		{
			if (shake.ShakeType == ShakeType.Explosion)
			{
				float distToExplosionSqr = (transform.position - shake.Position).sqrMagnitude;
				float explosionRadiusSqr = shake.Radius * shake.Radius;

				if (explosionRadiusSqr - distToExplosionSqr > 0f)
				{
					float distanceFactor = 1f - Mathf.Clamp01(distToExplosionSqr / explosionRadiusSqr);
					AddExplosionShake(distanceFactor * shake.Scale);
				}
			}
		}

		private void UpdateShakes()
		{
			if (m_Shakes.Count == 0)
				return;

			int i = 0;

			while (true)
			{
				if (m_Shakes[i].IsDone)
					m_Shakes.RemoveAt(i);
				else
				{
					m_Shakes[i].Update();
					i++;
				}

				if (i >= m_Shakes.Count)
					break;
			}
		}
		#endregion

		public void SetStateToVisualize(CameraMotionState state, float speed = 4f)
		{
			m_StateToVisualize = state;
			m_VisualizationSpeed = speed;
			m_CurrentMovingBobParam = 0f;
		}

		public void AdjustRecoilSprings(SpringSettings springSettings)
		{
			m_PositionSpring_Recoil.Adjust(springSettings.Position);
			m_RotationSpring_Recoil.Adjust(springSettings.Rotation);
		}

		public void AddPositionForce(Vector3 positionForce, int distribution = 1)
		{
			if (distribution <= 1)
				m_PositionSpring_Force.AddForce(positionForce);
			else
				m_PositionSpring_Force.AddDistributedForce(positionForce, distribution);
		}

		public void AddRotationForce(Vector3 rotationForce, int distribution = 1)
		{
			if (distribution <= 1)
				m_RotationSpring_Force.AddForce(rotationForce);
			else
				m_RotationSpring_Force.AddDistributedForce(rotationForce, distribution);
		}

		private void Awake()
		{
			PhysicsEnabled = true;

			var defaultSpringData = new Spring.Data(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.25f, 0.25f, 0.25f));

			//Main Springs
			m_PositionSpring = new Spring(Spring.Type.OverrideLocalPosition, transform, Vector3.zero);
			m_PositionSpring.Adjust(defaultSpringData);
			m_RotationSpring = new Spring(Spring.Type.OverrideLocalRotation, transform, Vector3.zero);
			m_PositionSpring.Adjust(defaultSpringData);

			// Force Springs
			m_PositionSpring_Force = new Spring(Spring.Type.AddToLocalPosition, transform, Vector3.zero);
			m_PositionSpring_Force.Adjust(defaultSpringData);
			m_RotationSpring_Force = new Spring(Spring.Type.AddToLocalRotation, transform, Vector3.zero);
			m_RotationSpring_Force.Adjust(defaultSpringData);

			// Recoil Springs
			m_PositionSpring_Recoil = new Spring(Spring.Type.AddToLocalPosition, transform, Vector3.zero, m_SpringController.SpringLerpSpeed);
			m_PositionSpring_Recoil.Adjust(defaultSpringData);
			m_RotationSpring_Recoil = new Spring(Spring.Type.AddToLocalRotation, transform, Vector3.zero, m_SpringController.SpringLerpSpeed);
			m_RotationSpring_Recoil.Adjust(defaultSpringData);

			// Shake Springs
			m_PositionShakeSpring = new Spring(Spring.Type.AddToLocalPosition, transform, Vector3.zero);
			m_PositionShakeSpring.Adjust(m_CameraPhysicsPreset.CameraShakes.ShakeSpringSettings.Position);
			m_RotationShakeSpring = new Spring(Spring.Type.AddToLocalRotation, transform, Vector3.zero);
			m_RotationShakeSpring.Adjust(m_CameraPhysicsPreset.CameraShakes.ShakeSpringSettings.Rotation);

			Player.FallImpact.AddListener(OnFallImpact);
			Player.Jump.AddStartListener(On_Jump);
			Player.MoveCycleEnded.AddListener(On_StepTaken);
			Player.ChangeHealth.AddListener(OnPlayerHealthChanged);
			Player.Death.AddListener(() => PhysicsEnabled = false);
			Player.Respawn.AddListener(OnPlayerRespawn);

			ShakeManager.ShakeEvent.AddListener(OnShakeEvent);
		}

		private void OnPlayerRespawn() 
		{
			PhysicsEnabled = true;

			m_PositionSpring.Reset();
			m_RotationSpring.Reset();
			m_PositionSpring_Force.Reset();
			m_RotationSpring_Force.Reset();
			m_PositionSpring_Recoil.Reset();
			m_RotationSpring_Recoil.Reset();
			m_PositionShakeSpring.Reset();
			m_RotationShakeSpring.Reset();
		}

		private void OnDestroy()
		{
			ShakeManager.ShakeEvent.RemoveListener(OnShakeEvent);
		}

		private void FixedUpdate()
		{
			if (PhysicsEnabled)
			{
				if (m_CameraPhysicsPreset != null)
				{
					m_StatePosition = Vector3.zero;
					m_StateRotation = Vector3.zero;

					UpdateState();

					UpdateOffset();
					UpdateMovementBob(Time.fixedDeltaTime);
					UpdateStationaryBob(Time.fixedDeltaTime);
					UpdateSway();
					UpdateNoise();

					m_StatePosition *= m_SpringController.SpringForceMultiplier;
					m_StateRotation *= m_SpringController.SpringForceMultiplier;

					m_PositionSpring.AddForce(m_StatePosition);
					m_RotationSpring.AddForce(m_StateRotation);
				}

				m_PositionSpring.FixedUpdate();
				m_RotationSpring.FixedUpdate();
				m_PositionSpring_Force.FixedUpdate();
				m_RotationSpring_Force.FixedUpdate();
				m_PositionSpring_Recoil.FixedUpdate();
				m_RotationSpring_Recoil.FixedUpdate();
			
				m_PositionShakeSpring.FixedUpdate();
				m_RotationShakeSpring.FixedUpdate();

				UpdateShakes();
			}
		}

		private void LateUpdate()
		{
			if (PhysicsEnabled)
			{
				m_PositionSpring.Update();
				m_RotationSpring.Update();

				m_PositionSpring_Force.Update();
				m_RotationSpring_Force.Update();

				m_PositionSpring_Recoil.Update();
				m_RotationSpring_Recoil.Update();
			
				m_PositionShakeSpring.Update();
				m_RotationShakeSpring.Update();

				UpdateQueuedCamForces();
			}
		}

		private void UpdateState()
		{
			if (m_StateToVisualize != null)
				TrySetState(m_StateToVisualize);
			else
			{
				if (Player.Run.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
					TrySetState(m_CameraPhysicsPreset.RunState);
				else if (Player.Crouch.Active)
					TrySetState(m_CameraPhysicsPreset.CrouchState);
				else if (Player.Prone.Active)
					TrySetState(m_CameraPhysicsPreset.ProneState);
				else if (Player.Walk.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
					TrySetState(m_CameraPhysicsPreset.WalkState);
				else
					TrySetState(m_CameraPhysicsPreset.IdleState);
			}
		}

		private void TrySetState(CameraMotionState state)
		{
			if (m_CurrentState != state)
			{
				if (m_CurrentState != null && m_CurrentState.ExitForces != null)
					PlayDelayedCameraForces(m_CurrentState.ExitForces);

				m_PositionSpring.Adjust(state.SpringSettings.Position);
				m_RotationSpring.Adjust(state.SpringSettings.Rotation);

				if (state.EnterForces != null)
					PlayDelayedCameraForces(state.EnterForces);

				m_CurrentState = state;
			}
		}

		private void UpdateStationaryBob(float deltaTime)
		{
			if (!Player.Aim.Active)
				return;

			m_CurrentStaticBobParam += deltaTime * m_CameraPhysicsPreset.AimState.Bob.BobSpeed;

			if (m_CurrentStaticBobParam >= Mathf.PI * 2f)
				m_CurrentStaticBobParam -= Mathf.PI * 2f;

			UpdateBob(m_CurrentStaticBobParam, m_CameraPhysicsPreset.AimState.Bob, AimHeadbobMod);
		}

		private void UpdateMovementBob(float deltaTime)
		{
			if (!m_CurrentState.Bob.Enabled)
				return;

			if (m_StateToVisualize != null)
			{
				m_CurrentMovingBobParam += deltaTime * m_VisualizationSpeed * 2;

				if (!m_FirstStepTriggered && m_CurrentMovingBobParam >= Mathf.PI)
				{
					m_FirstStepTriggered = true;
					ApplyStepForce();
				}

				if (m_CurrentMovingBobParam >= Mathf.PI * 2f)
				{
					m_CurrentMovingBobParam -= Mathf.PI * 2f;
					m_FirstStepTriggered = false;
					ApplyStepForce();
				}
			}
			else
			{
				m_CurrentMovingBobParam = Player.MoveCycle.Get() * Mathf.PI;

				if (m_LastFootDown != 0)
					m_CurrentMovingBobParam += Mathf.PI;
			}

			UpdateBob(m_CurrentMovingBobParam, m_CurrentState.Bob);
		}

		private void UpdateBob(float currentBobParam, EquipmentMotionState.BobModule bob, float mod = 1f)
		{
			// Update position bob
			Vector3 posBobAmplitude = bob.PositionAmplitude * 0.0001f;
			posBobAmplitude.x *= -1;

			m_StatePosition.x += Mathf.Cos(currentBobParam + m_SpringController.PositionBobOffset) * posBobAmplitude.x * mod;
			m_StatePosition.y += Mathf.Cos(currentBobParam * 2 + m_SpringController.PositionBobOffset) * posBobAmplitude.y * mod;
			m_StatePosition.z += Mathf.Cos(currentBobParam + m_SpringController.PositionBobOffset) * posBobAmplitude.z * mod;

			// Update rotation bob
			Vector3 rotBobAmplitude = bob.RotationAmplitude * 0.001f;

			m_StateRotation.x += Mathf.Cos(currentBobParam * 2 + m_SpringController.RotationBobOffset) * rotBobAmplitude.x * mod;
			m_StateRotation.y += Mathf.Cos(currentBobParam + m_SpringController.RotationBobOffset) * rotBobAmplitude.y * mod;
			m_StateRotation.z += Mathf.Cos(currentBobParam + m_SpringController.RotationBobOffset) * rotBobAmplitude.z * mod;
		}

		private void UpdateOffset()
		{
			if (!m_CurrentState.Offset.Enabled || Player.Reload.Active)
				return;

			m_StatePosition += m_CurrentState.Offset.PositionOffset * 0.0001f;
			m_StateRotation += m_CurrentState.Offset.RotationOffset * 0.02f;
		}

		private void UpdateSway()
		{
			// Sway Multiplier
			float multiplier = Player.Aim.Active ? m_CameraPhysicsPreset.Sway.AimMultiplier : 1f;
			multiplier *= Time.fixedDeltaTime;

			// Look Input
			Vector2 lookInput = Player.LookInput.Get();

			lookInput *= m_CameraPhysicsPreset.Sway.LookInputMultiplier;
			lookInput = Vector2.ClampMagnitude(lookInput, m_CameraPhysicsPreset.Sway.MaxLookInput);

			// Sway Velocity
			Vector2 swayVelocity = Player.Velocity.Get();

			if (Mathf.Abs(swayVelocity.y) < 1.5f)
				swayVelocity.y = 0f;

			Vector3 localVelocity = transform.InverseTransformDirection(swayVelocity / 60);

			// Look position sway
			m_PositionSpring.AddForce(new Vector3(
				lookInput.x * m_CameraPhysicsPreset.Sway.LookPositionSway.x * 0.125f,
				lookInput.y * m_CameraPhysicsPreset.Sway.LookPositionSway.y * -0.125f,
				lookInput.y * m_CameraPhysicsPreset.Sway.LookPositionSway.z * -0.125f) * multiplier);

			// Look rotation sway
			m_RotationSpring.AddForce(new Vector3(
				lookInput.y * m_CameraPhysicsPreset.Sway.LookRotationSway.x * 1.25f,
				lookInput.x * m_CameraPhysicsPreset.Sway.LookRotationSway.y * -1.25f,
				lookInput.x * m_CameraPhysicsPreset.Sway.LookRotationSway.z * -1.25f) * multiplier);

			// Falling
			var fallSway = m_CameraPhysicsPreset.Sway.FallSway * swayVelocity.y * 0.2f * multiplier;
			if (Player.IsGrounded.Get())
				fallSway *= (15f * multiplier);

			fallSway.z = Mathf.Max(0f, m_CameraPhysicsPreset.Sway.FallSway.z);
			m_RotationSpring.AddForce(fallSway);

			// Strafe position sway
			m_PositionSpring.AddForce(new Vector3(
				localVelocity.x * m_CameraPhysicsPreset.Sway.StrafePositionSway.x * 0.08f,
				-Mathf.Abs(localVelocity.x * m_CameraPhysicsPreset.Sway.StrafePositionSway.y * 0.08f),
				-localVelocity.z * m_CameraPhysicsPreset.Sway.StrafePositionSway.z * 0.08f) * multiplier);

			// Strafe rotation sway
			m_RotationSpring.AddForce(new Vector3(
				-Mathf.Abs(localVelocity.x * m_CameraPhysicsPreset.Sway.StrafeRotationSway.x * 8f),
				-localVelocity.x * m_CameraPhysicsPreset.Sway.StrafeRotationSway.y * 8f,
				localVelocity.x * m_CameraPhysicsPreset.Sway.StrafeRotationSway.z * 8f) * multiplier);
		}

		private void UpdateNoise()
		{
			if (m_CurrentState.Noise.Enabled)
			{
				var noiseModule = Player.Aim.Active ? m_CameraPhysicsPreset.AimState.Noise : m_CurrentState.Noise;

				float jitter = Random.Range(0, m_CurrentState.Noise.MaxJitter);
				float timeScale = Time.time * m_CurrentState.Noise.NoiseSpeed;

				m_StatePosition.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * noiseModule.PosNoiseAmplitude.x / 1000;
				m_StatePosition.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * noiseModule.PosNoiseAmplitude.y / 1000;
				m_StatePosition.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * noiseModule.PosNoiseAmplitude.z / 1000;

				m_StateRotation.x += (Mathf.PerlinNoise(jitter, timeScale) - 0.5f) * noiseModule.RotNoiseAmplitude.x / 10;
				m_StateRotation.y += (Mathf.PerlinNoise(jitter + 1f, timeScale) - 0.5f) * noiseModule.RotNoiseAmplitude.y / 10;
				m_StateRotation.z += (Mathf.PerlinNoise(jitter + 2f, timeScale) - 0.5f) * noiseModule.RotNoiseAmplitude.z / 10;
			}
		}

		private void On_StepTaken()
		{
			if (Player.Velocity.Val.sqrMagnitude > 0.2f && m_CameraPhysicsPreset != null)
				ApplyStepForce();

			m_LastFootDown = m_LastFootDown == 0 ? 1 : 0;
		}

		private void ApplyStepForce()
		{
			EquipmentPhysics.StepForceModule stepForce = null;

			stepForce = m_CurrentState.StepForce;

			if (stepForce != null && stepForce.Enabled && !Player.Aim.Active)
			{
				m_PositionSpring.AddForce(stepForce.PositionForce.Force * 0.0001f, stepForce.PositionForce.Distribution);
				m_RotationSpring.AddForce(stepForce.RotationForce.Force * 0.01f, stepForce.RotationForce.Distribution);
			}
		}

		private void OnPlayerHealthChanged(DamageInfo healthEventData)
		{
			if (healthEventData.Delta < -8f)
			{
				Vector3 posForce = healthEventData.HitDirection == Vector3.zero ? Random.onUnitSphere : healthEventData.HitDirection.normalized;
				posForce *= Mathf.Abs(healthEventData.Delta / 80f);

				Vector3 rotForce = Random.onUnitSphere;

				AddPositionForce(transform.InverseTransformVector(posForce) * m_CameraPhysicsPreset.GetHitForce.PosForce);
				AddRotationForce(rotForce * m_CameraPhysicsPreset.GetHitForce.RotForce);
			}
		}

		private void OnFallImpact(float impactVelocity)
		{
			float impactVelocityAbs = Mathf.Abs(impactVelocity);

			if (impactVelocityAbs > m_CameraPhysicsPreset.FallImpact.FallImpactRange.x)
			{
				float multiplier = Mathf.Clamp01(impactVelocityAbs / m_CameraPhysicsPreset.FallImpact.FallImpactRange.y);

				AddPositionForce(transform.InverseTransformVector(m_CameraPhysicsPreset.FallImpact.PosForce.Force) * multiplier, m_CameraPhysicsPreset.FallImpact.PosForce.Distribution);
				AddRotationForce(m_CameraPhysicsPreset.FallImpact.RotForce.Force * multiplier, m_CameraPhysicsPreset.FallImpact.RotForce.Distribution);
			}
		}

		private void On_Jump()
		{
			if (m_CameraPhysicsPreset == null || !m_CameraPhysicsPreset.Jump.Enabled) return;

			m_PositionSpring.AddDistributedForce(m_CameraPhysicsPreset.Jump.PositionForce.Force / 100, m_CameraPhysicsPreset.Jump.PositionForce.Distribution);
			m_RotationSpring.AddDistributedForce(m_CameraPhysicsPreset.Jump.RotationForce.Force / 10, m_CameraPhysicsPreset.Jump.RotationForce.Distribution);
		}

		private void UpdateQueuedCamForces() 
		{
			for (int i = 0; i < m_QueuedCamForces.Count; i++)
			{
				if (Time.time >= m_QueuedCamForces[i].PlayTime)
				{
					var force = m_QueuedCamForces[i].DelayedForce.Force;
					Player.Camera.Physics.AddRotationForce(force.Force, force.Distribution);
					m_QueuedCamForces.RemoveAt(i);
				}
			}
		}
    }
}