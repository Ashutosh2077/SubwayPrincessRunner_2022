using System;
using System.Collections;
using UnityEngine;

public class IngameLoop : MonoBehaviour, SoundLoop
{
	private void Awake()
	{
		this.backgroundMusic = base.gameObject.AddComponent<AudioSource>();
		base.gameObject.AddComponent<AudioLowPassFilter>();
		this.backgroundMusic.clip = this.bgclip;
		this.musicVolume = this.ingameMusicVolume;
		this.UpdateBackgroundMusic();
		this.backgroundMusic.loop = true;
		this.backgroundMusic.playOnAwake = true;
		this.backgroundMusic.spatialBlend = 0f;
		this.SetGameSoundVolume();
		this.backgroundMusic.bypassEffects = true;
		this.backgroundMusic.Play();
		this.infading = false;
	}

	public void Play()
	{
		if (!this.hasPlayedIntro)
		{
			this.hasPlayedIntro = true;
		}
		else
		{
			this.backgroundMusic.timeSamples = 0;
		}
		this.backgroundMusic.bypassEffects = true;
		this.musicVolume = this.ingameMusicVolume;
		this.UpdateBackgroundMusic();
		this.ResetAudioClip();
	}

	public void Stop()
	{
		if (this.hasPlayedIntro)
		{
			this.backgroundMusic.bypassEffects = false;
			this.musicVolume = this.menuMusicVolume;
			this.UpdateBackgroundMusic();
		}
	}

	public IEnumerator MusicFader(float fadeupTime, float pauseTime)
	{
		float factor = 0f;
		float start = this.musicVolume;
		this.infading = true;
		while (factor < 1f)
		{
			this.musicVolume = Mathf.SmoothStep(start, 0f, factor);
			this.UpdateBackgroundMusic();
			factor += Time.deltaTime / this.fadeDownTime;
			yield return null;
		}
		this.musicVolume = 0f;
		this.UpdateBackgroundMusic();
		this.infading = false;
		yield return new WaitForSeconds(pauseTime);
		this.setNextClip();
		factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			this.musicVolume = Mathf.SmoothStep(0f, this.ingameMusicVolume, factor);
			this.UpdateBackgroundMusic();
			yield return null;
		}
		this.musicVolume = this.ingameMusicVolume;
		this.UpdateBackgroundMusic();
		yield break;
	}

	public IEnumerator MusicFader(float fadeupTime)
	{
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			this.musicVolume = Mathf.Lerp(this.musicVolume, this.ingameMusicVolume, factor);
			yield return null;
		}
		this.musicVolume = this.ingameMusicVolume;
		this.UpdateBackgroundMusic();
		yield break;
	}

	private void SetGameSoundVolume()
	{
		if (Settings.optionSound)
		{
			AudioListener.volume = 1f;
		}
		else
		{
			AudioListener.volume = 0f;
		}
	}

	public void ChangeAudioClip(AudioClip clip)
	{
		if (this.backgroundMusic.clip != clip)
		{
			if (this.infading)
			{
				this.nextClip = clip;
			}
			else
			{
				this.nextClip = null;
				this.backgroundMusic.clip = clip;
				this.backgroundMusic.Play();
			}
		}
	}

	public void ResetAudioClip()
	{
		this.ChangeAudioClip(this.bgclip);
	}

	public void setNextClip()
	{
		if (this.nextClip != null && this.backgroundMusic.clip != this.nextClip)
		{
			this.backgroundMusic.clip = this.nextClip;
			this.backgroundMusic.Play();
			this.nextClip = null;
		}
	}

	private void UpdateBackgroundMusic()
	{
		this.backgroundMusic.volume = this.musicVolume;
	}

	private AudioSource backgroundMusic;

	public AudioClip bgclip;

	public float menuMusicVolume = 0.3f;

	public float ingameMusicVolume = 0.3f;

	public float fadeDownTime = 0.5f;

	private bool hasPlayedIntro;

	private bool infading;

	private AudioClip nextClip;

	private float musicVolume;
}
