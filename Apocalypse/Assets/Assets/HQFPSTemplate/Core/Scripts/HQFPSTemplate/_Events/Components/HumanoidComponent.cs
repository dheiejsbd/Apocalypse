namespace HQFPSTemplate
{
    public class HumanoidComponent : EntityComponent
    {
        public Humanoid Humanoid
        {
            get
            {
                if(m_Humanoid == null)
                    m_Humanoid = GetComponent<Humanoid>();
                if(m_Humanoid == null)
                    m_Humanoid = GetComponentInParent<Humanoid>();

                return m_Humanoid;
            }
        }

        private Humanoid m_Humanoid;
    }
}