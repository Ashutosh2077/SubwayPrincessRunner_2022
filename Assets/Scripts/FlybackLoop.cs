using System;
using UnityEngine;

public class FlybackLoop : MonoBehaviour, SoundLoop
{
	public void Awake()
	{
		this.flypackSource = base.gameObject.AddComponent<AudioSource>();
		this.flypackSource.clip = this.flypackLoop;
		this.flypackSource.volume = this.flypackVolume;
		this.flypackSource.loop = true;
		this.flypackSource.playOnAwake = false;
		this.flypackSource.spatialBlend = 0f;
	}

	public void Play()
	{
		this.flypackSource.Play();
		SoundManager.Instance.ingame.ingameMusicVolume *= 0.5f;
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.fadeUpTime, this.pauseTime));
	}

	public void Stop()
	{
		if (this.flypackSource.isPlaying)
		{
			this.flypackSource.Stop();
		}
		SoundManager.Instance.ingame.ingameMusicVolume *= 2f;
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.fadeUpTime));
	}

	public AudioClip flypackLoop;

	public float flypackVolume = 0.4f;

	public float pauseTime = 2f;

	public float fadeUpTime = 3f;

	private AudioSource flypackSource;
}
