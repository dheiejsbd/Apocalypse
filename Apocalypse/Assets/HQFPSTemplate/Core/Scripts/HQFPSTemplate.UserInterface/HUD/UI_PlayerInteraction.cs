using UnityEngine;
using UnityEngine.UI;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.UserInterface
{
	public class UI_PlayerInteraction : UserInterfaceBehaviour
	{
		#region Anim Hashing
		//Hashed animator strings (Improves performance)
		private readonly int animHash_Show = Animator.StringToHash("Show");
        #endregion

        [BHeader("Generic Interaction", true)]

		[SerializeField]
		private Animator m_GenericInteractionAnimator = null;

		[SerializeField]
		private Text m_GenericText = null;

		[BHeader("Equipment Specific Interaction", true)]

		[SerializeField]
		[PlayerItemContainer]
		private string m_HolsterContainerName = "Holster";

		[SerializeField]
		private Animator m_EquipmentInteractionAnimator = null;

		[Space]

		[SerializeField]
		private Image m_SwapIcon;

		[SerializeField]
		private Image m_EquippedItemImg;

		[SerializeField]
		private Image m_GroundItemImg;

		private ItemContainer m_HolsterContainer;

		private RaycastInfo m_RaycastData;
		private bool m_SwapUIEnabled;


		public override void OnPostAttachment()
		{
			Player.RaycastInfo.AddChangeListener(OnPlayerRaycastChanged);
			Player.EquippedItem.AddChangeListener((Item item) => { OnPlayerRaycastChanged(Player.RaycastInfo.Val); } );

			m_HolsterContainer = Player.Inventory.GetContainerWithName(m_HolsterContainerName);
		}

		private void OnPlayerRaycastChanged(RaycastInfo raycastData)
		{
			bool show = raycastData != null && raycastData.IsInteractive;

			if(m_RaycastData != null)
				m_RaycastData.InteractiveObject.InteractionText.RemoveChangeListener(UpdateInteractText);

			m_RaycastData = raycastData;

			if (show)
			{
				ItemPickup itemPickup = m_RaycastData.InteractiveObject as ItemPickup;

				//Enable Swap Weapon UI
				if (itemPickup != null && IsSwappable(itemPickup.ItemInstance))
					UpdateSwapUI(itemPickup, true);
				//Enable Main Interaction UI
				else
				{
					if(m_SwapUIEnabled)
						UpdateSwapUI(null, false);

					m_GenericInteractionAnimator.SetBool(animHash_Show, true);
					UpdateInteractText(m_RaycastData.InteractiveObject.InteractionText.Val);

					m_RaycastData.InteractiveObject.InteractionText.AddChangeListener(UpdateInteractText);
				}
			}
			else
			{
				m_GenericInteractionAnimator.SetBool(animHash_Show, false);
				m_EquipmentInteractionAnimator.SetBool(animHash_Show, false);
			}
		}

		private void UpdateSwapUI(ItemPickup pickup, bool enable)
		{
			if (pickup != null)
			{
				if (enable)
					pickup.InteractionProgress.AddChangeListener(UpdateInteractProgressIMG);

				UpdateInteractProgressIMG(0f);

				m_EquippedItemImg.sprite = Player.EquippedItem.Val.Info.Icon;
			    m_GroundItemImg.sprite = pickup.ItemInstance.Info.Icon;

				m_GenericInteractionAnimator.SetBool(animHash_Show, false);
				m_EquipmentInteractionAnimator.SetBool(animHash_Show, enable);

				m_SwapUIEnabled = true;
			}
			else
			{
				m_EquipmentInteractionAnimator.SetBool(animHash_Show, false);

				m_SwapUIEnabled = false;
			}
		}

		private void UpdateInteractText(string text) 
		{
			m_GenericText.text = text;
		}

		private void UpdateInteractProgressIMG(float amount)
		{
			if (amount < 0.1f)
				amount = 0f;

			m_SwapIcon.fillAmount = Mathf.Min(amount * 2f, 1f);
		}

		private bool IsSwappable(Item item) => Player.EquippedItem.Get() != null && m_HolsterContainer.AllowsItem(item);
    }
}
