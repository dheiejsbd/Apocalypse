using UnityEngine;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate.UserInterface
{
	public class UI_CrosshairManager : UserInterfaceBehaviour
	{
		[BHeader("GENERAL", true)]

		[SerializeField]
		private UI_Crosshair m_Crosshair;

		[SerializeField]
		private UI_CrosshairInfo[] m_CrosshairsInfo;

		[Space]

		[SerializeField]
		private UI_Crosshair m_Hitmarker;

		[SerializeField]
		private UI_CrosshairInfo m_HitmarkerInfo;

		private UI_CrosshairInfo m_CurrentCrosshairInfo;


		public override void OnPostAttachment()
		{
			if (!gameObject.activeSelf)
				return;

			m_Hitmarker.EnableCrosshair(false);

			Player.UseItem.AddListener(ApplyUsePunch);

            Player.Aim.AddStartListener(() => { if (!m_CurrentCrosshairInfo.GraphicsInfo.ShowWhenAiming) EnableCanvas(false); });
            Player.Aim.AddStopListener(() => { EnableCanvas(true); });

			Player.Pause.AddStartListener(() => EnableCanvas(false));
			Player.Pause.AddStopListener(() => EnableCanvas(true));

			Player.ActiveEquipmentItem.AddChangeListener(OnChanged_HeldItem);
		}

		private void FixedUpdate()
		{
			if (m_CurrentCrosshairInfo != null)
			{
				Vector2 stateScale = GetStateScale();

				m_Crosshair.AddSpringForce(stateScale);

				if (m_Hitmarker != null && m_Hitmarker)
					m_Hitmarker.AddSpringForce(stateScale);

				UpdateCrosshairColor(Player.RaycastInfo.Get());
			}
		}

		private Vector2 GetStateScale()
		{
			Vector2 stateScale = Vector2.one;

			if (Player.Run.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.RunScale;
			else if (Player.Crouch.Active)
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.CrouchScale;
			else if (Player.Prone.Active)
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.ProneScale;
			else if (Player.Walk.Active && Player.Velocity.Val.sqrMagnitude > 0.2f)
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.WalkScale;
			else
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.IdleScale;

			if (!Player.IsGrounded.Get())
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.AirborneMultiplier;

			if (Player.Aim.Active)
				stateScale *= m_CurrentCrosshairInfo.ScaleInfo.AimScaleMultiplier;

			return stateScale * m_CurrentCrosshairInfo.ScaleInfo.ScaleMultiplier;
		}

		private void UpdateCrosshairColor(RaycastInfo raycastInfo)
		{
			Color stateColor;
			var equipmentItem = Player.ActiveEquipmentItem.Get();

			if (equipmentItem != null)
			{
				if (!equipmentItem.CanBeUsed())
					stateColor = m_CurrentCrosshairInfo.GraphicsInfo.UnusableColor;
				else
				{
					if (raycastInfo != null && raycastInfo.Collider != null && raycastInfo.Collider.GetComponent<Hitbox>())
						stateColor = m_CurrentCrosshairInfo.GraphicsInfo.OnEntityColor;
					else
						stateColor = m_CurrentCrosshairInfo.GraphicsInfo.NormalColor;
				}

				m_Crosshair.ChangeColor(stateColor);
			}
		}

		private void ApplyUsePunch(bool continuosly, int useType) 
		{
			m_Crosshair.AddSpringForce(m_CurrentCrosshairInfo.ScaleInfo.ItemUseScaleForce);
		}

		private void EnableCanvas(bool enable) 
		{
			m_Crosshair.EnableCrosshair(enable);
		}

		private void OnChanged_HeldItem(EquipmentItem eItem)
		{
			int crosshairID = Mathf.Min(eItem.EInfo.General.CrosshairID, m_CrosshairsInfo.Length - 1);

			//Update the current crosshair type if the item's id is different than the previous one
			if (crosshairID != m_Crosshair.CrosshairID)
			{
				m_CurrentCrosshairInfo = m_CrosshairsInfo[crosshairID];
				m_Crosshair.UpdateInfo(m_CurrentCrosshairInfo, crosshairID);
			}
		}
	}
}
