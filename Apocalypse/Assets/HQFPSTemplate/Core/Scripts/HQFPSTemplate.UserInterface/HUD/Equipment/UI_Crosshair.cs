using UnityEngine.UI;
using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.UserInterface
{
    public class UI_Crosshair : MonoBehaviour
    {
		public int CrosshairID { get; private set; } = -1;

		[SerializeField]
		private CanvasGroup m_CanvasGroup;

		[SerializeField]
		private RectTransform m_RectTransform;

		[Space]

		[SerializeField]
		private Image m_LeftImg;

		[SerializeField]
		private Image m_RightImg,
					  m_CenterImg,
					  m_TopImg,
					  m_BottomImg;

		[Space]

		[SerializeField]
		[Range(0.1f, 100f)]
		private float m_SpringLerpSpeed = 25f;

		[SerializeField]
		[Range(0.01f, 10f)]
		private float m_AlphaLerpSpeed = 0.35f;

		[Space]

		[SerializeField]
		private Sprite m_EmptySprite = null;

		private UI_Spring m_Spring;
		private Coroutine m_FadeCanvasRoutine;


		public void AddSpringForce(Vector2 force) => m_Spring.AddForce(force);
		public void AddSpringForce(UI_SpringForce force) => m_Spring.AddForce(force);

		public virtual void UpdateInfo(UI_CrosshairInfo crosshairInfo, int crosshairId)
		{
			//Update the crosshair sprites

			m_LeftImg.sprite = (crosshairInfo.GraphicsInfo.LeftSprite.Sprite != null) ? crosshairInfo.GraphicsInfo.LeftSprite.Sprite : m_EmptySprite;
			m_LeftImg.rectTransform.sizeDelta = crosshairInfo.GraphicsInfo.LeftSprite.Size;

			m_RightImg.sprite = (crosshairInfo.GraphicsInfo.RightSprite.Sprite != null) ? crosshairInfo.GraphicsInfo.RightSprite.Sprite : m_EmptySprite;
			m_RightImg.rectTransform.sizeDelta = crosshairInfo.GraphicsInfo.RightSprite.Size;

			m_CenterImg.sprite = (crosshairInfo.GraphicsInfo.CenterSprite.Sprite != null) ? crosshairInfo.GraphicsInfo.CenterSprite.Sprite : m_EmptySprite;
			m_CenterImg.rectTransform.sizeDelta = crosshairInfo.GraphicsInfo.CenterSprite.Size;

			m_TopImg.sprite = (crosshairInfo.GraphicsInfo.TopSprite.Sprite != null) ? crosshairInfo.GraphicsInfo.TopSprite.Sprite : m_EmptySprite;
			m_TopImg.rectTransform.sizeDelta = crosshairInfo.GraphicsInfo.TopSprite.Size;

			m_BottomImg.sprite = (crosshairInfo.GraphicsInfo.BottomSprite.Sprite != null) ? crosshairInfo.GraphicsInfo.BottomSprite.Sprite : m_EmptySprite;
			m_BottomImg.rectTransform.sizeDelta = crosshairInfo.GraphicsInfo.BottomSprite.Size;

			m_RectTransform.localEulerAngles = new Vector3(0, 0, crosshairInfo.GraphicsInfo.PivotRotation);
			
			//Adjust the crosshair's spring
			if(m_Spring == null)
				m_Spring = new UI_Spring(m_RectTransform, m_RectTransform.sizeDelta, m_SpringLerpSpeed);

			m_Spring.Adjust(crosshairInfo.ScaleInfo.ScaleSpringData);

			CrosshairID = crosshairId;
		}

		public virtual void EnableCrosshair(bool enable = true) 
		{
			if (m_FadeCanvasRoutine != null)
				StopCoroutine(m_FadeCanvasRoutine);

			m_FadeCanvasRoutine = StartCoroutine(C_FadeCanvasAlpha(enable));
		}

        public virtual void ChangeColor(Color color)
		{
			m_LeftImg.color = color;
			m_RightImg.color = color;
			m_CenterImg.color = color;
			m_TopImg.color = color;
			m_BottomImg.color = color;
		}

        private void Start()
        {
			//Adjust the crosshair's spring
			if (m_Spring == null)
				m_Spring = new UI_Spring(m_RectTransform, m_RectTransform.sizeDelta, m_SpringLerpSpeed);
		}

        private void FixedUpdate()
		{
			m_Spring.FixedUpdate();
		}

		private void Update()
        {
			m_Spring.Update();
		}

        private IEnumerator C_FadeCanvasAlpha(bool enable)
		{
			float targetAlpha = enable ? 1f : 0f;

            while (Mathf.Abs(m_CanvasGroup.alpha - targetAlpha) > 0.01f)
            {
				m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, targetAlpha, m_AlphaLerpSpeed * Time.deltaTime);

				yield return null;
			}
		}
	}
}
