using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Launcher Info", menuName = "HQ FPS Template/Equipment/Launcher")]
    public class LauncherInfo : ProjectileWeaponInfo
    {
		#region Internal
		[Serializable]
		public class LaunchingInfo
		{
			public ShaftedProjectile Prefab = null;

			[Space]

			public Vector3 SpawnOffset = Vector3.zero;
			public Vector3 AngularVelocity = Vector3.zero;

			[Range(0.01f, 10f)]
			public float LaunchSpread = 1f;

			[Range(0f, 100f)]
			public float LaunchSpeed = 15f;

			[Range(0, 5f)]
			public float LaunchDelay = 0.3f;
		}
		#endregion

		[Group("5: ")] public LaunchingInfo Launching;
	}
}
