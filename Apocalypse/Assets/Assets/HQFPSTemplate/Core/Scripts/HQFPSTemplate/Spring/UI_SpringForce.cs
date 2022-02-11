using UnityEngine;
using System;

namespace HQFPSTemplate.UserInterface
{
    [Serializable]
    public struct UI_SpringForce
    {
		public Vector2 Force;

		[Range(1, 20)]
		public int Distribution;


		public UI_SpringForce(Vector2 force, int distribution)
		{
			Force = force;
			Distribution = Mathf.Max(distribution, 1);
		}
	}
}
