using UnityEngine;
using System;

namespace HQFPSTemplate
{
    [Serializable]
    public class RecoilForce
    {
        #region Internal
        [System.Serializable]
        public struct ForceJitter
        {
            [Range(0, 1)]
            public float xJitter;

            [Range(0, 1)]
            public float yJitter;

            [Range(0, 1)]
            public float zJitter;
        }
        #endregion

        public Vector3 RotationForce => Vector3Utils.JitterVector(m_RotForce, JitterForce.xJitter, JitterForce.yJitter, JitterForce.zJitter);
        public Vector3 PositionForce => Vector3Utils.JitterVector(m_PosForce, JitterForce.xJitter, JitterForce.yJitter, JitterForce.zJitter) / 100;

        [SerializeField]
        private Vector3 m_RotForce;

        [SerializeField]
        private Vector3 m_PosForce;

        [Range(0, 20)]
        public int Distribution;

        [Space]

        [Tooltip("max randomness for each axis")]
        [Group]
        public ForceJitter JitterForce;
    }
}

