using UnityEngine;
using System;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate
{
	[CreateAssetMenu(fileName = "Camera Physics Preset", menuName = "HQ FPS Template/Camera Physics Preset")]
	public class CameraPhysicsPreset : ScriptableObject
    {
		#region Internal
		public struct CustomSprings
		{
			public SpringSettings ForceSpringSettings;
		}

		[Serializable]
		public struct ShakesModule
		{
			[Group] public SpringSettings ShakeSpringSettings;
			[Group] public CameraShakeSettings ExplosionShake;
		}

		[Serializable]
		public struct FallImpactModule
		{
			[MinMax(0, 50)]
			public Vector2 FallImpactRange;

			[Space(3f)]

			public SpringForce PosForce;
			public SpringForce RotForce;
		}
		#endregion

		[BHeader("General", true)]

		[Group] public EquipmentPhysics.SwayModule Sway = null;
		[Group] public EquipmentPhysics.JumpModule Jump = null;

		[Space]

		[Group] public FallImpactModule FallImpact = new FallImpactModule();
		[Group] public SimpleSpringForce GetHitForce = new SimpleSpringForce();
		[Group] public ShakesModule CameraShakes = new ShakesModule();

		[Space(3f)]

		[BHeader("States", true, order = 2)]

		[Group] public SimpleCameraMotionState AimState = null;

		[Space(3f)]

		[Group] public CameraMotionState IdleState = null;
		[Group] public CameraMotionState WalkState = null;
		[Group] public CameraMotionState RunState = null;
		[Group] public CameraMotionState CrouchState = null;
		[Group] public CameraMotionState ProneState = null;
	}
}
