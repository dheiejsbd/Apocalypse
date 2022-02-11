using System.Collections;
using System;
using UnityEngine;

namespace HQFPSTemplate
{
	public class RootHeightHandler : PlayerComponent  
	{
		#region Internal
		#pragma warning disable CS0649
		[Serializable]
		private class HeightChangeState
		{
			[Range(-2f, 0f)]
			public float CameraOffset;

			public EasingOptions Easing;
		}
		#pragma warning restore CS0649
		#endregion

		[SerializeField]
		[Group]
		private HeightChangeState m_CrouchState;

		[SerializeField]
		[Group]
		private HeightChangeState m_ProneState;

		private HeightChangeState m_CurrentState;

		private float m_CurrentOffsetOnY;
		private float m_InitialHeight;

		private Easer m_HeightEaser;


		private void Start()
		{
			Player.Crouch.AddStartListener(() => { OnControllerHeightChange(m_CrouchState); });
			Player.Crouch.AddStopListener(() => { OnControllerHeightChange(null); });
			Player.Prone.AddStartListener(() => { OnControllerHeightChange(m_ProneState); });
			Player.Prone.AddStopListener(() => { OnControllerHeightChange(null); });

			m_InitialHeight = transform.localPosition.y;
		}

		private void OnControllerHeightChange(HeightChangeState heightChangeState) 
		{
			float verticalOffset = 0f;

			if (heightChangeState != null)
			{
				float easerDuration = heightChangeState.Easing.Duration;

				if (m_CurrentState != null)
					easerDuration = Mathf.Abs(m_CurrentState.Easing.Duration - heightChangeState.Easing.Duration);

				m_HeightEaser = new Easer(heightChangeState.Easing.Function, easerDuration);

				verticalOffset = heightChangeState.CameraOffset;
			}

			m_CurrentState = heightChangeState;

			StopAllCoroutines();
			StartCoroutine(SetVerticalOffset(verticalOffset));
		}

		private IEnumerator SetVerticalOffset(float offset)
		{
			var startOffset = m_CurrentOffsetOnY;
			m_HeightEaser.Reset();

			while(m_HeightEaser.InterpolatedValue < 1f)
			{
				m_HeightEaser.Update(Time.deltaTime);
				m_CurrentOffsetOnY = Mathf.Lerp(startOffset, offset, m_HeightEaser.InterpolatedValue);

				transform.localPosition = Vector3.up * (m_CurrentOffsetOnY + m_InitialHeight);

				yield return null;
			}
		}
	}
}
