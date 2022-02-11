using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Throwing Weapon Info", menuName = "HQ FPS Template/Equipment/Throwing Weapon")]
    public class ThrowingWeaponInfo : EquipmentItemInfo
    {
		#region Internal
		[Serializable]
		public class ThrowingInfo
		{
			public Projectile Projectile = null;

			[Space]

			[Tooltip("Time to spawn the projectile")]
			public float SpawnDelay = 1f;

			[Tooltip("Time to disable the mesh of the throwable")]
			public float ModelDisableDelay = 0.9f;

			public Vector3 SpawnOffset = Vector3.zero;

			[Range(0f, 100f)]
			public float ThrowVelocity = 15f;

			[Clamp(0f, 1000f)]
			public float AngularSpeed = 0f;

			[BHeader("( Misc )", order = 2)]

			[Tooltip("e.g. the prefab of the grenade clip.")]
			public GameObject ObjectToSpawn = null;

			public Vector3 ObjectToSpawnOffset = Vector3.zero;

			[BHeader("( Animation )", order = 2)]

			public float AnimThrowSpeed = 1f;

			[BHeader("( Audio )", order = 2)]

			public DelayedSound[] ThrowAudio = null;

			[BHeader("( Camera )", order = 2)]

			public DelayedCameraForce[] ThrowCamForces = null;
		}
		#endregion

		[Group("5: ")] public ThrowingInfo Throwing = null;
	}
}
