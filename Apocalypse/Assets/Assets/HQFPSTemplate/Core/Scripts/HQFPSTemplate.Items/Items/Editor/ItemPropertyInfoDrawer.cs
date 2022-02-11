using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(ItemPropertyInfo))]
	public class ItemPropertyInfoDrawer : PropertyDrawer
	{
		private static ItemPropertyDefinition[] m_Properties;
		private static string[] m_PropertyNames;

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

			position.height = EditorGUIUtility.singleLineHeight;

			SerializedProperty nameProp = property.FindPropertyRelative("m_Name");
			SerializedProperty typeProp = property.FindPropertyRelative("m_Type");

			if(m_Properties != null && m_Properties.Length > 0)
			{
				Rect popupRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);

				nameProp.stringValue = EditorGUICustom.StringAtIndex(EditorGUI.Popup(popupRect, EditorGUICustom.IndexOfString(nameProp.stringValue, m_PropertyNames), m_PropertyNames), m_PropertyNames);

				int selectedPropertyIdx = EditorGUICustom.IndexOfString(nameProp.stringValue, m_PropertyNames);
				typeProp.enumValueIndex = (int)m_Properties[selectedPropertyIdx].Type;

				ItemPropertyType propType = m_Properties[selectedPropertyIdx].Type;

				Rect descriptionRect = new Rect(position.xMax - position.width * 0.2f + EditorGUIUtility.standardVerticalSpacing, position.y, position.width * 0.2f - EditorGUIUtility.standardVerticalSpacing, position.height);
				EditorGUI.LabelField(descriptionRect, "Type: " + propType.ToString().DoUnityLikeNameFormat(), EditorStyles.miniLabel);

				SerializedProperty valueProp = property.FindPropertyRelative("m_FixedValue");
				SerializedProperty isRandomProp = property.FindPropertyRelative("m_UseRandomValue");
				SerializedProperty randomValueRangeProp = property.FindPropertyRelative("m_RandomValueRange");

				position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;

				if(propType == ItemPropertyType.Boolean)
				{
					bool boolean = Mathf.Approximately(valueProp.floatValue, 0f) ? false : true;

					EditorGUI.LabelField(position, "True/False");
					boolean = EditorGUI.Toggle(new Rect(position.x + 86f, position.y, 16f, position.height), boolean);

					valueProp.floatValue = boolean ? 1f : 0f;
				}
				else if(propType == ItemPropertyType.Float || propType == ItemPropertyType.Integer)
				{
					Rect selectModeRect = new Rect(position.x, position.y, position.width * 0.35f, position.height);

					int selectedMode = GUI.Toolbar(selectModeRect, isRandomProp.boolValue == true ? 1 : 0, new string[] { "Fixed", "Random" });
					isRandomProp.boolValue = selectedMode == 1 ? true : false;

					Rect valueRect = new Rect(selectModeRect.xMax + EditorGUIUtility.singleLineHeight, position.y, position.width - selectModeRect.width - EditorGUIUtility.singleLineHeight, position.height);

					if(selectedMode == 0)
					{
						if(propType == ItemPropertyType.Float)
							valueProp.floatValue = EditorGUI.FloatField(valueRect, valueProp.floatValue);
						else
							valueProp.floatValue = Mathf.Clamp(EditorGUI.IntField(valueRect, Mathf.RoundToInt(valueProp.floatValue)), -9999999, 9999999);
					}
					else
					{
						float[] randomRange = new float[] { randomValueRangeProp.vector2Value.x, randomValueRangeProp.vector2Value.y };

						if(propType == ItemPropertyType.Float)
							randomValueRangeProp.vector2Value = EditorGUI.Vector2Field(valueRect, GUIContent.none, randomValueRangeProp.vector2Value);
						else
							randomValueRangeProp.vector2Value = EditorGUI.Vector2IntField(valueRect, GUIContent.none, new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(randomRange[0]), -9999999, 9999999), Mathf.Clamp(Mathf.RoundToInt(randomRange[1]), -9999999, 9999999)));
					}
				}
				else if(propType == ItemPropertyType.ItemId)
				{
					EditorGUI.LabelField(position, "Target Item");

					Rect itemPopupRect = EditorGUI.IndentedRect(position);
					itemPopupRect = new Rect(itemPopupRect.x + 80f, itemPopupRect.y, itemPopupRect.width * 0.8f - 80f, itemPopupRect.height);

					int itemId = Mathf.RoundToInt(valueProp.floatValue);

					if(itemId == 0)
						itemId = ItemDatabase.GetItemAtIndex(0).Id;

					int selectedItem = ItemDatabase.IndexOfItem(itemId);
					selectedItem = EditorGUI.Popup(itemPopupRect, selectedItem, m_AllItemsFull);
					
					valueProp.floatValue = System.Convert.ToSingle(ItemDatabase.GetItemAtIndex(selectedItem).Id);
				}
			}
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property)
		{
			return false;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
		}

		private void GetDataFromDatabase()
		{
			m_Properties = ItemDatabase.GetProperties();
			m_PropertyNames = ItemDatabase.GetPropertyNames();

			m_AllItemsFull = ItemDatabase.GetItemNamesFull().ToArray();
		}
	}
}