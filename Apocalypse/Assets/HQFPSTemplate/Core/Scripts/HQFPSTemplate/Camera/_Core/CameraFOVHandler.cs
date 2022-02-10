using System.Collections;
using UnityEngine;
using System;

namespace HQFPSTemplate
{
    public class CameraFOVHandler : PlayerComponent
    {
		[Serializable]
		private class FOVCameraState
		{
			[Range(0f, 100f)]
			public float FOVSetSpeed = 30f;

			[Range(30f, 120f)]
			public float TargetFOV = 90f;
		}

		[SerializeField]
		[Range(0.1f,2f)]
		private float m_GlobalFOVMod = 1f;

		[Space]

		[BHeader("FOV per Player State", true, order = 2)]
		[SerializeField]
		[Group]
		private FOVCameraState m_IdleCameraFOV;

		[SerializeField]
		[Group]
		private FOVCameraState m_CrouchCameraFOV, m_RunCameraFOV, m_ProneCameraFOV;

		[Space]

		[SerializeField]
		[Group]
		private FOVCameraState m_AimCameraFOV;

		private Camera m_PlayerCam;
		private FOVCameraState m_CurrentFOVState;
		private Coroutine m_FOVSetter;


		public override void OnEntityStart()
        {
			m_PlayerCam = Player.Camera.UnityCamera;

			ChangeFOVState(m_IdleCameraFOV);

			Player.Aim.AddStartListener(() => ChangeFOVState(m_AimCameraFOV));
			Player.Aim.AddStopListener(() => ChangeFOVState(m_IdleCameraFOV));

			Player.Run.AddStartListener(() => ChangeFOVState(m_RunCameraFOV));
			Player.Run.AddStopListener(() => ChangeFOVState(m_IdleCameraFOV));

			Player.Crouch.AddStartListener(() => ChangeFOVState(m_CrouchCameraFOV));
			Player.Crouch.AddStopListener(() => ChangeFOVState(m_IdleCameraFOV));

			Player.Prone.AddStartListener(() => ChangeFOVState(m_ProneCameraFOV));
			Player.Prone.AddStopListener(() => ChangeFOVState(m_IdleCameraFOV));
		}

		private void ChangeFOVState(FOVCameraState fovCamState) 
		{
			m_CurrentFOVState = fovCamState;

			if (m_FOVSetter != null)
				StopCoroutine(m_FOVSetter);

			m_FOVSetter = StartCoroutine(C_SetFOV());
		}

		private IEnumerator C_SetFOV()
		{
			float targetFOV = Camera.HorizontalToVerticalFieldOfView(m_CurrentFOVState.TargetFOV * m_GlobalFOVMod, m_PlayerCam.aspect);
			float currentFOV = m_PlayerCam.fieldOfView;

			// if the target FOV is ~= with the normal FOV, use the normal FOV value instead. (used to never lose any float accuracy)
			while (Mathf.Abs(currentFOV - targetFOV) > Mathf.Epsilon)
			{
				currentFOV = Mathf.MoveTowards(currentFOV, targetFOV, Time.deltaTime * m_CurrentFOVState.FOVSetSpeed);
				m_PlayerCam.fieldOfView = currentFOV;

				yield return null;
			}
		}
	}
}
