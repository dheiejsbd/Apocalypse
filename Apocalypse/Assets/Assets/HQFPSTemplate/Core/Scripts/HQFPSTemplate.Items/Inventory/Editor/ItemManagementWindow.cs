using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HQFPSTemplate.Items
{
    public class ItemManagementWindow : EditorWindow
	{
		/// <summary>
		/// This is a hack for avoiding an issue with the ReorderableList's DrawHeader method. 
		/// </summary>
		public static bool DrawingItemWindow { get; private set; }

		private int m_SelectedTab;
		private SerializedObject m_ItemDatabase;

		private ReorderableList m_CategoryList;
		private ReorderableList m_ItemList;

		private Vector2 m_CategoriesScrollPos;
		private Vector2 m_ItemsScrollPos;
		private Vector2 m_ItemInspectorScrollPos;


		[MenuItem("HQ FPS Template/Item Management...", false, priority = 7)]
		public static void Init()
		{
			var window = GetWindow<ItemManagementWindow>(true, "Item Management");

			window.minSize = new Vector2(512, 512);
		}

		public void OnGUI()
		{
			DrawingItemWindow = true;

			if(m_ItemDatabase == null)
			{
				EditorGUILayout.HelpBox("No ItemDatabase was found in the Resources folder!", MessageType.Error);

				if(GUILayout.Button("Refresh"))
					InitializeWindow();

				if(m_ItemDatabase == null)
					return;
			}

			// Display the database path
			EditorGUILayout.LabelField(string.Format("Database path: '{0}'", AssetDatabase.GetAssetPath(m_ItemDatabase.targetObject)), new GUIStyle(EditorGUICustom.BoldMiniGreyLabel) { alignment = TextAnchor.UpperLeft }); ;

			// Display the shortcuts
			EditorGUI.LabelField(new Rect(position.width - 262f, 2f, 256f, 16f), "'Ctrl + D' to duplicate", EditorGUICustom.BoldMiniGreyLabel);
			EditorGUI.LabelField(new Rect(position.width - 262f, 17f, 256f, 16f), "'Delete' to delete", EditorGUICustom.BoldMiniGreyLabel);

			Vector2 buttonSize = new Vector2(192f, 32f);
			float topPadding = 32f;

			m_SelectedTab = GUI.Toolbar(new Rect(position.width * 0.5f - buttonSize.x, topPadding, buttonSize.x * 2, buttonSize.y), m_SelectedTab, new string[] { "Items", "Properties" });

			// Horizontal line.
			GUI.Box(new Rect(0f, topPadding + buttonSize.y * 1.25f, position.width, 1f), "");

			// Draw the item editors.
			m_ItemDatabase.Update();

			float innerWindowPadding = 8f;
			Rect innerWindowRect = new Rect(innerWindowPadding, topPadding + buttonSize.y * 1.25f + innerWindowPadding, position.width - innerWindowPadding * 2f, position.height - (topPadding + buttonSize.y * 1.25f + innerWindowPadding * 3f));

			// Inner window box.
			GUI.backgroundColor = Color.grey;
			GUI.Box(innerWindowRect, "");
			GUI.backgroundColor = Color.white;

			// GUI Style used for the list titles (e.g. Item list, Categories, etc.) 
			var listTitleBoxes = new GUIStyle(EditorStyles.toolbar);
			listTitleBoxes.fontSize = 12;
			listTitleBoxes.alignment = TextAnchor.MiddleCenter;

			if (m_SelectedTab == 0)
				DrawItemEditor(innerWindowRect, listTitleBoxes);
			else if(m_SelectedTab == 1)
				DrawPropertiesEditor(innerWindowRect, listTitleBoxes);

			m_ItemDatabase.ApplyModifiedProperties();

			DrawingItemWindow = false;
		}

		private void OnEnable()
		{
			InitializeWindow();

			Undo.undoRedoPerformed += Repaint;
		}

		private void InitializeWindow()
		{
			var database = Resources.LoadAll<ItemDatabase>("")[0];

			if(database)
			{
				m_ItemDatabase = new SerializedObject(database);

				m_CategoryList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_Categories"), true, true ,true ,true);
				m_CategoryList.drawElementCallback += DrawCategory;
				m_CategoryList.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
				m_CategoryList.onSelectCallback += On_SelectedCategory;
				m_CategoryList.onRemoveCallback = (ReorderableList list)=> { m_CategoryList.serializedProperty.DeleteArrayElementAtIndex(m_CategoryList.index); };
			}
		}

		private void On_SelectedCategory(ReorderableList list)
		{
			m_ItemList = new ReorderableList(m_ItemDatabase, m_CategoryList.serializedProperty.GetArrayElementAtIndex(Mathf.Clamp(m_CategoryList.index, 0, m_CategoryList.count - 1)).FindPropertyRelative("m_Items"), true, true, true, true);
			m_ItemList.drawElementCallback += DrawItem;
			m_ItemList.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
			m_ItemList.onRemoveCallback = OnItemRemoved;
			m_ItemList.onSelectCallback += On_SelectedItem;
			m_ItemList.onChangedCallback += On_SelectedItem;
		}

		private void OnItemRemoved(ReorderableList list)
		{
			m_ItemList.serializedProperty.DeleteArrayElementAtIndex(m_ItemList.index);
		}

		private void On_SelectedItem(ReorderableList list)
		{
			if(m_ItemList == null || m_ItemList.count == 0 || m_ItemList.index == -1 || m_ItemList.index >= m_ItemList.count)
				return;
		}

		private void DrawItemEditor(Rect totalRect, GUIStyle listTitleBoxes)
		{
			// Inner window cross (partitioning in 4 smaller boxes)
			GUI.Box(new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f, totalRect.width / 2f, 1f), "");
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f, totalRect.y, 1f, totalRect.height), "");

			Vector2 labelSize = new Vector2(192f, 20f);

			// Draw the item list
			string selCategoryName = (m_CategoryList.count == 0 || m_CategoryList.index == -1) ? "None" : m_CategoryList.serializedProperty.GetArrayElementAtIndex(Mathf.Clamp(m_CategoryList.index, 0, m_CategoryList.count - 1)).FindPropertyRelative("m_Name").stringValue;
			string itemListName = string.Format("Item List ({0})", selCategoryName);

			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), itemListName, listTitleBoxes);
			Rect itemListRect = new Rect(totalRect.x + 4f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 10f, totalRect.height * 0.5f - labelSize.y - 5f);

			if(m_CategoryList.count != 0 && m_CategoryList.index != -1 && m_CategoryList.index < m_CategoryList.count)
				DrawList(m_ItemList, itemListRect, ref m_ItemsScrollPos);
			else
			{
				itemListRect.x += 8f;
				GUI.Label(itemListRect, "Select a category...", EditorGUICustom.TitleLabel);
			}

			// Draw the categories
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y + totalRect.height * 0.5f + 2f, labelSize.x, labelSize.y), "Category List", listTitleBoxes);

			int categoryCountBefore = m_CategoryList.count;
			DrawList(m_CategoryList, new Rect(totalRect.x + 4f, totalRect.y + totalRect.height * 0.5f + labelSize.y + 6f, totalRect.width * 0.5f - 10f, totalRect.height * 0.5f - labelSize.y - 7f), ref m_CategoriesScrollPos);

			if(categoryCountBefore != m_CategoryList.count && m_CategoryList.count > 0)
				On_SelectedCategory(m_CategoryList);

			// Inspector label
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.75f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Item Inspector", listTitleBoxes);

			// Draw the inspector
			bool itemIsSelected = m_CategoryList.count != 0 && m_ItemList != null && m_ItemList.count != 0 && m_ItemList.index != -1 && m_ItemList.index < m_ItemList.count;
			Rect inspectorRect = new Rect(totalRect.x + totalRect.width * 0.5f + 8f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 13f, totalRect.height - labelSize.y - 9f);

			if(itemIsSelected)
				DrawItemInspector(inspectorRect);
			else
			{
				inspectorRect.x += 4f;
				inspectorRect.y += 4f;
				GUI.Label(inspectorRect, "Select an item to inspect...", EditorGUICustom.TitleLabel);
			}
		}

		private void DrawList(ReorderableList list, Rect totalRect, ref Vector2 scrollPosition)
		{
			float scrollbarWidth = 16f;

			Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);
			Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, (list.count + 4) * list.elementHeight);

			scrollPosition = GUI.BeginScrollView(onlySeenRect, scrollPosition, allContentRect, false, true);

			// Draw the clear button.
			Vector2 buttonSize = new Vector2(56f, 17f);

			if(list.count > 0 && GUI.Button(new Rect(allContentRect.x + 2f, allContentRect.yMax - 55f, buttonSize.x, buttonSize.y), "Clear"))
			if(EditorUtility.DisplayDialog("Warning!", "Are you sure you want the list to be cleared? (All elements will be deleted)", "Yes", "Cancel"))
				list.serializedProperty.ClearArray();

			list.DoList(allContentRect);

			GUI.EndScrollView();
		}

		private void DrawCategory(Rect rect, int index, bool isActive, bool isFocused) 
		{
			ItemManagementUtility.DrawListElementByName(m_CategoryList, index, rect, "m_Name", isFocused, this);
		}

		private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			if(m_ItemList.serializedProperty.arraySize > index)
				ItemManagementUtility.DrawListElementByName(m_ItemList, index, rect, "m_Name", isFocused, this);
		}

		private void DrawItemInspector(Rect viewportRect)
		{
			var item = m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index);
			item.isExpanded = true;

			float indentation = 4f;
			Rect rect = new Rect(viewportRect.x + indentation, viewportRect.y + indentation, viewportRect.width - indentation * 2, viewportRect.height - indentation * 2);

			m_ItemInspectorScrollPos = GUI.BeginScrollView(viewportRect, m_ItemInspectorScrollPos, new Rect(rect.x, rect.y, rect.width - 16f, 28f + EditorGUI.GetPropertyHeight(item, true)));

			// Draw item name
			rect.xMin += indentation;
			rect.xMax -= 16f;
			rect.yMin += indentation;

			GUI.Label(rect, item.FindPropertyRelative("m_Name").stringValue, EditorGUICustom.ShortTitleLabel);

			// Draw all item fields
			rect.yMax -= 16f;
			rect.y += 28f;

			var properties = item.Copy().GetChildren();

			rect.y += EditorGUIUtility.standardVerticalSpacing;

			int i = 0;

			float rectHeight = 0f;
			float rectYStart = rect.y - 3f;

			Rect itemRect = new Rect(rect);

			rect.width += 12f;
			rect.x -= 3f;

			foreach (var prop in properties)
			{
				//Draw the background box
				if (i == 3)
				{
					Color previousColor = GUI.backgroundColor;
					GUI.backgroundColor = new Color(0.35f, 0.35f, 0.35f, 1f);

					GUI.Box(new Rect(rect.x, rectYStart, rect.width, rectHeight + 12f), "", EditorStyles.helpBox);
					rectYStart += rectHeight + 12f;

					itemRect.y += itemRect.height / 4;

					GUI.Box(new Rect(rect.x, rectYStart + itemRect.height / 4, rect.width, viewportRect.height - itemRect.height / 2), "", EditorStyles.helpBox);

					GUI.backgroundColor = previousColor;
				}

				itemRect.height = EditorGUI.GetPropertyHeight(prop, true);
				EditorGUI.PropertyField(itemRect, prop, true);
				itemRect.y += itemRect.height + EditorGUIUtility.standardVerticalSpacing;
				prop.isExpanded = true;

				rectHeight += itemRect.height;

				i++;
			}

			GUI.EndScrollView();
		}

		private void DrawPropertiesEditor(Rect totalRect, GUIStyle listTitleBoxes)
		{
			SerializedProperty propertiesProp = m_ItemDatabase.FindProperty("m_ItemProperties");

			Vector2 labelSize = new Vector2(128f, 20f);

			// Properties label
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Properties", listTitleBoxes);

			float propertiesListPadding = totalRect.width * 0.1f;

			// Properties list
			Rect propertiesRect = new Rect(totalRect.x + propertiesListPadding, totalRect.y + labelSize.y + 4f, totalRect.width - propertiesListPadding * 2f, EditorGUI.GetPropertyHeight(propertiesProp));

			EditorGUI.PropertyField(propertiesRect, propertiesProp);
		}
	}

	public static class ItemManagementUtility
	{
		public static void DoListElementBehaviours(ReorderableList list, int index, bool isFocused, EditorWindow window = null)
		{
			var current = Event.current;

			if(current.type == EventType.KeyDown)
			{
				if(list.index == index && isFocused)
				{
					if(current.keyCode == KeyCode.Delete)
					{
						int newIndex = 0;
						if(list.count == 1)
							newIndex = -1;
						else if(index == list.count - 1)
							newIndex = index - 1;
						else if(index > 0)
							newIndex = index - 1;

						list.serializedProperty.DeleteArrayElementAtIndex(index);

						if(newIndex != -1)
						{
							list.index = newIndex;
							if(list.onSelectCallback != null)
								list.onSelectCallback(list);
						}

						Event.current.Use();
						if(window)
							window.Repaint();
					}
					else if(current.control && current.keyCode == KeyCode.D)
					{
						list.serializedProperty.InsertArrayElementAtIndex(list.index);
						list.index ++;
						if(list.onSelectCallback != null)
							list.onSelectCallback(list);

						Event.current.Use();
						if(window)
							window.Repaint();
					}
				}
			}
		}

		public static string[] GetItemNamesFull(SerializedProperty categoryList)
		{
			List<string> names = new List<string>();

			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
					names.Add(category.FindPropertyRelative("m_Name").stringValue + "/" + itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
			}

			return names.ToArray();
		}

		public static string[] GetItemNames(SerializedProperty categoryList)
		{
			List<string> names = new List<string>();
			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
					names.Add(itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
			}

			return names.ToArray();
		}

		public static int GetItemIndex(SerializedProperty categoryList, string itemName)
		{
			int index = 0;
			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
				{
					var name = itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue;
					if(name == itemName)
						return index;

					index ++;
				}
			}

			return -1;
		}

		public static void DrawListElementByName(ReorderableList list, int index, Rect rect, string nameProperty, bool isFocused, EditorWindow window)
		{
			if(list.serializedProperty.arraySize == index)
				return;

			rect.y += 2;
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var name = element.FindPropertyRelative(nameProperty);

			name.stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, 256f, 16f), name.stringValue);

			DoListElementBehaviours(list, index, isFocused, window);
		}

		public static string[] GetStringNames(SerializedProperty property, string subProperty = "")
		{
			List<string> strings = new List<string>();
			for(int i = 0;i < property.arraySize;i ++)
			{
				if(subProperty == "")
					strings.Add(property.GetArrayElementAtIndex(i).stringValue);
				else
					strings.Add(property.GetArrayElementAtIndex(i).FindPropertyRelative(subProperty).stringValue);
			}

			return strings.ToArray();
		}

		public static int GetStringIndex(string str, string[] strings)
		{
			for(int i = 0;i < strings.Length;i ++)
				if(strings[i] == str)
					return i;

			return 0;
		}
	}
}