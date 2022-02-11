using UnityEngine;
using UnityEngine.Events;

namespace HQFPSTemplate
{
	[DisallowMultipleComponent()]
	public class OnCollisionBehaviour : MonoBehaviour, IObjectReferenceFiller
	{
		public bool ListenForCollisions { get => m_ListenForCollisions; set { m_ListenForCollisions = value; } }

		[SerializeField]
		private bool m_ListenForCollisions = true;

		[Space]

		[SerializeField]
		private LayerMask m_LayerMask = new LayerMask();

		[SerializeField]
		private AudioSource m_AudioSource = null;

		[Space]

		[SerializeField]
		[EnableIf("m_ListenForCollisions", true)]
		private int m_MaxCollisionsAmount = 0;

		[SerializeField]
		[EnableIf("m_ListenForCollisions", true)]
		private float m_CollisionTimeThreshold = 0.5f;

		[SerializeField]
		[EnableIf("m_ListenForCollisions", true)]
		private float m_CollisionVelocityThreshold = 2f;

		[Space]

		[SerializeField]
		private UnityEvent m_OnCollisionEvent = new UnityEvent();

		[SerializeField]
		[Group]
		private SoundPlayer m_OnCollisionAudio = null;

		private int m_CurrentCollisionsAmount = 0;
		private float m_NextTimeStartCollisionEvent = 0f;


		public void TryAutoFillObjectReferences()
		{
			m_AudioSource = GetComponent<AudioSource>();
		}

		private void OnCollisionEnter(Collision col)
		{
			if ((m_MaxCollisionsAmount > 0 && m_CurrentCollisionsAmount > m_MaxCollisionsAmount) || // Return if the collisions max limit is hit
				(Time.time < m_NextTimeStartCollisionEvent) || // Return if the collision time threshold is not met
				!(m_LayerMask == (m_LayerMask | (1 << col.collider.gameObject.layer))) || // Return if the layer of the object that has been collided with is not found in the layer mask
				(col.relativeVelocity.magnitude < m_CollisionVelocityThreshold)) // Return if the collision velocity doesn't go above the threshold 
				return;
			else
			{
				float impactSoundVolume = Mathf.Clamp(col.relativeVelocity.sqrMagnitude / m_CollisionVelocityThreshold / 10, 0.2f, 1f);

				if (m_AudioSource != null)
					m_OnCollisionAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource, impactSoundVolume);
				else
					m_OnCollisionAudio.PlayAtPosition(ItemSelection.Method.RandomExcludeLast, transform.position, impactSoundVolume);

				m_OnCollisionEvent.Invoke();

				m_NextTimeStartCollisionEvent = Time.time + m_CollisionTimeThreshold;
				m_CurrentCollisionsAmount++;
			}
		}
    }
}
