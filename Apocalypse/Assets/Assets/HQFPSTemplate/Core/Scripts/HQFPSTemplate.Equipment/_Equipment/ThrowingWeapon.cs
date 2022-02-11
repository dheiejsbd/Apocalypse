using System;
using System.Collections;
using UnityEngine;
using HQFPSTemplate.Pooling;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.Equipment
{
	public partial class ThrowingWeapon : EquipmentItem
	{
		#region Internal
		[Serializable]
		public class ThrowingWeaponSettings
		{
			public GameObject FPObject = null;
			public float ModelDisableDelay = 0.9f;
		}
		#endregion

		#region Anim Hashing
		//Hashed animator strings (Improves performance)
		private readonly int animHash_LongThrowSpeed = Animator.StringToHash("Long Throw Speed");
		private readonly int animHash_LongThrow = Animator.StringToHash("Long Throw");
		#endregion

		[SerializeField]
		[Group]
		private ThrowingWeaponSettings m_ThrowingWeaponSettings = null;

		private ThrowingWeaponInfo m_TW;

		private bool m_IsThrown;


		public override void Initialize(EquipmentHandler eHandler)
		{
			base.Initialize(eHandler);

			m_TW = EInfo as ThrowingWeaponInfo;

			if (m_TW.Throwing.ObjectToSpawn != null)
				PoolingManager.Instance.CreatePool(m_TW.Throwing.ObjectToSpawn, 3, 6, true, m_TW.Throwing.ObjectToSpawn.GetInstanceID().ToString(), 10);
		}

		public override void Equip(Item item)
		{
			base.Equip(item);

			EHandler.Animator_SetFloat(animHash_LongThrowSpeed, m_TW.Throwing.AnimThrowSpeed);

			m_ThrowingWeaponSettings.FPObject.SetActive(true);
			m_IsThrown = false;
		}

		public override bool TryUseOnce(Ray[] itemUseRays, int useType)
		{
			if (m_IsThrown)
				return false;

			m_GeneralEvents.OnUse.Invoke();

			EHandler.Animator_SetTrigger(animHash_LongThrow);
			EHandler.PlayDelayedSounds(m_TW.Throwing.ThrowAudio);

			StartCoroutine(C_ThrowWithDelay(useType));

			return true;
		}

        public override bool CanBeUsed() => !m_IsThrown;

		private IEnumerator C_ThrowWithDelay(int throwIndex)
		{
			m_IsThrown = true;

			float animSpeedFactor = 1f / m_TW.Throwing.AnimThrowSpeed;

			Player.Camera.Physics.PlayDelayedCameraForces(m_TW.Throwing.ThrowCamForces);

			yield return new WaitForSeconds(m_TW.Throwing.ModelDisableDelay * animSpeedFactor);

			m_ThrowingWeaponSettings.FPObject.SetActive(false);

			yield return new WaitForSeconds((m_TW.Throwing.SpawnDelay - m_TW.Throwing.ModelDisableDelay) * animSpeedFactor);

			// Small hack until a better solution is found
			Ray[] itemUseRay = EHandler.GenerateItemUseRays(Player, EHandler.ItemUseTransform, 1, 1f);

			Vector3 position = itemUseRay[0].origin + transform.TransformVector(m_TW.Throwing.SpawnOffset);
			Quaternion rotation = Quaternion.LookRotation(itemUseRay[0].direction, transform.up);

			Projectile projectile = Instantiate(m_TW.Throwing.Projectile, position, rotation);
			Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

			projectileRB.velocity = (projectile.transform.forward * m_TW.Throwing.ThrowVelocity) + Player.Velocity.Get();
			projectileRB.angularVelocity = UnityEngine.Random.onUnitSphere * m_TW.Throwing.AngularSpeed;

			projectile.Launch(Player);

			if(m_TW.Throwing.ObjectToSpawn != null)
				PoolingManager.Instance.GetObject(m_TW.Throwing.ObjectToSpawn, itemUseRay[0].origin + transform.TransformVector(m_TW.Throwing.ObjectToSpawnOffset), Quaternion.identity, null);

			Player.DestroyEquippedItem.Try();
		}

		#if UNITY_EDITOR
		private void OnValidate()
		{
			if (EHandler != null)
			{
				EHandler.Animator_SetFloat(animHash_LongThrowSpeed, m_TW.Throwing.AnimThrowSpeed);
			}
		}
		#endif
	}
}