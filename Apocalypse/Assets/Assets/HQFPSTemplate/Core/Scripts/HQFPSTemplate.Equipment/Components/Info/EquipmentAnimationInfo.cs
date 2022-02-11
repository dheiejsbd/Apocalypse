using System.Collections.Generic;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Equipment Animation", menuName = "HQ FPS Template/Equipment Component/Animation")]
    public class EquipmentAnimationInfo : ScriptableObject
    {
        [SerializeField]
        public AnimationOverrideClips m_EquipmentClips = null, m_FPArmsClips = null;


        public void AssignEquipmentAnimation(Animator animator)
        {
            AssignAnimations(animator, m_EquipmentClips);
        }

        public void AssignArmAnimations(Animator animator)
        {
            AssignAnimations(animator, m_FPArmsClips);
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
