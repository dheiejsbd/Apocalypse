using System.Collections.Generic;
using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate
{
	public class Inventory : EntityComponent
	{
		public Message ContainerChanged = new Message();

		public List<ItemContainer> Containers
		{
			get
			{ 
				if(m_AllContainers == null)
					InitiateContainers();

				return m_AllContainers;
			} 
		}

		public ContainerGenerator[] StartupContainers { get => m_InitialContainers.ToArray(); }

		[SerializeField]
		[Reorderable]
		private ContainerGeneratorList m_InitialContainers = null;

		private List<ItemContainer> m_SavableContainers;
		private List<ItemContainer> m_AllContainers;


		public string[] GetAllContainerNames() 
		{
			string[] containerNames = new string[m_InitialContainers.Count];

            for (int i = 0; i < containerNames.Length; i++)
            {
				containerNames[i] = m_InitialContainers[i].Name;
			}

			return containerNames;
		}

		public void AddContainer(ItemContainer itemContainer, bool add)
		{
			if(add && !Containers.Contains(itemContainer))
			{
				Containers.Add(itemContainer);
				AddListeners(itemContainer, true);
			}
			else if(!add && Containers.Contains(itemContainer))
			{
				Containers.Remove(itemContainer);
				AddListeners(itemContainer, false);
			}
		}

		public bool HasContainerWithFlags(ItemContainerFlags flags)
		{
			for(int i = 0;i < Containers.Count;i ++)
				if(flags.HasFlag(Containers[i].Flag))
					return true;

			return false;
		}

		public ItemContainer GetContainerWithFlags(ItemContainerFlags flags)
		{
			for(int i = 0;i < Containers.Count;i ++)
				if(flags.HasFlag(Containers[i].Flag))
					return Containers[i];

			return null;
		}

		public ItemContainer GetContainerWithName(string name)
		{
			for(int i = 0;i < Containers.Count;i ++)
				if(Containers[i].Name == name)
					return Containers[i];

			return null;
		}

		public bool AddItem(Item item, ItemContainerFlags flags)
		{
			for(int i = 0;i < Containers.Count;i ++)
			{
				if(flags.HasFlag(Containers[i].Flag))
				{
					bool added = Containers[i].AddItem(item);

					if(added)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// </summary>
		public int AddItem(string itemName, int amountToAdd, ItemContainerFlags flags)
		{
			int addedInTotal = 0;

			for(int i = 0;i < Containers.Count;i ++)
			{
				if(flags.HasFlag(m_AllContainers[i].Flag))
				{
					int addedNow = Containers[i].AddItem(itemName, amountToAdd);
					addedInTotal += addedNow;
					if(addedNow == addedInTotal)
						return addedInTotal;
				}
			}

			return addedInTotal;
		}

		public int AddItem(int id, int amountToAdd, ItemContainerFlags flags)
		{
			int addedInTotal = 0;

			for (int i = 0; i < Containers.Count; i++)
			{
				if (flags.HasFlag(m_AllContainers[i].Flag))
				{
					int addedNow = Containers[i].AddItem(id, amountToAdd);
					addedInTotal += addedNow;
					if (addedNow == addedInTotal)
						return addedInTotal;
				}
			}

			return addedInTotal;
		}

		public bool RemoveItem(Item item)
		{
			for(int i = 0;i < Containers.Count;i ++)
			{
				if(m_AllContainers[i].RemoveItem(item))
					return true;
			}

			return false;
		}

		public int RemoveItemsWithID(int id, int amountToRemove, ItemContainerFlags flags)
		{
			int removedInTotal = 0;

			for (int i = 0; i < Containers.Count; i++)
			{
				if (flags.HasFlag(Containers[i].Flag))
				{
					int removedNow = Containers[i].RemoveItem(id, amountToRemove);
					removedInTotal += removedNow;

					if (removedInTotal == amountToRemove)
						break;
				}
			}

			return removedInTotal;
		}

		public int RemoveItemsWithName(string itemName, int amountToRemove, ItemContainerFlags flags)
		{
			int removedInTotal = 0;

			for (int i = 0; i < Containers.Count; i++)
			{
				if (flags.HasFlag(Containers[i].Flag))
				{
					int removedNow = Containers[i].RemoveItem(itemName, amountToRemove);
					removedInTotal += removedNow;

					if (removedInTotal == amountToRemove)
						break;
				}
			}

			return removedInTotal;
		}

		/// <summary>
		/// Counts all the items with name itemName, from all containers.
		/// </summary>
		public int GetItemCount(string itemName)
		{
			int count = 0;
			for(int i = 0;i < Containers.Count;i ++)
				count += Containers[i].GetItemCount(itemName);

			return count;
		}

		/// <summary>
		/// Counts all the items with name itemName, from all containers.
		/// </summary>
		public int GetItemCount(int id)
		{
			int count = 0;
			for (int i = 0; i < Containers.Count; i++)
				count += Containers[i].GetItemCount(id);

			return count;
		}

		public ItemSlot GetItemSlot(Item item)
		{
			foreach (var container in m_SavableContainers)
			{
				foreach (ItemSlot slot in container)
				{
					if (slot.Item == item)
						return slot;
				}
			}

			return null;
		}

		private void Awake()
		{
			if(ItemDatabase.Instance == null)
			{
				Debug.LogError("No ItemDatabase found, this storage component will be disabled!", this);
				enabled = false;

				return;
			}
	
			InitiateContainers();
		}

		private void InitiateContainers()
		{
			m_SavableContainers = new List<ItemContainer>();

			for (int i = 0; i < m_InitialContainers.Count; i++)
			{
				var container = m_InitialContainers[i].GenerateContainer(transform);
				m_SavableContainers.Add(container);

				AddListeners(container, true);
			}

			m_AllContainers = new List<ItemContainer>(m_SavableContainers);
		}

		private void AddListeners(ItemContainer container, bool add)
		{
			if (add)
				container.Changed.AddListener(OnContainerChanged);
			else
				container.Changed.RemoveListener(OnContainerChanged);
		}

		private void OnContainerChanged(ItemSlot slot)
		{
			try
			{
				ContainerChanged.Send();
			}
			catch {
			};
		}
	}
}
