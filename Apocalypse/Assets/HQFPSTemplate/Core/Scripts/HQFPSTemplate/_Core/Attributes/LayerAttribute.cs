#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace HQFPSTemplate
{
    public class LayerAttribute : PropertyAttribute
    {
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType != SerializedPropertyType.Integer)
                EditorGUI.HelpBox(position, "The Layer attribute works just on integers.", MessageType.Error);
            else
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
#endif
}