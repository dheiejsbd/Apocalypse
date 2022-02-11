using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Examples
{
    public class AddEquipmentWindow : EditorWindow
    {
        private EquipmentInstance m_EquipmentInstance;
        private SerializedObject m_EquipmentInstanceSerlz;
        private SerializedProperty m_EquipmentInfoProp;
        private string m_EquipmentName;

        private bool showGeneralInfo = true;
        private bool drawPrefabsInfo = true;

        private const int m_NumberOfFields = 4;


        [MenuItem("HQ FPS Template/Add Custom Equipment...", false)]
        public static void Init()
        {
            var window = GetWindow<AddEquipmentWindow>(true, "Add Equipment");
            window.minSize = new Vector2(512, 512);
        }

        private void OnEnable()
        {
            LoadEquipmentInstance();
        }

        private void LoadEquipmentInstance()
        {
            var equipmentItems = Resources.LoadAll<EquipmentInstance>("");

            foreach (var eItem in equipmentItems)
            {
                m_EquipmentInstance = eItem;
                m_EquipmentInstanceSerlz = new SerializedObject(m_EquipmentInstance);
                m_EquipmentInfoProp = m_EquipmentInstanceSerlz.FindProperty("m_EquipmentInfo");
                break;
            }
        }

        private void OnGUI()
        {
            if (m_EquipmentInstance != null)
            {
                Color lastColor = GUI.color;
                GUI.color = Color.grey;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUI.color = lastColor;

                m_EquipmentInstanceSerlz.Update();

                EditorGUILayout.HelpBox($"Define Equipment Info  |  {m_EquipmentInstance.GetNumberOfDefinedFields()} / {m_NumberOfFields}", MessageType.Info);

                //Draw General Info Foldout
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                showGeneralInfo = EditorGUILayout.Foldout(showGeneralInfo, "General Info", EditorGUICustom.FoldOutStyle);
                if (showGeneralInfo) DrawGeneralInfo();
                EditorGUILayout.EndVertical();

                //Draw Prefabs Info Foldout
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                drawPrefabsInfo = EditorGUILayout.Foldout(drawPrefabsInfo, "Prefabs", EditorGUICustom.FoldOutStyle);
                if (drawPrefabsInfo) DrawPrefabsInfo();
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                if (m_EquipmentInstance.GetNumberOfDefinedFields() < m_NumberOfFields)
                    GUI.enabled = false;

                if (GUILayout.Button($"Add: ''{m_EquipmentName}''", EditorGUICustom.LargeButtonStyle))
                {
                    DoStuff();
                    m_EquipmentInstance.ClearInfo();
                }

                m_EquipmentInstanceSerlz.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("No Equipment Instance Scriptable Object Found", MessageType.Error);
            }
        }

        private void DrawGeneralInfo() 
        {
            //Draw "Use Custom Category" Bool Field
            var useCustomCategory = m_EquipmentInfoProp.FindPropertyRelative("useCustomCategory");
            EditorGUILayout.PropertyField(useCustomCategory);
            if (!useCustomCategory.boolValue)
                GUI.enabled = false;

            //Draw Category Field
            EditorGUILayout.PropertyField(m_EquipmentInfoProp.FindPropertyRelative("itemCategory"));
            if (GUI.enabled == false)
                GUI.enabled = true;

            //Draw Equipment Name Field
            var nameProperty = m_EquipmentInfoProp.FindPropertyRelative("equipmentName");
            EditorGUILayout.PropertyField(nameProperty);
            m_EquipmentName = nameProperty.stringValue;
        }

        private void DrawPrefabsInfo() 
        {
            //Draw Player field
            EditorGUILayout.PropertyField(m_EquipmentInfoProp.FindPropertyRelative("player"));

            //Draw Equipment Item field
            var baseEquipmentItem = m_EquipmentInfoProp.FindPropertyRelative("baseEquipmentItem");
            EditorGUILayout.PropertyField(baseEquipmentItem);

            EditorGUICustom.Separator();
            DrawEquipmentPrefabInfo(baseEquipmentItem.objectReferenceValue != null);
        }

        private void DrawEquipmentPrefabInfo(bool enableGUI) 
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.enabled = enableGUI;

            EditorGUILayout.PropertyField(m_EquipmentInfoProp.FindPropertyRelative("itemFOV"));

            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }

        private void DoStuff()
        {

        }
    }
}
