using System;
using UnityEngine;

public class UnlockSound : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.unlockSource = base.gameObject.AddComponent<AudioSource>();
		this.unlockSource.clip = this.unlockSound;
		this.unlockSource.volume = this.unlockSoundVolume;
		this.unlockSource.playOnAwake = false;
		this.unlockSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.unlockSource.Play();
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.fadeUpTime, this.pauseTime));
	}

	public void Stop()
	{
		if (this.unlockSource.isPlaying)
		{
			this.unlockSource.Stop();
		}
	}

	public AudioClip unlockSound;

	public float unlockSoundVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource unlockSource;
}
