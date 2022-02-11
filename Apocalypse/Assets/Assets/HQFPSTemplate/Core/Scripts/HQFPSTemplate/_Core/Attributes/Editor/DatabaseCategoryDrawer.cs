using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(DatabaseCategory))]
	public class DatabaseCategoryDrawer : PropertyDrawer
	{
		private static string[] m_AllCategories;
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
			{
				EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "The ItemCategory attribute runs just on strings.", MessageType.Error);
				return;
			}

			if(m_AllCategories != null)
				property.stringValue = IndexToString(EditorGUI.Popup(position, label.text, StringToIndex(property.stringValue), m_AllCategories));
		}

		private int StringToIndex(string s)
		{
			for(int i = 0;i < m_AllCategories.Length;i ++)
			{
				if(m_AllCategories[i] == s)
					return i;
			}

			return 0;
		}

		private string IndexToString(int i)
		{
			return m_AllCategories.Length > i ? m_AllCategories[i] : "";
		}

		private void GetDataFromDatabase()
		{
			m_AllCategories = ItemDatabase.GetCategoryNames().ToArray();
		}
	}
}