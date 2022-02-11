using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HQFPSTemplate.UserInterface
{
    public class UI_PlayerVitals : UserInterfaceBehaviour
	{
        #region Internal
        [Serializable]
		private class PlayerStatHUD
		{
			[SerializeField]
			private Image m_StatBar, m_StatBarBG;

			[SerializeField]
			private Gradient m_StatColorOverTime = null;

			[Space]
			
			[SerializeField]
			private bool m_FastChangeBarEnabled;

			[SerializeField]
			[ShowIf("m_FastChangeBarEnabled", true)]
			[Range(0.01f, 100f)]
			private float m_FastChangeThreshold = 15f;

			[SerializeField]
			[ShowIf("m_FastChangeBarEnabled", true)]
			private float m_FastChangeBarStay = 1f;

			[SerializeField]
			[ShowIf("m_FastChangeBarEnabled", true)]
			[Range(0.01f, 1f)]
			private float m_FastChangeBarSpeed = 0.1f;

			[SerializeField]
			[ShowIf("m_FastChangeBarEnabled", true)]
			private Image m_FastChangeBar = null;

			[Space]

			[SerializeField]
			private bool m_AnimateStatBarBG = false;

			[SerializeField]
			[ShowIf("m_AnimateStatBarBG", true)]
			[Range(0.01f, 1f)]
			private float m_StatBGAnimSpeed = 0.1f;

			[SerializeField]
			[ShowIf("m_AnimateStatBarBG", true)]
			private Gradient m_StatBGColorOverTime = null;

			private Value<float> m_AttachedStatValue;

			private float m_CurrentAnimStatus = 0f;

			private float m_NextTimeRestoreFastChangeBar;
			private bool m_FastChangeBarActive;


			public void UpdateHUD()
			{
				//Animate the stat background if the attached stat value is lower than the max value
				if (m_AttachedStatValue.Val < 100) // Replace '100' with the stat max amount
				{
					if (m_AnimateStatBarBG)
					{
						m_CurrentAnimStatus = Mathf.MoveTowards(m_CurrentAnimStatus, 1f, m_StatBGAnimSpeed / 100);

						if (Mathf.Abs(1 - m_CurrentAnimStatus) < 0.01f)
							m_CurrentAnimStatus = 0f;

						m_StatBarBG.color = m_StatBGColorOverTime.Evaluate(m_CurrentAnimStatus);
					}

					if (m_FastChangeBarActive && m_NextTimeRestoreFastChangeBar < Time.time)
					{
						m_FastChangeBar.fillAmount = Mathf.MoveTowards(m_FastChangeBar.fillAmount, 0f, m_FastChangeBarSpeed / 100);

						if (Mathf.Abs(m_FastChangeBar.fillAmount - m_StatBar.fillAmount) < Mathf.Epsilon)
							m_FastChangeBarActive = false;
					}
				}
			}

			public bool TryInitialize(Value<float> eventValue) 
			{
				if (m_StatBar == null)
				{
					Debug.LogWarning("Player Vitals GUI doesn't have all of the stat images assigned");

					return false;
				}

				m_AttachedStatValue = eventValue;
				m_AttachedStatValue.AddChangeListener(OnValueChange);
				OnValueChange(eventValue.Val);

				return true;
			}

			private void OnValueChange(float statValue) 
			{
				float normalizedStatValue = statValue / 100f;
				float prevNormalizedStatValue = m_AttachedStatValue.GetPreviousValue() / 100f;

				m_StatBar.fillAmount = normalizedStatValue;
				m_StatBar.color = m_StatColorOverTime.Evaluate(normalizedStatValue);

				if (m_FastChangeBarEnabled &&
					m_FastChangeBar != null)
				{
					if (prevNormalizedStatValue - normalizedStatValue > m_FastChangeThreshold / 100f)
					{
						m_FastChangeBar.fillAmount = prevNormalizedStatValue;

						m_NextTimeRestoreFastChangeBar = Time.time + m_FastChangeBarStay;
						m_FastChangeBarActive = true;
					}
				}
			}
		}
        #endregion

        [SerializeField]
		[Group]
		private PlayerStatHUD m_HealthHUD = new PlayerStatHUD();

		[SerializeField]
		[Group]
		private PlayerStatHUD m_StaminaHUD = new PlayerStatHUD();

		private List<PlayerStatHUD> m_PlayerVitalSettings = new List<PlayerStatHUD>();


		public override void OnPostAttachment()
		{
			if(m_HealthHUD.TryInitialize(Player.Health))
				m_PlayerVitalSettings.Add(m_HealthHUD);

			if (m_StaminaHUD.TryInitialize(Player.Stamina))
				m_PlayerVitalSettings.Add(m_StaminaHUD);

			//Here you can add custom states
			//e.g. if (m_HungerHUD.TryInitialize(Player.Hunger))
			//e.g.  	m_PlayerVitalSettings.add(m_HungerHUD);
		}

		private void Update() 
		{
            foreach (PlayerStatHUD playerStatHUD in m_PlayerVitalSettings)
				playerStatHUD.UpdateHUD();
		}
	}
}
