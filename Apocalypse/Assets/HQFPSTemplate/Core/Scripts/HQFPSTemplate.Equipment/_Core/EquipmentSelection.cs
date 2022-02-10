using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.Equipment
{
    public class EquipmentSelection : PlayerComponent
    {
        [BHeader("General")]

        [SerializeField]
        [PlayerItemContainer]
        private string m_HolsterContainerName;

        [SerializeField]
        [Range(1, 8)]
        private int m_FirstSelected = 1;

        [BHeader("Navigation")]

        [SerializeField]
        private bool m_EnableScrolling = true;

        [SerializeField]
        [ShowIf("m_EnableScrolling", true)]
        private bool m_InvertScrollDirection;

        [SerializeField]
        [ShowIf("m_EnableScrolling", true)]
        [Clamp(0f, 1f)]
        private float m_ScrollThreshold = 0.3f;

        [SerializeField]
        [ShowIf("m_EnableScrolling", true)]
        [Clamp(0f, 1f)]
        private float m_ScrollPause = 0.3f;

        [SerializeField]
        [ShowIf("m_EnableScrolling", true)]
        private bool m_ScrollThroughEmptySlots = false;

        [SerializeField]
        private bool m_SelectByDigits = true;

        [SerializeField]
        [ShowIf("m_SelectByDigits", true)]
        [Clamp(0f, 1f)]
        private float m_SelectThreshold = 0.3f;

        private ItemContainer m_HolsterContainer;

        private int m_CurrentScrollIndex;
        private float m_CurScrollValue;
        private float m_NextTimeCanSelect;
        private float m_CanSelectTime;


        private void Start()
        {
            m_HolsterContainer = Player.Inventory.GetContainerWithName(m_HolsterContainerName);
            Player.DropItem.AddListener(OnPlayerDropItem);
            Player.Respawn.AddListener(OnPlayerRespawn);

            if (m_HolsterContainer != null)
            {
                m_HolsterContainer.SelectedSlot.AddChangeListener(TrySelectSlot);

                int selectIndex = m_FirstSelected - 1;

                if (Player.EquippedItem.Get() != null)
                    selectIndex = m_HolsterContainer.GetPositionOfItem(Player.EquippedItem.Get());

                m_HolsterContainer.SelectedSlot.SetAndForceUpdate(selectIndex);
                m_HolsterContainer.Changed.AddListener(OnHolsterContainerUpdate);
            }

            m_CanSelectTime = Time.time + 0.3f;
        }

        private void OnPlayerRespawn() 
        {
            TrySelectSlot(m_FirstSelected);
        }

        private void OnPlayerDropItem(Item droppedItem) 
        {
            Player.EquipItem.Try(null, true);
        }

        private void OnHolsterContainerUpdate(ItemSlot slot) 
        {
            if (m_CanSelectTime < Time.time)
            {
                if (slot.Item != null)
                {
                    int slotIndex = m_HolsterContainer.GetPositionOfItem(slot.Item);

                    m_HolsterContainer.SelectedSlot.SetAndForceUpdate(slotIndex);
                }
            }
        }

        private void Update()
        {
            if(Player == null || Player.Healing.Active)
                return;

            if(m_SelectByDigits && Input.anyKeyDown && m_NextTimeCanSelect < Time.time)
            {
                if(int.TryParse(Input.inputString, out int keyNumber))
                    m_HolsterContainer.SelectedSlot.Set(keyNumber - 1);

                m_NextTimeCanSelect = Time.time + m_SelectThreshold;
            }

            if(m_EnableScrolling && !Player.Pause.Active)
            {
                var playerScrollValue = Player.ScrollValue.Get();
                playerScrollValue *= m_InvertScrollDirection ? 1 : -1;

                m_CurScrollValue = Mathf.Clamp(m_CurScrollValue + playerScrollValue, -m_ScrollThreshold, m_ScrollThreshold);

                if(Mathf.Abs(m_CurScrollValue - m_ScrollThreshold * Mathf.Sign(playerScrollValue)) < Mathf.Epsilon && m_NextTimeCanSelect < Time.time)
                {
                    m_CurScrollValue = 0f;

                    int lastScrollIndex = m_CurrentScrollIndex;

                    if (!m_ScrollThroughEmptySlots)
                    {
                        while (true)
                        {
                            m_CurrentScrollIndex = (int)Mathf.Repeat(m_CurrentScrollIndex + (playerScrollValue >= 0f ? 1 : -1), m_HolsterContainer.Slots.Length);

                            if (m_HolsterContainer.Slots[m_CurrentScrollIndex].HasItem)
                            {
                                m_HolsterContainer.SelectedSlot.Set(m_CurrentScrollIndex);
                                break;
                            }

                            if (lastScrollIndex == m_CurrentScrollIndex)
                                break;
                        }
                    }
                    else
                    {
                        m_CurrentScrollIndex = (int)Mathf.Repeat(m_CurrentScrollIndex + (playerScrollValue >= 0f ? 1 : -1), m_HolsterContainer.Slots.Length);
                        m_HolsterContainer.SelectedSlot.Set(m_CurrentScrollIndex);
                    }

                    m_NextTimeCanSelect = Time.time + m_ScrollPause;
                }
            }
        }

        private void TrySelectSlot(int index)
        {
            var slot = m_HolsterContainer.Slots[Mathf.Clamp(index, 0, m_HolsterContainer.Slots.Length - 1)];
            Player.EquipItem.Try(slot.Item, false);
        }
    }
}