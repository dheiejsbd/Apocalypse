using UnityEngine;
using System;

namespace HQFPSTemplate
{
	[Serializable]
	public struct SpringForce
	{
		public Vector3 Force;

		[Range(1, 20)]
		public int Distribution;


		public SpringForce(Vector3 force, int distribution)
		{
			Force = force;
			Distribution = Mathf.Max(distribution, 1);
		}

		public static SpringForce operator *(SpringForce a, float b) => new SpringForce((a.Force * b), a.Distribution);
	}

	[Serializable]
	public struct SimpleSpringForce
	{
		[Range(0f, 10f)]
		public float PosForce;

		[Range(0f, 10f)]
		public float RotForce;

		[Range(1, 20)]
		public int Distribution;


		public SimpleSpringForce(float posForceAmount = 1f, float rotForceAmount = 1f, int distribution = 1)
		{
			PosForce = posForceAmount;
			RotForce = rotForceAmount;
			Distribution = Mathf.Max(distribution, 1);
		}
	}
}