using System;
using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.Items
{
	[Serializable]
	public class ItemContainer : IEnumerable
	{
		public ItemSlot this[int i] { get { return m_Slots[i]; } set { m_Slots[i] = value; } }

		/// <summary>Slot count.</summary>
		public int Count { get { return m_Slots.Length; } }

		/// <summary>
		/// Useful for component syncing (e.g. item wheel and equipment selection)
		/// </summary>
		public Value<int> SelectedSlot = new Value<int>();

		public ItemSlot[] Slots { get { return m_Slots; } }

		public string Name { get { return m_Name; } }

		public ItemContainerFlags Flag { get { return m_Flag; } }
		public Transform Transform { get { return m_Transform; } }

		public Message<ItemSlot> Changed = new Message<ItemSlot>();

		private Transform m_Transform;

        [SerializeField]
		private string m_Name;

        [SerializeField]
        private ItemContainerFlags m_Flag;

        [SerializeField]
        private ItemSlot[] m_Slots;

		[SerializeField]
		private bool m_OneStackPerItem;

		[SerializeField]
        private string[] m_ValidCategories;

        [SerializeField]
        private string[] m_RequiredProperties;


		public ItemContainer(string name, int size, Transform transform, ItemContainerFlags flag, bool oneStackPerItem, string[] validCategories, string[] requiredProperties)
		{
			m_Name = name;
			m_Slots = new ItemSlot[size];

			for(int i = 0;i < m_Slots.Length;i ++)
			{
				m_Slots[i] = new ItemSlot();
				m_Slots[i].Changed.AddListener(OnSlotChanged);
			}

			m_Transform = transform;
			m_Flag = flag;
			m_OneStackPerItem = oneStackPerItem;
			m_ValidCategories = validCategories;
			m_RequiredProperties = requiredProperties;

			SelectedSlot.SetFilter(FilterSelectedSlotIndex);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Slots.GetEnumerator();
		}

		//Filter for the Selected Slot (clamps its value to the slots array size)
		private int FilterSelectedSlotIndex(int prevIndex, int newIndex) => Mathf.Clamp(newIndex, 0, Slots.Length - 1);

		public int AddItem(ItemInfo itemInfo, int amount, ItemProperty[] customProperties = null)
		{
			if(itemInfo == null || !AllowsItem(itemInfo))
				return 0;

			int added = 0;

			if (m_OneStackPerItem)
			{
				int slotIndex = GetSlotIndexForItem(itemInfo.Id);

				if (slotIndex == -1)
				{
                    for (int i = 0; i < m_Slots.Length; i++)
                    {
						if (!m_Slots[i].HasItem)
						{
							added += AddItemToSlot(m_Slots[i], itemInfo, amount, customProperties);
							break;
						}
                    }
				}
				else
					added += AddItemToSlot(m_Slots[slotIndex], itemInfo, amount, customProperties);
			}
			else
			{
				// Go through each slot and see where we can add the item(s)
				for (int i = 0; i < m_Slots.Length; i++)
				{
					added += AddItemToSlot(m_Slots[i], itemInfo, amount, customProperties);

					// We've added all the items, we can stop now
					if (added == amount)
						return added;
				}
			}

			return added;
		}

		public int AddItem(string name, int amount, ItemProperty[] customProperties = null)
		{
			ItemInfo itemInfo;

			if (!ItemDatabase.TryGetItemByName(name, out itemInfo) || !AllowsItem(itemInfo))
				return 0;

			return AddItem(itemInfo, amount, customProperties);
		}

		public int AddItem(int id, int amount, ItemProperty[] customProperties = null)
		{
			ItemInfo itemInfo;

			if (!ItemDatabase.TryGetItemById(id, out itemInfo) || !AllowsItem(itemInfo))
				return 0;

			return AddItem(itemInfo, amount, customProperties);
		}

		public bool AddItem(Item item)
		{
			if(AllowsItem(item))
			{
				if (item.Info.StackSize > 1)
					return AddItem(item.Info, item.CurrentStackSize, item.Properties) > 0;
				else
				{
					// The item's not stackable, try find an empty slot for it
					for(int i = 0;i < m_Slots.Length;i ++)
					{
						if(!m_Slots[i].HasItem)
						{
							m_Slots[i].SetItem(item);
							return true;
						}
					}

					return false;
				}
			}
			else
				return false;
		}

		public bool AddOrSwap(ItemContainer slotParent, ItemSlot slot)
		{
			if(!slot.HasItem)
				return false;

			Item item = slot.Item;

			if(AllowsItem(item))
			{
				// Go through each slot and see where we can add the item
				for(int i = 0;i < m_Slots.Length;i++)
				{
					if(!m_Slots[i].HasItem)
					{
						m_Slots[i].SetItem(item);
						slot.SetItem(null);

						return true;
					}
				}

				if(slotParent.AllowsItem(m_Slots[0].Item))
				{
					Item tempItem = m_Slots[0].Item;
					m_Slots[0].SetItem(item);
					slot.SetItem(tempItem);

					return true;
				}

				return false;
			}
			else
				return false;
		}

		public int RemoveItem(string name, int amount)
		{
			int removed = 0;

			for(int i = 0;i < m_Slots.Length;i ++)
			{
				if(m_Slots[i].HasItem && m_Slots[i].Item.Name == name)
				{
					removed += m_Slots[i].RemoveFromStack(amount - removed);

					// We've removed all the items, we can stop now
					if(removed == amount)
						return removed;
				}
			}

			return removed;
		}

		public int RemoveItem(int id, int amount)
		{
			int removed = 0;

			for (int i = 0; i < m_Slots.Length; i++)
			{
				if (m_Slots[i].HasItem && m_Slots[i].Item.Id == id)
				{
					removed += m_Slots[i].RemoveFromStack(amount - removed);

					// We've removed all the items, we can stop now
					if (removed == amount)
						return removed;
				}
			}

			return removed;
		}

		public bool RemoveItem(Item item)
		{
			for(int i = 0;i < m_Slots.Length;i ++)
				if(m_Slots[i].Item == item)
				{
					m_Slots[i].SetItem(null);
					return true;
				}

			return false;
		}

		public bool ContainsItem(Item item)
		{
			for(int i = 0;i < m_Slots.Length;i ++)
				if(m_Slots[i].Item == item)
					return true;

			return false;
		}

		public int GetItemCount(string name)
		{
			int count = 0;

			for(int i = 0;i < m_Slots.Length;i ++)
			{
				if(m_Slots[i].HasItem && m_Slots[i].Item.Name == name)
					count += m_Slots[i].Item.CurrentStackSize;
			}

			return count;
		}

		public int GetItemCount(int id)
		{
			int count = 0;

			for (int i = 0; i < m_Slots.Length; i++)
			{
				if (m_Slots[i].HasItem && m_Slots[i].Item.Id == id)
					count += m_Slots[i].Item.CurrentStackSize;
			}

			return count;
		}

		public bool AllowsItem(Item item) => AllowsItem(item.Info);

		public bool AllowsItem(ItemInfo itemInfo)
		{
			// Check category
			bool isFromValidCategories = m_ValidCategories == null || m_ValidCategories.Length == 0;

			if(m_ValidCategories != null)
			{
				for(int i = 0;i < m_ValidCategories.Length;i ++)
				{
					if(m_ValidCategories[i] == itemInfo.Category)
						isFromValidCategories = true;
				}
			}
		
			if(!isFromValidCategories)
				return false;

			// Check properties
			if(m_RequiredProperties != null)
			{
				for(int i = 0;i < m_RequiredProperties.Length;i ++)
				{
					bool hasProperty = false;

					for(int p = 0;p < itemInfo.Properties.Length;p ++)
					{
						if(itemInfo.Properties[p].Name == m_RequiredProperties[i])
						{
							hasProperty = true;
							break;
						}
					}

					if(!hasProperty)
						return false;
				}
			}
			
			return true;
		}

		private int GetSlotIndexForItem(int id)
		{
			for (int i = 0; i < m_Slots.Length; i++)
			{
				if (m_Slots[i].HasItem)
				{
					if (m_Slots[i].Item.Id == id)
						return i;
				}
			}

			return -1;
		}


		private int AddItemToSlot(ItemSlot slot, ItemInfo itemInfo, int amount, ItemProperty[] properties = null)
		{
			if (slot.HasItem && itemInfo.Name != slot.Item.Name)
				return 0;

			bool wasEmpty = false;

			if (!slot.HasItem)
			{
				slot.SetItem(new Item(itemInfo, 1, properties));
				amount--;
				wasEmpty = true;
			}

			int addedToStack = slot.AddToStack(amount);

			return addedToStack + (wasEmpty ? 1 : 0);
		}

		public int GetPositionOfItem(Item item)
		{
			if (item == null)
				return -1;

			for (int i = 0; i < m_Slots.Length; i++)
				if (m_Slots[i].Item == item)
					return i;

			return -1;
		}

		private void OnSlotChanged(ItemSlot slot, SlotChangeType changeType)
		{
			Changed.Send(slot);
		}
	}
}