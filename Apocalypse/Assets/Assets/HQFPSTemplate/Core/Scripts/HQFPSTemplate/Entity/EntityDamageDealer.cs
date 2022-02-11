using UnityEngine;

#if INVECTOR_AI_TEMPLATE
using Invector;
#endif

#if EMERALD_AI_PRESENT
using EmeraldAI;
#endif

namespace HQFPSTemplate
{
    [RequireComponent(typeof(Entity))]
    public class EntityDamageDealer : EntityComponent
    {
        private void Start() => Entity.DealDamage.SetTryer(DealDamage);

        protected virtual bool DealDamage(DamageInfo damageInfo, IDamageable damageable = null) 
        {
            #region AI Systems Integration

            #if INVECTOR_AI_TEMPLATE
            if (damageInfo.HitObject.TryGetComponent(out vIDamageReceiver vDamReceiver))
            {
                vDamage vdamage = new vDamage()
                {
                    damageValue = -(int)damageInfo.Delta,
                    force = damageInfo.HitImpulse * damageInfo.HitDirection,
                    hitPosition = damageInfo.HitPoint,
                    sender = damageInfo.Source.transform
                };

                vDamReceiver.TakeDamage(vdamage);

                return true;
            }
            #endif

            #if EMERALD_AI_PRESENT
            if (damageInfo.HitObject.TryGetComponent(out EmeraldAISystem emAI))
            {
                emAI.Damage(-(int)damageInfo.Delta, EmeraldAISystem.TargetType.Player, Entity.transform, (int)damageInfo.HitImpulse);

                return true;
            }
            #endif

            #endregion

            if (damageable != null)
            {
                DealDamage(damageable, damageInfo);

                return true;
            }
            else if (damageInfo.HitObject.TryGetComponent(out IDamageable dmgObject))
            {
                DealDamage(dmgObject, damageInfo);

                return true;
            }

            else return false;
        }

        protected virtual void DealDamage(IDamageable damageable, DamageInfo damageInfo) 
        {
            damageable.TakeDamage(damageInfo);
        }
    }
}
