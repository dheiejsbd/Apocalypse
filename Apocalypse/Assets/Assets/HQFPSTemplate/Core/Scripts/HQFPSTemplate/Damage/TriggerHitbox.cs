using UnityEngine;
using UnityEngine.Events;
using System;

namespace HQFPSTemplate
{
	/// <summary>
	/// Will register damage events from outside and pass them to the parent entity.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class TriggerHitbox : MonoBehaviour, IDamageable
	{
        #region Internal
        [Serializable]
		public class DamageEvent : UnityEvent<DamageInfo> { }

		[Serializable]
		public class DamageEventSimple : UnityEvent<float> { }
        #endregion

		[SerializeField]
		private DamageEvent m_OnDamageEvent = null;

		[SerializeField]
		private DamageEventSimple m_OnSimpleDamageEvent = null;


		public void TakeDamage(DamageInfo damageData)
		{
			m_OnDamageEvent.Invoke(damageData);
			m_OnSimpleDamageEvent.Invoke(damageData.Delta);
		}
	}
}
