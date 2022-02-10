using System;
using UnityEngine;

namespace HQFPSTemplate.Items
{
	public enum SlotChangeType
	{
		ItemChanged,
		StackChanged,
		PropertyChanged
	}

	[Serializable]
	public class ItemSlot
	{
		/// <summary>Sent when this slot has changed (e.g. when the item has changed).</summary>
		public Message<ItemSlot, SlotChangeType> Changed = new Message<ItemSlot, SlotChangeType>();

		public bool HasItem { get { return Item != null; } }
		public Item Item { get { return m_Item; } }

        [SerializeField]
		private Item m_Item;


		public static implicit operator bool(ItemSlot slot) 
		{
			return slot != null;
		}

		public void OnDeserialization(object sender)
		{
			Changed = new Message<ItemSlot, SlotChangeType>();

			if(Item)
			{
				if(Item.PropertyChanged == null)
				{
					Item.PropertyChanged = new Message<ItemProperty>();
					Item.StackChanged = new Message();
				}
				
				Item.PropertyChanged.AddListener(OnPropertyChanged);
				Item.StackChanged.AddListener(OnStackChanged);
			}
		}

		public void SetItem(Item item)
		{
			Item oldItem = Item;

			if(Item != null)
			{
				Item.PropertyChanged.RemoveListener(OnPropertyChanged);
				Item.StackChanged.RemoveListener(OnStackChanged);
			}

			m_Item = item;

			if(Item != null)
			{
				Item.PropertyChanged.AddListener(OnPropertyChanged);
				Item.StackChanged.AddListener(OnStackChanged);
			}

			if(m_Item != oldItem)
				Changed.Send(this, SlotChangeType.ItemChanged);
        }

		public int RemoveFromStack(int amount)
		{
			if(!HasItem)
				return 0;

			if(amount >= Item.CurrentStackSize)
			{
				int stackSize = Item.CurrentStackSize;
				SetItem(null);

				return stackSize;
			}

			int oldStack = Item.CurrentStackSize;
			Item.CurrentStackSize = Mathf.Max(Item.CurrentStackSize - amount, 0);

			if(oldStack != Item.CurrentStackSize)
				Changed.Send(this, SlotChangeType.StackChanged);

			return oldStack - Item.CurrentStackSize;
		}

		public int AddToStack(int amount)
		{
			if(!HasItem || Item.Info.StackSize <= 1)
				return 0;

			int oldStackCount = Item.CurrentStackSize;
			int surplus = amount + oldStackCount - Item.Info.StackSize;
			int currentStackCount = oldStackCount;

			if(surplus <= 0)
				currentStackCount += amount;
			else
				currentStackCount = Item.Info.StackSize;

			Item.CurrentStackSize = currentStackCount;

			return currentStackCount - oldStackCount;
		}

		private void OnPropertyChanged(ItemProperty itemProperty)
		{
			Changed.Send(this, SlotChangeType.PropertyChanged);
		}

		private void OnStackChanged()
		{
			Changed.Send(this, SlotChangeType.StackChanged);
		}
	}
}
