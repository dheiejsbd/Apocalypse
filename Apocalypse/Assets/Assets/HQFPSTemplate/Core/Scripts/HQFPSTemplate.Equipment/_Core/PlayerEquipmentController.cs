using HQFPSTemplate.Items;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    /// <summary>
    /// Main component that controls how the first person equipment should behave
    /// </summary>
    public class PlayerEquipmentController : PlayerComponent
	{
        public EquipmentHandler ActiveEHandler { get; private set; }

		[SerializeField]
		private Camera m_FPCamera;

		[SerializeField]
		private bool m_AimWhileReloading;

		[SerializeField]
		private bool m_ReloadWhileProne;

		[SerializeField]
		private bool m_AutoReloadOnEmpty = true;

		[Space]
		
		[SerializeField]
		private EquipmentHandler[] m_EquipmentHandlers = null;

		private float m_NextTimeCanAutoReload;
		private float m_NextTimeToEquip;
		private bool m_WaitingToEquip;


        private void Awake()
		{
			Player.EquipItem.SetTryer(TryChangeItem);
			Player.SwapItem.SetTryer(TrySwapItems);
			Player.DestroyEquippedItem.SetTryer(TryDestroyHeldItem);
			
			Player.Death.AddListener(OnDeath);

			Player.UseItem.SetTryer(TryUse);

			Player.Aim.SetStartTryer(TryStartAim);
			Player.Aim.AddStopListener(OnAimStop);

			Player.Reload.SetStartTryer(OnReloadStart);
			Player.Reload.AddStopListener(OnReloadStop);

			Player.ChangeUseMode.SetTryer(TryChangeUseMode);

			Player.ObjectInProximity.AddChangeListener(OnChanged_ObjectInProximity);
			Player.IsGrounded.AddChangeListener(OnGroundedChange);

			ActiveEHandler = m_EquipmentHandlers[0];
		}
		
		private void OnDeath() 
		{
			m_NextTimeCanAutoReload = 0f;
			m_NextTimeToEquip = 0f;
			m_WaitingToEquip = false;

			ActiveEHandler.Reset();
	}

        private void Update()
		{
			if (Player.Reload.Active)
			{
				bool endedReloading = ActiveEHandler.IsDoneReloading();

				if (endedReloading)
					Player.Reload.ForceStop();
			}

			//Equip the new item after the previous one has been unequipped
			if (m_WaitingToEquip && Time.time > m_NextTimeToEquip)
			{
				Equip(Player.EquippedItem.Get());
				m_WaitingToEquip = false;
			}
		}

        private bool TryChangeUseMode()
		{
			if (Player.Reload.Active || Player.Run.Active || ActiveEHandler.EquipmentItem == null)
				return false;

			return Player.ActiveEquipmentItem.Get().TryChangeUseMode();
		}

		private bool TryChangeItem(Item item, bool instantly)
		{
			if (Player.EquippedItem.Get() == item && item != null)
				return false;

			ChangeItem(item, instantly);

			return true;
		}

		private void ChangeItem(Item item, bool instantly)
		{
			// Register the equipment item for equipping
			m_WaitingToEquip = true;

			// Register the current equipped item for disabling
			if (ActiveEHandler.EquipmentItem != null)
			{
				if (ActiveEHandler.UsingItem.Active)
				{
					ActiveEHandler.UsingItem.ForceStop();
					ActiveEHandler.EquipmentItem.OnUseEnd();
				}

				if (Player.Aim.Active)
					Player.Aim.ForceStop();

				if (Player.Reload.Active)
					Player.Reload.ForceStop();

				ActiveEHandler.UnequipItem();

				if (!instantly)
					m_NextTimeToEquip = Time.time + ActiveEHandler.EquipmentItem.EInfo.Unequipping.Duration;
			}

			Player.EquippedItem.Set(item);
		}

		private bool TryDestroyHeldItem()
		{
			if (Player.EquippedItem.Get() == null)
				return false;
			else
			{
				Player.Inventory.RemoveItem(Player.EquippedItem.Get());
				Player.EquipItem.Try(null, true);

				return true;
			}
		}

		private bool TrySwapItems(Item item)
		{
			Item currentItem = Player.EquippedItem.Get();

			if (currentItem != null && ContainsEquipmentItem(item)) // Check if the passed item is swappable
			{
				ItemSlot itemSlot = Player.Inventory.GetItemSlot(currentItem);

				if (Player.DropItem.Try(currentItem))
				{
					Player.DestroyEquippedItem.Try();
					Player.EquipItem.Try(item, true);

					itemSlot.SetItem(item);

					return true;
				}
			}

			return false;
		}

		private void Equip(Item item)
		{
			// Search for the equipment item that corresponds with 'item'
			for (int i = 0; i < m_EquipmentHandlers.Length; i++)
            {
				if (item != null)
				{
					if (m_EquipmentHandlers[i].ContainsEquipmentItem(item.Id))
					{
						ActiveEHandler = m_EquipmentHandlers[i];
						break;
					}
				}
			}

			if (Player.Aim.Active)
				Player.Aim.ForceStop();

			if (Player.Reload.Active)
				Player.Reload.ForceStop();

			ActiveEHandler.EquipItem(item);

			Player.ActiveEquipmentItem.Set(ActiveEHandler.EquipmentItem);
			m_FPCamera.fieldOfView = ActiveEHandler.EquipmentItem.EModel.TargetFOV;
		}

		private bool TryUse(bool continuously, int useIndex)
		{
			var eItem = ActiveEHandler.EquipmentItem;
			float staminaTakePerUse = eItem.EInfo.General.StaminaTakePerUse;
			bool eItemCanBeUsed = eItem.CanBeUsed();

			// Interrupt the reload if possible
			if (!continuously && Player.Reload.Active && eItem.EInfo.General.CanStopReloading && eItemCanBeUsed)
				Player.Reload.ForceStop();

			if (CanUseItem(eItem))
			{
				bool usedSuccessfully = ActiveEHandler.TryUse(continuously, useIndex);

				if (usedSuccessfully)
				{
					if(staminaTakePerUse > 0f)
						Player.Stamina.Set(Mathf.Max(Player.Stamina.Get() - staminaTakePerUse, 0f));

					m_NextTimeCanAutoReload = Time.time + 0.35f;
				}

				//Try reload the item if the item can't be used (e.g. out of ammo) and 'Reload on empty' is active
				if (!eItemCanBeUsed && m_AutoReloadOnEmpty && !continuously && m_NextTimeCanAutoReload < Time.time)
					Player.Reload.TryStart();

				return usedSuccessfully;
			}

			return false;
		}

		private void OnGroundedChange(bool grounded) => ActiveEHandler.OnGroundedChange(grounded);

		private bool TryStartAim()
		{
			if (Player.Run.Active ||
				Player.Reload.Active ||
				(!m_AimWhileReloading && Player.Aim.Active))
				return false;

			return ActiveEHandler.TryStartAim();
		}

		private void OnAimStop() => ActiveEHandler.OnAimStop();

		private bool CanUseItem(EquipmentItem eItem)
		{
			if (eItem != null)
			{
				float staminaTakePerUse = eItem.EInfo.General.StaminaTakePerUse;

				bool airborneCondition = Player.IsGrounded.Get() || eItem.EInfo.General.UseWhileAirborne;
				bool runningCondition = !Player.Run.Active || eItem.EInfo.General.UseWhileRunning;
				bool staminaCondition = staminaTakePerUse == 0f || Player.Stamina.Get() > staminaTakePerUse;

				return airborneCondition && staminaCondition && runningCondition && !Player.Reload.Active;
			}

			return false;
		}

		private bool OnReloadStart()
		{
			//Reload-start conditions
			if (Player.Prone.Active && !m_ReloadWhileProne)
				return false;

			bool reloadStarted = ActiveEHandler.TryStartReload();

			if (reloadStarted)
			{
				if (Player.Aim.Active && !m_AimWhileReloading)
					Player.Aim.ForceStop();
			}

			return reloadStarted;
		}

		private void OnReloadStop() => ActiveEHandler.OnReloadStop();

		private bool ContainsEquipmentItem(Item item)
		{
			foreach (var handler in m_EquipmentHandlers)
			{
				if (handler.ContainsEquipmentItem(item.Id))
					return true;
			}

			return false;
		}

		private void OnChanged_ObjectInProximity(Collider col)
		{
			if (ActiveEHandler.EquipmentItem != null && Player.ObjectInProximity.Get() && Player.Aim.Active)
				Player.Aim.ForceStop();
		}
	}
}