using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
    public class PreviewSpriteDrawer : PropertyDrawer
    {
        const float imageHeight = 64;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + imageHeight + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        static string GetPath(SerializedProperty property)
        {
            string path = property.propertyPath;
            int index = path.LastIndexOf(".");
            return path.Substring(0, index + 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw the normal property field
            EditorGUI.PropertyField(position, property, label, true);

            if(property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var sprite = property.objectReferenceValue as Sprite;

                if(sprite != null)
                {
                    position.x += EditorGUIUtility.labelWidth;
                    position.y += EditorGUI.GetPropertyHeight(property, label, true) + 5;
                    position.height = imageHeight;
                    position.width = position.height = imageHeight;

                    GUI.DrawTexture(position, sprite.texture);
                }
            }
        }
    }
}
