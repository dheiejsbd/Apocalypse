using System;
using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
	public class RevolverCyllinderCorrector : WeaponCorrector
	{
		#region Internal
		[Serializable]
		private class CyllinderCorrector
		{
			[BHeader("General", true)]

			public Transform Cyllinder = null;

			[Space]

			public Vector3 RotationAxis = Vector3.zero;

			[SerializeField]
			[Range(0f, 10f)]
			public float RotationDelay = 0.5f;

			[SerializeField]
			[Range(0f, 25f)]
			public float RotationSpeed = 1f;

			[Space]

			[SerializeField]
			public float ReloadResetCyllinderDelay = 0f;
		}
		#endregion

		[SerializeField, Group]
		private CyllinderCorrector m_CyllinderCorrector = null;

		private Vector3 m_CyllinderRot;
		private Vector3 m_NewCyllinderRot;

		private WaitForSeconds m_RotationWait;


		public override void Initialize(EquipmentItem equipmentItem)
		{
			base.Initialize(equipmentItem);

			m_RotationWait = new WaitForSeconds(m_CyllinderCorrector.RotationDelay);
		}

		protected override void OnReload()
		{
			base.OnReload();

			StartCoroutine(C_ReloadResetCyllinder());
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			m_CyllinderRot = Vector3.Lerp(m_CyllinderRot, m_NewCyllinderRot, m_CyllinderCorrector.RotationSpeed * Time.deltaTime);

			m_CyllinderCorrector.Cyllinder.Rotate(m_CyllinderRot,Space.Self);
		}

		protected override void OnAmmoChanged(ProjectileWeapon.AmmoInfo ammoInfo)
		{
			base.OnAmmoChanged(ammoInfo);

			// Return if this weapon has been reloaded
			if (ammoInfo.CurrentInMagazine >= m_Weapon.CurrentAmmoInfo.PrevVal.CurrentInMagazine || m_Weapon.Player.Reload.Active)
				return;

			StartCoroutine(C_DelayedRotation());
		}

		private void OnValidate()
		{
			m_RotationWait = new WaitForSeconds(m_CyllinderCorrector.RotationDelay);
		}

		private IEnumerator C_ReloadResetCyllinder()
		{
			yield return new WaitForSeconds(m_CyllinderCorrector.ReloadResetCyllinderDelay);

			m_CyllinderRot = Vector3.zero;
			m_NewCyllinderRot = Vector3.zero;
		}

        private IEnumerator C_DelayedRotation()
		{
			yield return m_RotationWait;

			if (!m_Weapon.Player.Reload.Active)
				m_NewCyllinderRot = m_CyllinderRot + m_CyllinderCorrector.RotationAxis;
		}
    }
}