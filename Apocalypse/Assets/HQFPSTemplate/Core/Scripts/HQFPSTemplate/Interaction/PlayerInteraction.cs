using UnityEngine;
using System.Collections;

namespace HQFPSTemplate
{
	/// <summary>
	/// Sends a ray from the center of the camera, in the game world.
	/// Gathers data about what is in front of the player, and stores it in a variable.
	/// </summary>
	public class PlayerInteraction : PlayerComponent
	{
        #region Internal
        public enum LoopingMethod
		{
			EveryFrame,
			EveryFrameFixed,
			Periodically
		}
        #endregion

		[SerializeField]
		private LayerMask m_LayerMask = new LayerMask();

		[Space]

		[SerializeField]
		[Tooltip("The looping method used for updating the interaction system.")]
		private LoopingMethod m_LoopingMethod = LoopingMethod.EveryFrame;

		[SerializeField]
		[ShowIf("m_LoopingMethod", (int)LoopingMethod.Periodically)]
		private float m_UpdateTime = 0.03f;

		[Space]

		[SerializeField]
		[Tooltip("The maximum distance at which you can interact with objects.")]
		private float m_InteractionDistance = 2f;

		[SerializeField]
		[Range(0f, 60f)]
		private float m_MaxInteractionAngle = 30f;

		private InteractiveObject m_InteractedObject;
		private InteractiveObject m_ClosestObject;

		private Collider[] m_CollidersInRange;

		private Transform m_WorldCamera; 
		private int m_ClosestObjectIndex = -1;
		private float m_SmallestAngle;


        private void Start()
        {
			Player.Death.AddListener(() => { StopAllCoroutines(); });

			m_WorldCamera = Player.Camera.UnityCamera.transform;
		}

        private void OnEnable()
		{
			Player.Interact.AddChangeListener(OnChanged_WantsToInteract);

			if (m_LoopingMethod == LoopingMethod.Periodically)
				StartCoroutine(C_UpdateInteraction());
		}

		private void Update() { if (m_LoopingMethod == LoopingMethod.EveryFrame) UpdateInteraction(); }

		private void FixedUpdate() { if (m_LoopingMethod == LoopingMethod.EveryFrameFixed) UpdateInteraction(); }

		private IEnumerator C_UpdateInteraction() 
		{
			var wait = new WaitForSeconds(Mathf.Max(m_UpdateTime, 0.01f));

			while (enabled)
			{
				UpdateInteraction();

				yield return wait;
			}
		}

		private void OnChanged_WantsToInteract(bool wantsToInteract)
		{
			var raycastData = Player.RaycastInfo.Get();

			var wantedToInteractPreviously = Player.Interact.GetPreviousValue();
			var wantsToInteractNow = wantsToInteract;

			if (raycastData != null && raycastData.IsInteractive)
			{
				if (!wantedToInteractPreviously && wantsToInteractNow)
				{
					raycastData.InteractiveObject.OnInteractionStart(Player);
					m_InteractedObject = raycastData.InteractiveObject;
				}
			}

			if (m_InteractedObject != null && wantedToInteractPreviously && !wantsToInteractNow)
			{
				m_InteractedObject.OnInteractionEnd(Player);
				m_InteractedObject = null;
			}
		}

		private void UpdateInteraction() 
		{
			var lastRaycastData = Player.RaycastInfo.Get();

			m_SmallestAngle = 1000f;
			m_ClosestObject = null;
			m_ClosestObjectIndex = -1;

			Vector3 cameraPosition = m_WorldCamera.transform.position;
			Vector3 cameraDirection = m_WorldCamera.transform.forward;

			if (Physics.Raycast(cameraPosition, cameraDirection, out RaycastHit hit, m_InteractionDistance, m_LayerMask, QueryTriggerInteraction.Collide))
			{
				if(hit.collider.TryGetComponent(out InteractiveObject interactiveObject))
				{
					m_ClosestObject = interactiveObject;
					m_SmallestAngle = 0f;
					m_ClosestObjectIndex = 0;

					m_CollidersInRange = new Collider[1];
					m_CollidersInRange[0] = hit.collider;
				}
			}

			//If the ray sent directly from the camera doesn't catch anything, then try to search for interactive objects based on angles.
			if (m_ClosestObject == null)
			{
				//Gets all of the Physics
				m_CollidersInRange = Physics.OverlapSphere(m_WorldCamera.transform.position, m_InteractionDistance, m_LayerMask, QueryTriggerInteraction.Collide);

				//Checks for any the closest interactive object to the Player angle wise.
				for (int i = 0; i < m_CollidersInRange.Length; i++)
				{
					if (m_CollidersInRange[i].TryGetComponent(out InteractiveObject interactiveObject))
					{
						if (Physics.Linecast(cameraPosition, interactiveObject.transform.position + (interactiveObject.transform.position - cameraPosition).normalized * 0.05f, out RaycastHit hitInfo, m_LayerMask))
						{
							if (hitInfo.collider == null || hitInfo.collider == m_CollidersInRange[i])
							{
								float angle = Vector3.Angle(cameraDirection, interactiveObject.transform.position - cameraPosition);

								if (angle < m_SmallestAngle)
								{
									m_SmallestAngle = angle;
									m_ClosestObject = interactiveObject;
									m_ClosestObjectIndex = i;
								}
							}
						}
					}
				}
			}

			if (m_SmallestAngle < m_MaxInteractionAngle && ((lastRaycastData != null && lastRaycastData.Collider != m_CollidersInRange[m_ClosestObjectIndex]) || lastRaycastData == null))
			{
				var raycastData = new RaycastInfo(m_CollidersInRange[m_ClosestObjectIndex], m_ClosestObject);
				Player.RaycastInfo.Set(raycastData);

				//Notify the object the ray is on it.
				if (raycastData != null && raycastData.IsInteractive)
					raycastData.InteractiveObject.OnRaycastStart(Player);

				//Notify the previous object the ray is not on it anymore.
				if (lastRaycastData != null && lastRaycastData.InteractiveObject != null)
					lastRaycastData.InteractiveObject.OnRaycastEnd(Player);
			}
			else if (m_SmallestAngle > m_MaxInteractionAngle)
			{
				Player.RaycastInfo.Set(null);

				// Let the object know the ray it's not on it anymore.
				if (lastRaycastData != null && lastRaycastData.IsInteractive)
				{
					if (lastRaycastData.IsInteractive)
						lastRaycastData.InteractiveObject.OnRaycastEnd(Player);
				}
			}

			if (m_InteractedObject != null)
				m_InteractedObject.OnInteractionUpdate(Player);
		}
	}
}