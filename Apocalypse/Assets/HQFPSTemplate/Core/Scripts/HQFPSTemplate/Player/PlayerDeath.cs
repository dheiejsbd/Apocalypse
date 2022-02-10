using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HQFPSTemplate
{
	public class PlayerDeath : PlayerComponent
	{
		[BHeader("Stuff To Disable On Death", true)]

		[SerializeField]
		private GameObject[] m_ObjectsToDisable = null;

		[SerializeField]
		private Behaviour[] m_BehavioursToDisable = null;

		[SerializeField]
		private Collider[] m_CollidersToDisable = null;

		[BHeader("Player Head Hitbox")]

		[SerializeField]
		private Rigidbody m_Head = null;

		[BHeader("Respawn", true)]

		[SerializeField]
		private bool m_Respawn = true;

		[SerializeField]
		[EnableIf("m_Respawn", true)]
		private float m_RespawnDuration = 5f;

		[SerializeField]
		[EnableIf("m_Respawn", true)]
		private bool m_RestartSceneOnRespawn = false;

		private Transform m_CameraStartParent;
		private Quaternion m_CameraStartRotation;
		private Vector3 m_CameraStartPosition;

		private Vector3 m_HeadStartPosition;
		private Quaternion m_HeadStartRotation;


        public override void OnEntityStart()
        {
			Player.Health.AddChangeListener(OnChanged_Health);

			m_Head.isKinematic = true;
			m_Head.gameObject.SetActive(false);

			// Camera set up
			m_CameraStartRotation = Player.Camera.transform.localRotation;
			m_CameraStartPosition = Player.Camera.transform.localPosition;
			m_CameraStartParent = Player.Camera.transform.parent;

			// Player head set up
			m_HeadStartPosition = m_Head.transform.localPosition;
			m_HeadStartRotation = m_Head.transform.localRotation;
		}

		private void OnChanged_Health(float health)
		{
			if (health == 0f)
				StartCoroutine(C_OnDeath());
		}

		private IEnumerator C_OnDeath()
		{
			Player.DropItem.Try(Player.EquippedItem.Get());

			yield return null;

			foreach (var obj in m_ObjectsToDisable)
			{
				if (obj != null)
					obj.SetActive(false);
				else
					Debug.LogWarning("Check out PlayerDeath for missing references, an object reference was found null!", this);
			}

			foreach (var behaviour in m_BehavioursToDisable)
			{
				if (behaviour != null)
					behaviour.enabled = false;
				else
					Debug.LogWarning("Check out PlayerDeath for missing references, a behaviour reference was found null!", this);
			}

			foreach (var collider in m_CollidersToDisable)
			{
				if (collider != null)
					collider.enabled = false;
				else
					Debug.LogWarning("Check out PlayerDeath for missing references, a collider reference was found null!", this);
			}

			Player.Camera.transform.parent = m_Head.transform;

			m_Head.gameObject.SetActive(true);
			m_Head.isKinematic = false;
			m_Head.AddForce(Vector3.ClampMagnitude(Player.Velocity.Get() * 0.5f, 10f), ForceMode.Force);
			m_Head.AddRelativeTorque(new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 35, ForceMode.Force);

			Player.Death.Send();

			if (m_Respawn)
			{
				yield return new WaitForSeconds(m_RespawnDuration);

				Respawn();
			}
		}

		private void Respawn() 
		{
			if (m_RestartSceneOnRespawn)
				GameManager.Instance.StartGame();
			else
			{
				GameManager.Instance.SetPlayerPosition();

				Player.Camera.transform.parent = m_CameraStartParent;
				Player.Camera.transform.localRotation = m_CameraStartRotation;
				Player.Camera.transform.localPosition = m_CameraStartPosition;

				m_Head.isKinematic = true;
				m_Head.transform.localPosition = m_HeadStartPosition;
				m_Head.transform.localRotation = m_HeadStartRotation;
				m_Head.gameObject.SetActive(false);

				Player.Respawn.Send();

				foreach (var obj in m_ObjectsToDisable)
					obj.SetActive(true);

				foreach (var behaviour in m_BehavioursToDisable)
					behaviour.enabled = true;

				foreach (var collider in m_CollidersToDisable)
					collider.enabled = true;

				if (Player.OnLadder.Active) Player.OnLadder.TryStop();
				if (Player.Run.Active) Player.Run.ForceStop();
				if (Player.Crouch.Active) Player.Crouch.TryStop();
				if (Player.Prone.Active) Player.Prone.TryStop();
				if (Player.Swimming.Active) Player.Swimming.TryStop();

				Player.MoveInput.Set(Vector2Int.zero);
				Player.RaycastInfo.Set(null);
				Player.Health.Set(100f);
				Player.Stamina.Set(100f);
			}
		}
	}
}
