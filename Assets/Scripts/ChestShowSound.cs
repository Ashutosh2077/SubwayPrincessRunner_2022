using System;
using UnityEngine;

public class ChestShowSound : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.chestRewardSource = base.gameObject.AddComponent<AudioSource>();
		this.chestRewardSource.clip = this.chestRewardShowSound;
		this.chestRewardSource.volume = this.chestRewardVolume;
		this.chestRewardSource.playOnAwake = false;
		this.chestRewardSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.chestRewardSource.Play();
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.fadeUpTime, this.pauseTime));
	}

	public void Stop()
	{
		if (this.chestRewardSource.isPlaying)
		{
			this.chestRewardSource.Stop();
		}
	}

	public AudioClip chestRewardShowSound;

	public float chestRewardVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource chestRewardSource;
}
