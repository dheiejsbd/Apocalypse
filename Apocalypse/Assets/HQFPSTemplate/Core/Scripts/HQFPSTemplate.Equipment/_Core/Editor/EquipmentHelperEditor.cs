using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Equipment
{
    [CustomEditor(typeof(EquipmentFixer))]
    public class EquipmentHelperEditor : Editor
    {
        private EquipmentFixer m_EHelper;

        private const int m_ButtonHeight = 19;
        private const string m_separatorString = "  |  ";
        private int m_SelectedTab = 0;
        private int m_EquipmentLayer = 9;


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.8f,0.8f,0.8f,1f);

            GUIStyle buttonStyle = new GUIStyle(EditorGUICustom.StandardButtonStyle);
            buttonStyle.fontSize = 12;
            buttonStyle.normal.textColor = new Color(0.9f, 0.85f, 0.85f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(GetEquipmentListInfo(), MessageType.Info);

            EditorGUICustom.Separator();
            m_SelectedTab = GUILayout.Toolbar(m_SelectedTab, new string[] { "Prefabs", "Tools" });
            EditorGUICustom.Separator();

            GUI.backgroundColor = prevColor;

            DrawEquipmentEnablerButtons(buttonStyle);
            EditorGUILayout.Space();

            if (m_SelectedTab == 0)
                DrawEquipmentPrefabsTab(buttonStyle);
            else if (m_SelectedTab == 1)
                DrawEquipmentToolsTab(buttonStyle);

            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEquipmentEnablerButtons(GUIStyle buttonStyle)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Enable All", buttonStyle, GUILayout.Height(m_ButtonHeight)))
                m_EHelper.EnableAllEquipment(true);

            if (GUILayout.Button("Disable All", buttonStyle, GUILayout.Height(m_ButtonHeight)))
                m_EHelper.EnableAllEquipment(false);

            GUILayout.EndHorizontal();
        }

        private void DrawEquipmentToolsTab(GUIStyle buttonStyle) 
        {
            m_EquipmentLayer = EditorGUILayout.LayerField("Equipment Layer", m_EquipmentLayer);

            EditorGUILayout.Space();

            if (m_EHelper.TryGetFixedItems(out bool[] fixedItems, out int fixedCount, m_EquipmentLayer))
            {
                //Draw Warning Box if the there's at least one item that's not fixed
                if (fixedCount < m_EHelper.m_EquipmentItems.Count)
                {
                    string helpBoxText = $"  Fixed Items: {fixedCount} / {m_EHelper.m_EquipmentItems.Count}";
                    EditorGUILayout.HelpBox(helpBoxText, MessageType.Warning);
                }

                GUI.enabled = false;

                string[] itemNames = GetItemNames();
                for (int i = 0; i < itemNames.Length; i++)
                {
                    string itemName = itemNames[i];
                    itemName += fixedItems[i] ? " - Fixed" : " - Needs Fixing.....";

                    GUILayout.Label(itemName);
                }

                if (fixedCount < m_EHelper.m_EquipmentItems.Count)
                    GUI.enabled = true;
            }

            EditorGUICustom.Separator();

            if (GUILayout.Button("FIX Equipment (Animator & Renderer & FOV)", buttonStyle, GUILayout.Height(m_ButtonHeight)))
                m_EHelper.FixEquipmentItems(m_EquipmentLayer);
        }

        private void DrawEquipmentPrefabsTab(GUIStyle buttonStyle)
        {
            if (m_EHelper.TryGetAppliedPrefabs(out bool[] appliedPrefabs, out int appliedCount))
            {
                //Draw Warning Box if the there's at least one item that's not fixed
                if (appliedCount < m_EHelper.m_EquipmentItems.Count)
                {
                    string helpBoxText = $"  Applied Prefabs: {appliedCount} / {m_EHelper.m_EquipmentItems.Count}";
                    EditorGUILayout.HelpBox(helpBoxText, MessageType.Warning);
                }

                GUI.enabled = false;

                string[] itemNames = GetItemNames();
                for (int i = 0; i < itemNames.Length; i++)
                {
                    string itemName = itemNames[i];
                    itemName += appliedPrefabs[i] ? " - Applied" : " - Not Applied.....";

                    GUILayout.Label(itemName);
                }

                if (appliedCount < m_EHelper.m_EquipmentItems.Count)
                    GUI.enabled = true;
            }

            EditorGUICustom.Separator();

            if (GUILayout.Button("Apply Prefabs (Only Enabled)", buttonStyle, GUILayout.Height(m_ButtonHeight)))
                m_EHelper.ApplyEquipmentPrefabs();

            if (GUILayout.Button("Enable and Apply Prefabs (Recommended)", buttonStyle, GUILayout.Height(m_ButtonHeight)))
                m_EHelper.ApplyEquipmentPrefabs(true);
        }

        private string[] GetItemNames() 
        {
            string[] itemNames = new string[m_EHelper.m_EquipmentItems.Count];

            for (int i = 0; i < m_EHelper.m_EquipmentItems.Count; i++)
            {
                //Small Hack
                if (m_EHelper.m_EquipmentItems[i] as Unarmed == true)
                    itemNames[i] = "Unarmed";
                else
                    itemNames[i] = m_EHelper.m_EquipmentItems[i].CorrespondingItemName;
            }

            return itemNames;
        }

        private string GetEquipmentListInfo() 
        {
            string helpBoxText = $"  Equipment Count: {m_EHelper.m_EquipmentItems.Count}";
            helpBoxText += m_separatorString + $"Enabled Count: {m_EHelper.GetEnabledItemsCount()} / {m_EHelper.m_EquipmentItems.Count}";

            return helpBoxText;
        }

        private void OnEnable()
        {
            m_EHelper = serializedObject.targetObject as EquipmentFixer;
            m_EHelper.Initialize();
        }
    }
}
