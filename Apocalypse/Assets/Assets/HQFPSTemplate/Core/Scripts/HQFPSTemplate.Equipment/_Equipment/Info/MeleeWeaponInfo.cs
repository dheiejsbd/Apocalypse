using System;
using UnityEngine;
using HQFPSTemplate.Surfaces;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Melee Weapon Info", menuName = "HQ FPS Template/Equipment/Melee Weapon")]
    public class MeleeWeaponInfo : EquipmentItemInfo
    {
		#region Internal
		[Serializable]
		public class MeleeSettingsInfo
		{
			[BHeader("General", true)]

			public LayerMask HitMask = new LayerMask();

			[Range(0f, 3f)]
			[Tooltip("How far can this weapon hit stuff?")]
			public float MaxHitDistance = 1.5f;

			[Space(3f)]

			public SurfaceEffects ImpactEffect = SurfaceEffects.Slash;
			public DamageType DamageType = DamageType.Hit;

			[Space(3f)]

			[BHeader("( Swings )", order = 2)]

			public bool CanContinuouslyAttack = false;
			public bool ResetSwingsIfNotUsed = false;

			[ShowIf("ResetSwingsIfNotUsed", true, 10f)]
			public float ResetSwingsDelay = 1f;

			public ItemSelection.Method SwingSelection = ItemSelection.Method.RandomExcludeLast;
			public SwingData[] Swings = null;
		}

		[Serializable]
		public class SwingData
		{
			public string SwingName = "Strong Attack";

			[Space(3)]

			[Tooltip("Useful for limiting the number of hits you can do in a period of time.")]
			public float Cooldown = 1f;

			[Range(0.01f, 5f)]
			public float CastDelay = 0.4f;

			[Range(0.01f, 10f)]
			public float CastRadius = 0.2f;

			[Range(1f, 500f)]
			public float HitDamage = 15f;

			[Range(1f, 500f)]
			public float HitImpact = 30f;

			[BHeader("( Animation )", order = 2)]

			public int AnimationIndex;
			public float AnimationSpeed = 1f;

			[BHeader("( Audio )", order = 2)]

			public DelayedSound SwingAudio;
			public DelayedSound HitAudio;

			[BHeader("( Camera )", order = 2)]

			public RecoilForce SwingCamForces;
			public RecoilForce HitCamForces;
		}
		#endregion

		[Group("5: ")] public MeleeSettingsInfo MeleeSettings = null;
	}
}
