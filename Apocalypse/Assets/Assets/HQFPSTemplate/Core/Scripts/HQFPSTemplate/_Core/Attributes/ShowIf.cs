using System;
using UnityEngine;

namespace HQFPSTemplate
{
	[AttributeUsage(AttributeTargets.Field)]
    public class ShowIf : PropertyAttribute
    {
        public readonly string m_PropertyName;
        public readonly float m_Indentation;

        public readonly bool m_RequiredBool = false;
		public readonly int m_RequiredInt = -1;


		public ShowIf(string propertyName, bool requiredValue, float indentation = 10f)
        {
            m_PropertyName = propertyName;
            m_RequiredBool = requiredValue;

            m_Indentation = indentation;
        }

		public ShowIf(string propertyName, int requiredValue, float indentation = 10f)
		{
			m_PropertyName = propertyName;
			m_RequiredInt = requiredValue;

			m_Indentation = indentation;
		}
    }
}