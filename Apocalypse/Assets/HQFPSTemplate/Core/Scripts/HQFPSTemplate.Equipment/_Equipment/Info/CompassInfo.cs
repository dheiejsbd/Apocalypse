using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Compass Info", menuName = "HQ FPS Template/Equipment/Compass")]
    public class CompassInfo : EquipmentItemInfo
    {
        #region Internal
        [Serializable]
        public class CompassSettingsInfo
        {
            public Vector3 CompassRoseRotationAxis = new Vector3(0, 0, 1);
        }
        #endregion

        [Group("5: ")] public CompassSettingsInfo CompassSettings = null;
    }
}
