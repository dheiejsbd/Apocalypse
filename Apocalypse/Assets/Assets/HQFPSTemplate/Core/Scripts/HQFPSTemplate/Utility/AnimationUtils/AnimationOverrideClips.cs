using System;
using UnityEngine;

namespace HQFPSTemplate
{
    [Serializable]
    public class AnimationOverrideClips : ICloneable
    {
        [Serializable]
        public struct AnimationClipPair
        {
            public AnimationClip Original;
            public AnimationClip Override;
        }

        public RuntimeAnimatorController Controller { get => m_Controller; }
        public AnimationClipPair[] Clips { get => m_Clips; }

        [SerializeField]
        private RuntimeAnimatorController m_Controller = null;

        [SerializeField]
        private AnimationClipPair[] m_Clips = null;

        public object Clone() => MemberwiseClone();
    }
}