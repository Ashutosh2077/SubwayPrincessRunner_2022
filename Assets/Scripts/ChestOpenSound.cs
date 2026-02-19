using System;
using UnityEngine;

public class ChestOpenSound : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.chestSource = base.gameObject.AddComponent<AudioSource>();
		this.chestSource.clip = this.chestOpenSound;
		this.chestSource.volume = this.chestVolume;
		this.chestSource.playOnAwake = false;
		this.chestSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.chestSource.Play();
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.fadeUpTime, this.pauseTime));
	}

	public void Stop()
	{
		if (this.chestSource.isPlaying)
		{
			this.chestSource.Stop();
		}
	}

	public AudioClip chestOpenSound;

	public float chestVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource chestSource;
}
