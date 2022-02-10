using System.Collections.Generic;
using UnityEngine;

namespace HQFPSTemplate
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerCamera : PlayerComponent
	{
		public float SensitivityFactor { get; set; }
		public CameraPhysicsHandler Physics { get => m_CameraPhysicsHandler; }
		public Camera UnityCamera { get => m_WorldCamera; }

		public Vector2 LookAngles
		{
			get => m_LookAngles;

			set
			{
				m_LookAngles = value;
			}
		}

		public Vector2 LastMovement { get; private set; }

		[BHeader("General", true)]

		[SerializeField] 
		[Tooltip("The camera root which will be rotated up & down (on the X axis).")]
		private Transform m_LookRoot = null;

		[SerializeField]
		private Transform m_PlayerRoot = null;

		[SerializeField]
		private Camera m_WorldCamera = null;

		[SerializeField]
		private CameraPhysicsHandler m_CameraPhysicsHandler = null;

		[Space]

		[SerializeField]
		[Tooltip("The up & down rotation will be inverted, if checked.")]
		private bool m_Invert = false;

		[BHeader("Mouse Sensitivity")]

		[SerializeField]
		[Tooltip("The higher it is, the faster the camera will rotate.")]
		private float m_Sensitivity = 5f;

		[SerializeField]
		private float m_AirborneSensitivity = 1.5f;

		[BHeader("Mouse Smoothing")]

		[SerializeField]
		private bool m_Raw;

		[SerializeField]
		[ShowIf("m_Raw", false)]
		[Range(0, 20)]
		private int m_SmoothSteps = 10;

		[SerializeField]
		[ShowIf("m_Raw", false)]
		[Range(0f, 1f)]
		private float m_SmoothWeight = 0.4f;

		[BHeader("Rotation Limits")]

		[SerializeField]
		private Vector2 m_DefaultLookLimits = new Vector2(-60f, 90f);

		[BHeader("Equipment")]

		[SerializeField]
		[Range(0.01f, 10f)]
		private float m_EquipmentWorldScale = 0.5f;

		private Vector2 m_LookAngles;
		private float m_CurrentSensitivity;
		private Vector2 m_CurrentMouseLook;
		private Vector2 m_SmoothMove;
		private List<Vector2> m_SmoothBuffer = new List<Vector2>();

		private float m_SensitivityMod = 1f;

		private bool m_Loaded;


		public void MoveCamera(float verticalMove, float horizontalMove)
		{
			LookAngles += new Vector2(verticalMove, horizontalMove);
		}

		public void OnLoad()
		{
			m_Loaded = true;
		}

		private void Awake()
		{
			SensitivityFactor = 1f;
			transform.localScale = new Vector3(m_EquipmentWorldScale, m_EquipmentWorldScale, m_EquipmentWorldScale);
		}

		private void Start()
		{
			if(!m_LookRoot)
			{
				Debug.LogErrorFormat(this, "Assign the view root in the inspector!", name);
				enabled = false;
			}

			if(!m_Loaded)
				m_LookAngles = new Vector2(transform.localEulerAngles.x, m_PlayerRoot.localEulerAngles.y);
		}

		private void LateUpdate()
		{
			Vector2 prevLookAngles = m_LookAngles;

			float targetSensitivity = Player.IsGrounded.Is(true) ? m_Sensitivity : m_AirborneSensitivity;

			targetSensitivity *= m_SensitivityMod;

			m_CurrentSensitivity = Mathf.Lerp(m_CurrentSensitivity, targetSensitivity, Time.deltaTime * 8f);

			if(!Player.Pause.Active)
			MoveView(new Vector2(Player.LookInput.Get().y, Player.LookInput.Get().x), Time.deltaTime);

			LastMovement = m_LookAngles - prevLookAngles;
		}

		private void MoveView(Vector2 lookInput, float deltaTime)
		{
			if (!m_Raw)
			{
				CalculateSmoothLookInput(lookInput, deltaTime);

				m_LookAngles.x += m_CurrentMouseLook.x * m_CurrentSensitivity * (m_Invert ? 1f : -1f);
				m_LookAngles.y += m_CurrentMouseLook.y * m_CurrentSensitivity;

				m_LookAngles.x = ClampAngle(m_LookAngles.x, m_DefaultLookLimits.x, m_DefaultLookLimits.y);
			}
			else
			{
				m_LookAngles.x += lookInput.x * m_CurrentSensitivity * (m_Invert ? 1f : -1f);
				m_LookAngles.y += lookInput.y * m_CurrentSensitivity;

				m_LookAngles.x = ClampAngle(m_LookAngles.x, m_DefaultLookLimits.x, m_DefaultLookLimits.y);
			}

			m_PlayerRoot.localRotation = Quaternion.Euler(0f, m_LookAngles.y, 0f);
			m_LookRoot.localRotation = Quaternion.Euler(m_LookAngles.x, 0f, 0f);

			Entity.LookDirection.Set(m_LookRoot.forward);
		}

		/// <summary>
		/// Clamps the given angle between min and max degrees.
		/// </summary>
		private float ClampAngle(float angle, float min, float max)
		{
			if (angle > 360f)
				angle -= 360f;
			else if (angle < -360f)
				angle += 360f;

			return Mathf.Clamp(angle, min, max);
		}

		private void CalculateSmoothLookInput(Vector2 lookInput, float deltaTime)
		{
			if (deltaTime == 0f)
				return;

			m_SmoothMove = new Vector2(lookInput.x, lookInput.y);

			m_SmoothSteps = Mathf.Clamp(m_SmoothSteps, 1, 20);
			m_SmoothWeight = Mathf.Clamp01(m_SmoothWeight);

			while (m_SmoothBuffer.Count > m_SmoothSteps)
				m_SmoothBuffer.RemoveAt(0);

			m_SmoothBuffer.Add(m_SmoothMove);

			float weight = 1f;
			Vector2 average = Vector2.zero;
			float averageTotal = 0f;

			for (int i = m_SmoothBuffer.Count - 1; i > 0; i--)
			{
				average += m_SmoothBuffer[i] * weight;
				averageTotal += weight;
				weight *= m_SmoothWeight / (deltaTime * 60f);
			}

			averageTotal = Mathf.Max(1f, averageTotal);
			m_CurrentMouseLook = average / averageTotal;
		}
	}
}
