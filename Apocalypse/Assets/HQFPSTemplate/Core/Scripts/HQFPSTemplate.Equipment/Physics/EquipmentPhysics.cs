using System;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
	public class EquipmentPhysics : PlayerComponent, IEquipmentComponent
	{
		#region Internal
		[Serializable]
		public class BaseSettings
		{
			public Transform Pivot;

			[EnableIf("Pivot", true)]
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

		public BaseSettings GeneralSettings = null;

		[Space]

		public SwayModule Sway = null;
		public JumpModule Jump = null;
		public FallImpactModule FallImpact = null;

		[Space(3f)]

		[BHeader("Step Forces", true, order = 2)]

		public StepForceModule WalkStepForce = null;
		public StepForceModule CrouchStepForce = null;
		public StepForceModule RunStepForce = null;

		[Space(3f)]

		[BHeader("States", true, order = 2)]

		public EquipmentMotionState IdleState = null;
		public EquipmentMotionState WalkState = null;
		public EquipmentMotionState RunState = null;
		public EquipmentMotionState AimState = null;
		public EquipmentMotionState CrouchState = null;
		public EquipmentMotionState ProneState = null;

		private EquipmentPhysicsHandler m_EPhysicsHandler = null;

		public void Initialize(EquipmentItem equipmentItem)
		{
			m_EPhysicsHandler = equipmentItem.EHandler.EPhysicsHandler;

			if (GeneralSettings.Pivot == null)
			{
				GeneralSettings.Pivot = transform.Find("Pivot");

				if (GeneralSettings.Pivot == null)
				{
					Transform pivot = new GameObject("Pivot").transform;
					GeneralSettings.Pivot = pivot;
				}
			}
		}

		public void OnSelected() { }

		private void OnValidate()
		{
			if (m_EPhysicsHandler != null)
			{
				m_EPhysicsHandler.ReadjustSprings();
				m_EPhysicsHandler.SetOffset();
			}
		}
    }
}