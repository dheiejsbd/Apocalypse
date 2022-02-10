using UnityEngine;

namespace HQFPSTemplate
{
    public class SimpleMinMaxAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public SimpleMinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}