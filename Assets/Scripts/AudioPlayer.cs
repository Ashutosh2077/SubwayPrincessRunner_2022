using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	public static AudioPlayer Instance
	{
		get
		{
			return AudioPlayer._instance;
		}
	}

	[ContextMenu("Resetdd")]
	private void Resetss()
	{
		Transform transform = base.transform.Find("Sound");
		if (transform == null)
		{
			transform = new GameObject("Sound").transform;
			transform.parent = base.transform;
			transform.transform.position = Vector3.zero;
			transform.transform.rotation = Quaternion.identity;
			this.soundSrcs = new List<AudioSource>(this.sounds.Count);
			int i = 0;
			int count = this.sounds.Count;
			while (i < count)
			{
				this.soundSrcs.Add(this.Warmup(this.sounds[i], transform));
				i++;
			}
		}
		Transform transform2 = base.transform.Find("Music");
		if (transform2 == null)
		{
			transform2 = new GameObject("Music").transform;
			transform2.parent = base.transform;
			transform2.transform.position = Vector3.zero;
			transform2.transform.rotation = Quaternion.identity;
			this.musicSrcs = new List<AudioSource>(this.musics.Count);
			int j = 0;
			int count2 = this.musics.Count;
			while (j < count2)
			{
				this.musicSrcs.Add(this.Warmup(this.musics[j], transform2));
				j++;
			}
		}
		Transform transform3 = base.transform.Find("Recycle");
		if (transform3 == null)
		{
			transform3 = new GameObject("Recycle").transform;
			transform3.parent = base.transform;
			transform3.transform.position = Vector3.zero;
			transform3.transform.rotation = Quaternion.identity;
		}
		this.audioSoundRecycles = new List<AudioSoundRecycle>();
		this.currentMusicAudioSource = null;
	}

	private void Awake()
	{
		this.Resetss();
		if (AudioPlayer._instance == null)
		{
			AudioPlayer._instance = this;
		}
		this.isFading = false;
		this.WarmUp("leyou_Hr_coin");
		this.WarmUp("leyou_Hr_run_dodge");
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			if (this.currentMusicAudioSource != null && this.currentMusicAudioSource.isPlaying)
			{
				this.audioPlayPaused = true;
				this.currentMusicAudioSourceTime = this.currentMusicAudioSource.time;
				this.currentMusicAudioSource.Stop();
			}
		}
		else if (this.audioPlayPaused)
		{
			this.audioPlayPaused = false;
			this.currentMusicAudioSource.time = this.currentMusicAudioSourceTime;
			this.currentMusicAudioSource.Play();
		}
	}

	private AudioSource Warmup(AudioPlayer.AudioClipInfo info, Transform parent)
	{
		if (info.Clip == null)
		{
			return null;
		}
		AudioSource audioSource = new GameObject(info.Clip.name)
		{
			transform = 
			{
				parent = parent,
				position = Vector3.zero,
				rotation = Quaternion.identity
			}
		}.AddComponent<AudioSource>();
		audioSource.clip = info.Clip;
		audioSource.volume = UnityEngine.Random.Range(info.minVolume, info.maxVolume);
		audioSource.pitch = UnityEngine.Random.Range(info.minPitch, info.maxPitch);
		audioSource.loop = info.loop;
		audioSource.playOnAwake = info.playOnAwake;
		audioSource.rolloffMode = info.Rollof;
		return audioSource;
	}

	public AudioSource GetAudioSource(string name)
	{
		return this.GetAudioSource(name, this.soundSrcs);
	}

	private AudioSource GetAudioSource(string soundName, List<AudioSource> sources)
	{
		AudioSource result = null;
		int i = 0;
		int count = sources.Count;
		while (i < count)
		{
			if (sources[i].clip.name == soundName)
			{
				result = sources[i];
				break;
			}
			i++;
		}
		return result;
	}

	private AudioSoundRecycle GetAudioSourceRecycle(string name)
	{
		AudioSoundRecycle result = null;
		int i = 0;
		int count = this.audioSoundRecycles.Count;
		while (i < count)
		{
			if (this.audioSoundRecycles[i].origGo.name == name)
			{
				result = this.audioSoundRecycles[i];
				break;
			}
			i++;
		}
		return result;
	}

	public AudioSoundRecycle AddAudioSourceRecycle(AudioSource audioSource)
	{
		AudioSoundRecycle audioSoundRecycle = new GameObject(audioSource.name + "_recycle")
		{
			transform = 
			{
				parent = base.transform.Find("Recycle"),
				position = Vector3.zero,
				localEulerAngles = Vector3.zero
			}
		}.AddComponent<AudioSoundRecycle>();
		audioSoundRecycle.origGo = audioSource.gameObject;
		this.audioSoundRecycles.Add(audioSoundRecycle);
		return audioSoundRecycle;
	}

	public AudioSoundRecycle AddAudioSourceRecycle(string name)
	{
		AudioSoundRecycle audioSoundRecycle = new GameObject(name + "_recycle")
		{
			transform = 
			{
				parent = base.transform.Find("Recycle"),
				position = Vector3.zero,
				localEulerAngles = Vector3.zero
			}
		}.AddComponent<AudioSoundRecycle>();
		audioSoundRecycle.origGo = this.GetAudioSource(name, this.soundSrcs).gameObject;
		this.audioSoundRecycles.Add(audioSoundRecycle);
		return audioSoundRecycle;
	}

	private AudioSoundRecycle GetOrNewAudioSourceRecycle(string name)
	{
		AudioSource audioSource = this.GetAudioSource(name, this.soundSrcs);
		AudioSoundRecycle audioSoundRecycle = this.GetAudioSourceRecycle(audioSource.name);
		if (audioSoundRecycle == null)
		{
			audioSoundRecycle = this.AddAudioSourceRecycle(audioSource);
		}
		return audioSoundRecycle;
	}

	private AudioSoundRecycle GetOrNewAudioSourceRecycle(AudioSource audioSource)
	{
		AudioSoundRecycle audioSoundRecycle = this.GetAudioSourceRecycle(audioSource.name);
		if (audioSoundRecycle == null)
		{
			audioSoundRecycle = this.AddAudioSourceRecycle(audioSource);
		}
		return audioSoundRecycle;
	}

	public bool IsPlaying(string name)
	{
		return this.IsPlaying(this.GetAudioSource(name, this.soundSrcs));
	}

	private bool IsPlaying(AudioSource audioSource)
	{
		return !(audioSource == null) && audioSource.isPlaying;
	}

	public virtual void Loop(string name)
	{
		AudioSource audioSource = this.GetAudioSource(name, this.soundSrcs);
		if (audioSource != null)
		{
			audioSource.loop = true;
		}
	}

	public void PlaySound(string name, bool restart = false)
	{
		this.PlaySound(this.GetAudioSource(name, this.soundSrcs), restart);
	}

	private void PlaySound(AudioSource audioSource, bool restart = false)
	{
		if (restart || (audioSource != null && !audioSource.isPlaying))
		{
			audioSource.Play();
			if (!audioSource.loop)
			{
				this.StopSoundDelay(audioSource, audioSource.clip.length);
			}
		}
	}

	public void PlaySound(string name, float delay, bool restart = false)
	{
		this.PlaySoundDelay(this.GetAudioSource(name, this.soundSrcs), delay, restart);
	}

	private void PlaySoundDelay(AudioSource audioSource, float delay, bool restart = false)
	{
		base.StartCoroutine(this.PlaySoundDelay_C(audioSource, delay, restart));
	}

	private IEnumerator PlaySoundDelay_C(AudioSource audioSource, float delay, bool restart = false)
	{
		yield return new WaitForSeconds(delay);
		this.PlaySound(audioSource, restart);
		yield break;
	}

	public void PlaySound(string name, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		this.PlaySoundFade(this.GetAudioSource(name, this.soundSrcs), fadeDownTime, pauseTime, fadeupTime);
	}

	private void PlaySoundFade(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (this.isFading)
		{
			this.PlaySound(audioSource, true);
		}
		else
		{
			base.StartCoroutine(this.PlaySoundFade_C(audioSource, fadeDownTime, pauseTime, fadeupTime));
		}
	}

	private IEnumerator PlaySoundFade_C(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (audioSource == null)
		{
			yield break;
		}
		audioSource.Play();
		float factor = 0f;
		float start = this.currentMusicAudioSource.volume;
		this.isFading = true;
		while (factor < 1f)
		{
			this.currentMusicAudioSource.volume = Mathf.SmoothStep(start, 0f, factor);
			factor += Time.deltaTime / fadeDownTime;
			yield return null;
		}
		this.currentMusicAudioSource.volume = 0f;
		yield return new WaitForSeconds(pauseTime);
		factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			this.currentMusicAudioSource.volume = Mathf.SmoothStep(0f, start, factor);
			yield return null;
		}
		this.isFading = false;
		yield break;
	}

	public void StopSound(string name)
	{
		this.StopSound(this.GetAudioSource(name, this.soundSrcs));
	}

	private void StopSound(AudioSource audioSource)
	{
		if (audioSource != null)
		{
			audioSource.Stop();
		}
	}

	private void StopSoundDelay(AudioSource audioSource, float delay)
	{
		base.StartCoroutine(this.StopSoundDelay_C(audioSource, delay));
	}

	private IEnumerator StopSoundDelay_C(AudioSource audioSource, float delay)
	{
		yield return new WaitForSeconds(delay);
		audioSource.Stop();
		yield break;
	}

	public void PauseSound(string name)
	{
		this.PauseSound(this.GetAudioSource(name, this.soundSrcs));
	}

	private void PauseSound(AudioSource audioSource)
	{
		if (audioSource != null)
		{
			audioSource.Pause();
		}
	}

	public void PlaySound(string name, Vector3 position)
	{
		this.PlaySoundASR(this.GetAudioSource(name, this.soundSrcs), position);
	}

	private void PlaySoundASR(AudioSource audioSource, Vector3 position)
	{
		base.StartCoroutine(this.PlaySoundASR_C(audioSource, audioSource.pitch, position));
	}

	public void PlaySound(string name, float pitch, Vector3 position)
	{
		this.PlaySoundASR(this.GetAudioSource(name, this.soundSrcs), pitch, position);
	}

	private void PlaySoundASR(AudioSource audioSource, float pitch, Vector3 position)
	{
		base.StartCoroutine(this.PlaySoundASR_C(audioSource, pitch, position));
	}

	public void PlaySound(string name, float delay, float pitch, Vector3 position)
	{
		this.PlaySoundASRDelay(this.GetAudioSource(name, this.soundSrcs), delay, pitch, position);
	}

	private void PlaySoundASRDelay(AudioSource audioSource, float delay, float pitch, Vector3 position)
	{
		base.StartCoroutine(this.PlaySoundASRDelay_C(audioSource, delay, pitch, position));
	}

	private IEnumerator PlaySoundASR_C(AudioSource audioSource, float pitch, Vector3 position)
	{
		if (audioSource == null)
		{
			yield break;
		}
		AudioSoundRecycle asr = this.GetOrNewAudioSourceRecycle(audioSource);
		AudioRecycleItem item = asr.Retain();
		if (item == null)
		{
			yield break;
		}
		item.audio.transform.position = position;
		item.audio.pitch = pitch;
		yield return new WaitForSeconds(item.audio.clip.length);
		item.Release();
		yield break;
	}

	private IEnumerator PlaySoundASRDelay_C(AudioSource audioSource, float delay, float pitch, Vector3 position)
	{
		yield return new WaitForSeconds(delay);
		this.PlaySoundASR(audioSource, pitch, position);
		yield break;
	}

	public void PlayMusic(string name, bool restart = false)
	{
		this.PlayMusic(this.GetAudioSource(name, this.musicSrcs), false);
	}

	private void PlayMusic(AudioSource music, bool restart = false)
	{
		if (this.currentMusicAudioSource == music || music == null)
		{
			return;
		}
		if (music != null && !music.isPlaying)
		{
			if (this.currentMusicAudioSource != null)
			{
				this.currentMusicAudioSource.Stop();
				float num = this.currentMusicAudioSource.volume / this.currentMusicMaxVolume;
				this.currentMusicAudioSource.volume = this.currentMusicMaxVolume;
				this.currentMusicMaxVolume = music.volume;
				music.volume *= num;
			}
			this.currentMusicAudioSource = music;
			this.currentMusicAudioSource.Play();
		}
		else if (restart)
		{
			this.currentMusicAudioSource.Play();
		}
	}

	public void PlayMusic(string name, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		this.PlayMusicFade(this.GetAudioSource(name, this.musicSrcs), fadeDownTime, pauseTime, fadeupTime);
	}

	public void PlayMusic(string name, float fadeupTime)
	{
		this.PlayMusicFade(this.GetAudioSource(name, this.musicSrcs), fadeupTime);
	}

	private void PlayMusicFade(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (this.currentMusicAudioSource != null)
		{
			if (this.isFading)
			{
				this.PlayMusic(audioSource, false);
			}
			else
			{
				base.StartCoroutine(this.MusicFader(audioSource, fadeDownTime, pauseTime, fadeupTime));
			}
		}
		else
		{
			this.PlayMusicFade(audioSource, fadeupTime);
		}
	}

	private void PlayMusicFade(AudioSource audioSource, float fadeupTime)
	{
		if (this.isFading)
		{
			this.PlayMusic(audioSource, false);
		}
		else
		{
			base.StartCoroutine(this.MusicFader(audioSource, fadeupTime));
		}
	}

	private IEnumerator MusicFader(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		float factor = 0f;
		float start = this.currentMusicAudioSource.volume;
		this.isFading = true;
		while (factor < 1f)
		{
			this.currentMusicAudioSource.volume = Mathf.SmoothStep(start, 0f, factor);
			factor += Time.deltaTime / fadeDownTime;
			yield return null;
		}
		yield return new WaitForSeconds(pauseTime);
		this.currentMusicAudioSource.volume = 0f;
		this.currentMusicAudioSource.Stop();
		this.currentMusicAudioSource.volume = this.currentMusicMaxVolume;
		this.currentMusicMaxVolume = audioSource.volume;
		audioSource.volume = 0f;
		this.currentMusicAudioSource = audioSource;
		start = this.currentMusicMaxVolume;
		this.currentMusicAudioSource.Play();
		yield return new WaitForSeconds(pauseTime);
		factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			this.currentMusicAudioSource.volume = Mathf.SmoothStep(0f, start, factor);
			yield return null;
		}
		this.currentMusicAudioSource.volume = start;
		this.isFading = false;
		yield break;
	}

	private IEnumerator MusicFader(AudioSource audioSource, float fadeupTime)
	{
		float factor = 0f;
		if (this.currentMusicAudioSource != null)
		{
			this.currentMusicAudioSource.Stop();
			this.currentMusicAudioSource.volume = this.currentMusicMaxVolume;
		}
		this.currentMusicAudioSource = audioSource;
		this.currentMusicMaxVolume = audioSource.volume;
		audioSource.volume = 0f;
		this.currentMusicAudioSource.Play();
		this.isFading = true;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			this.currentMusicAudioSource.volume = Mathf.Lerp(0f, this.currentMusicMaxVolume, factor);
			yield return null;
		}
		this.currentMusicAudioSource.volume = this.currentMusicMaxVolume;
		this.isFading = false;
		yield break;
	}

	public void WarmUp(string name)
	{
		this.GetOrNewAudioSourceRecycle(name).WarmUp();
	}

	public List<AudioPlayer.AudioClipInfo> musics;

	private List<AudioSource> musicSrcs;

	public List<AudioPlayer.AudioClipInfo> sounds;

	private List<AudioSource> soundSrcs;

	private List<AudioSoundRecycle> audioSoundRecycles;

	private AudioSource currentMusicAudioSource;

	private bool isFading;

	private float currentMusicMaxVolume;

	private float musicVolumnRateWithSound;

	private bool audioPlayPaused;

	private float currentMusicAudioSourceTime;

	private static AudioPlayer _instance;

	[Serializable]
	public class AudioClipInfo
	{
		public AudioClip Clip;

		public float minPitch = 0.8f;

		public float maxPitch = 1.1f;

		public float minVolume = 0.5f;

		public float maxVolume = 0.7f;

		public bool playOnAwake;

		public bool loop;

		public AudioRolloffMode Rollof = AudioRolloffMode.Linear;
	}
}
