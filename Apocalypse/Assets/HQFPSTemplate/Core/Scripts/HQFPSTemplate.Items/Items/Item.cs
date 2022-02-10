using System;
using UnityEngine;

namespace HQFPSTemplate.Items
{
	/// <summary>
	/// Item Instance
	/// </summary>
	[Serializable]
	public class Item
	{
		[NonSerialized]
		public Message<ItemProperty> PropertyChanged = new Message<ItemProperty>();

		[NonSerialized]
		public Message StackChanged = new Message();

		public ItemInfo Info { get { return ItemDatabase.GetItemById(m_Id); } }

		public int Id { get => m_Id; }
		public string Name { get { return m_Name; } }

		public int CurrentStackSize
		{
			get
			{
				return m_CurrentStackSize;
			}
			set
			{
				int oldStackSize = m_CurrentStackSize;
				m_CurrentStackSize = value;

				if(m_CurrentStackSize != oldStackSize)
					StackChanged.Send();
			}
		}

		public ItemProperty[] Properties { get => m_Properties; }

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_CurrentStackSize;

		[SerializeField]
		private ItemProperty[] m_Properties;


		public static implicit operator bool(Item item)
		{
			return item != null;
		}

		public static Item Create(string name, int count = 1)
		{
			ItemInfo itemInfo = null;

			if(ItemDatabase.Instance != null)
				itemInfo = ItemDatabase.GetItemByName(name);
			else
				Debug.LogWarning("Can't create item with name '" + name + "'. It doesn't exist in the database!");

			if(itemInfo != null)
				return new Item(itemInfo, count);
			else
				return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public Item(ItemInfo itemInfo, int count = 1, ItemProperty[] customProperties = null)
		{
			m_Id = itemInfo.Id;
			m_Name = itemInfo.Name;
		
			CurrentStackSize = Mathf.Clamp(count, 1, itemInfo.StackSize);

			if(customProperties != null)
				m_Properties = CloneProperties(customProperties);
			else
				m_Properties = InstantiateProperties(itemInfo.Properties);

			for(int i = 0;i < m_Properties.Length;i++)
				m_Properties[i].Changed.AddListener(OnPropertyChanged);
		}

		public bool HasProperty(string name)
		{
			for(int i = 0;i < m_Properties.Length;i++)
			{
				if(m_Properties[i].Name == name)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty GetProperty(string name)
		{
			ItemProperty itemProperty = null;

			for(int i = 0;i < m_Properties.Length;i++)
			{
				if(m_Properties[i].Name == name)
				{
					itemProperty = m_Properties[i];
					break;
				}
			}

			return itemProperty;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public bool TryGetProperty(string name, out ItemProperty itemProperty)
		{
			itemProperty = null;

			for(int i = 0;i < m_Properties.Length;i++)
			{
				if(m_Properties[i].Name == name)
				{
					itemProperty = m_Properties[i];
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			return "Item Name: " + m_Name + " | Stack Size: " + m_CurrentStackSize;
		}

		private ItemProperty[] CloneProperties(ItemProperty[] properties)
		{
			ItemProperty[] clonedProperties = new ItemProperty[properties.Length];

			for(int i = 0;i < properties.Length;i++)
				clonedProperties[i] = properties[i].GetMemberwiseClone();

			return clonedProperties;
		}

		private ItemProperty[] InstantiateProperties(ItemPropertyInfoList propertyInfos)
		{
			ItemProperty[] properties = new ItemProperty[propertyInfos.Length];

			for(int i = 0;i < propertyInfos.Length;i++)
				properties[i] = new ItemProperty(propertyInfos[i]);

			return properties;
		}

        private void OnPropertyChanged(ItemProperty itemProperty)
		{
			PropertyChanged.Send(itemProperty);
		}
	}
}