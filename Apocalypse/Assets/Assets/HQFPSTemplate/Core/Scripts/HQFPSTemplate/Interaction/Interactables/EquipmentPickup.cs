using UnityEngine;

namespace HQFPSTemplate
{
	public class EquipmentPickup : ItemPickup
	{
		protected override void TryPickUp(Humanoid humanoid, float interactProgress)
		{
			if (m_ItemInstance != null)
			{
				// Check if this Item is swappable
				if (interactProgress > 0.45f && humanoid.SwapItem.Try(m_ItemInstance))
					Destroy(gameObject);
				else
				{
					bool addedItem;

					// If the currently equipped item is null,
					// get the attached slot
					if (humanoid.EquippedItem.Get() == null)
					{
						var itemContainer = humanoid.Inventory.GetContainerWithFlags(m_TargetContainers);
						var selectedSlot = itemContainer.Slots[itemContainer.SelectedSlot.Get()];

						if (selectedSlot.Item == null)
							selectedSlot.SetItem(m_ItemInstance);

						addedItem = true;
					}
					else
						addedItem = humanoid.Inventory.AddItem(m_ItemInstance, m_TargetContainers);

					// Item added to inventory
					if (addedItem)
					{
						if (m_ItemInstance.Info.StackSize > 1)
							UI_MessageDisplayer.Instance.PushMessage(string.Format("Picked up <color={0}>{1}</color> x {2}", ColorUtils.ColorToHex(m_ItemCountColor), m_ItemInstance.Name, m_ItemInstance.CurrentStackSize), m_BaseMessageColor);
						else
							UI_MessageDisplayer.Instance.PushMessage(string.Format("Picked up <color={0}>{1}</color>", ColorUtils.ColorToHex(m_ItemCountColor), m_ItemInstance.Name), m_BaseMessageColor);

						Destroy(gameObject);
					}
					// Item not added to inventory
					else
					{
						UI_MessageDisplayer.Instance.PushMessage(string.Format("<color={0}>Inventory Full</color>", ColorUtils.ColorToHex(m_InventoryFullColor)), m_BaseMessageColor);
					}
				}
			}
			else
			{
				Debug.LogError("Item Instance is null, can't pick up anything.");
				return;
			}
		}
	}
}
