using HQFPSTemplate.Items;
using System;
using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
	/// <summary>
	/// Physical Projectile Weapon (e.g. Rocket Launchers , Bows, etc.)
	/// </summary>
	public class LauncherWeapon : ProjectileWeapon
	{
		#region Internal
		[Serializable]
		public class LauncherSettings
		{
			[Tooltip("Object to disable after using this weapon (e.g. arrow)")]
			public GameObject FPProjectile = null;

			[EnableIf("FPProjectile", true)]
			public float EnableFPProjectileReloadDelay = 1f;
		}
		#endregion

		[SerializeField]
		[Group]
		private LauncherSettings m_LauncherSettings = null;

		private LauncherInfo.LaunchingInfo m_L;
		private WaitForSeconds m_WaitAfterLaunch;


        public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

			m_L = (EInfo as LauncherInfo).Launching;

			m_WaitAfterLaunch = new WaitForSeconds(m_L.LaunchDelay);
		}

        public override void Equip(Item item)
        {
            base.Equip(item);

			if(m_LauncherSettings.FPProjectile != null)
				m_LauncherSettings.FPProjectile.SetActive(CanBeUsed());

			Player.Reload.AddStartListener(On_Reload);
		}

        public override void Unequip()
        {
            base.Unequip();

			Player.Reload.RemoveStartListener(On_Reload);
		}

        public override void Shoot(Ray[] itemUseRays)
		{
			base.Shoot(itemUseRays);

			StartCoroutine(C_LaunchWithDelay(itemUseRays));
		}

        public override float GetUseRaySpreadMod()
        {
			return m_L.LaunchSpread;
        }

		private void On_Reload() 
		{
			if (m_LauncherSettings.FPProjectile != null)
				StartCoroutine(C_OnReload());
		}

		private IEnumerator C_OnReload() 
		{
			yield return new WaitForSeconds(m_LauncherSettings.EnableFPProjectileReloadDelay);

			if(Player.Reload.Active)
				m_LauncherSettings.FPProjectile.SetActive(true);
		}

		private IEnumerator C_LaunchWithDelay(Ray[] itemUseRays) 
		{
			yield return m_WaitAfterLaunch;

			if (m_LauncherSettings.FPProjectile != null)
				m_LauncherSettings.FPProjectile.SetActive(false);

			if (!m_L.Prefab)
			{
				Debug.LogErrorFormat("No Projectile prefab assigned in the inspector! Please assign one.");
				yield return null;
			}

			Vector3 position = itemUseRays[0].origin + transform.TransformVector(m_L.SpawnOffset);
			Quaternion rotation = Quaternion.LookRotation(itemUseRays[0].direction, transform.up);

			ShaftedProjectile projectileObject = Instantiate(m_L.Prefab, position, rotation);

			projectileObject.GetComponent<Rigidbody>().velocity = projectileObject.transform.forward * m_L.LaunchSpeed;
			projectileObject.GetComponent<ShaftedProjectile>().Launch(Player);
			projectileObject.GetComponent<ShaftedProjectile>().CheckForSurfaces(itemUseRays[0]);

			RaycastHit hitInfo = new RaycastHit();

			FireHitPoints.Send(new Vector3[] { hitInfo.point });
		}
		
		#if UNITY_EDITOR
		private void OnValidate()
		{
			if (EHandler != null)
			{
				m_WaitAfterLaunch = new WaitForSeconds(m_L.LaunchDelay);
			}
		}
		#endif
	}
}
