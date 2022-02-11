using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Equipment
{
    [CustomEditor(typeof(EquipmentAnimationHandler))]
    public class EquipmentAnimationEditor : Editor
    {
        private SerializedProperty m_EquipmentClipsProp;
        private SerializedProperty m_FPArmsClipsProp;

        private static int m_SelectedToolbarIdx;


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int previousIndex = m_SelectedToolbarIdx;
            m_SelectedToolbarIdx = GUILayout.Toolbar(m_SelectedToolbarIdx, new string[] { "Equipment Clips", "Arm Clips"});

            if (m_SelectedToolbarIdx != previousIndex)
                SceneView.RepaintAll();

            EditorGUILayout.Space();

            if (m_SelectedToolbarIdx == 0)
                EditorGUILayout.PropertyField(m_EquipmentClipsProp);
            else if (m_SelectedToolbarIdx == 1)
                EditorGUILayout.PropertyField(m_FPArmsClipsProp);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_EquipmentClipsProp = serializedObject.FindProperty("m_EquipmentClips");
            m_FPArmsClipsProp = serializedObject.FindProperty("m_FPArmsClips");
        }
    }

    [CustomEditor(typeof(EquipmentAnimationInfo))]
    public class EquipmentAnimationsEditor : Editor
    {
        private SerializedProperty m_EquipmentClipsProp;
        private SerializedProperty m_FPArmsClipsProp;

        private static int m_SelectedToolbarIdx;


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int previousIndex = m_SelectedToolbarIdx;
            m_SelectedToolbarIdx = GUILayout.Toolbar(m_SelectedToolbarIdx, new string[] { "Equipment Clips", "Arm Clips" });

            if (m_SelectedToolbarIdx != previousIndex)
                SceneView.RepaintAll();

            EditorGUILayout.Space();

            if (m_SelectedToolbarIdx == 0)
                EditorGUILayout.PropertyField(m_EquipmentClipsProp);
            else if (m_SelectedToolbarIdx == 1)
                EditorGUILayout.PropertyField(m_FPArmsClipsProp);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_EquipmentClipsProp = serializedObject.FindProperty("m_EquipmentClips");
            m_FPArmsClipsProp = serializedObject.FindProperty("m_FPArmsClips");
        }
    }
}
