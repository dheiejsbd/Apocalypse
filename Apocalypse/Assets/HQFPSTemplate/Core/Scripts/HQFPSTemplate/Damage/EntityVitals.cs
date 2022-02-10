using UnityEngine;
using System;

namespace HQFPSTemplate
{
	/// <summary>
	/// 
	/// </summary>
	public class EntityVitals : GenericVitals
	{
		#region Internal
		[Serializable]
		private class FallImpactModule
		{
			public bool Enabled = true;

			[Range(1f, 30f)]
			[Tooltip("At which landing speed, the entity will start taking damage.")]
			public float MinFallSpeed = 12f;

			[Range(1f, 50f)]
			[Tooltip("At which landing speed, the entity will die, if it has no defense.")]
			public float FatalFallSpeed = 30f;
		}
		#endregion

		[SerializeField]
		[Group]
		private FallImpactModule m_FallDamage = new FallImpactModule();


		protected override void Awake()
		{
			base.Awake();

			Entity.FallImpact.AddListener(On_FallImpact);
		}

		private void On_FallImpact(float impactSpeed)
		{
			if (!m_FallDamage.Enabled)
				return;

			if (impactSpeed >= m_FallDamage.MinFallSpeed)
				Entity.ChangeHealth.Try(new DamageInfo(-100f * (impactSpeed / m_FallDamage.FatalFallSpeed)));
		}
	}
}
