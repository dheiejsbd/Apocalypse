using UnityEngine;

namespace HQFPSTemplate
{
	[System.Serializable]
	public class SceneField
	{
		public Object SceneAsset { get { return m_SceneAsset; } }

		[SerializeField]
		private Object m_SceneAsset = null;

		[SerializeField]
		private string m_SceneName = "";

		public string SceneName
		{
			get { return m_SceneName; }
		}

		// makes it work with the existing Unity methods (LoadLevel/LoadScene)
		public static implicit operator string(SceneField sceneField)
		{
			return sceneField.SceneName;
		}
	}
}