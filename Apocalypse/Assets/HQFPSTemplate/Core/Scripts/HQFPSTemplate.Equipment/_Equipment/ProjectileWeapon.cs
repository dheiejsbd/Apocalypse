using UnityEngine;
using System.Collections;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.Equipment
{
	/// <summary>
	/// Base class from which guns and any other projectile weapons derive from
	/// </summary>
	public abstract class ProjectileWeapon : EquipmentItem
	{
		#region Internal
		public struct AmmoInfo
		{
			public int CurrentInMagazine;
			public int CurrentInStorage;

			public override string ToString()
			{
				return string.Format("Ammo In Mag: {0}. Total Ammo: {1}", CurrentInMagazine, CurrentInStorage);
			}
		}
        #endregion

        #region Anim Hashing
        //Hashed animator strings (Improves performance)
        private readonly int animHash_Fire = Animator.StringToHash("Fire");
		private readonly int animHash_FireSpeed = Animator.StringToHash("Fire Speed");
		private readonly int animHash_FireIndex = Animator.StringToHash("Fire Index");
		private readonly int animHash_Reload = Animator.StringToHash("Reload");
		private readonly int animHash_EmptyReload = Animator.StringToHash("Empty Reload");
		private readonly int animHash_StartReload = Animator.StringToHash("Start Reload");
		private readonly int animHash_EndReload = Animator.StringToHash("End Reload");
		private readonly int animHash_ReloadSpeed = Animator.StringToHash("Reload Speed");
		private readonly int animHash_EmptyReloadSpeed = Animator.StringToHash("Empty Reload Speed");
		#endregion

		/// <summary>
		/// Raycast event, called when this weapon is used
		/// </summary>
		public Message<Vector3[]> FireHitPoints = new Message<Vector3[]>();
		public Message DryFire = new Message();
		public Value<AmmoInfo> CurrentAmmoInfo = new Value<AmmoInfo>();

		public int SelectedFireMode { get; protected set; } = 2;
		public int MagazineSize { get => m_PW.Shooting.MagazineSize; }
		public bool AmmoEnabled { get => m_PW.Shooting.EnableAmmo; }
		
		protected ProjectileWeaponInfo m_PW;

		// Shooting
		protected int m_CurrentFireAnimIndex = 0;

		// Reloading
		private int m_AmmoToAdd;
		private bool m_ReloadLoopStarted;
		private float m_ReloadLoopEndTime;
		private float m_ReloadStartTime;
		private bool m_EndReload;

		// Cache some properties of the item
		protected ItemProperty m_AmmoTypeProperty;
		protected ItemProperty m_AmmoProperty;

		// Caches for coroutines
		private WaitForSeconds m_BurstWait;


		public void UpdateAmmoInfo()
		{
			if (!m_PW.Shooting.EnableAmmo)
				return;

			CurrentAmmoInfo.Set(
				new AmmoInfo()
				{
					CurrentInMagazine = m_AmmoProperty.Integer,

					// Get the ammo count from the inventory
					CurrentInStorage = GetAmmoCount()
				});
		}

		public override float GetTimeBetweenUses()
		{
			return m_UseThreshold * Mathf.Clamp(1 / m_PW.Shooting.FireRateOverTime.Evaluate(EHandler.ContinuouslyUsedTimes / (float)MagazineSize), 0.1f, 10f);
		}

		public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

			m_PW = EInfo as ProjectileWeaponInfo;

			m_BurstWait = new WaitForSeconds(m_PW.Shooting.BurstDuration / m_PW.Shooting.BurstLength);
			UpdateFireModeSettings(SelectedFireMode);
		}

        public override void Equip(Item item)
		{
			base.Equip(item);

			m_AmmoProperty = item.GetProperty(EHandler.ItemProperties.AmmoProperty);
			m_AmmoTypeProperty = item.GetProperty(EHandler.ItemProperties.AmmoTypeProperty);

			// Handle ammo
			if (m_PW.Shooting.EnableAmmo)
			{
				if (m_AmmoProperty != null)
				{
					int extraAmmo = m_AmmoProperty.Integer - m_PW.Shooting.MagazineSize;

					if (extraAmmo > 0)
						AddAmmoToInventory(extraAmmo);

					m_AmmoProperty.Integer = Mathf.Clamp(m_AmmoProperty.Integer, 0, m_PW.Shooting.MagazineSize);
				}
				else
					Debug.LogError($"Equipment item with name '{name}' has ammo enabled but no ammo property found on the item.");

				UpdateAmmoInfo();
			}

			// Handle Animations
			EHandler.Animator_SetFloat(animHash_FireSpeed, m_PW.Shooting.FireAnimationSpeed);
			EHandler.Animator_SetFloat(animHash_EmptyReloadSpeed, m_PW.Reloading.EmptyReloadAnimationSpeed);
			EHandler.Animator_SetFloat(animHash_ReloadSpeed, m_PW.Reloading.ReloadAnimationSpeed);
		}

		public override bool TryUseOnce(Ray[] itemUseRays, int useType)
		{
			bool canUse = false;

			//Shooting
			if (Time.time > m_NextTimeCanUse)
			{
				canUse = (CurrentAmmoInfo.Val.CurrentInMagazine > 0 || !m_PW.Shooting.EnableAmmo) && SelectedFireMode != (int)ProjectileWeaponInfo.FireMode.Safety;

				if (canUse)
				{
					if (SelectedFireMode == (int) ProjectileWeaponInfo.FireMode.Burst)
						StartCoroutine(C_DoBurst());
					else
						Shoot(itemUseRays);

					m_NextTimeCanUse = Time.time + (m_UseThreshold * Mathf.Clamp(1 / m_PW.Shooting.FireRateOverTime.Evaluate(EHandler.ContinuouslyUsedTimes / (float)m_PW.Shooting.MagazineSize), 0.1f, 10f));

					m_GeneralEvents.OnUse.Invoke();
				}
				else
				{
					//Play Empty/Dry fire sound
					if (!Player.Reload.Active)
					{
						EHandler.PlaySound(m_PW.Shooting.DryShootAudio, 1f);

						if (m_PW.Shooting.HasDryFireAnim)
						{
							EHandler.Animator_SetFloat(animHash_FireIndex, 4);
							EHandler.Animator_SetTrigger(animHash_Fire);
						}

						DryFire.Send();

						m_NextTimeCanUse = Time.time + 0.1f;
					}
				}		
			}

			return canUse;
		}

		public override bool TryUseContinuously(Ray[] itemUseRays, int useType)
		{
			//Used to prevent calling the Play empty/dry fire functionality in continuous mode
			if ((CurrentAmmoInfo.Val.CurrentInMagazine == 0 && m_PW.Shooting.EnableAmmo) || SelectedFireMode == (int)ProjectileWeaponInfo.FireMode.Safety)
				return false;

			if (SelectedFireMode == (int)ProjectileWeaponInfo.FireMode.FullAuto)
				return TryUseOnce(itemUseRays, useType);

			return false;
		}

		public override void OnAimStart()
		{
			base.OnAimStart();

			EHandler.PlaySound(m_PW.Aiming.AimSounds, 1f);
		}

		public virtual void Shoot(Ray[] itemUseRays)
		{
			// Shoot sound
			EHandler.PlaySound(m_PW.Shooting.ShootAudio, 1f);

			// Handling sounds
			EHandler.PlayDelayedSounds(m_PW.Shooting.HandlingAudio);

			// Shell drop sounds
			if (Player.IsGrounded.Get() == true && m_PW.Shooting.CasingDropAudio.Length > 0)
				EHandler.PlayDelayedSounds(m_PW.Shooting.CasingDropAudio);

			// Play Fire Animation 
			int fireIndex;

			if (!Player.Aim.Active)
			{
				fireIndex = m_CurrentFireAnimIndex == 0 ? 0 : 2;

				if (m_PW.Shooting.HasAlternativeFireAnim)
					m_CurrentFireAnimIndex = m_CurrentFireAnimIndex == 0 ? 1 : 0;
			}
			else
			{
				fireIndex = m_CurrentFireAnimIndex == 0 ? 1 : 3;

				if (m_PW.Shooting.HasAlternativeFireAnim)
					m_CurrentFireAnimIndex = m_CurrentFireAnimIndex == 0 ? 1 : 0;
			}

			EHandler.Animator_SetFloat(animHash_FireIndex, fireIndex);
			EHandler.Animator_SetTrigger(animHash_Fire);

			// Cam Forces
			Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Shooting.HandlingCamForces);

			// Ammo
			if (m_PW.Shooting.EnableAmmo)
				m_AmmoProperty.Integer -= 1;
		}

		public override bool TryStartReload()
		{
			if (m_ReloadLoopEndTime < Time.time && m_PW.Shooting.EnableAmmo && CurrentAmmoInfo.Val.CurrentInMagazine < m_PW.Shooting.MagazineSize)
			{
				m_AmmoToAdd = m_PW.Shooting.MagazineSize - CurrentAmmoInfo.Val.CurrentInMagazine;

				if (CurrentAmmoInfo.Val.CurrentInStorage < m_AmmoToAdd)
					m_AmmoToAdd = CurrentAmmoInfo.Val.CurrentInStorage;

				if (m_AmmoToAdd > 0)
				{
					EHandler.ClearDelayedSounds();

					if (CurrentAmmoInfo.Val.CurrentInMagazine == 0 && m_PW.Reloading.HasEmptyReload)
					{
						//Dry Reload
						if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Once)
							m_ReloadLoopEndTime = Time.time + m_PW.Reloading.EmptyReloadDuration;
						else if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Progressive)
							m_ReloadStartTime = Time.time + m_PW.Reloading.EmptyReloadDuration;

						EHandler.Animator_SetTrigger(animHash_EmptyReload);

						Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.EmptyReloadLoopCamForces);
						EHandler.PlayDelayedSounds(m_PW.Reloading.EmptyReloadSounds);
					}
					else
					{
						//Tactical Reload
						if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Once)
						{
							m_ReloadLoopEndTime = Time.time + m_PW.Reloading.ReloadDuration;
							EHandler.Animator_SetTrigger(animHash_Reload);

							Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadLoopCamForces);
							EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadSounds);
						}
						else if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Progressive)
						{
							m_ReloadStartTime = Time.time + m_PW.Reloading.ReloadStartDuration;
							EHandler.Animator_SetTrigger(animHash_StartReload);

							Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadStartCamForces);
							EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadStartSounds);
						}
					}

					if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Once)
						UpdateAmmoInfo();

					m_GeneralEvents.OnReload.Invoke(true); // Invoke the Reload Start Unity Event

					return true;
				}
			}

			return false;
		}

		//This method is called by the 'Equipment Handler' to check if the reload is finished
		public override bool IsDoneReloading()
		{
			if (!m_ReloadLoopStarted)
			{
				if (Time.time > m_ReloadStartTime)
				{
					if (CurrentAmmoInfo.Val.CurrentInMagazine == 0 && m_PW.Reloading.HasEmptyReload)
					{
						//Empty/Dry Reload
						m_ReloadLoopStarted = true;

						if (m_PW.Reloading.ProgressiveEmptyReload && m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Progressive)
						{
							if (m_AmmoToAdd > 1)
							{
								//Play the reload start State after the empty reload
								Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadStartCamForces);
								EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadStartSounds);

								m_ReloadLoopEndTime = Time.time + m_PW.Reloading.ReloadStartDuration;
								EHandler.Animator_SetTrigger(animHash_StartReload);
							}
							else
							{
								GetAmmoFromInventory(1);

								m_AmmoProperty.Integer++;
								m_AmmoToAdd--;

								return true;
							}
						}
					}
					else
					{
						//Tactical Reload
						m_ReloadLoopStarted = true;
						m_ReloadLoopEndTime = Time.time + m_PW.Reloading.ReloadDuration;

						Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadLoopCamForces);
						EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadSounds);

						EHandler.Animator_SetTrigger(animHash_Reload);
					}
				}

				return false;
			}

			if (m_ReloadLoopStarted && Time.time >= m_ReloadLoopEndTime)
			{
				if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Once || (CurrentAmmoInfo.Val.CurrentInMagazine == 0 && !m_PW.Reloading.ProgressiveEmptyReload))
				{
					m_AmmoProperty.Integer += m_AmmoToAdd;
					GetAmmoFromInventory(m_AmmoToAdd);

					m_AmmoToAdd = 0;
				}
				else if (m_PW.Reloading.ReloadType == ProjectileWeaponInfo.ReloadType.Progressive)
				{
					if (m_AmmoToAdd > 0)
					{
						GetAmmoFromInventory(1);

						m_AmmoProperty.Integer++;
						m_AmmoToAdd--;
					}

					if (m_AmmoToAdd > 0)
					{
						Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadLoopCamForces);
						EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadSounds);

						EHandler.Animator_SetTrigger(animHash_Reload);
						m_ReloadLoopEndTime = Time.time + m_PW.Reloading.ReloadDuration;
					}
					else if (!m_EndReload)
					{
						EHandler.Animator_SetTrigger(animHash_EndReload);
						m_EndReload = true;
						m_ReloadLoopEndTime = Time.time + m_PW.Reloading.ReloadEndDuration;

						Player.Camera.Physics.PlayDelayedCameraForces(m_PW.Reloading.ReloadEndCamForces);
						EHandler.PlayDelayedSounds(m_PW.Reloading.ReloadEndSounds);
					}
					else
						m_EndReload = false;
				}

				UpdateAmmoInfo();

				return !m_EndReload && m_AmmoToAdd == 0;
			}

			return false;
		}

		public override void OnReloadStop()
		{
			m_ReloadLoopEndTime = Time.time;
			m_EndReload = false;
			m_ReloadLoopStarted = false;

			EHandler.ClearDelayedSounds();

			m_GeneralEvents.OnReload.Invoke(false); // Invoke the Reload Stop Unity Event
		}

		public override void OnUseEnd()
		{
			//Play shoot tail sound
			if (m_PW.Shooting.ShootTailAudio != null)
				EHandler.PlayPersistentAudio(m_PW.Shooting.ShootTailAudio, 1f, ItemSelection.Method.RandomExcludeLast);
		}

		public override bool CanBeUsed()
		{
			return CurrentAmmoInfo.Get().CurrentInMagazine > 0 || !m_PW.Shooting.EnableAmmo;
		}

        protected virtual void OnEnable() => Player.Inventory.ContainerChanged.AddListener(OnInventoryChanged);
		protected virtual void OnDisable() => Player.Inventory.ContainerChanged.RemoveListener(OnInventoryChanged);
		protected int GetAmmoCount() => Player.Inventory.GetItemCount(m_AmmoTypeProperty.ItemId);
		protected int GetAmmoFromInventory(int amount) => Player.Inventory.RemoveItemsWithID(m_AmmoTypeProperty.ItemId, amount, ItemContainerFlags.Storage);
		protected int AddAmmoToInventory(int amount) => Player.Inventory.AddItem(m_AmmoTypeProperty.ItemId, amount, ItemContainerFlags.Storage);

		protected virtual void UpdateFireModeSettings(int selectedMode) 
		{
			if ((int)ProjectileWeaponInfo.FireMode.Burst == selectedMode)
				m_UseThreshold = m_PW.Shooting.BurstDuration + m_PW.Shooting.BurstPause;
			else if ((int)ProjectileWeaponInfo.FireMode.FullAuto == selectedMode)
				m_UseThreshold = 60f / m_PW.Shooting.RoundsPerMinute;
			else if ((int)ProjectileWeaponInfo.FireMode.SemiAuto == selectedMode)
				m_UseThreshold = m_PW.Shooting.FireDuration;
			else if ((int)ProjectileWeaponInfo.FireMode.Safety == selectedMode)
				m_UseThreshold = m_PW.Shooting.FireDuration;
		}

		private void OnInventoryChanged()
		{
			if (m_PW.Shooting.EnableAmmo)
			{
				// Recalculate ammo and update the UI
				UpdateAmmoInfo();
			}
		}

		private IEnumerator C_DoBurst()
		{
			for (int i = 0; i < m_PW.Shooting.BurstLength; i++)
			{
				if (!CanBeUsed())
					yield break;

				// Simple hack until a better solution is found
				Shoot(EHandler.GenerateItemUseRays(Player, EHandler.ItemUseTransform, GetUseRaysAmount(), GetUseRaySpreadMod()));
				yield return m_BurstWait;
			}
		}
		
		#if UNITY_EDITOR
        private void OnValidate()
        {
			if (EHandler != null)
			{
				if (m_PW.Shooting != null)
				{
					m_BurstWait = new WaitForSeconds(m_PW.Shooting.BurstDuration / m_PW.Shooting.BurstLength);

					UpdateFireModeSettings(SelectedFireMode);
				}

				EHandler.Animator_SetFloat(animHash_FireSpeed, m_PW.Shooting.FireAnimationSpeed);
				EHandler.Animator_SetFloat(animHash_EmptyReloadSpeed, m_PW.Reloading.EmptyReloadAnimationSpeed);
				EHandler.Animator_SetFloat(animHash_ReloadSpeed, m_PW.Reloading.ReloadAnimationSpeed);
			}
		}
		#endif
    }
}