using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Items
{
	/// <summary>
	/// Represents an asset that stores all the user-defined items.
	/// </summary>
	[CreateAssetMenu(menuName = "HQ FPS Template/Item Database")]
	public class ItemDatabase : AssetSingleton<ItemDatabase>
	{
		public static bool AssetExists { get => Instance != null; }

		public ItemCategory[] Categories { get { return m_Categories; } }

		[SerializeField]
		private ItemCategory[] m_Categories;

		[SerializeField]
		[Reorderable]
		private ItemPropertyDefinitionList m_ItemProperties;

		private List<ItemInfo> m_Items = new List<ItemInfo>();
		private Dictionary<int, ItemInfo> m_ItemsById = new Dictionary<int, ItemInfo>();
		private Dictionary<string, ItemInfo> m_ItemsByName = new Dictionary<string, ItemInfo>();


		public static ItemInfo GetItemAtIndex(int index)
		{
			List<ItemInfo> items = Instance.m_Items;

			if(items != null && items.Count > 0)
				return items[Mathf.Clamp(index, 0, items.Count - 1)];
			else
				return null;
		}

		public static int IndexOfItem(int itemId)
		{
			List<ItemInfo> items = Instance.m_Items;

			for(int i = 0;i < items.Count;i++)
			{
				if(items[i].Id == itemId)
					return i;
			}

			return -1;
		}

		public static bool TryGetItemByName(string name, out ItemInfo itemInfo)
		{
			itemInfo = GetItemByName(name);

			return itemInfo != null;
		}

		public static bool TryGetItemById(int id, out ItemInfo itemInfo)
		{
			itemInfo = GetItemById(id);

			return itemInfo != null;
		}

		public static ItemInfo GetItemByName(string name)
		{
			if(Instance == null)
			{
				Debug.LogError("No item database asset found in the Resources folder!");
				return null;
			}

			if(Instance.m_ItemsByName.TryGetValue(name, out ItemInfo itemInfo))
				return itemInfo;
			else
				return null;
		}

		public static ItemInfo GetItemById(int id)
		{
			if(Instance == null)
			{
				Debug.LogError("No item database asset found in the Resources folder!");
				return null;
			}

			if(Instance.m_ItemsById.TryGetValue(id, out ItemInfo itemInfo))
				return itemInfo;
			else
				return null;
		}

		public static List<string> GetItemNames()
		{
			List<string> names = new List<string>();

			for(int i = 0;i < Instance.m_Categories.Length;i++)
			{
				var category = Instance.m_Categories[i];

				for(int j = 0;j < category.Items.Length;j++)
					names.Add(category.Items[j].Name);
			}

			return names;
		}

		public static List<string> GetItemNamesFull()
		{
			List<string> names = new List<string>();

			for(int i = 0;i < Instance.m_Categories.Length;i++)
			{
				var category = Instance.m_Categories[i];

				for(int j = 0;j < category.Items.Length;j++)
					names.Add(Instance.m_Categories[i].Name + "/" + category.Items[j].Name);
			}

			return names;
		}

		public static List<string> GetCategoryNames()
		{
			List<string> names = new List<string>();

			for(int i = 0;i < Instance.m_Categories.Length;i++)
				names.Add(Instance.m_Categories[i].Name);

			return names;
		}

		public static string[] GetPropertyNames()
		{
			string[] names = new string[Instance.m_ItemProperties.Length];

			for(int i = 0;i < Instance.m_ItemProperties.Length;i++)
				names[i] = Instance.m_ItemProperties[i].Name;

			return names;
		}

		public static ItemPropertyDefinition[] GetProperties()
		{
			return Instance.m_ItemProperties.ToArray();
		}

		public static ItemPropertyDefinition GetPropertyByName(string name)
		{
			foreach(var property in Instance.m_ItemProperties)
			{
				if(property.Name == name)
					return property;
			}

			return null;
		}

		public static ItemPropertyDefinition GetPropertyAtIndex(int index)
		{
			if(index >= Instance.m_ItemProperties.Length)
				return null;
			else
				return Instance.m_ItemProperties[index];
		}

		public static ItemCategory GetCategoryByName(string name)
		{
			for(int i = 0;i < Instance.m_Categories.Length;i ++)
				if(Instance.m_Categories[i].Name == name)
					return Instance.m_Categories[i];

			return null;
		}

		public static ItemCategory GetRandomCategory()
		{
			return Instance.m_Categories[Random.Range(0, Instance.m_Categories.Length)];
		}

		public static ItemInfo GetRandomItemFromCategory(string categoryName)
		{
			ItemCategory category = GetCategoryByName(categoryName);

			if(category != null && category.Items.Length > 0)
				return category.Items[Random.Range(0, category.Items.Length)];

			return null;
		}

		public static int GetItemCount()
		{
			int count = 0;

			for(int c = 0;c < Instance.m_Categories.Length;c ++)
				count += Instance.m_Categories[c].Items.Length;

			return count;
		}

		private void OnEnable()
		{
			GenerateDictionaries();
			RefreshItemIDs();
		}

		private void OnValidate()
		{
			int currentID = 0;

			foreach(var category in m_Categories)
			{
				for(int j = 0;j < category.Items.Length;j++)
				{
					category.Items[j].Category = category.Name;

					currentID++;
				}
			}

			GenerateDictionaries();
			RefreshItemIDs();
		}

		private void GenerateDictionaries()
		{
			m_Items = new List<ItemInfo>();
			m_ItemsByName = new Dictionary<string, ItemInfo>();
			m_ItemsById = new Dictionary<int, ItemInfo>();

			for(int c = 0;c < m_Categories.Length;c ++)
			{
				var category = m_Categories[c];

				for(int i = 0;i < category.Items.Length;i ++)
				{
					ItemInfo item = category.Items[i];

					m_Items.Add(item);

					if(!m_ItemsByName.ContainsKey(item.Name))
						m_ItemsByName.Add(item.Name, item);

					if(!m_ItemsById.ContainsKey(item.Id))
						m_ItemsById.Add(item.Id, item);
				}
			}
		}

		private void RefreshItemIDs()
		{
			int maxAssignmentTries = 50;

			List<int> idList = new List<int>();
			int i = 0;

			foreach(var category in m_Categories)
			{
				foreach(var item in category.Items)
				{
					idList.Add(item.Id);
				}
			}

			foreach(var category in m_Categories)
			{
				foreach(var item in category.Items)
				{
					int assignmentTries = 0;
					int assignedId = idList[i];
 
					while((assignedId == 0 || idList.Contains(assignedId) && (idList.IndexOf(assignedId) != i)) && assignmentTries < maxAssignmentTries)
					{
						assignedId = IdGenerator.GenerateIntegerId();
						assignmentTries++;
					}

					if(assignmentTries == maxAssignmentTries)
					{
						Debug.LogError("Couldn't generate an unique id for item: " + item.Name);
						return;
					}
					else
					{
						idList[i] = assignedId;
						AssignIdToItem(item, assignedId);
					}

					i++;
				}
			}
		}

		private int AssignIdToItem(ItemInfo itemInfo, int id)
		{
			Type itemInfoType = typeof(ItemInfo);
			FieldInfo idField = itemInfoType.GetField("m_Id", BindingFlags.NonPublic | BindingFlags.Instance);

			idField.SetValue(itemInfo, id);

			return id;
		}
	}
}