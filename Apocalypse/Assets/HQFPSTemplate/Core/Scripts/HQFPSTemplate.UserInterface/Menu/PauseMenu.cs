using UnityEngine;

namespace HQFPSTemplate.UserInterface
{
	public class PauseMenu : MonoBehaviour
	{
		[SerializeField]
		private Panel m_Panel = null;

		[SerializeField]
		private Panel m_MapSelectionPanel = null;

		[SerializeField]
		private bool m_UseKeyToPause = true;

		[SerializeField]
		[ShowIf("m_UseKeyToPause", true)]
		private KeyCode m_PauseKey = KeyCode.Escape;


		public void TogglePause(bool enable)
		{
			var player = GameManager.Instance.CurrentPlayer;

			if (enable)
				player.Pause.ForceStart();
			else
			{
				player.Pause.ForceStop();

				//Hide the map selection panel
				m_MapSelectionPanel.TryShow(false);
			}

			Time.timeScale = enable ? 0f : 1f;
			m_Panel.TryShow(enable);

			Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = enable;
		}

		public void LoadScene(int index)
		{
			TogglePause(false);
			GameManager.Instance.StartGame(index);
		}

		public void ToggleMapSelection()
		{
			m_MapSelectionPanel.TryShow(!m_MapSelectionPanel.IsVisible);
		}

		public void Quit()
		{
			GameManager.Instance.Quit();
		}

        private void Start()
        {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

        private void Update()
		{
			if(m_UseKeyToPause && Input.GetKeyDown(m_PauseKey))
				TogglePause(!GameManager.Instance.CurrentPlayer.Pause.Active);
		}
	}
}