using UnityEngine;
using System;

namespace HQFPSTemplate.UserInterface
{
    public class UI_Spring
    {
		public Vector2 Position { get { return m_LerpedPosition; } }

		public float Scale { get; set; }

		public float LerpSpeed { get; set; }

		private Vector2 m_Stiffness;

		private Vector2 m_Damping;

		private RectTransform m_RectTransform;

		private Vector2 m_Position;
		private Vector2 m_LerpedPosition;

		private Vector2 m_RestPosition;
		private Vector2 m_Velocity;
		private Vector2[] m_DistributedForce = new Vector2[100];


		public UI_Spring(RectTransform transform = null, Vector2 restVector = default, float lerpSpeed = 25f)
		{
			Scale = 1f;

			m_RectTransform = transform;
			m_RestPosition = restVector;

			LerpSpeed = lerpSpeed;
		}

		public void Reset()
		{
			m_Position = Vector2.zero;
			m_Velocity = Vector2.zero;

			for (int i = 0; i < 100; i++)
				m_DistributedForce[i] = Vector2.zero;
		}

		public void Adjust(Data data)
		{
			m_Stiffness = data.Stiffness;
			m_Damping = data.Damping;
		}

		public void FixedUpdate()
		{
			// Handle distributed forces.
			if (m_DistributedForce[0] != Vector2.zero)
			{
				AddForce(m_DistributedForce[0]);

				for (int i = 0; i < 100; i++)
				{
					m_DistributedForce[i] = i < 99 ? m_DistributedForce[i + 1] : Vector2.zero;
					if (m_DistributedForce[i] == Vector2.zero)
						break;
				}
			}

			UpdateSpring();
			UpdatePosition();
		}

		public void Update()
		{
			if (LerpSpeed > 0f)
				m_LerpedPosition = Vector2.Lerp(m_LerpedPosition, m_Position, Time.deltaTime * LerpSpeed);
			else
				m_LerpedPosition = m_Position;

			UpdateTransform();
		}

		public void AddForce(UI_SpringForce force)
		{
			if (force.Distribution > 1)
				AddDistributedForce(force.Force, force.Distribution);
			else
				AddForce(force.Force);
		}

		public void AddForce(Vector2 forceVector)
		{
			m_Velocity += forceVector;

			UpdatePosition();
		}

		private void AddDistributedForce(Vector2 force, int distribution)
		{
			distribution = Mathf.Clamp(distribution, 1, 100);

			AddForce(force / distribution);

			for (int i = 0; i < Mathf.RoundToInt(distribution) - 1; i++)
				m_DistributedForce[i] += force / distribution;
		}

		private void UpdateSpring()
		{
			m_Velocity += Vector2.Scale((m_RestPosition - m_Position), m_Stiffness);
			m_Velocity = Vector2.Scale(m_Velocity, Vector2.one - m_Damping);
		}

		private void UpdatePosition()
		{
			m_Position = Vector3Utils.GetNaNSafeVector3((m_Position + m_Velocity) * Scale);
		}

		private void UpdateTransform()
		{
			m_RectTransform.sizeDelta = m_LerpedPosition;
		}

		[Serializable]
		public struct Data
		{
			public static Data Default { get { return new Data() { Stiffness = Vector2.one * 0.1f, Damping = Vector2.one * 0.25f }; } }

			public Vector2 Stiffness;
			public Vector2 Damping;

			public Data(Vector2 stiffness, Vector2 damping)
			{
				Stiffness = stiffness;
				Damping = damping;
			}
		}
	}
}
