using System;
using UnityEngine;

namespace HQFPSTemplate.Items
{
    /// <summary>
    /// An item property can hold a float value which can also be manipulated as a bool and integer.
    /// </summary>
    [Serializable]
    public class ItemProperty
    {
        public Message<ItemProperty> Changed = new Message<ItemProperty>();

        public string Name { get => m_Name; }
        public ItemPropertyType Type { get => m_Type; }

        public bool Boolean
        {
            get
            {
                return m_Value > 0f;
            }
            set
            {
                if(m_Type == ItemPropertyType.Boolean)
                    SetInternalValue(value == true ? 1 : 0);
            }
        }

        public int Integer
        {
            get
            {
                return (int)m_Value;
            }
            set
            {
                if(m_Type == ItemPropertyType.Integer)
                    SetInternalValue(value);
            }
        }

        public float Float
        {
            get
            {
                return m_Value;
            }
            set
            {
                if(m_Type == ItemPropertyType.Float)
                    SetInternalValue(value);
            }
        }

        public int ItemId
        {
            get
            {
                return (int)m_Value;
            }
            set
            {
                if(m_Type == ItemPropertyType.ItemId)
                    SetInternalValue(Mathf.Clamp(value, -9999999, 9999999));
            }
        }

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ItemPropertyType m_Type;

        [SerializeField]
        private float m_Value;


        public ItemProperty(ItemPropertyInfo propertyInfo)
        {
            m_Name = propertyInfo.Name;
            m_Type = propertyInfo.Type;

            m_Value = propertyInfo.GetAsFloat();
        }

        public ItemProperty GetMemberwiseClone()
        {
            return (ItemProperty)MemberwiseClone();
        }

        private void SetInternalValue(float value)
        {
            float oldValue = m_Value;
            m_Value = value;

            if(oldValue != m_Value)
                Changed.Send(this);
        }
    }
}