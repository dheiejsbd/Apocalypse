using System;
using UnityEngine;

namespace HQFPSTemplate
{
    [Serializable]
    public class StaminaSettings
    {
        [Range(0.01f, 100f)]
        public float InitialValue = 100f;

        [Range(0.01f, 20f)]
        public float DepletionSpeed = 12f;

        [Range(0.01f, 100f)]
        public float JumpStaminaTake = 10f;

        [Range(0.5f, 10f)]
        public float RegenPause = 3f;

        [Range(1f, 100f)]
        public float RegenSpeed = 25f;
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlayerVitals : EntityVitals
    {
        [BHeader("Stamina", true)]

        [SerializeField]
        [Group]
        private StaminaSettings m_StaminaStat;

        private float m_NextAllowedStaminaRegen;

        private Player m_Player;


        protected override void Update()
        {
            base.Update();

            UpdateStats();
        }

        protected override void Awake()
        {
            base.Awake();

            m_Player = GetComponentInParent<Player>();

            m_Player.Stamina.Set(m_StaminaStat.InitialValue);
            m_Player.Stamina.AddChangeListener(On_StaminaChange);

            m_Player.Jump.AddStartListener(On_PlayerJump);
        }

        private void UpdateStats()
        {
            // Stamina
            if (m_Player.Run.Active)
            {
                float staminaDepletion = m_StaminaStat.DepletionSpeed * Time.deltaTime;
                float newStaminaValue = Mathf.Clamp(m_Player.Stamina.Get() - staminaDepletion, 0f, 100f);

                m_Player.Stamina.Set(newStaminaValue);
            }
            else if (Time.time > m_NextAllowedStaminaRegen)
            {
                float staminaIncrease = m_StaminaStat.RegenSpeed * Time.deltaTime;
                float newStaminaValue = Mathf.Clamp(m_Player.Stamina.Get() + staminaIncrease, 0f, 100f);

                m_Player.Stamina.Set(newStaminaValue);
            }
        }

        private void On_StaminaChange(float change)
        {
            if(change < m_Player.Stamina.GetPreviousValue())
                m_NextAllowedStaminaRegen = Time.time + m_StaminaStat.RegenPause;
        }

        private void On_PlayerJump() 
        {
            float newStaminaValue = Mathf.Clamp(m_Player.Stamina.Get() - m_StaminaStat.JumpStaminaTake, 0f, 100f);

            m_Player.Stamina.Set(newStaminaValue);
        }
    }
}