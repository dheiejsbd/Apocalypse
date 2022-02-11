using UnityEngine;
using UnityEngine.UI;

namespace HQFPSTemplate.Examples
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField]
        [Range(30,1000)]
        private float m_RequiredFPS;

        [SerializeField]
        private Gradient m_ColorGradient;

        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} Avg FPS";
        private Text m_Text;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            m_Text = GetComponent<Text>();
        }

        private void Update()
        {
            // Measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                m_Text.text = string.Format(display, m_CurrentFps);

                m_Text.color = m_ColorGradient.Evaluate(m_CurrentFps / m_RequiredFPS);
            }
        }
    }
}
