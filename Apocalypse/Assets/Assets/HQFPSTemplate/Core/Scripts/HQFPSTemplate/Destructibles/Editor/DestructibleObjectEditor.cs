using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
	[CustomEditor(typeof(DestructibleObject))]
	public class DestructibleObjectEditor : Editor
	{
		private Transform m_AutoSearchRoot;


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUICustom.Separator();

			m_AutoSearchRoot = (Transform)EditorGUILayout.ObjectField("Search Root", m_AutoSearchRoot, typeof(Transform), true);

			if(GUILayout.Button("Search For Fragments") && m_AutoSearchRoot != null)
			{
				var dynamicParts = new List<DestructibleObject.DebrisFragment>();

				foreach(Transform child in m_AutoSearchRoot)
				{
					var rigidbody = child.GetComponent<Rigidbody>();
					if(rigidbody != null)
						dynamicParts.Add(new DestructibleObject.DebrisFragment(rigidbody));

					serializedObject.Update();
					(serializedObject.targetObject as DestructibleObject).SetDebrisFragments (dynamicParts);
					serializedObject.ApplyModifiedProperties();
				}
			}
		}
	}
}