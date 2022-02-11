using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(ItemPropertyDefinition))]
	public class ItemPropertyDefinitionDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty itemNameProp = property.FindPropertyRelative("Name");
			EditorGUI.MultiPropertyField(position, new GUIContent[] { new GUIContent(""), new GUIContent("") }, itemNameProp);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
	}
}