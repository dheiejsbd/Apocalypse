using UnityEngine;

namespace HQFPSTemplate
{
	public class RaycastInfo
	{
		public Collider Collider { get; private set; }
		public InteractiveObject InteractiveObject { get; private set; }

		public bool IsInteractive { get; private set; }


		public RaycastInfo(Collider collider, InteractiveObject interactiveObject)
		{
			Collider = collider;
			InteractiveObject = interactiveObject;
			IsInteractive = (InteractiveObject != null) && InteractiveObject.InteractionEnabled;
		}
	}
}