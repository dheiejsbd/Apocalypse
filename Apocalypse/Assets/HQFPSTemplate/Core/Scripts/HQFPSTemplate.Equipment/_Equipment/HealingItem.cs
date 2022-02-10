using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.Equipment
{
    public class HealingItem : EquipmentItem
    {
        #region Anim Hashing
        //Hashed animator strings (Improves performance)
        private readonly int animHash_Use = Animator.StringToHash("Use");
        private readonly int animHash_UseSpeed = Animator.StringToHash("Use Speed");
        #endregion

        private HealingItemInfo m_H;
        private bool m_IsHealing;


        public override void Initialize(EquipmentHandler eHandler)
        {
            base.Initialize(eHandler);

            m_H = EInfo as HealingItemInfo;
        }

        public override void Unequip()
        {
            m_GeneralEvents.OnEquipped.Invoke(false);
        }

        public override void Equip(Item item)
        {
            if (m_IsHealing)
                return;

            m_IsHealing = true;

            EAnimation.AssignArmAnimations(EHandler.FPArmsHandler.Animator);

            EHandler.Animator_SetFloat(animHash_UseSpeed, m_H.HealingSettings.HealAnimSpeed);
            EHandler.Animator_SetTrigger(animHash_Use);

            m_GeneralInfo.EquipmentModel.UpdateSkinIDProperty(item);
            m_GeneralInfo.EquipmentModel.UpdateMaterialsFov();

            m_GeneralEvents.OnEquipped.Invoke(true);

            StartCoroutine(C_StartHealing());
        }

        IEnumerator C_StartHealing() 
        {
            float healDelay = m_H.HealingSettings.UpdateHealthDelay * (1 / m_H.HealingSettings.HealAnimSpeed);
            float delay = m_H.HealingSettings.UpdateHealthDelay - healDelay;

            Player.Camera.Physics.PlayDelayedCameraForces(m_H.HealingSettings.HealingCameraForces);
            EHandler.PlayDelayedSounds(m_H.HealingSettings.HealingAudio);

            EHandler.Animator_SetTrigger(animHash_Use);

            yield return new WaitForSeconds(healDelay);

            m_GeneralEvents.OnUse.Invoke();

            float healingAmount = Random.Range(m_H.HealingSettings.HealAmount.x, m_H.HealingSettings.HealAmount.y);
            DamageInfo healEvent = new DamageInfo(healingAmount);

            Player.ChangeHealth.Try(healEvent);

            yield return new WaitForSeconds(delay);

            Player.Healing.ForceStop();

            m_IsHealing = false;
        }
    }
}
