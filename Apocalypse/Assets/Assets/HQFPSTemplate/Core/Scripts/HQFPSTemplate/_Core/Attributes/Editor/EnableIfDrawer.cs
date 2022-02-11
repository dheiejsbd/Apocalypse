using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomPropertyDrawer(typeof(EnableIf))]
    public class EnableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool enableGUI = CanEnable(property);

            if (!enableGUI)
                GUI.enabled = false;

            float indentation = ((EnableIf)attribute).m_Indentation;

            position.x += indentation;
            position.width -= indentation;
            EditorGUI.PropertyField(position, property, label, true);

            if (!enableGUI)
                GUI.enabled = true;
        }


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, true);
		}

		private bool CanEnable(SerializedProperty property)
		{
			var attr = (EnableIf)attribute;

			string parentPath = string.Empty;
			parentPath = property.GetParentPath();
			SerializedProperty conditionProperty = property.serializedObject.FindProperty(parentPath + (parentPath != string.Empty ? "." : "") + attr.m_PropertyName);

			if (conditionProperty != null)
			{
				if (conditionProperty.propertyType == SerializedPropertyType.Boolean)
					return attr.m_RequiredBool == conditionProperty.boolValue;
				else if (conditionProperty.propertyType == SerializedPropertyType.ObjectReference)
					return attr.m_RequiredBool == (conditionProperty.objectReferenceValue != null);
				else if (conditionProperty.propertyType == SerializedPropertyType.Integer)
					return attr.m_RequiredInt == conditionProperty.intValue;
				else if (conditionProperty.propertyType == SerializedPropertyType.Enum)
					return attr.m_RequiredInt == conditionProperty.intValue;
			}

			return false;
		}
	}
}
