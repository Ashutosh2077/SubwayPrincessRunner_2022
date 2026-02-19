using System;
using UnityEngine;

public class PaticlesHelper : MonoBehaviour
{
	public bool IsPlaying { get; private set; }

	private void Awake()
	{
		this.particles = base.GetComponentsInChildren<ParticleSystem>();
	}

	public void Play()
	{
		this.IsPlaying = true;
		int i = 0;
		int num = this.particles.Length;
		while (i < num)
		{
			this.particles[i].Play();
			i++;
		}
	}

	public void Stop()
	{
		int i = 0;
		int num = this.particles.Length;
		while (i < num)
		{
			this.particles[i].Stop();
			i++;
		}
		this.IsPlaying = false;
	}

	private ParticleSystem[] particles;
}
