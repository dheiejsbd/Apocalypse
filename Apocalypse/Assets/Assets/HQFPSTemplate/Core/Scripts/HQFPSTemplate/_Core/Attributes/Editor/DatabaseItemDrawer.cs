using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(DatabaseItem))]
	public class DatabaseItemDrawer : PropertyDrawer 
	{
		private static string[] m_AllItems;
		private static string[] m_AllItemsFull;

		private static bool m_Initialized;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
		{
			if(!m_Initialized)
			{
				EditorGUICustom.OneSecondPassed += GetDataFromDatabase;
				GetDataFromDatabase();
				m_Initialized = true;
			}

			if(property.propertyType != SerializedPropertyType.String)
				EditorGUI.HelpBox(position, "The Item attribute runs just on strings.", MessageType.Error);

			if(m_AllItems != null)
			{
				int selectedItem = EditorGUICustom.IndexOfString(property.stringValue, m_AllItems);
				selectedItem = EditorGUI.Popup(position, label.text, selectedItem, m_AllItemsFull);
				property.stringValue = EditorGUICustom.StringAtIndex(selectedItem, m_AllItems);
			}
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property)
		{
			return false;
		}
		
		private void GetDataFromDatabase()
		{
			m_AllItems = ItemDatabase.GetItemNames().ToArray();
			m_AllItemsFull = ItemDatabase.GetItemNamesFull().ToArray();
		}
	}
}