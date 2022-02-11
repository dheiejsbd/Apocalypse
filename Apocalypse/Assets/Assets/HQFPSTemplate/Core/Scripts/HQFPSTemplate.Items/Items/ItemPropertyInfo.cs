using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Items
{
    /// <summary>
    /// This is data for a property that's defined in the item database.
    /// </summary>
    [Serializable]
    public class ItemPropertyInfo
    {
        public string Name { get => m_Name; }
        public ItemPropertyType Type { get => m_Type; }

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ItemPropertyType m_Type;

        [SerializeField]
        private float m_FixedValue;

        [SerializeField]
        private bool m_UseRandomValue;

        [SerializeField]
        private Vector2 m_RandomValueRange;


        /// <summary>
        /// Retrieves the internal value as a boolean.
        /// </summary>
        public bool GetAsBoolean()
        {
            return GetAsInteger() > 0;
        }

        /// <summary>
        /// Retrieves the internal value as an integer.
        /// </summary>
        public int GetAsInteger()
        {
            return (int)GetInternalValue();
        }

        /// <summary>
        /// Retrieves the internal value as a float.
        /// </summary>
        public float GetAsFloat()
        {
            return GetInternalValue();
        }

        private float GetInternalValue()
        {
            if(m_Type == ItemPropertyType.Boolean || m_Type == ItemPropertyType.ItemId)
                return m_FixedValue;
            else
            {
                float value = 0f;

                if(m_Type == ItemPropertyType.Float)
                    value = m_UseRandomValue ? Random.Range(m_RandomValueRange.x, m_RandomValueRange.y) : m_FixedValue;
                else if(m_Type == ItemPropertyType.Integer)
                    value = m_UseRandomValue ? Random.Range((int)m_RandomValueRange.x, (int)m_RandomValueRange.y) : m_FixedValue;

                return value;
            }
        }
    }
}