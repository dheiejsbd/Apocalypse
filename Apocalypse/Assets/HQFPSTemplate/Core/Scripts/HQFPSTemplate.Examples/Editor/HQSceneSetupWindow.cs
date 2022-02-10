using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Examples
{
    public class HQSceneSetupWindow : EditorWindow
    {
        private string m_CorePrefabsSearchPath = "HQFPSTemplate/_Prefabs/_Core/";
        private Vector2 m_CurrentScrollPosition;

        private bool m_AddSceneToBuildSettings = true;
        private bool m_AddAllCorePrefabsToScene = true;
        private bool m_AddPlayerToScene = false;
        private bool m_AddManagerToScene = false;
        private bool m_AddSceneUIToScene = false;
        private bool m_AddPlayerUIToScene = false;
        private bool m_AddPostProcessingToScene = false;


        [MenuItem("HQ FPS Template/Scene Setup...", false)]
        public static void Init() 
        {
            var window = GetWindow<HQSceneSetupWindow>(true, "Scene Setup");
            window.minSize = new Vector2(512, 512);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_CurrentScrollPosition = EditorGUILayout.BeginScrollView(m_CurrentScrollPosition);

            EditorGUILayout.HelpBox("Use this tool to Clean/Setup your Scenes to function correctly with ''HQ FPS Template''.", MessageType.Info);

            DrawCorePrefabsInfo();
            EditorGUILayout.Space(2);
            DrawMiscInfo();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Spawn Selected to Scene  ", EditorGUICustom.LargeButtonStyle))
            {
                SpawnCorePrefabs();
                ClearWindowInstanceData();
            }

            if (GUILayout.Button("Clean Selected from Scene", EditorGUICustom.LargeButtonStyle))
            {
                
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawMiscInfo()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Misc Settings", EditorGUICustom.BottomLeftBoldMiniLabel);

            m_AddSceneToBuildSettings = EditorGUILayout.ToggleLeft("Add Scene To Build Settings", m_AddSceneToBuildSettings);

            EditorGUILayout.EndVertical();
        }

        private void DrawCorePrefabsInfo() 
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Core Prefabs", EditorGUICustom.BottomLeftBoldMiniLabel);

            m_CorePrefabsSearchPath = EditorGUILayout.TextField("Core Prefabs Search Path:", m_CorePrefabsSearchPath);
            EditorGUICustom.Separator(new Color(0.4f, 0.4f, 0.4f, 1f));

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            m_AddAllCorePrefabsToScene = EditorGUILayout.Toggle("Select All", m_AddAllCorePrefabsToScene);

            if (m_AddAllCorePrefabsToScene)
                GUI.enabled = false;

            if (m_AddAllCorePrefabsToScene)
            {
                m_AddPlayerToScene = true;
                m_AddManagerToScene = true;
                m_AddSceneUIToScene = true;
                m_AddPlayerUIToScene = true;
                m_AddPostProcessingToScene = true;
            }

            m_AddPlayerToScene = EditorGUILayout.ToggleLeft("Player", m_AddPlayerToScene);
            m_AddManagerToScene = EditorGUILayout.ToggleLeft("Game Manager", m_AddManagerToScene);
            m_AddSceneUIToScene = EditorGUILayout.ToggleLeft("Scene UI", m_AddSceneUIToScene);
            m_AddPlayerUIToScene = EditorGUILayout.ToggleLeft("Player UI", m_AddPlayerUIToScene);
            m_AddPostProcessingToScene = EditorGUILayout.ToggleLeft("Post Processing Manager", m_AddPostProcessingToScene);

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void SpawnCorePrefabs()
        {
            var prefabs = new List<GameObject>();
            /*
            if (m_AddPlayerToScene)
            {
                var prefabGuid = Resources.LoadAll<Player>("");
                Debug.Log(prefabGuid[0].gameObject.name);
            }*/
        }

        private void DestroyCorePrefabs() 
        {
            
        }

        private void ClearWindowInstanceData()
        {

        }
    }
}
