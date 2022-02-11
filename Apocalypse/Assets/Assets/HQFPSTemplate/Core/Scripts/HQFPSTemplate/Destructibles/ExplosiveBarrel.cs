using UnityEngine;

namespace HQFPSTemplate
{
    public class ExplosiveBarrel : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float m_Health = 100f;

        [SerializeField]
        private DamageDealerObject m_Explosion = null;

        private bool m_Exploded;


        public void TakeDamage(DamageInfo damageData)
        {
            if (m_Exploded)
                return;

            m_Health += damageData.Delta;

            if (m_Health <= 0)
            {
                m_Exploded = true;

                m_Explosion.ActivateDamage(null);

                Destroy(gameObject);
            }
        }
    }
}
