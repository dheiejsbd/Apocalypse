using UnityEngine;

namespace HQFPSTemplate.UserInterface
{
	public class UIManager : MonoBehaviour
	{
		public readonly Value<bool> Dragging = new Value<bool>();
		public readonly Value<bool> DraggingItem = new Value<bool>();
		public readonly Message PointerDown = new Message();
		public readonly Activity OnConsoleOpened = new Activity();

		public Player Player { get; private set; }

		public Activity ItemWheel = new Activity();

		public KeyCode AutoMoveKey => AutoMoveKey;
		public Font MainFont => m_MainFont;

		/// <summary>The main Canvas that's used for the GUI elements.</summary>
		public Canvas Canvas { get { return m_Canvas; } }

		[BHeader("SETUP", true)]

		[SerializeField]
		private Canvas m_Canvas = null;

		[SerializeField]
		private KeyCode m_AutoMoveKey;

		[Space]

		[SerializeField]
		private Font m_MainFont = null;

		private UserInterfaceBehaviour[] m_UIBehaviours;


		public void AttachToPlayer(Player player)
		{
			if (!m_Canvas.isActiveAndEnabled)
				m_Canvas.gameObject.SetActive(true);

			if (m_UIBehaviours == null)
				m_UIBehaviours = GetComponentsInChildren<UserInterfaceBehaviour>(true);

			Player = player;

			for(int i = 0;i < m_UIBehaviours.Length;i ++)
				m_UIBehaviours[i].OnAttachment();

			for(int i = 0;i < m_UIBehaviours.Length;i ++)
				m_UIBehaviours[i].OnPostAttachment();
		}

		private void Update()
		{
			if(Input.GetMouseButtonDown(0))
				PointerDown.Send();
		}
	}
}
