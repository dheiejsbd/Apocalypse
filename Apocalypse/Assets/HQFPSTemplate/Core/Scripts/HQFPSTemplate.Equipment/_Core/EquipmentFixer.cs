using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Equipment
{
    public class EquipmentFixer : MonoBehaviour
    {
        [HideInInspector]
        public List<EquipmentItem> m_EquipmentItems = new List<EquipmentItem>();


        #if UNITY_EDITOR
        public void Initialize()
        {
            var childrens = GetComponentsInChildren<EquipmentItem>(true);

            m_EquipmentItems.Clear();

            foreach (var child in childrens)
            {
                if(!m_EquipmentItems.Contains(child))
                    m_EquipmentItems.Add(child);
            }
        }

        public void EnableAllEquipment(bool enable)
        {
            foreach (var eItem in m_EquipmentItems)
            {
                eItem.gameObject.SetActive(enable);
            }
        }

        public bool TryGetFixedItems(out bool[] isFixed, out int fixedCount, int equipmentLayer) 
        {
            isFixed = new bool[m_EquipmentItems.Count];
            fixedCount = 0;

            if (m_EquipmentItems.Count == 0)
                return false;

            for (int i = 0; i < m_EquipmentItems.Count; i++)
            {
                isFixed[i] = false;
                var animator = m_EquipmentItems[i].GetComponentInChildren<Animator>(true);

                if (animator != null)
                {
                    if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                        continue;
                }

                /*
                float modelFOV = m_EquipmentItems[i].EModel.GetMaterialFOV();

                if (modelFOV > 0 && modelFOV != SceneView.GetAllSceneCameras()[0].fieldOfView)
                    continue;*/

                var skinnedRenderers = m_EquipmentItems[i].GetComponentsInChildren<SkinnedMeshRenderer>(true);           

                if (skinnedRenderers != null)
                {
                    bool badRendererSettings = false;

                    foreach (var skinRenderer in skinnedRenderers)
                    {
                        if (skinRenderer.updateWhenOffscreen != true ||
                            skinRenderer.shadowCastingMode !=
                            UnityEngine.Rendering.ShadowCastingMode.Off ||
                            skinRenderer.skinnedMotionVectors ||
                            skinRenderer.allowOcclusionWhenDynamic ||
                            skinRenderer.gameObject.layer != equipmentLayer)
                        {
                            badRendererSettings = true;
                            continue;
                        }
                    }

                    if (badRendererSettings)
                        continue;
                }

                isFixed[i] = true;
                fixedCount++;
            }

            return true;
        }

        public bool TryGetAppliedPrefabs(out bool[] isApplied, out int appliedCount)
        {
            isApplied = new bool[m_EquipmentItems.Count];
            appliedCount = 0;

            if (m_EquipmentItems.Count == 0)
                return false;

            for (int i = 0; i < m_EquipmentItems.Count; i++)
            {
                isApplied[i] = false;

                if (PrefabUtility.HasPrefabInstanceAnyOverrides(m_EquipmentItems[i].gameObject, false))
                    continue;

                isApplied[i] = true;
                appliedCount++;
            }

            return true;
        }

        public int GetEnabledItemsCount()
        {
            int enabledCount = 0;

            foreach (var eItem in m_EquipmentItems)
            {
                if (eItem.gameObject.activeSelf == true)
                    enabledCount++;
            }

            return enabledCount;
        }

        public void FixEquipmentItems(int equipmentLayer) 
        {
            foreach (var eItem in m_EquipmentItems)
            {
                var animator = eItem.GetComponentInChildren<Animator>(true);
                var skinnedRenderers = eItem.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                if (animator != null)
                    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                if (skinnedRenderers != null)
                {
                    foreach (var skinRenderer in skinnedRenderers)
                    {
                        skinRenderer.updateWhenOffscreen = true;
                        skinRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        skinRenderer.skinnedMotionVectors = false;
                        skinRenderer.allowOcclusionWhenDynamic = false;

                        if (skinRenderer.gameObject.layer != equipmentLayer)
                            skinRenderer.gameObject.layer = equipmentLayer;
                    }
                }

                eItem.EModel.UpdateMaterialsFOV(SceneView.GetAllSceneCameras()[0].fieldOfView);
            }
        }

        public void ApplyEquipmentPrefabs(bool enableBeforeApplying = false) 
        {
            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(m_EquipmentItems[0].gameObject);

            bool hasRoot = false;
            if (prefabRoot != null && prefabRoot != m_EquipmentItems[0].gameObject)
                hasRoot = true;
            string rootPath = "";

            if (hasRoot)
            {
                rootPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
                PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }

            foreach (var eItem in m_EquipmentItems)
            {
                if (enableBeforeApplying && !eItem.gameObject.activeSelf)
                    eItem.gameObject.SetActive(true);

                if (eItem.gameObject.activeSelf)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(eItem.gameObject))
                    {
                        if (PrefabUtility.HasPrefabInstanceAnyOverrides(eItem.gameObject, false))
                            PrefabUtility.ApplyPrefabInstance(eItem.gameObject, InteractionMode.UserAction);
                    }
                }
            }

            if(hasRoot)
                PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, rootPath, InteractionMode.AutomatedAction);
        }

        private void Start()
        {
            if (Application.isPlaying && !Application.isEditor)
                Destroy(this);
        }

        private void OnDestroy()
        {
            Initialize();

            foreach (var eItem in m_EquipmentItems)
            {/*
                if (eItem.EModel != null)
                    eItem.EModel.UpdateMaterialsFOV(SceneView.GetAllSceneCameras()[0].fieldOfView);*/
            }
        }
        #endif
    }
}
