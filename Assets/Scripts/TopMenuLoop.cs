using System;
using UnityEngine;

public class TopMenuLoop : MonoBehaviour, SoundLoop
{
	public void Play()
	{
		SoundManager.Instance.ingame.ChangeAudioClip(this.topmenuLoop);
	}

	public void Stop()
	{
		SoundManager.Instance.ingame.ResetAudioClip();
	}

	public AudioClip topmenuLoop;
}
