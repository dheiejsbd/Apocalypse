using UnityEngine;
using UnityEngine.Events;

namespace HQFPSTemplate
{
	/// <summary>
	/// Base class for interactive objects (eg. buttons, item pickups).
	/// Has numerous raycast and interaction callbacks (overridable).
	/// </summary>
	public class InteractiveObject : MonoBehaviour
	{
		#region Internal
		[System.Serializable]
		protected struct InteractionAudio
		{
			public SoundPlayer RaycastStartAudio;
			public SoundPlayer RaycastEndAudio;
			public SoundPlayer InteractionStartAudio;
			public SoundPlayer InteractionEndAudio;
		}
		#endregion

		public readonly Value<float> InteractionProgress = new Value<float>();
		public readonly Value<string> InteractionText = new Value<string>();

		public bool InteractionEnabled { get { return m_InteractionEnabled; } set { m_InteractionEnabled = value; } }

		[BHeader("Interaction", true)]

		[SerializeField]
		private bool m_InteractionEnabled = true;

		[SerializeField]
		[Multiline]
		private string m_InteractionText = string.Empty;

		[SerializeField]
		private InteractionAudio m_InteractionAudio = new InteractionAudio();

		[Space(3f)]

		[SerializeField]
		private UnityEvent m_InteractionEvent;

		private float m_InteractStart = 0f;


		/// <summary>
		/// Called when a player starts looking at the object.
		/// </summary>
		public virtual void OnRaycastStart(Humanoid humanoid) 
		{
			m_InteractionAudio.RaycastStartAudio.Play2D();
		}

		/// <summary>
		/// Called while a player is looking at the object.
		/// </summary>
		public virtual void OnRaycastUpdate(Humanoid humanoid) {  }

		/// <summary>
		/// Called when a player stops looking at the object.
		/// </summary>
		public virtual void OnRaycastEnd(Humanoid humanoid) 
		{
			m_InteractionAudio.RaycastEndAudio.Play2D();
		}

		/// <summary>
		/// Called when a player starts interacting with the object.
		/// </summary>
		public virtual void OnInteractionStart(Humanoid humanoid) 
		{
			m_InteractionEvent.Invoke();

			m_InteractionAudio.InteractionStartAudio.Play2D();

			m_InteractStart = Time.time;
		}

		/// <summary>
		/// Called while a player is interacting with the object.
		/// </summary>
		public virtual void OnInteractionUpdate(Humanoid humanoid) 
		{
			InteractionProgress.Set(Time.time - m_InteractStart);
		}

		/// <summary>
		/// Called when a player stops interacting with the object.
		/// </summary>
		public virtual void OnInteractionEnd(Humanoid humanoid) 
		{
			InteractionProgress.Set(0f);

			m_InteractionAudio.InteractionEndAudio.Play2D(); 
		}

		protected virtual void Awake() 
		{
			InteractionText.Set(m_InteractionText);
		}
    }
}