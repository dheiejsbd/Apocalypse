using UnityEngine;

namespace HQFPSTemplate.UserInterface
{
	public class UserInterfaceBehaviour : MonoBehaviour 
	{
		public UIManager UIManager
		{
			get 
			{
				if(!m_UIManager)
					m_UIManager = GetComponentInChildren<UIManager>();
				if(!m_UIManager)
					m_UIManager = GetComponentInParent<UIManager>();

				return m_UIManager;
			}
		}

		public Player Player { get { return UIManager != null ? UIManager.Player : null; } }

		public Inventory PlayerStorage { get { return Player != null ? Player.Inventory : null; } }

		private UIManager m_UIManager;

		public virtual void OnAttachment() {  }

		public virtual void OnPostAttachment() {  }
	}
}
