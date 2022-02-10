using UnityEngine;
using System.Linq;
using System;
using HQFPSTemplate.Items;
using HQFPSTemplate.Surfaces;

namespace HQFPSTemplate.Equipment
{
	/// <summary>
	/// Hitscan Weapon - objects that shoot really fast moving projectiles (e.g. Handguns, Assault Rifles etc.)
	/// </summary>
	public partial class Gun : ProjectileWeapon
	{
		//Fire mode
		private float m_NextTimeCanChangeMode = -1f;
		private GunSettings.Shooting m_P;

		// Cache some properties of the item
		protected ItemProperty m_FireModes;


        public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

			m_P = (EInfo as GunInfo).Projectile;
		}

        public override void Equip(Item item)
		{
			base.Equip(item);

			//Select the firemode that the corresponding weapon is set on
			if (item.HasProperty(EHandler.ItemProperties.FireModeProperty))
			{
				m_FireModes = item.GetProperty(EHandler.ItemProperties.FireModeProperty);
				SelectedFireMode = m_FireModes.Integer;

				SelectFireMode(SelectedFireMode);
			}
		}

		//Try change the fire mode
		public override bool TryChangeUseMode()
		{
			if (m_FireModes != null && Time.time > m_NextTimeCanChangeMode)
			{
				m_NextTimeCanChangeMode = Time.time + 0.3f;
				m_NextTimeCanUse = Time.time + 0.3f;

				int nextFireMode = GetNextFireModeIndex(SelectedFireMode);

				if (nextFireMode == SelectedFireMode)
					return false;
				else
					SelectedFireMode = nextFireMode;

				SelectFireMode(SelectedFireMode);

				//Play Audio & Procedural animation
				EHandler.PlayDelayedSound(m_PW.Shooting.FireModeChangeAudio);

				return true;
			}

			return false;
		}

		public override void Shoot(Ray[] itemUseRays)
		{
			base.Shoot(itemUseRays);

			// The points in space that this gun's bullets hit
			Vector3[] hitPoints = new Vector3[m_P.RayCount];

			//Raycast Shooting with multiple rays (e.g. Shotgun)
			if (m_P.RayCount > 1)
			{
				for (int i = 0; i < m_P.RayCount; i++)
					hitPoints[i] = DoHitscan(itemUseRays[i]);
			}
			else
				//Raycast Shooting with one ray
				hitPoints[0] = DoHitscan(itemUseRays[0]);

			FireHitPoints.Send(hitPoints);
		}

        public override float GetUseRaySpreadMod()
        {
            return m_P.RaySpread * m_P.SpreadOverTime.Evaluate(EHandler.ContinuouslyUsedTimes / (float)MagazineSize);
		}

        public override int GetUseRaysAmount()
        {
			return m_P.RayCount;
        }

        protected Vector3 DoHitscan(Ray itemUseRay)
		{
			RaycastHit hitInfo;

			if (Physics.Raycast(itemUseRay, out hitInfo, m_P.RayImpact.MaxDistance, m_P.RayMask, QueryTriggerInteraction.Collide))
			{
				float impulse = m_P.RayImpact.GetImpulseAtDistance(hitInfo.distance);

				// Apply an impact impulse
				if (hitInfo.rigidbody != null)
					hitInfo.rigidbody.AddForceAtPosition(itemUseRay.direction * impulse, hitInfo.point, ForceMode.Impulse);

				// Get the damage amount
				float damage = m_P.RayImpact.GetDamageAtDistance(hitInfo.distance);

				var damageInfo = new DamageInfo(-damage, DamageType.Bullet, hitInfo.point, itemUseRay.direction, impulse * m_P.RayCount, hitInfo.normal, Player, hitInfo.transform);

				// Try to damage the Hit object
				Player.DealDamage.Try(damageInfo, null);

				SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.BulletHit, 1f);
			}
			else
				hitInfo.point = itemUseRay.GetPoint(10f);

			return hitInfo.point;
		}

		private void SelectFireMode(int selectedMode)
		{
			UpdateFireModeSettings(selectedMode);

			//Set the firemode to the coressponding saveable item
			m_FireModes.Integer = selectedMode;
		}

		private int GetNextFireModeIndex(int currentIndex) 
		{
			int lastEnumVal = (int)Enum.GetValues(typeof(ProjectileWeaponInfo.FireMode)).Cast<ProjectileWeaponInfo.FireMode>().Max();

			int i;

			int loopIndex = 0;
			int fireModeIndex = currentIndex;

			if (fireModeIndex == lastEnumVal)
				i = 1;
			else
				i = fireModeIndex * 2;

			while (i <= lastEnumVal)
			{
				if (loopIndex > 20)
					break;

				if (m_PW.Shooting.Modes.HasFlag((ProjectileWeaponInfo.FireMode)i))
				{
					fireModeIndex = i;
					break;
				}

				if (i == lastEnumVal)
					i = 1;
				else
					i *= 2;

				loopIndex++;
			}

			return fireModeIndex;
		}
	}
}