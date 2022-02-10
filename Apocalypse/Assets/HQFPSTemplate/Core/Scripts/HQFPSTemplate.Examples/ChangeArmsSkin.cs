using UnityEngine;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate.Examples
{
    [RequireComponent(typeof(FPArmsHandler))]
    public class ChangeArmsSkin : MonoBehaviour
    {
        [BHeader("Demo", true)]

        [SerializeField]
        private bool m_EnableChangeArms = false;

        [SerializeField]
        [ShowIf("m_EnableChangeArms", true)]
        private KeyCode m_ChangeArmsKey = KeyCode.P;

        private int m_SelectedArmsIndex = -1;
        private FPArmsHandler m_FPArmsHandler;


        private void Start()
        {
            m_FPArmsHandler = GetComponent<FPArmsHandler>();
            m_FPArmsHandler.UpdateArms(ref m_SelectedArmsIndex);
        }

        private void Update()
        {
            //DEMO: Change Arms
            if (!m_EnableChangeArms)
                return;

            if (Input.GetKeyDown(m_ChangeArmsKey))
                m_FPArmsHandler.UpdateArms(ref m_SelectedArmsIndex);
        }
    }
}
