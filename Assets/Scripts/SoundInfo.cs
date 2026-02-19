using System;
using System.Collections;
using UnityEngine;

public class SoundInfo
{
	public SoundInfo(SoundManager manager)
	{
		this._manager = manager;
		this.gameObject = new GameObject();
		this._name = "empty";
		this.gameObject.name = this._name;
		this.gameObject.transform.parent = manager.transform;
		this.audioSource = this.gameObject.AddComponent<AudioSource>();
	}

	public void DestroySelf()
	{
		this._manager.removeSound(this);
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public IEnumerator Play(AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		this.available = false;
		this.gameObject.transform.position = position;
		this.audioSource.rolloffMode = rolloff;
		this.audioSource.volume = UnityEngine.Random.Range(minVolume, maxVolume);
		this.audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
		this.audioSource.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(this.audioSource.clip.length + 0.1f);
		this.audioSource.Stop();
		if (this.destroyAfterPlay)
		{
			this.DestroySelf();
		}
		this.available = true;
		yield break;
	}

	public IEnumerator Play(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		this._name = audioClip.name;
		this.gameObject.name = this._name;
		this.audioSource.clip = audioClip;
		return this.Play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position);
	}

	public string name
	{
		get
		{
			return this._name;
		}
	}

	private SoundManager _manager;

	private string _name;

	public AudioSource audioSource;

	public bool available = true;

	public bool destroyAfterPlay;

	private GameObject gameObject;
}
