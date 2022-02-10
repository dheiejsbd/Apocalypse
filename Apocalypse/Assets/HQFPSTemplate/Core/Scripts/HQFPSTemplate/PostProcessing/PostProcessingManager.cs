using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace HQFPSTemplate
{
    public class PostProcessingManager : Singleton<PostProcessingManager>
    {
        [SerializeField]
        private PostProcessVolume m_MainPPVolume = null;

        [SerializeField]
        private PostProcessVolume m_WorldPPVolume = null;

        [BHeader("DepthOfField", true)]

        [SerializeField]
        private bool m_EnableAimDOF = true;

        [SerializeField]
        private bool m_EnableItemWheelDOF = true;

        [BHeader("DeathAnim", true)]

        [SerializeField]
        [Range(0.01f, 15f)]
        private float m_DeathAnimSpeed = 1f;

        [SerializeField]
        [Range(-1,0)]
        private float m_MinColorSaturation = -1f;

        private PostProcessProfile m_EditorMainProfile;
        private PostProcessProfile m_EditorWorldProfile;

        private PostProcessProfile m_MainProfile;
        private PostProcessProfile m_WorldProfile;

        private float m_DefaultSaturation;
        private bool m_PlayerDead;

        private ColorGrading m_ColorGrading;

        private DepthOfField m_MainDepthOfField;
        private DepthOfField m_WorldDepthOfField;

        private Player m_Player;
        private UserInterface.UIManager m_UIManager;


        private void EnableDOF(DepthOfField dofObject, bool enable)
        {
            if (dofObject.active == enable)
                return;

            dofObject.active = enable;
        }

        private void DoDeathAnim() 
        {
            m_PlayerDead = true;

            StartCoroutine(C_DoDeathAnim());
        }

        private void RestoreDefaultProfile() 
        {
            m_PlayerDead = false;
        }

        private void Start()
        {
            m_EditorMainProfile = m_MainPPVolume.profile;
            m_MainProfile = Instantiate(m_EditorMainProfile);
            m_MainPPVolume.profile = m_MainProfile;

            m_EditorWorldProfile = m_WorldPPVolume.profile;
            m_WorldProfile = Instantiate(m_EditorWorldProfile);
            m_WorldPPVolume.profile = m_WorldProfile;

            m_ColorGrading = m_MainPPVolume.profile.GetSetting<ColorGrading>();
            m_DefaultSaturation = m_ColorGrading.saturation;

            m_MainDepthOfField = m_MainProfile.GetSetting<DepthOfField>();
            m_WorldDepthOfField = m_WorldProfile.GetSetting<DepthOfField>();

            m_Player = GameManager.Instance.CurrentPlayer;
            m_UIManager = GameManager.Instance.CurrentInterface;

            if (m_Player != null)
            {
                m_Player.Death.AddListener(DoDeathAnim);
                m_Player.Respawn.AddListener(RestoreDefaultProfile);

                if (m_EnableAimDOF)
                {
                    m_Player.Aim.AddStartListener(() => EnableDOF(m_MainDepthOfField, GameManager.Instance.CurrentPlayer.ActiveEquipmentItem.Get().EInfo.Aiming.UseAimBlur));
                    m_Player.Aim.AddStopListener(() => EnableDOF(m_MainDepthOfField, false));
                }
            }

            if (m_UIManager != null)
            {
                if (m_EnableItemWheelDOF)
                {
                    m_UIManager.ItemWheel.AddStartListener(() => EnableDOF(m_WorldDepthOfField, true));
                    m_UIManager.ItemWheel.AddStopListener(() => EnableDOF(m_WorldDepthOfField, false));
                }
            }
        }

        private void OnDestroy()
        {
            m_MainPPVolume.profile = m_EditorMainProfile;
            m_WorldPPVolume.profile = m_EditorWorldProfile;
        }

        private IEnumerator C_DoDeathAnim() 
        {
            float saturation = m_ColorGrading.saturation.value;
            float requiredSaturation = m_MinColorSaturation * 100;

            while (m_PlayerDead) 
            {
                saturation = Mathf.Lerp(saturation, requiredSaturation, Time.deltaTime * m_DeathAnimSpeed);

                m_ColorGrading.saturation.value = saturation;

                yield return null;
            }

            if(!m_PlayerDead)
                m_ColorGrading.saturation.value = m_DefaultSaturation;
        }
    }
}