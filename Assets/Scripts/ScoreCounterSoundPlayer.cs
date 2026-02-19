using System;
using System.Collections;
using UnityEngine;

public class ScoreCounterSoundPlayer : MonoBehaviour
{
	private void Awake()
	{
		this.scoreSource = base.gameObject.AddComponent<AudioSource>();
		this.scoreSource.volume = this.volume;
		this.scoreSource.clip = this.scoreSound;
		this.scoreSource.playOnAwake = false;
		this.scoreSource.spatialBlend = 0f;
	}

	public void PlayCoinSound(float countFactor)
	{
		this.scoreSource.clip = this.scoreSound;
		this.count = countFactor;
		if (!this.playScore)
		{
			this.playScore = true;
			base.StartCoroutine(this.ScoreSoundTimer());
		}
	}

	public void PlayVipSound()
	{
		this.scoreSource.clip = this.vipSound;
		this.scoreSource.Play();
	}

	public void StopVipSound()
	{
		this.scoreSource.Stop();
	}

	private IEnumerator ScoreSoundTimer()
	{
		while (this.playScore)
		{
			this.scoreSource.pitch = Mathf.Pow(2f, this.count);
			this.scoreSource.Play();
			yield return new WaitForSeconds(this.stepDelay);
		}
		this.scoreSource.pitch = 2f;
		this.scoreSource.Play();
		yield break;
	}

	public void StopScoreSound()
	{
		this.playScore = false;
	}

	public AudioClip scoreSound;

	public AudioClip vipSound;

	public float volume = 0.3f;

	public float stepDelay = 0.0625f;

	private float count;

	private bool playScore;

	private AudioSource scoreSource;
}
