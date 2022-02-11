using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System;
using System.Linq;

namespace HQFPSTemplate
{
    [CustomPropertyDrawer(typeof(PlayerItemContainer))]
    public class InventoryContainerDrawer : PropertyDrawer
    {
		private static string[] m_AllContainers;
		private static bool m_Initialized;
		private Inventory m_Inventory;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!m_Initialized)
			{
				GetDataFromInventory();
				m_Initialized = true;
			}

			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "The Inventory Container attribute runs just on strings.", MessageType.Error);
			}
			else if (m_AllContainers == null)
			{
				EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "No Inventory component could be found.", MessageType.Error);
			}
			else
				property.stringValue = IndexToString(EditorGUI.Popup(position, label.text, StringToIndex(property.stringValue), m_AllContainers));
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property)
		{
			return false;
		}


		private int StringToIndex(string s)
		{
			for (int i = 0; i < m_AllContainers.Length; i++)
			{
				if (m_AllContainers[i] == s)
					return i;
			}

			return 0;
		}

		private void GetDataFromInventory()
		{
			m_Inventory = GameObject.FindObjectOfType<Player>().transform.root.GetComponentInChildren<Inventory>();

			if (m_Inventory != null)
				m_AllContainers = m_Inventory.GetAllContainerNames();
		}

		private string IndexToString(int i)
		{
			return m_AllContainers.Length > i ? m_AllContainers[i] : "";
		}
	}
}
