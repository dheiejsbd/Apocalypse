using System;
using UnityEngine;

namespace HQFPSTemplate
{
	[Serializable]
	public struct SpringSettings
	{
		public static SpringSettings Default { get { return new SpringSettings() { Position = Spring.Data.Default, Rotation = Spring.Data.Default }; } }

		public Spring.Data Position;
		public Spring.Data Rotation;

		public SpringSettings(Spring.Data positionData, Spring.Data rotationData)
		{
			Position = positionData;
			Rotation = rotationData;
		}
	}

	public class Spring
	{
		#region Internal
		public enum Type
		{
			DontUpdate,
			OverrideLocalPosition,
			AddToLocalPosition,
			OverrideLocalRotation,
			AddToLocalRotation,
			OverrideLocalScale,
			AddToLocalScale
		}

		[Serializable]
		public struct Data
		{
			public static Data Default { get { return new Data() { Stiffness = Vector3.one * 0.1f, Damping = Vector3.one * 0.25f }; } }

			public Vector3 Stiffness;
			public Vector3 Damping;

			public Data(Vector3 stiffness, Vector3 damping)
			{
				Stiffness = stiffness;
				Damping = damping;
			}

            public override string ToString()
            {
				return $"Stiffness: {Stiffness} | Damping: {Damping}";
            }
        }
		#endregion

		public Vector3 Position { get { return m_LerpedPosition; } }

		public float Scale { get; set; }
		public float LerpSpeed { get; set; }

		private Vector3 m_Stiffness;
		private Vector3 m_Damping;

		private Type m_Type;
		private Transform m_Transform;

		private Vector3 m_Position;
		private Vector3 m_LerpedPosition;

		private Vector3 m_RestPosition;
		private Vector3 m_Velocity;
		private Vector3[] m_DistributedForce = new Vector3[100];


		public Spring(Type type, Transform transform, Vector3 restVector, float lerpSpeed = 25f)
		{
			Scale = 1f;

			m_Type = type;
			m_Transform = transform;

			m_RestPosition = restVector;

			LerpSpeed = lerpSpeed;
		}

		public void Reset()
		{
			m_Position = Vector3.zero;
			m_Velocity = Vector3.zero;

			for (int i = 0; i < 100; i ++)
				m_DistributedForce[i] = Vector3.zero;
		}

		public void Adjust(Vector3 stiffness, Vector3 damping)
		{
			m_Stiffness = stiffness;
			m_Damping = damping;
		}

		public void Adjust(Data data)
		{
			m_Stiffness = data.Stiffness;
			m_Damping = data.Damping;
		}

		public void FixedUpdate()
		{
			// Handle distributed forces.
			if(m_DistributedForce[0] != Vector3.zero)
			{
				AddForce(m_DistributedForce[0]);

				for(int i = 0; i < 100; i ++)
				{
					m_DistributedForce[i] = i < 99 ? m_DistributedForce[i + 1] : Vector3.zero;
					if(m_DistributedForce[i] == Vector3.zero)
						break;
				}
			}

			UpdateSpring();
			UpdatePosition();
		}

		public void Update()
		{
			if (LerpSpeed > 0f)
				m_LerpedPosition = Vector3.Lerp(m_LerpedPosition, m_Position, Time.deltaTime * LerpSpeed);
			else
				m_LerpedPosition = m_Position;

			UpdateTransform();
		}

		public void AddForce(SpringForce force)
		{
			if(force.Distribution > 1)
				AddDistributedForce(force.Force, force.Distribution);
			else
				AddForce(force.Force);
		}

		public void AddForce(Vector3 forceVector)
		{
			m_Velocity += forceVector;

			UpdatePosition();
		}

		public void AddForce(Vector3 forceVector, int distribution)
		{
			if(distribution > 1)
				AddDistributedForce(forceVector, distribution);
			else
				AddForce(forceVector);
		}

		public void AddDistributedForce(Vector3 force, int distribution)
		{
			distribution = Mathf.Clamp(distribution, 1, 100);

			AddForce(force / distribution);

			for(int i = 0; i < Mathf.RoundToInt(distribution) - 1; i++)
				m_DistributedForce[i] += force / distribution;
		}

		public void AddDistributedForce(SpringForce force)
		{
			force.Distribution = Mathf.Clamp(force.Distribution, 1, 100);

			AddForce(force.Force / force.Distribution);

			for (int i = 0; i < Mathf.RoundToInt(force.Distribution) - 1; i++)
				m_DistributedForce[i] += force.Force / force.Distribution;
		}

		private void UpdateSpring()
		{
			m_Velocity += Vector3.Scale(m_RestPosition - m_Position, m_Stiffness);
			m_Velocity = Vector3.Scale(m_Velocity, Vector3.one - m_Damping);
		}

		private void UpdatePosition()
		{
			m_Position = Vector3Utils.GetNaNSafeVector3((m_Position + m_Velocity) * Scale);
		}

		private void UpdateTransform()
		{
			if(m_Type == Type.DontUpdate)
				return;

			if(m_Type == Type.OverrideLocalPosition)
				m_Transform.localPosition = m_LerpedPosition;
			else if(m_Type == Type.AddToLocalPosition)
				m_Transform.localPosition += m_LerpedPosition;
			else if(m_Type == Type.OverrideLocalRotation)
				m_Transform.localEulerAngles = m_LerpedPosition;
			else if(m_Type == Type.AddToLocalRotation)
				m_Transform.localEulerAngles += m_LerpedPosition;
			else if (m_Type == Type.OverrideLocalScale)
				m_Transform.localScale = m_LerpedPosition;
			else if (m_Type == Type.AddToLocalScale)
				m_Transform.localScale += m_LerpedPosition;
		}
	}
}