using UnityEngine;

namespace HQFPSTemplate
{
	/// <summary>
	/// Base Class for any Entity
	/// </summary>
	public class Entity : MonoBehaviour
	{
		public Inventory Inventory { get { return m_Inventory; } }

		/// <summary></summary>
		public readonly Value<float> Health = new Value<float>(100f);

		/// <summary> </summary>
		public readonly Attempt<DamageInfo> ChangeHealth = new Attempt<DamageInfo>();

		/// <summary> </summary>
		public readonly Attempt<DamageInfo, IDamageable> DealDamage = new Attempt<DamageInfo, IDamageable>();

		/// <summary> </summary>
		public readonly Value<bool> IsGrounded = new Value<bool>(true);

		/// <summary> </summary>
		public readonly Value<Vector3> Velocity = new Value<Vector3>(Vector3.zero);

		public Value<Vector3> LookDirection = new Value<Vector3>();

		/// <summary> </summary>
		public readonly Message<float> FallImpact = new Message<float>();

		/// <summary></summary>
		public readonly Message Death = new Message();

		/// <summary></summary>
		public readonly Message Respawn = new Message();

		public Hitbox[] Hitboxes;

		[SerializeField]
		private Inventory m_Inventory = null;


		private void Start()
		{
			Hitboxes = GetComponentsInChildren<Hitbox>();

			foreach (var component in GetComponentsInChildren<EntityComponent>(true))
				component.OnEntityStart();
		}
	}
}