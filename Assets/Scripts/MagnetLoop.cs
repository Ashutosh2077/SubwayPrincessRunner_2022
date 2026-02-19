using System;
using UnityEngine;

public class MagnetLoop : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.magnetSource = base.gameObject.AddComponent<AudioSource>();
		this.magnetSource.clip = this.magnetLoop;
		this.magnetSource.volume = this.magnetVolume;
		this.magnetSource.loop = true;
		this.magnetSource.playOnAwake = false;
		this.magnetSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		this.magnetSource.pitch = UnityEngine.Random.Range(this.magnetMinPitch, this.magnetMaxPitch);
		this.magnetSource.Play();
	}

	public void Stop()
	{
		if (this.magnetSource.isPlaying)
		{
			this.magnetSource.Stop();
		}
	}

	public AudioClip magnetLoop;

	public float magnetVolume = 0.4f;

	public float magnetMinPitch = 0.8f;

	public float magnetMaxPitch = 1.1f;

	private AudioSource magnetSource;
}
