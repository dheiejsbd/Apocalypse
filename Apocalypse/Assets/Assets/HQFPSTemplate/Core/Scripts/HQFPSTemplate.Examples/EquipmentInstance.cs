using HQFPSTemplate.Equipment;
using UnityEngine;
using System;

namespace HQFPSTemplate.Examples
{
    public class EquipmentInstance : ScriptableObject
    {
        [Serializable]
        public class EquipmentInfo 
        {
            public bool useCustomCategory;
            public string equipmentName;

            [DatabaseCategory] public string itemCategory;

            public Player player;
            public EquipmentItem baseEquipmentItem;
            public string equipmentHandlerName;

            [Range(10, 120)]
            public int itemFOV = 50;

            public UseConditions useConditions;
        }

        [Serializable]
        public struct UseConditions
        {
            [BHeader("Use Settings", order = 2)]

            public bool UseWhileAirborne;
            public bool UseWhileRunning;
            public bool CanStopReloading;
        }

        [HideInInspector]
        public EquipmentInfo m_EquipmentInfo;


        public void ClearInfo() 
        {
            m_EquipmentInfo.equipmentName = "";
            m_EquipmentInfo.itemCategory = "";
            m_EquipmentInfo.player = null;
            m_EquipmentInfo.baseEquipmentItem = null;
            m_EquipmentInfo.itemFOV = 50;
        }

        public int GetNumberOfDefinedFields() 
        {
            int numberOfDefinedFields = 0;

            if (!string.IsNullOrEmpty(m_EquipmentInfo.equipmentName)) numberOfDefinedFields++;
            if (!string.IsNullOrEmpty(m_EquipmentInfo.itemCategory)) numberOfDefinedFields++;
            if (m_EquipmentInfo.player != null) numberOfDefinedFields++;
            if (m_EquipmentInfo.baseEquipmentItem != null) numberOfDefinedFields++;
            if (m_EquipmentInfo.itemFOV >= 10) numberOfDefinedFields++;

            return numberOfDefinedFields;
        }
    }
}
