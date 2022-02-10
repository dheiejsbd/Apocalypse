using UnityEngine.UI;
using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.UserInterface
{
    public class UI_HealsInfo : UserInterfaceBehaviour
    {
        [BHeader("General")]

        [SerializeField]
        private Text m_HealsAmountText = null;

        [SerializeField]
        [PlayerItemContainer]
        private string m_HealsContainerName = "Heals Pouch";

        [BHeader("Low Health Image")]

        [SerializeField]
        private float m_LowHealthThreshold = 0f;

        [SerializeField]
        private CanvasGroup m_HealCanvas = null;

        private ItemContainer m_HealsContainer;


        public override void OnPostAttachment()
        {
            m_HealsContainer = Player.Inventory.GetContainerWithName(m_HealsContainerName);

            OnContainerChanged(null);

            m_HealsContainer.Changed.AddListener(OnContainerChanged);
            Player.Healing.AddStopListener(OnEndHealing);
            Player.Health.AddChangeListener(OnPlayerChangeHealth);

            OnPlayerChangeHealth(Player.Health.Val);
        }

        private void OnEndHealing()
        {
            OnContainerChanged(null);
        }

        private void OnPlayerChangeHealth(float healthAmount) 
        {
            if (Player.Health.Val == 0)
            {
                m_HealCanvas.alpha = 0;
                return;
            }

            if (healthAmount < m_LowHealthThreshold)
                m_HealCanvas.alpha = 1;
            else if (m_HealCanvas.gameObject.activeSelf)
                m_HealCanvas.alpha = 0;
        }

        private void OnContainerChanged(ItemSlot itemSlot)
        {
            int healsAmount = 0;

            if (m_HealsContainer != null)
            {
                foreach (var slot in m_HealsContainer.Slots)
                {
                    if (slot.HasItem)
                        healsAmount += slot.Item.CurrentStackSize;
                }

                m_HealsAmountText.text = string.Format("x {0}", healsAmount.ToString());
            }
        }
    }
}
