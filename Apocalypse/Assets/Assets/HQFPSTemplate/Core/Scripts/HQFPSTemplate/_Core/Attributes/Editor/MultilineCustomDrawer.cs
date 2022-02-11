using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomPropertyDrawer(typeof(MultilineCustom))]
    public class MultilineCustomDrawer : PropertyDrawer
    {
        private const int kLineHeight = 13;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType == SerializedPropertyType.String)
            {
                label = EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, 0, label);

                EditorGUI.BeginChangeCheck();
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0; // The MultiFieldPrefixLabel already applied indent, so avoid indent of TextArea itself.
                string newValue = EditorGUI.TextArea(position, property.stringValue);

                EditorGUI.indentLevel = oldIndent;

                if(EditorGUI.EndChangeCheck())
                    property.stringValue = newValue;

                EditorGUI.EndProperty();
            }
            else
                EditorGUI.LabelField(position, label.text, "Use Multiline with string.");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return kLineHeight // first line
                + (((MultilineCustom)attribute).Lines - 1) * kLineHeight; // remaining lines
        }
    }
}