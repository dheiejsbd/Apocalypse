using System.Collections;
using UnityEngine;

namespace HQFPSTemplate.UserInterface
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_PlayerDeath : UserInterfaceBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup = null;

        [SerializeField]
        private float m_FadeSpeed = 1f;

        private Coroutine m_CanvasFader;


        private void Start()
        {
            Player.Death.AddListener(OnPlayerDeath);
            Player.Respawn.AddListener(OnPlayerRespawn);
        }

        private void OnDestroy()
        {
            Player.Death.RemoveListener(OnPlayerDeath);
            Player.Respawn.RemoveListener(OnPlayerRespawn);
        }

        private void OnPlayerDeath() 
        {
            if (m_CanvasFader != null)
                StopCoroutine(m_CanvasFader);

            m_CanvasFader = StartCoroutine(FadeCanvasAlpha(0f));
        }

        private void OnPlayerRespawn()
        {
            if (m_CanvasFader != null)
                StopCoroutine(m_CanvasFader);

            m_CanvasFader = StartCoroutine(FadeCanvasAlpha(1f));
        }

        private IEnumerator FadeCanvasAlpha(float targetAlpha) 
        {
            float currentAlpha = m_CanvasGroup.alpha;

            while (Mathf.Abs(currentAlpha - targetAlpha) > 0.001f)
            {
                currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, m_FadeSpeed * Time.deltaTime);

                m_CanvasGroup.alpha = currentAlpha;

                yield return null;
            }
        }
    }
}
