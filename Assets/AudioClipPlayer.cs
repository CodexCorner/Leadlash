using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour {

	public bool inPool;
	public static List<AudioClipPlayer> pool = new List<AudioClipPlayer>();
	public static int[] steps = {2, 4, 5, 7, 9, 11, 12};
	private AudioSource audioSource;

	public static int GetRandomStep () {
		return steps[Random.Range(0,7)];
	}

	public static void Play (AudioClip clip, float step, float volume, GameObject fallback) {

		float pitch = Mathf.Pow(1.05946f,step);

		if (pool.Count > 0) {
			pool[pool.Count - 1].Initialize(clip,pitch,volume);
		} else {
			((GameObject)Instantiate(fallback)).GetComponent<AudioClipPlayer>().Initialize(clip,pitch,volume);
		}

	}

	public void Initialize (AudioClip clip, float pitch, float volume) {

		inPool = false;
		pool.Remove(this);

		if (audioSource == null) audioSource = GetComponent<AudioSource>();
		audioSource.pitch = pitch;
		audioSource.volume = volume;
		audioSource.clip = clip;
		audioSource.Play();

	}

	void Update () {

		if (!inPool) {
			if (!audioSource.isPlaying) {
				inPool = true;
				pool.Add(this);
			}
		}

	}

}
