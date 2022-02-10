using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.Equipment
{
	public class HealsManager : PlayerComponent
	{
		[SerializeField]
		[PlayerItemContainer]
		private string m_HealsContainerName = "Backpack";

		[SerializeField]
		private bool m_HealWhileRunning = false;

		[SerializeField]
		private bool m_HealWithMaxHealth = false;

		private ItemContainer m_HealsContainer;


		private void Start()
        {
			Player.Healing.SetStartTryer(TryStart_Healing);
			Player.Healing.AddStopListener(OnStop_Healing);

			m_HealsContainer = Player.Inventory.GetContainerWithName(m_HealsContainerName);
		}

        private bool TryStart_Healing()
		{
			if ((Player.Run.Active && !m_HealWhileRunning ||
				(Player.Health.Get() >= 100 && !m_HealWithMaxHealth)) ||
				Player.Healing.Active)
				return false;

			bool startedHealing = false;

			Item healingItem = TryGetHealingItem();

			if (healingItem != null)
			{
				startedHealing = true;

				if (Player.Reload.Active)
					Player.Reload.ForceStop();

				if (Player.Aim.Active)
					Player.Aim.ForceStop();

				Player.EquipItem.Try(healingItem, false);
			}

			return startedHealing;
		}

		private void OnStop_Healing()
		{
			Player.Inventory.RemoveItemsWithName(Player.EquippedItem.Val.Info.Name, 1, ItemContainerFlags.Storage);
			Player.EquipItem.Try(Player.EquippedItem.GetPreviousValue(), false);
		}

		private Item TryGetHealingItem()
		{
			foreach (var slot in m_HealsContainer.Slots)
			{
				if (slot.HasItem)
					return slot.Item;
			}

			return null;
		}
	}
}
