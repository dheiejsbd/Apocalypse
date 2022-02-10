using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    public partial class Compass : EquipmentItem
    {
        #region Internal
        [Serializable]
        private class CompassSettings
        {
            public Transform CompassRose;
        }
        #endregion

        [SerializeField]
        [Group]
        private CompassSettings m_CompassSettings;

        private CompassInfo m_CompassInfo;       

        private Vector3 m_CurrentRoseRotation;
        private Vector3 m_NorthDirection;


        public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

            m_CompassInfo = EInfo as CompassInfo;

            if (WorldManager.Instance != null)
                m_NorthDirection = WorldManager.Instance.NorthDirection;
            else
                m_NorthDirection = Vector3.forward;
        }

        private void LateUpdate()
        {
            float angle = -Vector3.SignedAngle(Player.transform.forward, m_NorthDirection, Vector3.up);

            m_CurrentRoseRotation = Vector3.Scale(new Vector3(angle, angle, angle), m_CompassInfo.CompassSettings.CompassRoseRotationAxis.normalized);
            m_CompassSettings.CompassRose.localRotation = Quaternion.Euler(m_CompassSettings.CompassRose.localEulerAngles + m_CurrentRoseRotation);
        }
    }
}
