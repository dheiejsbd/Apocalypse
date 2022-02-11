using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFPSTemplate
{
	public class AudioUtils : Singleton<AudioUtils> 
	{
		private readonly Dictionary<AudioSource, Coroutine> m_LevelSetters = new Dictionary<AudioSource, Coroutine>();

		[SerializeField]
		private AudioSource m_2DAudioSource = null;


		/// <summary>
		/// Play a given Audio Clip one time
		/// </summary>
		public void Play2D(AudioClip clip, float volume)
		{
			if (m_2DAudioSource)
				m_2DAudioSource.PlayOneShot(clip, volume);
		}

		/// <summary>
		/// Create an Audio Source with the given values
		/// </summary>
		public static AudioSource CreateAudioSource(string name, Transform parent, Vector3 localPosition, bool is2D = false, float startVolume = 1f, float minDistance = 1f) 
		{
			var audioObject = new GameObject(name, typeof(AudioSource));
			
			audioObject.transform.parent = parent;
			audioObject.transform.localPosition = localPosition;

			var audioSource = audioObject.GetComponent<AudioSource>();
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			return audioSource;
		}
			
		public void LerpVolumeOverTime(AudioSource audioSource, float targetVolume, float speed) 
		{
			if(m_LevelSetters.ContainsKey(audioSource)) 
			{
				if(m_LevelSetters[audioSource] != null)
					StopCoroutine(m_LevelSetters[audioSource]);
				
				m_LevelSetters[audioSource] = StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed));
			} 
			else 
				m_LevelSetters.Add(audioSource, StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed)));
		}

		private IEnumerator C_LerpVolumeOverTime(AudioSource audioSource, float volume, float speed) 
		{
			while(audioSource != null && Mathf.Abs(audioSource.volume - volume) > 0.01f) 
			{
				audioSource.volume = Mathf.MoveTowards(audioSource.volume, volume, Time.deltaTime * speed);
				yield return null;
			}

			if(audioSource.volume == 0f)
				audioSource.Stop();
			
			m_LevelSetters.Remove(audioSource);
		}
	}
}