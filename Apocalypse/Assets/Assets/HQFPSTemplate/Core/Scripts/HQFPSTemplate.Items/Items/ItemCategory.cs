using System;
using UnityEngine;

namespace HQFPSTemplate.Items
{
    [Serializable]
    public class ItemCategory
    {
        public string Name { get { return m_Name; } }

        public ItemInfo[] Items { get { return m_Items; } }

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ItemInfo[] m_Items;
    }
}