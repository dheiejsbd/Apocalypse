using UnityEngine;
using System.Collections;
using HQFPSTemplate.Pooling;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Equipment
{
	[RequireComponent(typeof(ProjectileWeapon))]
	public partial class ProjectileWeaponVFX : PlayerComponent, IEquipmentComponent, IObjectReferenceFiller
	{
		[SerializeField]
		private ProjectileWeaponVFXInfo m_VFXInfo = null;

		[Space]

		[SerializeField]
		private Transform m_Muzzle = null;

		[SerializeField]
		private Transform m_CasingEjectionPoint = null;

		[SerializeField]
		private Transform m_MagazineEjectionPoint = null;

		[Space]

		[SerializeField]
		private LightEffect m_LightEffect = null;

		private ProjectileWeapon m_Weapon;
		private WaitForSeconds m_CasingSpawnDelay;


		public void TryAutoFillObjectReferences()
		{
			m_Muzzle = transform.FindDeepChild("Muzzle");
			m_LightEffect = transform.FindDeepChild("Light").GetComponent<LightEffect>();
			m_MagazineEjectionPoint = transform.FindDeepChild("MagazineEjection");
			m_CasingEjectionPoint = transform.FindDeepChild("CasingEjection");
		}

		public void Initialize(EquipmentItem equipmentItem)
		{
			m_Weapon = equipmentItem as ProjectileWeapon;

			m_CasingSpawnDelay = new WaitForSeconds(m_VFXInfo.CasingEjection.SpawnDelay);

			// Create a pool for each gun effect, to help performance
			int minPoolSize = m_Weapon.MagazineSize * 2;
			int maxPoolSize = minPoolSize * 2;

			if (m_VFXInfo.ParticleEffects.MuzzleFlashPrefab != null)
				PoolingManager.Instance.CreatePool(m_VFXInfo.ParticleEffects.MuzzleFlashPrefab, minPoolSize, maxPoolSize, true, m_VFXInfo.ParticleEffects.MuzzleFlashPrefab.GetInstanceID().ToString(), 1f);

			if (m_VFXInfo.ParticleEffects.TracerPrefab != null)
				PoolingManager.Instance.CreatePool(m_VFXInfo.ParticleEffects.TracerPrefab, minPoolSize, maxPoolSize, true, m_VFXInfo.ParticleEffects.TracerPrefab.GetInstanceID().ToString(), 3f);

			if (m_VFXInfo.MagazineEjection.MagazinePrefab != null)
				PoolingManager.Instance.CreatePool(m_VFXInfo.MagazineEjection.MagazinePrefab, 3, 10, true, m_VFXInfo.MagazineEjection.MagazinePrefab.GetInstanceID().ToString(), 10f);

			if (m_VFXInfo.CasingEjection.CasingPrefab != null)
				PoolingManager.Instance.CreatePool(m_VFXInfo.CasingEjection.CasingPrefab, minPoolSize, maxPoolSize, true, m_VFXInfo.CasingEjection.CasingPrefab.GetInstanceID().ToString(), 5f);
		}

		public void OnSelected()
		{
			m_Weapon.FireHitPoints.AddListener(SpawnEffects);
			Player.Reload.AddStartListener(SpawnMagazine);
		}

        private void OnDisable()
        {
			m_Weapon.FireHitPoints.RemoveListener(SpawnEffects);
			Player.Reload.RemoveStartListener(SpawnMagazine);
		}

        private void SpawnMagazine() 
		{
			// Create the magazine if a prefab is assigned and if the weapon uses bullets.
			if (!m_Weapon.CanBeUsed() && m_VFXInfo.MagazineEjection.MagazinePrefab != null && m_MagazineEjectionPoint != null)
				StartCoroutine(C_SpawnMagazine());
		}

		private void SpawnEffects(Vector3[] hitPoints)
		{
			if (gameObject.activeSelf == false)
				return;

			if (m_Muzzle != null)
			{
				// Create the bullet tracers if a prefab is assigned
				if (m_VFXInfo.ParticleEffects.TracerPrefab)
				{
					for (int i = 0; i < hitPoints.Length; i++)
					{
						PoolingManager.Instance.GetObject(
							m_VFXInfo.ParticleEffects.TracerPrefab,
							m_Muzzle.position + m_Muzzle.TransformVector(!Player.Aim.Active ? m_VFXInfo.ParticleEffects.TracerOffset : Vector3.zero),
							Quaternion.LookRotation(hitPoints[i] - Player.Camera.Physics.transform.position)
						);
					}
				}

				// Muzzle flash
				if (m_VFXInfo.ParticleEffects.MuzzleFlashPrefab != null)
				{
					var muzzleFlash = PoolingManager.Instance.GetObject(
						m_VFXInfo.ParticleEffects.MuzzleFlashPrefab,
						Vector3.zero,
						Quaternion.identity,
						m_Muzzle
					);

					muzzleFlash.transform.localPosition = m_VFXInfo.ParticleEffects.MuzzleFlashOffset;

					var randomMuzzleFlashRot = m_VFXInfo.ParticleEffects.MuzzleFlashRandomRot;

					randomMuzzleFlashRot = new Vector3(
						Random.Range(-randomMuzzleFlashRot.x, randomMuzzleFlashRot.x),
						Random.Range(-randomMuzzleFlashRot.y, randomMuzzleFlashRot.y),
						Random.Range(-randomMuzzleFlashRot.z, randomMuzzleFlashRot.z));

					muzzleFlash.transform.localRotation = Quaternion.Euler(randomMuzzleFlashRot);

					float randomMuzzleFlashScale = Random.Range(m_VFXInfo.ParticleEffects.MuzzleFlashRandomScale.x, m_VFXInfo.ParticleEffects.MuzzleFlashRandomScale.y);

					muzzleFlash.transform.localScale = new Vector3(randomMuzzleFlashScale, randomMuzzleFlashScale, randomMuzzleFlashScale);
				}
			}

			// Light
			if (m_LightEffect != null)
				m_LightEffect.Play(false);

			// Spawn the shell if a prefab is assigned
			if (m_VFXInfo.CasingEjection.CasingPrefab != null && m_CasingEjectionPoint != null)
				StartCoroutine(C_SpawnCasing());
		}

		private void OnValidate()
		{
			m_CasingSpawnDelay = new WaitForSeconds(m_VFXInfo != null ? m_VFXInfo.CasingEjection.SpawnDelay : 0f);
		}

		private IEnumerator C_SpawnMagazine() 
		{
			yield return new WaitForSeconds(m_VFXInfo.MagazineEjection.SpawnDelay);

			PoolableObject magazine = PoolingManager.Instance.GetObject(
				m_VFXInfo.MagazineEjection.MagazinePrefab,
				m_MagazineEjectionPoint.position,
				Quaternion.identity);

			if(magazine.TryGetComponent(out Rigidbody rigidB))
			{
				rigidB.AddRelativeForce(m_VFXInfo.MagazineEjection.MagazineVelocity);
				rigidB.AddRelativeTorque(m_VFXInfo.MagazineEjection.MagazineAngularVelocity);
			}
		}

		private IEnumerator C_SpawnCasing()
		{
			yield return m_CasingSpawnDelay;

			Quaternion cassingSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
			Vector3 cassingSpawnPosition = m_CasingEjectionPoint.TransformVector(!Player.Aim.Active ? m_VFXInfo.CasingEjection.SpawnOffset : m_VFXInfo.CasingEjection.AimSpawnOffset);

			var cassing = PoolingManager.Instance.GetObject(m_VFXInfo.CasingEjection.CasingPrefab.gameObject, m_CasingEjectionPoint.position + cassingSpawnPosition, cassingSpawnRotation);
			cassing.transform.localScale = new Vector3(m_VFXInfo.CasingEjection.CasingScale, m_VFXInfo.CasingEjection.CasingScale, m_VFXInfo.CasingEjection.CasingScale);

			var cassingRB = cassing.GetComponent<Rigidbody>();

			cassingRB.maxAngularVelocity = 10000f;

			cassingRB.velocity = transform.TransformVector(new Vector3(
				m_VFXInfo.CasingEjection.SpawnVelocity.x * Random.Range(0.75f, 1.15f),
				m_VFXInfo.CasingEjection.SpawnVelocity.y * Random.Range(0.9f, 1.1f),
				m_VFXInfo.CasingEjection.SpawnVelocity.z * Random.Range(0.85f, 1.15f))) + Player.Velocity.Get();

			float spin = m_VFXInfo.CasingEjection.Spin;

			cassingRB.angularVelocity = new Vector3(
				Random.Range(-spin, spin),
				Random.Range(-spin, spin),
				Random.Range(-spin, spin));
		}
    }
}
