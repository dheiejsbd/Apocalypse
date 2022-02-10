using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Surfaces
{
    [CustomEditor(typeof(SurfaceManager))]
    public class SurfaceManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if(GUILayout.Button("Open Surface Editor", EditorStyles.miniButtonMid))
                SurfaceManagementWindow.Init();
        }
    }
}