using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // cast the attribute to make life easier
            MinMaxAttribute minMax = attribute as MinMaxAttribute;

            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Color originalGUIColor = GUI.backgroundColor;

            // This only works on a vector2 and vector2Int! ignore on any other property type (we should probably draw an error message instead!)
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                // pull out a bunch of helpful min/max values....
                float minValue = property.vector2Value.x; // the currently set minimum and maximum value
                float maxValue = property.vector2Value.y;
                float minLimit = minMax.MinLimit; // the limit for both min and max, min can't go lower than minLimit and max can't top maxLimit
                float maxLimit = minMax.MaxLimit;
                
                // and ask unity to draw them all nice for us!
                EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

                GUI.backgroundColor = new Color(0.68f, 0.7f, 0.71f, 1f);

                var vec = Vector2.zero; // save the results into the property!
                vec.x = minValue;
                vec.y = maxValue;

                property.vector2Value = vec;

                // move the draw rect on by one line
                position.y += EditorGUIUtility.singleLineHeight;
                position.x += 10f;

                float[] vals = new float[] { minValue, maxValue }; // shove the values and limits into a vector4 and draw them all at once
                EditorGUI.MultiFloatField(position, new GUIContent("Range"), new GUIContent[] { new GUIContent("MinVal:  "), new GUIContent("MaxVal:  ") }, vals);

                if (minMax.DrawRangeValue)
                {
                    GUI.enabled = false; // the range part is always read only
                    position.y += (EditorGUIUtility.singleLineHeight + 2f);

                    EditorGUI.FloatField(position, "Selected Range", (float)System.Math.Round(maxValue - minValue, 3));
                    GUI.enabled = true; // remember to make the UI editable again!
                }

                property.vector2Value = new Vector2(vals[0], vals[1]); // save off any change to the value~
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                // pull out a bunch of helpful min/max values....
                float minValue = property.vector2IntValue.x; // the currently set minimum and maximum value
                float maxValue = property.vector2IntValue.y;
                int minLimit = Mathf.RoundToInt(minMax.MinLimit); // the limit for both min and max, min can't go lower than minLimit and max can't top maxLimit
                int maxLimit = Mathf.RoundToInt(minMax.MaxLimit);

                // and ask unity to draw them all nice for us!
                EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

                GUI.backgroundColor = new Color(0.68f, 0.7f, 0.71f, 1f);

                var vec = Vector2Int.zero; // save the results into the property!
                vec.x = Mathf.RoundToInt(minValue);
                vec.y = Mathf.RoundToInt(maxValue);

                property.vector2IntValue = vec;

                // move the draw rect on by one line
                position.y += EditorGUIUtility.singleLineHeight;
                position.x += 10f;

                float[] vals = new float[] { minValue, maxValue }; // shove the values and limits into a vector4 and draw them all at once
                EditorGUI.MultiFloatField(position, new GUIContent("Range"), new GUIContent[] { new GUIContent("Min Value: "), new GUIContent("Max Value: ") }, vals);

                if (minMax.DrawRangeValue)
                {
                    GUI.enabled = false; // the range part is always read only
                    position.y += (EditorGUIUtility.singleLineHeight + 2f);

                    EditorGUI.FloatField(position, "Selected Range", maxValue - minValue);
                    GUI.enabled = true; // remember to make the UI editable again!
                }

                property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(vals[0]), Mathf.RoundToInt(vals[1])); // save off any change to the value~
            }

            GUI.backgroundColor = originalGUIColor;
        }

        // this method lets unity know how big to draw the property. We need to override this because it could end up being more than one line big
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MinMaxAttribute minMax = attribute as MinMaxAttribute;

            // by default just return the standard line height
            float size = EditorGUIUtility.singleLineHeight;

            // if we have a special mode, add two extra lines!
            size += EditorGUIUtility.singleLineHeight * (minMax.DrawRangeValue ? 2 : 1);

            return size;
        }
    }
}