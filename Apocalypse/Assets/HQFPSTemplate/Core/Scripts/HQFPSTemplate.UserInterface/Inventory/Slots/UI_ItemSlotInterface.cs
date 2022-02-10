using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.UserInterface
{
	public class UI_ItemSlotInterface : UI_Slot
	{
		public ItemSlot ItemSlot
		{
			get
			{
				if(m_ItemSlot != null)
					return m_ItemSlot;
				else
				{
					Debug.LogError("No item slot is linked to this interface.");

					return null;
				}
			}
		}

		public bool HasItem { get { return m_ItemSlot == null ? false : m_ItemSlot.HasItem; } }

		public Item Item { get { return m_ItemSlot == null ? null : m_ItemSlot.Item; } }

		public UI_ItemContainerInterface Parent { get; private set; }

		[BHeader("Item Slot")]

		[SerializeField]
		private Image m_Icon = null;

		[Space]

		[SerializeField]
		private Text m_Stack = null;

		[SerializeField]
		private Color m_NormalStackColor = Color.grey;

		[SerializeField]
		private Color m_HighlightStackColor = Color.black;

		protected ItemSlot m_ItemSlot;


		public void LinkToSlot(ItemSlot itemSlot)
		{
			m_ItemSlot = itemSlot;

			if(m_ItemSlot != null)
				m_ItemSlot.Changed.RemoveListener(OnSlotChanged);

			m_ItemSlot.Changed.AddListener(OnSlotChanged);

			DoRefresh();
		}

		public void UnlinkFromSlot()
		{
			if(m_ItemSlot == null)
				return;

			m_ItemSlot.Changed.RemoveListener(OnSlotChanged);
		}

		public virtual void DoRefresh()
		{
			m_Icon.enabled = HasItem;

            if(m_Stack != null)
			    m_Stack.enabled = HasItem && Item.CurrentStackSize > 1;

			if(m_Icon.enabled)
				m_Icon.sprite = Item.Info.Icon;
			
			if(m_Stack != null && m_Stack.enabled)
				m_Stack.text = "x" + Item.CurrentStackSize.ToString();

			Refresh.Send(this);
		}
			
		// <summary>
		/// Will return a clone of this slot, without the background.
		/// </summary>
		public RectTransform GetItemUI(Item item, float alpha)
		{
			var itemUI = Instantiate(this);

			// Disable the slot UI
			itemUI.enabled = false;
			itemUI._Graphic.enabled = false;

			// Set up the icon
			itemUI.m_Icon.enabled = true;
			itemUI.m_Icon.sprite = item.Info.Icon;

            // Set up the stack text
            if(m_Stack != null)
            {
                itemUI.m_Stack.enabled = item.CurrentStackSize > 1;
                itemUI.m_Stack.text = string.Format("x{0}", item.CurrentStackSize);
            }

			// Add a CanvasGroup so we can set a global alpha value
			var group = itemUI.gameObject.AddComponent<CanvasGroup>();
			group.alpha = alpha;
			group.interactable = false;

			return itemUI.GetComponent<RectTransform>();
		}

		public override void OnPointerDown(PointerEventData data)
		{
			base.OnPointerDown(data);
		}

		protected override void Awake()
		{
			base.Awake();

			Parent = GetComponentInParent<UI_ItemContainerInterface>();
			StateChanged.AddListener(OnStateChanged);
		}

		protected override void OnDestroy()
		{
			base.Awake();

			if(m_ItemSlot != null)
				m_ItemSlot.Changed.RemoveListener(OnSlotChanged);
		}

		private void OnSlotChanged(ItemSlot itemSlot, SlotChangeType slotChangeType)
		{
			DoRefresh();
		}

		private void OnStateChanged(State state)
		{
            if(m_Stack != null)
			    m_Stack.color = state == State.Normal ? m_NormalStackColor : m_HighlightStackColor;
		}
	}
}