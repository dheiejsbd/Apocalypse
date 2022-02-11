using UnityEngine;

namespace HQFPSTemplate
{
    public class AssetSingleton<T> : ScriptableObject where T : Object
    {
        public static T Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    var allFiles = Resources.LoadAll<T>("");
                    if(allFiles != null && allFiles.Length > 0)
                        m_Instance = allFiles[0];
                }

                return m_Instance;
            }
        }

        private static T m_Instance;
    }
}