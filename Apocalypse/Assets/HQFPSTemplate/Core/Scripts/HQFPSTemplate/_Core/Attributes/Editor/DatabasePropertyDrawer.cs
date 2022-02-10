using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(DatabaseProperty))]
	public class DatabasePropertyDrawer : PropertyDrawer
	{
		private static string[] m_AllProperties;
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
				EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "The ItemProperty attribute runs just on strings.", MessageType.Error);
				return;
			}

			if(m_AllProperties != null)
				property.stringValue = IndexToString(EditorGUI.Popup(position, label.text, StringToIndex(property.stringValue), m_AllProperties));
		}

		private int StringToIndex(string s)
		{
			for(int i = 0;i < m_AllProperties.Length;i ++)
			{
				if(m_AllProperties[i] == s)
					return i;
			}

			return 0;
		}

		private string IndexToString(int i)
		{
			return m_AllProperties.Length > i ? m_AllProperties[i] : "";
		}

		private void GetDataFromDatabase()
		{
			m_AllProperties = ItemDatabase.GetPropertyNames();
		}
	}
}