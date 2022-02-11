using UnityEngine;

namespace HQFPSTemplate
{
	public abstract class Projectile : MonoBehaviour
	{
		public abstract void Launch(Entity launcher);
	}
}