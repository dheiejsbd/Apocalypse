using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate
{
	public class ItemPickup : InteractiveObject
	{
		#region Internal
		public enum PickUpMethod
		{
			TriggerBased,
			InteractionBased
		}
        #endregion

        public Item ItemInstance { get { return m_ItemInstance; } }

		[BHeader("Item",true, order = 100)]

		[SerializeField]
		[DatabaseItem]
		protected string m_Item = string.Empty;

		[SerializeField]
		[Range(0,100)]
		protected int m_ItemCount = 1;

		[SerializeField]
		[Tooltip("In what container of the Player will the picked up item go")]
		protected ItemContainerFlags m_TargetContainers = ItemContainerFlags.Storage;

		[BHeader("Pick Up", true, order = 2)]

		[SerializeField]
		protected PickUpMethod m_PickUpMethod = PickUpMethod.InteractionBased;

		[SerializeField]
		[ShowIf("m_PickUpMethod", (int)PickUpMethod.TriggerBased)]
		[Tooltip("The radius of the auto-created trigger.")]
		protected float m_TriggerRadius = 0.5f;

		[Space]

		[SerializeField]
		protected Color m_BaseMessageColor = new Color(1f, 1f, 1f, 0.678f);

		[SerializeField]
		protected Color m_ItemCountColor = new Color(0.976f, 0.6f, 0.129f, 1f);

		[SerializeField]
		protected Color m_InventoryFullColor = Color.red;

		protected Item m_ItemInstance;
		private string m_InitialInteractionText;


        public override void OnInteractionEnd(Humanoid humanoid)
		{
			TryPickUp(humanoid, InteractionProgress.Get());

			base.OnInteractionEnd(humanoid);
		}

		public void SetItem(Item item)
		{
			m_ItemInstance = item;

			if(m_ItemInstance != null)
			{
				m_Item = m_ItemInstance.Name;
				SetInteractionText(item);
			}
		}

		protected override void Awake()
		{
			base.Awake();

			m_InitialInteractionText = InteractionText.Val;

			if(m_PickUpMethod != PickUpMethod.InteractionBased)
				InteractionEnabled = false;

			if (ItemDatabase.TryGetItemByName(m_Item, out ItemInfo itemInfo))
			{
				m_ItemInstance = new Item(itemInfo, m_ItemCount);

				// Create a trigger if the pickup method is set to WalkOver
				if (m_PickUpMethod == PickUpMethod.TriggerBased)
				{
					var sphereCol = gameObject.AddComponent<SphereCollider>();
					sphereCol.isTrigger = true;
					sphereCol.radius = m_TriggerRadius;
				}

				if (m_ItemInstance != null)
					SetInteractionText(m_ItemInstance);
			}
			else
				InteractionEnabled = false;
		}

		protected virtual void TryPickUp(Humanoid humanoid, float interactProgress)
		{
			if (m_ItemInstance != null)
			{
				// Item added to inventory
				if (humanoid.Inventory.AddItem(m_ItemInstance, m_TargetContainers))
				{
					if (m_ItemInstance.Info.StackSize > 1)
						UI_MessageDisplayer.Instance.PushMessage(string.Format("Picked up <color={0}>{1}</color> x {2}", ColorUtils.ColorToHex(m_ItemCountColor), m_ItemInstance.Name, m_ItemInstance.CurrentStackSize), m_BaseMessageColor);
					else
						UI_MessageDisplayer.Instance.PushMessage(string.Format("Picked up <color={0}>{1}</color>", ColorUtils.ColorToHex(m_ItemCountColor), m_ItemInstance.Name), m_BaseMessageColor);

					Destroy(gameObject);
				}
				// Item not added to inventory
				else
				{
					UI_MessageDisplayer.Instance.PushMessage(string.Format("<color={0}>Inventory Full</color>", ColorUtils.ColorToHex(m_InventoryFullColor)), m_BaseMessageColor);
				}
			}
			else
			{
				Debug.LogError("Item Instance is null, can't pick up anything.");
				return;
			}
		}

		private void SetInteractionText(Item item)
		{
			if (item.CurrentStackSize < 2)
				InteractionText.Set(string.Format(m_InitialInteractionText, item.Name.ToUpper()));
			else
				InteractionText.Set(string.Format(m_InitialInteractionText + " x " + item.CurrentStackSize, item.Name.ToUpper()));
		}

		private void OnTriggerEnter(Collider col)
		{
			if(m_PickUpMethod != PickUpMethod.TriggerBased)
				return;

			if(col.TryGetComponent(out Humanoid humanoid))
				TryPickUp(humanoid, 0f);
		}

		private void OnDrawGizmosSelected()
		{
			if(m_PickUpMethod == PickUpMethod.TriggerBased)
			{
				var prevColor = Gizmos.color;
				Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.2f);
				Gizmos.DrawSphere(transform.position, m_TriggerRadius);
				Gizmos.color = prevColor;
			}
		}
	}
}
