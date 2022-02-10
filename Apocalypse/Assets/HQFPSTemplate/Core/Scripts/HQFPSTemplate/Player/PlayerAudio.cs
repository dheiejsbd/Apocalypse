using UnityEngine;
using System;
using HQFPSTemplate.Surfaces;

namespace HQFPSTemplate
{
	/// <summary>
	/// Will play a footstep sound when the character travels enough distance on a surface
	/// </summary>
	public class PlayerAudio : PlayerComponent
	{
		#region Internal
		#pragma warning disable CS0649
		[Serializable]
		private struct PlayerMovementAudio
		{
			[Group]
			public SoundPlayer JumpAudio, CrouchAudio, ProneAudio, StandUpAudio;
		}

		[Serializable]
		private struct PlayerVitalsAudio
		{
			[BHeader("Health", true)]

			[Group]
			[Tooltip("The sounds that will be played when this entity receives damage.")]
			public SoundPlayer HurtAudio;

			[SerializeField]
			public float TimeBetweenScreams;

			[Space]

			[Group]
			public SoundPlayer FallDamageAudio;

			[Space]

			[Group]
			public SoundPlayer EarRingingAudio;

			[Range(0f, 1f)]
			public float EarRingVolumeDecrease;
			public float EarRingVolumeGainSpeed;

			[Space]

			[Group]
			public SoundPlayer DeathAudio;

			[BHeader("Stamina", true)]

			[Group]
			public SoundPlayer BreathingHeavyAudio;

			public float BreathingHeavyDuration;
		}

		[Serializable]
		private struct PlayerFootstepsAudio
		{
			public LayerMask GroundMask;

			[Range(0f, 1f)]
			public float RaycastDistance;

			[Range(0f, 10f)]
			[Tooltip("If the impact speed is higher than this threeshold, an effect will be played.")]
			public float FallImpactThreeshold;

			[Range(0f, 1f)]
			public float WalkVolume;

			[Range(0f, 1f)]
			public float CrouchVolume;

			[Range(0f, 1f)]
			public float ProneVolume;

			[Range(0f, 1f)]
			public float RunVolume;
		}
		#pragma warning restore CS0649
		#endregion

		[SerializeField]
		private AudioSource m_AudioSource = null;

		[Space]

		[SerializeField, Group]
		private PlayerMovementAudio m_PlayerMovementAudio = new PlayerMovementAudio();

		[SerializeField, Group]
		private PlayerVitalsAudio m_PlayerVitalsAudio = new PlayerVitalsAudio();

		[SerializeField, Group]
		private PlayerFootstepsAudio m_PlayerFootsteps = new PlayerFootstepsAudio();

		private float m_LastHeavyBreathTime;
		private float m_NextTimeCanScream;


		private void Start()
		{
			Player.MoveCycleEnded.AddListener(PlayFootstep);
			Player.FallImpact.AddListener(On_FallImpact);

			Player.Death.AddListener(() => { m_PlayerVitalsAudio.DeathAudio.Play(m_AudioSource); });
			Player.Jump.AddStartListener(() => { m_PlayerMovementAudio.JumpAudio.Play(m_AudioSource); });
			Player.Crouch.AddStartListener(() => { m_PlayerMovementAudio.CrouchAudio.Play(m_AudioSource); });
			Player.Crouch.AddStopListener(() => { m_PlayerMovementAudio.StandUpAudio.Play(m_AudioSource); });
			Player.Prone.AddStartListener(() => { m_PlayerMovementAudio.ProneAudio.Play(m_AudioSource); });
			Player.Prone.AddStopListener(() => { m_PlayerMovementAudio.StandUpAudio.Play(m_AudioSource); });

			Player.Health.AddChangeListener(OnChanged_Health);
			Player.Stamina.AddChangeListener(OnChanged_Stamina);

			ShakeManager.ShakeEvent.AddListener(OnShakeEvent);
		}

		private void Update()
		{	
			AudioListener.volume = Mathf.MoveTowards(AudioListener.volume, 1f, m_PlayerVitalsAudio.EarRingVolumeGainSpeed * Time.deltaTime);
		}

		private void OnDestroy()
		{
			ShakeManager.ShakeEvent.RemoveListener(OnShakeEvent);
		}

		#region Player Footsteps
		private void PlayFootstep()
		{
			if (Player.Velocity.Val.sqrMagnitude > 0.1f)
			{
				SurfaceEffects footstepEffect = SurfaceEffects.SoftFootstep;

				if (Player.Run.Active)
					footstepEffect = SurfaceEffects.HardFootstep;

				float volumeFactor = m_PlayerFootsteps.WalkVolume;

				if (Player.Crouch.Active)
					volumeFactor = m_PlayerFootsteps.CrouchVolume;
				else if (Player.Prone.Active)
					volumeFactor = m_PlayerFootsteps.ProneVolume;
				else if (Player.Run.Active)
					volumeFactor = m_PlayerFootsteps.RunVolume;

				RaycastHit hitInfo;

				if (CheckGround(out hitInfo))
					SurfaceManager.SpawnEffect(hitInfo, footstepEffect, volumeFactor);
			}
		}

		private void On_FallImpact(float fallImpactSpeed)
		{
			// Don't play the clip when the impact speed is low
			bool wasHardImpact = Mathf.Abs(fallImpactSpeed) >= m_PlayerFootsteps.FallImpactThreeshold;

			if (wasHardImpact)
			{
				if (CheckGround(out RaycastHit hitInfo))
					SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.FallImpact, 1f);

				if (Player.Health.GetPreviousValue() > Player.Health.Get())
					m_PlayerVitalsAudio.FallDamageAudio.Play(ItemSelection.Method.Random, m_AudioSource);
			}
		}

		private bool CheckGround(out RaycastHit hitInfo)
		{
			Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

			return Physics.Raycast(ray, out hitInfo, m_PlayerFootsteps.RaycastDistance, m_PlayerFootsteps.GroundMask, QueryTriggerInteraction.Ignore);
		}
		#endregion

		#region Player Vitals
		private void OnShakeEvent(ShakeEventData shake)
		{
			if (shake.ShakeType == ShakeType.Explosion)
			{
				float distToExplosionSqr = (transform.position - shake.Position).sqrMagnitude;
				float explosionRadiusSqr = shake.Radius * shake.Radius;

				float distanceFactor = 1f - Mathf.Clamp01(distToExplosionSqr / explosionRadiusSqr);

				AudioListener.volume = 1f - m_PlayerVitalsAudio.EarRingVolumeDecrease * distanceFactor;

				m_PlayerVitalsAudio.EarRingingAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource, distanceFactor);
			}
		}

		private void OnChanged_Health(float health)
		{
			float delta = health - Entity.Health.GetPreviousValue();

			if (delta < 0f)
			{
				if (Time.time > m_NextTimeCanScream)
				{
					m_PlayerVitalsAudio.HurtAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource);
					m_NextTimeCanScream = Time.time + m_PlayerVitalsAudio.TimeBetweenScreams;
				}
			}
		}

		private void OnChanged_Stamina(float stamina)
		{
			if (Player.Stamina.GetPreviousValue() == stamina)
				return;

			if (stamina == 0 && Time.time - m_LastHeavyBreathTime > m_PlayerVitalsAudio.BreathingHeavyDuration)
			{
				m_LastHeavyBreathTime = Time.time;
				m_PlayerVitalsAudio.BreathingHeavyAudio.Play(m_AudioSource);
			}
		}
		#endregion

		#region Player Movement
		#endregion
	}
}