using UnityEngine;
using UnityEngine.UI;

namespace HQFPSTemplate.Examples
{
	public class DemoInstructions : MonoBehaviour
	{
		[SerializeField]
		private bool m_InstructionsEnabledOnStart = false;

		[Space(3f)]

		[SerializeField]
		private Text m_MessageToggleText = null;

		[SerializeField]
		private GameObject m_InstructionsObject = null;

		[SerializeField]
		private Color m_KeyColor = Color.red;

		private bool m_InstructionsEnabled;


		private void Awake()
		{
			m_InstructionsEnabled = m_InstructionsEnabledOnStart;
			Refresh();
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.F12))
			{
				m_InstructionsEnabled = !m_InstructionsEnabled;
				Refresh();
			}
		}

        private void OnValidate()
        {
			Refresh();
        }

        private void Refresh()
		{
			m_InstructionsObject.gameObject.SetActive(m_InstructionsEnabled);
			m_MessageToggleText.text = $"Press<b> <color={ColorUtils.ColorToHex(m_KeyColor)}>F12</color></b> to " + (m_InstructionsEnabled ? "hide" : "show") + " the instructions.";
		}
	}
}