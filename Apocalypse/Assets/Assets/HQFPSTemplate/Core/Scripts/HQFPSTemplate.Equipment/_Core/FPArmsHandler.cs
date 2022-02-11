using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HQFPSTemplate.Equipment
{
    public class FPArmsHandler : MonoBehaviour
    {
        public Animator Animator => m_Animator;

        [SerializeField]
        private Animator m_Animator = null;

        [SerializeField]
        private EquipmentHandler m_EquipmentHandler = null;

        [Space]

        [SerializeField]
        private string m_FovProperty = "_FOV";

        [SerializeField]
        private SkinnedMeshRenderer m_LeftArm = null;

        [SerializeField]
        private SkinnedMeshRenderer m_RightArm = null;

        [Space]

        [SerializeField]
        [Reorderable]
        private FPArmsInfoList m_FPArms = null;


        public void UpdateArms(ref int selectedArmsIndex) 
        {
            var armsInfo = m_FPArms.ToArray().Select(ref selectedArmsIndex, ItemSelection.Method.Sequence);

            m_LeftArm.sharedMesh = armsInfo.LeftArm.sharedMesh;
            m_LeftArm.sharedMaterials = armsInfo.LeftArm.sharedMaterials;

            m_RightArm.sharedMesh = armsInfo.RightArm.sharedMesh;
            m_RightArm.sharedMaterials = armsInfo.RightArm.sharedMaterials;
        }

        private void Awake()
        {
            m_EquipmentHandler.OnChangeItem.AddListener(UpdateFOV);
        }

        private void OnDestroy()
        {
            m_EquipmentHandler.OnChangeItem.RemoveListener(UpdateFOV);

            #if UNITY_EDITOR
            if (Application.isEditor)
                ResetArms();
            #endif
        }

        private void UpdateFOV()
        {
            float fov = m_EquipmentHandler.EquipmentItem.EModel.TargetFOV;

            foreach (var material in m_LeftArm.sharedMaterials)
                material.SetFloat(m_FovProperty, fov);

            foreach (var material in m_RightArm.sharedMaterials)
                material.SetFloat(m_FovProperty, fov);
        }

        #if UNITY_EDITOR
        private void ResetArms()
        {
            List<Material> armMaterials = new List<Material>();

            // Get all of the distinct materials used on the arms
            foreach (var armSet in m_FPArms)
            {
                foreach (var material in armSet.LeftArm.sharedMaterials)
                {
                    if (!armMaterials.Contains(material))
                        armMaterials.Add(material);
                }

                foreach (var material in armSet.RightArm.sharedMaterials)
                {
                    if (!armMaterials.Contains(material))
                        armMaterials.Add(material);
                }
            }

            /*
            // Reset FOV
            for (int i = 0; i < armMaterials.Count; i++)
                armMaterials[i].SetFloat(m_FovProperty, SceneView.GetAllSceneCameras()[0].fieldOfView);*/
        }
        #endif
    }
}
