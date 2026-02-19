using System;
using UnityEngine;

public class StartSound : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.startSource = base.gameObject.AddComponent<AudioSource>();
		this.startSource.clip = this.startSound;
		this.startSource.volume = this.startVolume;
		this.startSource.playOnAwake = false;
		this.startSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.startSource.Play();
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.startFadeUpTime, this.startPuseTime));
	}

	public void Stop()
	{
		if (this.startSource.isPlaying)
		{
			this.startSource.Stop();
		}
	}

	public AudioClip startSound;

	public float startVolume = 1f;

	public float startPuseTime = 1f;

	public float startFadeUpTime = 2f;

	private AudioSource startSource;
}
