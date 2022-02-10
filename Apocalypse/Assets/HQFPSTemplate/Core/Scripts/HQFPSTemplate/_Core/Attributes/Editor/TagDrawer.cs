using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
	[CustomPropertyDrawer(typeof(TagAttribute))]
	public class TagDrawer : PropertyDrawer 
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
		{
            if(property.propertyType != SerializedPropertyType.String)
                EditorGUI.HelpBox(position, "The Tag attribute works just on strings.", MessageType.Error);
            else
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
		}
	}
}