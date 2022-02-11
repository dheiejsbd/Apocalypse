using System;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    public class GunSettings
    {
		[Serializable]
		public class Shooting : ICloneable
		{
			[Tooltip("The layers that will be affected when you fire.")]
			public LayerMask RayMask;

			[Range(1, 30)]
			[Tooltip("The amount of rays that will be sent in the world " +
			"(basically the amount of projectiles / bullets that will be fired at a time).")]
			public int RayCount = 1;

			[Range(0f, 10f)]
			public float RaySpread = 1f;

			[Tooltip("How the bullet spread will transform (in continuous use) on the duration of the magazine, the max x value(1) will be used if the whole magazine has been used")]
			public AnimationCurve SpreadOverTime = new AnimationCurve(
				new Keyframe(0f, .8f),
				new Keyframe(1f, 1f));

			[Space]

			public HitscanImpact RayImpact;

			public object Clone() => MemberwiseClone();
        }

		[Serializable]
		public class HitscanImpact
		{
			public float MaxDistance => m_MaxDistance;

			[Range(0f, 1000f)]
			[SerializeField]
			[Tooltip("The damage at close range.")]
			private float m_MaxDamage = 15f;

			[Range(0f, 1000f)]
			[SerializeField]
			[Tooltip("The impact impulse that will be transfered to the rigidbodies at contact.")]
			private float m_MaxImpulse = 15f;

            [SerializeField]
            [Tooltip("How damage and impulse lowers over distance.")]
            private AnimationCurve m_DistanceCurve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.8f, 0.5f),
                new Keyframe(1f, 0f));

			[SerializeField]
			[Tooltip("If something is farther than this distance threeshold, it will not be affected by the shot.")]
			[Range(0.1f, 1000f)]
			private float m_MaxDistance = 150f;


			/// <param name="distance"></param>
			/// <param name="maxDistance"></param>
			public float GetDamageAtDistance(float distance)
			{
				return ApplyCurveToValue(m_MaxDamage, distance);
			}

			/// <returns>The impulse at distance.</returns>
			/// <param name="distance">Distance.</param>
			/// <param name="maxDistance">Max distance.</param>
			public float GetImpulseAtDistance(float distance)
			{
				return ApplyCurveToValue(m_MaxImpulse, distance);
			}

			private float ApplyCurveToValue(float value, float distance)
			{
				float maxDistanceAbsolute = Mathf.Abs(m_MaxDistance);
				float distanceClamped = Mathf.Clamp(distance, 0f, maxDistanceAbsolute);

				return value * m_DistanceCurve.Evaluate(distanceClamped / maxDistanceAbsolute);
			}
		}
	}
}