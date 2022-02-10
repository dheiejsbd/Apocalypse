using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Healing Item Info", menuName = "HQ FPS Template/Equipment/Healing Item")]
    public class HealingItemInfo : EquipmentItemInfo
    {
        #region Internal
        [Serializable]
        public class HealingSettingsInfo
        {
            [Range(0.1f, 5f)]
            public float HealTime = 2f;

            [Range(0f, 10f)]
            public float UpdateHealthDelay = 1f;

            [Space(3f)]

            [MinMax(0, 100f)]
            public Vector2 HealAmount = new Vector2(40, 50);

            [Space(3f)]

            [BHeader("( Animation )", order = 2)]

            public float HealAnimSpeed = 1f;

            [BHeader("( Audio )")]

            [Group]
            public DelayedSound[] HealingAudio;

            [BHeader("( Camera )")]

            [Group]
            public DelayedCameraForce[] HealingCameraForces;
        }
        #endregion

        [Group("5: ")] public HealingSettingsInfo HealingSettings;
    }
}
