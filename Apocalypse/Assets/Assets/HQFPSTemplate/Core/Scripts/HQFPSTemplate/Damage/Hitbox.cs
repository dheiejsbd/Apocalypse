using UnityEngine;
using UnityEngine.Events;
using System;

namespace HQFPSTemplate
{
	/// <summary>
	/// Will register damage events from outside and pass them to the parent entity.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class Hitbox : MonoBehaviour, IDamageable 
	{
		#region Internal
		[Serializable]
		public class DamageEvent : UnityEvent<DamageInfo>
		{ }

		[Serializable]
		public class DamageEventSimple : UnityEvent<float>
		{ }
		#endregion

		public Collider Collider => m_Collider;
		public Rigidbody Rigidbody => m_Rigidbody;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_DamageMultiplier = 1f;

		[Space]

		[SerializeField]
		private DamageEvent m_OnDamageEvent = null;

		[SerializeField]
		private DamageEventSimple m_OnDamageEventSimple = null;

		[SerializeField]
		[Group]
		private SoundPlayer m_GroundImpactSound = null;

		private Collider m_Collider;
		private Rigidbody m_Rigidbody;
		private Entity m_ParentEntity;

		private bool m_HitboxImpact;


		public void TakeDamage(DamageInfo damageData)
		{
			if (enabled)
			{
				m_OnDamageEvent.Invoke(damageData);
				m_OnDamageEventSimple.Invoke(damageData.Delta);

				if (m_ParentEntity != null)
				{
					if (m_ParentEntity.Health.Get() > 0f)
					{
						damageData.Delta *= m_DamageMultiplier;
						m_ParentEntity.ChangeHealth.Try(damageData);
					}

					if (m_Rigidbody != null)
					{
						if (m_ParentEntity.Health.Get() == 0f)
							m_Rigidbody.AddForceAtPosition(damageData.HitDirection * damageData.HitImpulse, damageData.HitPoint, ForceMode.Impulse);
					}
				}
			}
		}

		private void Awake()
		{
			m_ParentEntity = GetComponentInParent<Entity>();

			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();

			m_ParentEntity.Respawn.AddListener(Respawn);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (m_Rigidbody != null)
			{
				if (collision.relativeVelocity.sqrMagnitude > 5f && !m_Rigidbody.isKinematic && !m_HitboxImpact)
				{
					m_GroundImpactSound.PlayAtPosition(ItemSelection.Method.RandomExcludeLast, transform.position, 1f);
					m_HitboxImpact = true;
				}
			}
		}

		private void Respawn() 
		{
			m_HitboxImpact = false;
		}
	}
}
