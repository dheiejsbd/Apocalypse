using UnityEditor;
using UnityEngine;

namespace HQFPSTemplate
{
    [CustomEditor(typeof(LightEffect))]
    public class LightEffectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if(!Application.isPlaying)
                GUI.enabled = false;

            if(GUILayout.Button("Play (fadeIn = true)"))
                (target as LightEffect).Play(true);

            if(GUILayout.Button("Play (fadeIn = false)"))
                (target as LightEffect).Play(false);

            if(!Application.isPlaying)
                GUI.enabled = true;
        }
    }
}