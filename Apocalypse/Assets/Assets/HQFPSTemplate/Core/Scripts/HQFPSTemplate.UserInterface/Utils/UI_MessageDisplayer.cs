using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HQFPSTemplate
{
	/// <summary>
	/// Class used to push messages on the screen
	/// </summary>
	public class UI_MessageDisplayer : Singleton<UI_MessageDisplayer>
	{
		[SerializeField]
		private GameObject m_MessageTemplate = null;

		[SerializeField]
		private Color m_BaseMessageColor = Color.yellow;

		[SerializeField]
		private float m_FadeDelay = 3f;

		[SerializeField]
		private float m_FadeSpeed = 0.3f;


		public void PushMessage(string message, Color color = default, int lineHeight = 16)
		{
			if (color == default)
				color = m_BaseMessageColor;

			var messageObject = Instantiate(m_MessageTemplate, m_MessageTemplate.transform.parent, false);
			messageObject.SetActive(true);
			messageObject.transform.SetAsLastSibling();

			Text text = messageObject.GetComponentInChildren<Text>();
			CanvasGroup group = messageObject.GetComponent<CanvasGroup>();

			if(text && group)
			{
				text.text = message.ToUpper();
				text.color = new Color(color.r, color.g, color.b, 1f);

				text.GetComponent<LayoutElement>().preferredHeight = lineHeight;

				group.alpha = color.a;
				StartCoroutine(FadeMessage(group));
			}
		}

		private void Start()
		{
			m_MessageTemplate.SetActive(false);
		}

		private IEnumerator FadeMessage(CanvasGroup group)
		{
			if(!group)
				yield break;

			yield return new WaitForSeconds(m_FadeDelay);
			
			while(group.alpha > Mathf.Epsilon)
			{
				group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.deltaTime * m_FadeSpeed);
				yield return null;
			}

			Destroy(group.gameObject);
		}
	}
}
