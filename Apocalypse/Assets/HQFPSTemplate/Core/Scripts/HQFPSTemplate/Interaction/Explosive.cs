using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HQFPSTemplate
{
	public class Explosive : Projectile
	{
		[SerializeField]
		private bool m_DetonateOnImpact = false;

		[SerializeField]
		[Range(0f, 15f)]
		private float m_DetonationDelay = 1.5f;

		[Space]

		[SerializeField]
		private UnityEvent m_OnExplosiveLaunched = null, m_OnExplosiveDetonate = null;

		private DamageDealerObject[] m_DamageDealers;
		private Entity m_Detonator;
		private bool m_IsDetonating;


		public Entity GetEntity() => null;

		public override void Launch(Entity launcher)
		{
			if(m_IsDetonating)
				return;

			m_IsDetonating = true;

			m_OnExplosiveLaunched.Invoke();

			m_DamageDealers = GetComponentsInChildren<DamageDealerObject>(true);

			if (!m_DetonateOnImpact)
				StartCoroutine(C_DetonateWithDelay(launcher));

			m_Detonator = launcher;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (m_DetonateOnImpact && m_IsDetonating)
				StartCoroutine(C_DetonateWithDelay(m_Detonator));
		}

		protected virtual void Detonate(Entity launcher)
		{
			m_DetonateOnImpact = false;

			m_OnExplosiveDetonate.Invoke();

            for (int i = 0; i < m_DamageDealers.Length; i++)
            {
				m_DamageDealers[i].gameObject.SetActive(true);
				m_DamageDealers[i].ActivateDamage(launcher);
			}

			Destroy(gameObject);
		}

        private IEnumerator C_DetonateWithDelay(Entity launcher)
		{
			yield return new WaitForSeconds(m_DetonationDelay);

			Detonate(launcher);
		}
    }
}