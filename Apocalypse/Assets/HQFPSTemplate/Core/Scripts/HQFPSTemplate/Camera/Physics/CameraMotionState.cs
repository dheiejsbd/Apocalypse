using System;
using UnityEngine;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate
{
	[Serializable]
	public class CameraMotionState
	{
		[BHeader("Main Settings")]

		[Group] public SpringSettings SpringSettings;

		[Space]

		[Group] public EquipmentMotionState.OffsetModule Offset;
		[Group] public EquipmentMotionState.BobModule Bob;
		[Group] public EquipmentMotionState.NoiseModule Noise;

		[BHeader("Additional Forces", order = 2)]

		public EquipmentPhysics.StepForceModule StepForce;

		public DelayedCameraForce[] EnterForces;
		public DelayedCameraForce[] ExitForces;
	}
	
	[Serializable]
	public class SimpleCameraMotionState
	{
		[Group] public EquipmentMotionState.BobModule Bob;
		[Group] public EquipmentMotionState.NoiseModule Noise;
	}
}
