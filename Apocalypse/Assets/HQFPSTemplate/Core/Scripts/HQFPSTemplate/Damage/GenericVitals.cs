using UnityEngine;
using System;

namespace HQFPSTemplate
{
	[Serializable]
	public class DamageResistance
	{
		[SerializeField]
		[Range(0f, 1f)]
		private float m_GenericResistance = 0.1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_CutResistance = 0.1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HitResistance = 0.1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_StabResistance = 0.1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_BulletResistance = 0.1f;


		public float GetDamageResistance(DamageInfo damageData)
		{
			if(damageData.DamageType == DamageType.Generic)
				return m_GenericResistance;
			else if(damageData.DamageType == DamageType.Cut)
				return m_CutResistance;
			else if(damageData.DamageType == DamageType.Hit)
				return m_HitResistance;
			else if(damageData.DamageType == DamageType.Stab)
				return m_StabResistance;
			else if(damageData.DamageType == DamageType.Bullet)
				return m_BulletResistance;

			return 0f;
		}
	}

	[Serializable]
	public class GenericStatData
	{
		public float InitialValue { get => m_InitialValue; }

		public bool CanRegenerate { get => m_RegenEnabled && !IsPaused; }

		public bool IsPaused { get => Time.time < m_NextRegenTime; }

		public float RegenDelta { get => m_RegenSpeed * Time.deltaTime; }

		[SerializeField]
		[Range(0.01f,100f)]
		private float m_InitialValue = 100f;

		[SerializeField]
		private bool m_RegenEnabled = true;

		[SerializeField]
		[ShowIf("m_RegenEnabled", true)]
		private float m_RegenPause = 2f;

		[SerializeField]
		[ShowIf("m_RegenEnabled", true)]
		[Clamp(0f, 1000f)]
		private float m_RegenSpeed = 10f;

		private float m_NextRegenTime;


		public void Pause()
		{
			m_NextRegenTime = Time.time + m_RegenPause;
		}
	}

    public class GenericVitals : EntityComponent
    {
        [BHeader("Health & Damage", true)]

		[SerializeField]
		[Group]
		private GenericStatData m_HealthStat = null;

		[Space]

		[SerializeField]
		[Group]
		private DamageResistance m_DamageResistance = null;


		protected virtual void Awake() 
		{
			Entity.ChangeHealth.SetTryer(Try_ChangeHealth);

			SetOriginalMaxHealth();
		}

        protected virtual void Update()
		{
			if(m_HealthStat.CanRegenerate && Entity.Health.Get() < 100f && Entity.Health.Get() > 0f)
			{
				var data = new DamageInfo(m_HealthStat.RegenDelta);
				Entity.ChangeHealth.Try(data);
			}
		}

		protected virtual bool Try_ChangeHealth(DamageInfo healthEventData)
		{
			if(Entity.Health.Get() == 0f)
				return false;
			if(healthEventData.Delta > 0f && Entity.Health.Get() == 100f)
				return false;

			float healthDelta = healthEventData.Delta;

			if(healthDelta < 0f)
				healthDelta *= (1f - m_DamageResistance.GetDamageResistance(healthEventData));

			float newHealth = Mathf.Clamp(Entity.Health.Get() + healthDelta, 0f, 100f);
			Entity.Health.Set(newHealth);

			if(healthDelta < 0f)
				m_HealthStat.Pause();

			return true;
		}

		private void SetOriginalMaxHealth() 
		{
			Entity.Health.Set(m_HealthStat.InitialValue);
		}
    }
}