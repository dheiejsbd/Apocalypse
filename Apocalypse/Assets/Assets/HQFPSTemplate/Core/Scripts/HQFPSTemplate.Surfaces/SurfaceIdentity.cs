using UnityEngine;

namespace HQFPSTemplate.Surfaces
{
	public class SurfaceIdentity : MonoBehaviour
	{
		public SurfaceInfo Surface { get => m_Surface; set => m_Surface = value; }

		[SerializeField]
		private SurfaceInfo m_Surface = null;
	}
}
