using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Equipment Physics", menuName = "HQ FPS Template/Equipment Component/Physics")]
    public class EquipmentPhysicsInfo : ScriptableObject
    {
		#region Internal
		[Serializable]
		public class GeneralSettingsInfo
		{
			public Vector3 BasePosOffset, BaseRotOffset;
		}

		[Serializable]
		public class SwayModule : CloneableObject<SwayModule>
		{
			[Range(0f, 20f)]
			public float LookInputMultiplier = 1f;

			[Clamp(0f, 100)]
			public float MaxLookInput = 5f;

			[Clamp(0f, 100)]
			public float AimMultiplier = 0.2f;

			[Space]

			public Vector3 LookPositionSway;
			public Vector3 LookRotationSway;

			[Space]

			public Vector3 StrafePositionSway;
			public Vector3 StrafeRotationSway;

			[Space]

			public Vector3 FallSway;
		}

		[Serializable]
		public class JumpModule : CloneableObject<JumpModule>
		{
			public bool Enabled = true;

			[EnableIf("Enabled", true)]
			public SpringForce PositionForce, RotationForce;
		}

		[Serializable]
		public class FallImpactModule : CloneableObject<FallImpactModule>
		{
			public bool Enabled = true;

			[EnableIf("Enabled", true)]
			public SpringForce PositionForce, RotationForce;
		}

		[Serializable]
		public class StepForceModule : CloneableObject<StepForceModule>
		{
			public bool Enabled = true;

			[EnableIf("Enabled", true)]
			public SpringForce PositionForce, RotationForce;
		}
		#endregion

		[BHeader("Main Settings", true)]

		[Group] public GeneralSettingsInfo GeneralSettings = null;

		[Space]

		[Group] public SwayModule Sway = null;
		[Group] public JumpModule Jump = null;
		[Group] public FallImpactModule FallImpact = null;

		[Space(3f)]

		[BHeader("Step Forces", true, order = 2)]

		[Group] public StepForceModule WalkStepForce = null;
		[Group] public StepForceModule CrouchStepForce = null;
		[Group] public StepForceModule RunStepForce = null;

		[Space(3f)]

		[BHeader("States", true, order = 2)]

		[Group] public EquipmentMotionState IdleState = null;
		[Group] public EquipmentMotionState WalkState = null;
		[Group] public EquipmentMotionState RunState = null;
		[Group] public EquipmentMotionState AimState = null;
		[Group] public EquipmentMotionState CrouchState = null;
		[Group] public EquipmentMotionState ProneState = null;
	}
}
