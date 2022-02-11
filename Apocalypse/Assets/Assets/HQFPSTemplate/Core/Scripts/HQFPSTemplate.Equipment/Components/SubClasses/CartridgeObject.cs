using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    public class CartridgeObject : MonoBehaviour
    {
        [SerializeField]
        private string m_FOVProperty = "_FOV";

        [Space]

        [SerializeField]
        private MeshRenderer m_ObjectToDisable = null;

        [SerializeField]
        private MeshRenderer m_ObjectToEnable = null;


        public void ChangeState(bool enable)
        {
            if (m_ObjectToDisable)
                m_ObjectToDisable.enabled = enable;

            if(m_ObjectToEnable)
                m_ObjectToEnable.enabled = !enable;
        }

        public void SetFOV(float fov) 
        {
            if(m_ObjectToDisable)
                m_ObjectToDisable.sharedMaterial.SetFloat(m_FOVProperty, fov);

            if (m_ObjectToEnable)
                m_ObjectToEnable.sharedMaterial.SetFloat(m_FOVProperty, fov);
        }
    }
}
