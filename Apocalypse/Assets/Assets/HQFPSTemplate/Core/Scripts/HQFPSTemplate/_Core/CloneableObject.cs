using UnityEngine;

namespace HQFPSTemplate
{
	public abstract class CloneableObject<T>
	{
		public T GetMemberwiseClone()
		{
			return (T)MemberwiseClone();
		}
	}
}
