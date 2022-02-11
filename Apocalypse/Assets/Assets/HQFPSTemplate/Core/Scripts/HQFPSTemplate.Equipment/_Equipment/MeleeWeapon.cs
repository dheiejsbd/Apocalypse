using System.Collections;
using UnityEngine;
using HQFPSTemplate.Surfaces;

namespace HQFPSTemplate.Equipment
{
	public partial class MeleeWeapon : EquipmentItem
	{
		#region Anim Hashing
		//Hashed animator strings (Improves performance)
		private readonly int animHash_SwingSpeed = Animator.StringToHash("Swing Speed");
		private readonly int animHash_SwingIndex = Animator.StringToHash("Swing Index");
		private readonly int animHash_Swing = Animator.StringToHash("Swing");
		#endregion

		private MeleeWeaponInfo m_MW;

		private int m_LastFreeSwing;
		private float m_NextResetSwingSelectionTime;


        public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

			m_MW = EInfo as MeleeWeaponInfo;
		}

        public override bool TryUseOnce(Ray[] itemUseRays, int useType)
		{
			if(Time.time < m_NextTimeCanUse)
				return false;

			MeleeWeaponInfo.SwingData swing;

			//Select Swing
			if (Time.time > m_NextResetSwingSelectionTime && m_MW.MeleeSettings.ResetSwingsIfNotUsed)
				swing = m_MW.MeleeSettings.Swings[0];
			else
				swing = m_MW.MeleeSettings.Swings.Select(ref m_LastFreeSwing, m_MW.MeleeSettings.SwingSelection);

			m_UseThreshold = swing.Cooldown;
			m_NextTimeCanUse = Time.time + m_UseThreshold;

			if (m_MW.MeleeSettings.ResetSwingsIfNotUsed)
				m_NextResetSwingSelectionTime = Time.time + m_MW.MeleeSettings.ResetSwingsDelay;

			EHandler.Animator_SetFloat(animHash_SwingIndex, swing.AnimationIndex);
			EHandler.Animator_SetTrigger(animHash_Swing);
			EHandler.Animator_SetFloat(animHash_SwingSpeed, swing.AnimationSpeed);

			Player.Camera.Physics.AddPositionForce(swing.SwingCamForces.PositionForce);
			Player.Camera.Physics.AddRotationForce(swing.SwingCamForces.RotationForce);

			EHandler.PlayDelayedSound(swing.SwingAudio);

			StartCoroutine(C_SphereCastDelayed(swing));

			return true;
		}

		public override bool TryUseContinuously(Ray[] itemUseRays, int useType)
		{
			if (!m_MW.MeleeSettings.CanContinuouslyAttack)
				return false;

			return TryUseOnce(itemUseRays, useType);
		}

		protected virtual IDamageable SphereCast(Ray itemUseRays, MeleeWeaponInfo.SwingData swing)
		{
			IDamageable damageable = null;

			if(Physics.SphereCast(itemUseRays.origin, swing.CastRadius, itemUseRays.direction, out RaycastHit hitInfo, m_MW.MeleeSettings.MaxHitDistance, m_MW.MeleeSettings.HitMask, QueryTriggerInteraction.Collide))
			{
				SurfaceManager.SpawnEffect(hitInfo, m_MW.MeleeSettings.ImpactEffect, 1f);

				// Apply an impact impulse
				if(hitInfo.rigidbody != null)
					hitInfo.rigidbody.AddForceAtPosition(itemUseRays.direction * swing.HitImpact, hitInfo.point, ForceMode.Impulse);

				var damageData = new DamageInfo(-swing.HitDamage, m_MW.MeleeSettings.DamageType, hitInfo.point, itemUseRays.direction, swing.HitImpact, Player, hitInfo.transform);

				// Audio
				EHandler.PlayDelayedSound(swing.HitAudio);

				// Camera force
				Player.Camera.Physics.AddPositionForce(swing.HitCamForces.PositionForce);
				Player.Camera.Physics.AddRotationForce(swing.HitCamForces.RotationForce);

				// Try to damage the Hit object
				Player.DealDamage.Try(damageData, null);
			}

			return damageable;
		}

		private IEnumerator C_SphereCastDelayed(MeleeWeaponInfo.SwingData swing)
		{
			yield return new WaitForSeconds(swing.CastDelay);

			m_GeneralEvents.OnUse.Invoke();

			// Small hack until a better solution is found
			Ray[] itemUseRay = EHandler.GenerateItemUseRays(Player, EHandler.ItemUseTransform, 1, 1f);

			SphereCast(itemUseRay[0], swing);
		}
    }
}