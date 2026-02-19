using System;
using UnityEngine;

public class ReviveLoop : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.reviveSource = base.gameObject.AddComponent<AudioSource>();
		this.reviveSource.clip = this.reviveSound;
		this.reviveSource.volume = this.reviveVolume;
		this.reviveSource.playOnAwake = false;
		this.reviveSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.reviveSource.Play();
		base.StartCoroutine(SoundManager.Instance.ingame.MusicFader(this.reviveFadeUpTime, this.revivePauseTime));
	}

	public void Stop()
	{
		if (this.reviveSource.isPlaying)
		{
			this.reviveSource.Stop();
		}
	}

	public AudioClip reviveSound;

	public float reviveVolume = 1f;

	public float revivePauseTime = 1f;

	public float reviveFadeUpTime = 2f;

	private AudioSource reviveSource;
}
