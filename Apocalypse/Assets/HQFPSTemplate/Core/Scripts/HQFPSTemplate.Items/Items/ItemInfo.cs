using System;
using UnityEngine;

namespace HQFPSTemplate.Items
{
	/// <summary>
	/// Information about an item, which should be set in the item database.
	/// </summary>
	[Serializable]
	public class ItemInfo
	{
		public int Id { get => m_Id; }

		public string Name { get { return m_Name; } }

		public string Category { get { return m_Category; } set { m_Category = value; } }

		public Sprite Icon { get { return m_Icon; } }

		public string Description { get { return m_Description; } }

		public GameObject Pickup { get { return m_Pickup; } }

		public int StackSize { get { return m_StackSize; } }

		public ItemPropertyInfoList Properties { get { return m_Properties; } }

		[SerializeField]
		private string m_Name;

		[Space]

		[SerializeField]
		[ReadOnly]
		private int m_Id;

		[SerializeField]
		[ReadOnly]
		private string m_Category;

		[Space]

		[SerializeField]
		[PreviewSprite]
		private Sprite m_Icon;

		[Space]

		[SerializeField]
		[MultilineCustom(5)]
		private string m_Description;

		[SerializeField]
		private GameObject m_Pickup;

		[SerializeField]
		[Clamp(1, 1000)]
		private int m_StackSize = 1;

		[Space]

		[SerializeField]
		[Reorderable]
		private ItemPropertyInfoList m_Properties;
	}
}