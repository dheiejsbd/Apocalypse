using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    public class ObjectReferenceFillerEditor : Editor
    {
        private IObjectReferenceFiller m_ObjectReferenceFiller;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUICustom.Separator();

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.85f, 0.3f);

            if (GUILayout.Button("Try Auto-Fill Object References", EditorGUICustom.LargeButtonStyle))
            {
                TryAutoFillObjReferences();
            }

            GUI.backgroundColor = prevColor;
        }

        private void OnEnable()
        {
            m_ObjectReferenceFiller = target as IObjectReferenceFiller;
        }

        private void TryAutoFillObjReferences()
        {
            m_ObjectReferenceFiller.TryAutoFillObjectReferences();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
