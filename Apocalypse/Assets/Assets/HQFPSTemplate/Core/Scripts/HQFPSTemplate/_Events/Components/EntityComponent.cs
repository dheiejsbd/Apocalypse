using UnityEngine;

namespace HQFPSTemplate
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class EntityComponent : MonoBehaviour 
	{
		public Entity Entity
		{
			get 
			{
				if(!m_Entity)
					m_Entity = GetComponent<Entity>();
				if(!m_Entity)
					m_Entity = GetComponentInParent<Entity>();
				
				return m_Entity;
			}
		}

		private Entity m_Entity;

		public virtual void OnEntityStart() {  }
	}
}
