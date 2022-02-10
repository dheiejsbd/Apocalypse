using UnityEngine;

#if INVECTOR_AI_TEMPLATE
using Invector;
#endif

#if EMERALD_AI_PRESENT
using EmeraldAI;
#endif

namespace HQFPSTemplate
{
    public class Explosion : DamageDealerObject
	{
		[SerializeField]
		private bool m_DetonateOnStart = false;

		[SerializeField]
		private float m_Force = 105f;

		[SerializeField]
		[Range(0f, 1000f)]
		private float m_Damage = 100f;

		[SerializeField]
		private float m_Radius = 15f;

		[SerializeField]
		[Range(0f, 10f)]
		private float m_Scale = 1f;

		[SerializeField]
		private LayerMask m_AffectedLayers;

		[Space]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private ParticleSystem m_ParticleSystem;

		[Space]

		[SerializeField]
		private bool m_DrawRadiusGizmo = true;


		public override void ActivateDamage(Entity source)
		{
			transform.parent = null;

			Explode(source);
		}

		private void Explode (Entity detonator) 
		{
			var cols = Physics.OverlapSphere(transform.position, m_Radius, m_AffectedLayers, QueryTriggerInteraction.Collide);

			Rigidbody rigidB;
			DamageInfo dmgInfo;

			foreach (var col in cols)
			{
				#region AI Systems Integration

				#if INVECTOR_AI_TEMPLATE
				if (col.TryGetComponent(out vIDamageReceiver vDamReceiver))
				{
					dmgInfo = CreateDamageBasedOnDistance(col.transform, detonator);

					vDamage vdamage = new vDamage()
					{
						damageValue = -(int)dmgInfo.Delta,
						force = dmgInfo.HitImpulse * dmgInfo.HitDirection,
						hitPosition = dmgInfo.HitPoint,
						sender = transform
					};

					vDamReceiver.TakeDamage(vdamage);

					continue;
				}
				#endif

				#if EMERALD_AI_PRESENT
				if (col.TryGetComponent(out EmeraldAISystem emAI))
				{
					dmgInfo = CreateDamageBasedOnDistance(col.transform, detonator);

					emAI.Damage(-(int)dmgInfo.Delta, EmeraldAISystem.TargetType.NonAITarget, transform, (int)dmgInfo.HitImpulse);

					continue;
				}
				#endif

				#endregion

				if (col.TryGetComponent(out IDamageable damageable))
				{
					float distToObject = (transform.position - col.transform.position).sqrMagnitude;
					float explosionRadiusSqr = m_Radius * m_Radius;

					dmgInfo = CreateDamageBasedOnDistance(col.transform, detonator);

					if (detonator != null)
						detonator.DealDamage.Try(dmgInfo, damageable);
					else
						damageable.TakeDamage(dmgInfo);
				}

				rigidB = col.attachedRigidbody;

				if (rigidB != null)
					rigidB.AddExplosionForce(m_Force, transform.position, 2f, m_Radius, ForceMode.Impulse);
			}

			if (m_AudioSource != null)
				m_AudioSource.Play();

			if (m_ParticleSystem != null)
				m_ParticleSystem.Play();

			ShakeManager.ShakeEvent.Send(new ShakeEventData(transform.position, m_Radius, m_Scale, ShakeType.Explosion));
		}

		private DamageInfo CreateDamageBasedOnDistance(Transform col, Entity detonator) 
		{
			float distToObject = (transform.position - col.transform.position).sqrMagnitude;
			float explosionRadiusSqr = m_Radius * m_Radius;

			float distanceFactor = 1f - Mathf.Clamp01(distToObject / explosionRadiusSqr);

			var damageInfo = new DamageInfo(-m_Damage * distanceFactor, DamageType.Explosion, transform.position, (col.transform.position - transform.position).normalized,
				m_Force, Vector3.zero, detonator, col);

			return damageInfo;
		}

		private void Start()
		{
			if (m_DetonateOnStart)
				ActivateDamage(null);
		}

		private void OnDrawGizmosSelected()
        {
			Gizmos.color = Color.red;

			if (m_DrawRadiusGizmo)
				Gizmos.DrawWireSphere(transform.position, m_Radius);
        }
	}
}