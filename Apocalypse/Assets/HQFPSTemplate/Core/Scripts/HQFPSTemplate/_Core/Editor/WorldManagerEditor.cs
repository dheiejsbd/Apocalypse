using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomEditor(typeof(WorldManager))]
    public class WorldManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(3f);

            GUILayout.Label("Note: The North direction is represented by a Gizmo");

            base.OnInspectorGUI();
        }
    }
}
