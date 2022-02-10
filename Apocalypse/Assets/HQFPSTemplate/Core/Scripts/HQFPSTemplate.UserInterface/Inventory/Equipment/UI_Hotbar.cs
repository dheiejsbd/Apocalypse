using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HQFPSTemplate.UserInterface
{
	public class UI_Hotbar : UserInterfaceBehaviour
	{
		[SerializeField]
		private UI_ItemContainerInterface m_ContainerInterface;

		[Space]

		[SerializeField]
		private bool m_FadeHotbarWhenNotUsed;

		[SerializeField]
		[EnableIf("m_FadeHotbarWhenNotUsed", true)]
		private CanvasGroup m_CanvasGroup;

		[SerializeField]
		[EnableIf("m_FadeHotbarWhenNotUsed", true)]
		private float m_HotbarFadeSpeed;

		[SerializeField]
		[EnableIf("m_FadeHotbarWhenNotUsed", true)]
		private float m_HotbarStayAmount;

		[Space]

		[BHeader("Selection Graphics", order = 100)]

		[SerializeField]
		private Color m_FrameColor = Color.cyan;

		[SerializeField]
		private Sprite	m_FrameSprite;

		[BHeader("Slot Numbering")]

		[SerializeField]
		private bool m_EnableCustomNumbering;

		[SerializeField]
		[Tooltip("If left empty, the default font from the UI manager will be used.")]
		private Font m_CustomNumberingFont;

		[SerializeField]
		[Range(0, 30)]
		private int m_NumberFontSize = 12;

		[SerializeField]
		private TextAnchor m_NumberAnchor = TextAnchor.UpperLeft;

		[SerializeField]
		private Vector2 m_NumberOffset;

		[SerializeField]
		private Color m_NumberColor = Color.grey;

		private UI_ItemSlotInterface m_SelectedSlot;
		private Image m_Frame;

		private Coroutine m_AlphaSetter;
		private float m_NextTimeDisableHotbar;
		private bool m_HotbarDisabled;


        private void Start()
        {
			m_ContainerInterface.ItemContainer.SelectedSlot.AddChangeListener(SelectSlot);		

			m_Frame = GUIUtils.CreateImageUnder("Frame", GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
			m_Frame.sprite = m_FrameSprite;
			m_Frame.color = m_FrameColor;
			m_Frame.transform.SetAsFirstSibling();

			if (m_EnableCustomNumbering)
			{
				for (int i = 0; i < m_ContainerInterface.SlotInterfaces.Length; i++)
				{
					var text = GUIUtils.CreateTextUnder("Number", m_ContainerInterface.SlotInterfaces[i].GetComponent<RectTransform>(), m_NumberAnchor, m_NumberOffset);
					text.text = (i + 1).ToString();
					text.transform.SetAsLastSibling();

					text.color = m_NumberColor;
					text.font = m_CustomNumberingFont == null ? UIManager.MainFont : m_CustomNumberingFont;
					text.fontSize = m_NumberFontSize;
					text.supportRichText = false;
					text.raycastTarget = false;
					text.verticalOverflow = VerticalWrapMode.Overflow;
					text.horizontalOverflow = HorizontalWrapMode.Overflow;
				}
			}

			SelectSlot(m_ContainerInterface.ItemContainer.SelectedSlot.Get());
		}
		
        private void LateUpdate()
        {
			if (m_FadeHotbarWhenNotUsed)
			{
				if (m_NextTimeDisableHotbar < Time.time && !m_HotbarDisabled)
					m_AlphaSetter = StartCoroutine(C_FadeHotbarAlpha(0f));
			}
		}

        private void SelectSlot(int slotIndex)
		{
			m_SelectedSlot = m_ContainerInterface.SlotInterfaces[slotIndex];

			m_Frame.enabled = true;
			var frameRT = m_Frame.GetComponent<RectTransform>();
			frameRT.SetParent(m_SelectedSlot.transform);
			frameRT.localPosition = Vector2.zero;
			frameRT.sizeDelta = m_SelectedSlot.GetComponent<RectTransform>().sizeDelta;
			frameRT.localScale = Vector3.one;
			frameRT.SetAsFirstSibling();

			if (m_FadeHotbarWhenNotUsed)
			{
				if (m_AlphaSetter != null)
					StopCoroutine(m_AlphaSetter);

				if (m_HotbarDisabled)
					m_AlphaSetter = StartCoroutine(C_FadeHotbarAlpha(1f));

				m_HotbarDisabled = false;
				m_NextTimeDisableHotbar = Time.time + m_HotbarStayAmount;
			}
		}

		private IEnumerator C_FadeHotbarAlpha(float targetAlpha)
		{
			float alpha = m_CanvasGroup.alpha;
			m_HotbarDisabled = true;

			while (Mathf.Abs(alpha - targetAlpha) > Mathf.Epsilon)
			{
				alpha = Mathf.MoveTowards(alpha, targetAlpha, m_HotbarFadeSpeed);

				m_CanvasGroup.alpha = alpha;
				yield return null;
			}
		}
	}
}