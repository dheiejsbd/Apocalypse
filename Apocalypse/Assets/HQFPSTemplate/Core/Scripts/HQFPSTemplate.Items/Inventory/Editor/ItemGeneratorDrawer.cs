using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
	[CustomPropertyDrawer(typeof(ItemGenerator))]
	public class ItemGeneratorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var method = property.FindPropertyRelative("GenerateMethod");
			var category = property.FindPropertyRelative("Category");
			var item = property.FindPropertyRelative("Name");
			var countRange = property.FindPropertyRelative("CountRange");

			position.x -= 4f;
			float spacing = 4f;

			EditorGUI.indentLevel -= 1;

			// Method
			position.height = 16f;
			position.y += spacing;
			EditorGUI.PropertyField(position, method);

			ItemGenerator.Method methodParsed = (ItemGenerator.Method)method.enumValueIndex;

			if(methodParsed == ItemGenerator.Method.RandomFromCategory)
			{
				// Category
				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, category);
			}
			else if(methodParsed == ItemGenerator.Method.Specific)
			{
				// Item
				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, item);
			}

			// Count Range
			position.y = position.yMax + spacing;
			EditorGUI.PropertyField(position, countRange);

			EditorGUI.indentLevel += 1;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ItemGenerator.Method method = (ItemGenerator.Method)property.FindPropertyRelative("GenerateMethod").enumValueIndex;

			float defaultHeight = 16f;
			float height = 26;
			float spacing = 4f;

			if (method == ItemGenerator.Method.Random)
				height += (defaultHeight + spacing) * 2;
			else
				height += (defaultHeight + spacing) * 3;

			return height;
		}
	}
}