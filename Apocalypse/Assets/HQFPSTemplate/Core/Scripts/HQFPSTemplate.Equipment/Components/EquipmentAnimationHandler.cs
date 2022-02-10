using System.Collections.Generic;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    [RequireComponent(typeof(EquipmentItem))]
    public class EquipmentAnimationHandler : MonoBehaviour, IEquipmentComponent
    {
        [SerializeField]
        public AnimationOverrideClips m_EquipmentClips = null, m_FPArmsClips = null;

        private EquipmentItem m_EItem;


        public void Initialize(EquipmentItem equipmentItem)
        {
            m_EItem = equipmentItem;

            //Assign the equipment animations
            AssignAnimations(m_EItem.Animator, m_EquipmentClips);          
        }

        public void OnSelected() 
        {
            //Assign the arm animations
            AssignAnimations(m_EItem.EHandler.FPArmsHandler.Animator, m_FPArmsClips);
        }

        private void AssignAnimations(Animator animator, AnimationOverrideClips animationOverrideClips)
        {
            if (animator != null && animationOverrideClips.Controller != null)
            {
                var overrideController = new AnimatorOverrideController(animationOverrideClips.Controller);
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

                foreach (var clipPair in animationOverrideClips.Clips)
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clipPair.Original, clipPair.Override));

                overrideController.ApplyOverrides(overrides);
                animator.runtimeAnimatorController = overrideController;
            }
        }
    }
}