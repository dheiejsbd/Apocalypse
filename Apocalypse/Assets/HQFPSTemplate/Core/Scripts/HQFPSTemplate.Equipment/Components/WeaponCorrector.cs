using System;
using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    public partial class WeaponCorrector : MonoBehaviour, IEquipmentComponent
    {
		#region Internal
		#pragma warning disable CS0649
		[Serializable]
		private class MovingPartsCorrector
		{
			public float MoveDelay = 0.1f;

			[Group]
			public MovingPart[] MovingParts = new MovingPart[0];

			[Serializable]
			public struct MovingPart
			{
				public Transform MovingPartTransform;
				public Vector3 EmptyPosition, EmptyRotation;
			}		
		}

		[Serializable]
		private class CartridgeCorrector
		{
			[HideInInspector]
			public CartridgeObject[] Cartridges = null;

			public CartridgeObject CartridgePrefab = null;
			public Transform[] CartridgeTransforms = null;

			[Space(5f)]

			public bool ReloadReverseOrderUpdate = false;
			public float ReloadUpdateDelay = 0.5f;
		}

		#pragma warning restore CS0649
		#endregion

		[SerializeField, Group]
		private MovingPartsCorrector m_MovingPartsCorrector = null;

		[SerializeField, Group]
		private CartridgeCorrector m_CartridgeCorrector = null;

		protected ProjectileWeapon m_Weapon;
		private float m_CanUpdateMovingPartsTime;
		private bool m_UpdateMovingParts;


		public virtual void Initialize(EquipmentItem equipmentItem) 
		{
			m_Weapon = equipmentItem as ProjectileWeapon;

			if (m_CartridgeCorrector.CartridgePrefab != null)
			{
				m_CartridgeCorrector.Cartridges = new CartridgeObject[m_CartridgeCorrector.CartridgeTransforms.Length];

				for (int i = 0; i < m_CartridgeCorrector.Cartridges.Length; i++)
					m_CartridgeCorrector.Cartridges[i] = Instantiate(m_CartridgeCorrector.CartridgePrefab, m_CartridgeCorrector.CartridgeTransforms[i]);
			}
		} 

		public void OnSelected()
		{
			m_Weapon.CurrentAmmoInfo.AddChangeListener(OnAmmoChanged);
			m_Weapon.Player.Reload.AddStartListener(OnReload);

			if(m_CartridgeCorrector.CartridgePrefab != null)
				m_CartridgeCorrector.CartridgePrefab.SetFOV(m_Weapon.EModel.TargetFOV);
		}

		protected virtual void OnAmmoChanged(ProjectileWeapon.AmmoInfo ammoInfo)
		{
			// Return if this weapon has been reloaded
			if (ammoInfo.CurrentInMagazine >= m_Weapon.CurrentAmmoInfo.PrevVal.CurrentInMagazine || m_Weapon.Player.Reload.Active)
				return;

			int currentInMag = ammoInfo.CurrentInMagazine;
			int magSize = m_Weapon.MagazineSize;

			if (magSize - (magSize - currentInMag) < m_CartridgeCorrector.Cartridges.Length)
				m_CartridgeCorrector.Cartridges[magSize - (magSize - currentInMag)].ChangeState(false);

			if (currentInMag == 0)
			{
				m_CanUpdateMovingPartsTime = Time.time + m_MovingPartsCorrector.MoveDelay;
				m_UpdateMovingParts = true;
			}
		}

		protected virtual void LateUpdate()
		{
			// If this weapon's magazine is empty, update the moving parts
			if (m_UpdateMovingParts)
			{
				if (m_CanUpdateMovingPartsTime < Time.time)
					UpdateMovingParts();
			}
		}

		protected virtual void OnReload() 
		{
			m_UpdateMovingParts = false;

			int currentInMag = m_Weapon.CurrentAmmoInfo.Get().CurrentInMagazine;
			int magSize = m_Weapon.MagazineSize;
			int reloadingAmount = Mathf.Clamp(m_Weapon.CurrentAmmoInfo.Get().CurrentInStorage, 1, magSize);

			StartCoroutine(C_ReloadUpdateCartridges(currentInMag, reloadingAmount));
		}

		private void OnDisable()
		{
			m_Weapon.Player.Reload.RemoveStartListener(OnReload);
			m_Weapon.CurrentAmmoInfo.RemoveChangeListener(OnAmmoChanged);
		}

		private void UpdateMovingParts() 
		{
			for (int i = 0; i < m_MovingPartsCorrector.MovingParts.Length; i++)
			{
				if (m_MovingPartsCorrector.MovingParts[i].EmptyPosition != Vector3.zero)
				{
					Vector3 newPosition = new Vector3(
							m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localPosition.x + m_MovingPartsCorrector.MovingParts[i].EmptyPosition.x,
							m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localPosition.y + m_MovingPartsCorrector.MovingParts[i].EmptyPosition.y,
							m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localPosition.z + m_MovingPartsCorrector.MovingParts[i].EmptyPosition.z);

					m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localPosition = newPosition;
				}

				if (m_MovingPartsCorrector.MovingParts[i].EmptyRotation != Vector3.zero)
				{
					Vector3 originalEulerAngles = m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localEulerAngles;

					Vector3 newEulerAngles = new Vector3(
							originalEulerAngles.x + m_MovingPartsCorrector.MovingParts[i].EmptyRotation.x,
							originalEulerAngles.y + m_MovingPartsCorrector.MovingParts[i].EmptyRotation.y,
							originalEulerAngles.z + m_MovingPartsCorrector.MovingParts[i].EmptyRotation.z);

					m_MovingPartsCorrector.MovingParts[i].MovingPartTransform.localEulerAngles = newEulerAngles;
				}
			}
		}

		private IEnumerator C_ReloadUpdateCartridges(int currentInMag, int reloadingAmount) 
		{
			yield return new WaitForSeconds(m_CartridgeCorrector.ReloadUpdateDelay);

			int numberOfCartridgesToEnable = Mathf.Clamp(reloadingAmount - currentInMag + 1, 0, m_CartridgeCorrector.Cartridges.Length);

			for (int i = 0; i < numberOfCartridgesToEnable; i++)
			{
				m_CartridgeCorrector.Cartridges[i].ChangeState(true);
			}
		}
    }
}
