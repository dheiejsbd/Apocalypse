using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Projectile Weapon Info", menuName = "HQ FPS Template/Equipment/Projectile Weapon")]
    public class ProjectileWeaponInfo : EquipmentItemInfo
    {
		#region Internal
		[Flags]
		public enum FireMode
		{
			None = 0,
			Safety = 1,
			SemiAuto = 2,
			Burst = 4,
			FullAuto = 8,
			All = ~0
		}

		public enum ReloadType 
		{
			Once,
			Progressive
		}

		[Serializable]
		public class ShootingInfo
		{
			[BHeader("Ammo", true)]

			public bool EnableAmmo = true;

			[EnableIf("EnableAmmo", true, 10f)]
			public int MagazineSize = 30;

			[Space]

			public FireMode Modes = FireMode.SemiAuto;

			[Tooltip("How the fire rate will transform (in continuous use) on the duration of the magazine, the max x value(1) will be used if the whole magazine has been used")]
			public AnimationCurve FireRateOverTime = new AnimationCurve(
				new Keyframe(0f, 1f),
				new Keyframe(1f, 1f));

			[Tooltip("The minimum time that can pass between consecutive shots.")]
			public float FireDuration = 0.22f;

			[Space(3f)]

			[Tooltip("How many shots will the gun fire when in Burst-mode.")]
			public int BurstLength = 3;

			[Tooltip("How much time it takes to fire all the shots.")]
			public float BurstDuration = 0.3f;

			[Tooltip("The minimum time that can pass between consecutive bursts.")]
			public float BurstPause = 0.35f;

			[Space(3f)]

			[Tooltip("The maximum amount of shots that can be executed in a minute.")]
			public int RoundsPerMinute = 450;

			[Space(3f)]

			[BHeader("( Animation )", order = 2)]

			public bool HasDryFireAnim = false;

			public bool HasAlternativeFireAnim = false;

			[Tooltip("The minimum time that can pass between consecutive shots.")]
			[Range(0.1f, 5f)]
			public float FireAnimationSpeed = 1f;

			[BHeader("( Audio )", order = 2)]

			[Tooltip("Sounds that will play when firing the gun.")]
			public SoundPlayer ShootAudio;
			public SoundPlayer ShootTailAudio;
			public SoundPlayer DryShootAudio;

			[Space(3f)]

			public DelayedSound FireModeChangeAudio;

			[Space(3f)]

			public DelayedSound[] HandlingAudio;
			public DelayedSound[] CasingDropAudio;

			[BHeader("( Camera )", order = 2)]

			public DelayedCameraForce[] HandlingCamForces;
		}

		[Serializable]
		public class ReloadingInfo
		{
			[BHeader("( General )")]

			public ReloadType ReloadType = ReloadType.Once;

			[Tooltip("The time between reloading starts and the first bullet insert.")]
			[EnableIf("ReloadType", (int)ReloadType.Progressive, 0f)]
			public float ReloadStartDuration;

			[Tooltip("How much time it takes to reload the gun.")]
			public float ReloadDuration = 2.5f;

			[EnableIf("ReloadType", (int)ReloadType.Progressive, 0f)]
			public float ReloadEndDuration;

			[Space(4f)]

			public bool HasEmptyReload = false;

			[Tooltip("How much time it takes to reload the gun and chamber the first bullet.")]
			[EnableIf("HasEmptyReload", true, 5f)]
			public float EmptyReloadDuration = 3f;

			[EnableIf("HasEmptyReload", true, 5f)]
			public bool ProgressiveEmptyReload;

			[BHeader("( Animation )")]

			[SerializeField]
			public float ReloadAnimationSpeed = 1f;

			[SerializeField]
			[EnableIf("HasEmptyReload", true)]
			public float EmptyReloadAnimationSpeed = 1f;

			[BHeader("( Audio )", order = 2)]

			public DelayedSound[] ReloadSounds;
			public DelayedSound[] ReloadStartSounds;
			public DelayedSound[] ReloadEndSounds;
			public DelayedSound[] EmptyReloadSounds;

			[BHeader("( Camera )", order = 2)]

			public DelayedCameraForce[] ReloadLoopCamForces;
			public DelayedCameraForce[] ReloadStartCamForces;
			public DelayedCameraForce[] ReloadEndCamForces;
			public DelayedCameraForce[] EmptyReloadLoopCamForces;
		}
		#endregion

		[Space]

		[Group("5: ")] public ShootingInfo Shooting;
		[Group("6: ")] public ReloadingInfo Reloading;
	}
}
