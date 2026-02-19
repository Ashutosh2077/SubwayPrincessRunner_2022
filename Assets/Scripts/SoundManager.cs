using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private void Awake()
	{
		if (SoundManager.Instance != null)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			SoundManager.Instance = this;
			this._soundList = new List<SoundInfo>(this.initialCapacity);
			for (int i = 0; i < this.initialCapacity; i++)
			{
				this._soundList.Add(new SoundInfo(this));
			}
			this.initLoop();
		}
	}

	private void initLoop()
	{
		this.flyback = base.GetComponentInChildren<FlybackLoop>();
		this.magnet = base.GetComponentInChildren<MagnetLoop>();
		this.revive = base.GetComponentInChildren<ReviveLoop>();
		this.unlock = base.GetComponentInChildren<UnlockSound>();
		this.chestOpen = base.GetComponentInChildren<ChestOpenSound>();
		this.chestShow = base.GetComponentInChildren<ChestShowSound>();
		this.start = base.GetComponentInChildren<StartSound>();
		this.topmenu = base.GetComponentInChildren<TopMenuLoop>();
		this.ingame = base.GetComponentInChildren<IngameLoop>();
		this.currentSound = null;
	}

	public SoundInfo PlayAudioClip(AudioClipInfo audioClip)
	{
		if (audioClip.Clip == null)
		{
			return null;
		}
		return this.PlayAudioClip(audioClip.Clip, audioClip.Rollof, audioClip.minVolume, audioClip.maxVolume, audioClip.minPitch, audioClip.maxPitch, base.transform.position);
	}

	public SoundInfo PlayAudioClip(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		SoundInfo soundInfo = null;
		bool flag = false;
		bool flag2 = false;
		string name = audioClip.name;
		int i = 0;
		int count = this._soundList.Count;
		while (i < count)
		{
			if (this._soundList[i].available)
			{
				if (this._soundList[i].name == name)
				{
					soundInfo = this._soundList[i];
					flag = true;
					break;
				}
				if (!flag2)
				{
					if (this._soundList[i].name == "empty")
					{
						flag2 = true;
					}
					soundInfo = this._soundList[i];
				}
			}
			i++;
		}
		if (soundInfo == null)
		{
			soundInfo = this._soundList[0];
			this._soundList.Add(soundInfo);
		}
		if (flag)
		{
			base.StartCoroutine(soundInfo.Play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position));
			return soundInfo;
		}
		base.StartCoroutine(soundInfo.Play(audioClip, rolloff, minVolume, maxVolume, minPitch, maxPitch, position));
		return soundInfo;
	}

	public void removeSound(SoundInfo s)
	{
		this._soundList.Remove(s);
	}

	public void SetLoopState(LoopType loop)
	{
		switch (loop)
		{
		case LoopType.flypack:
			this.currentSound = this.flyback;
			break;
		case LoopType.magnet:
			this.currentSound = this.magnet;
			break;
		case LoopType.revive:
			this.currentSound = this.revive;
			break;
		case LoopType.unlock:
			this.currentSound = this.unlock;
			break;
		case LoopType.start:
			this.currentSound = this.start;
			break;
		case LoopType.chestopen:
			this.currentSound = this.chestOpen;
			break;
		case LoopType.chestshow:
			this.currentSound = this.chestShow;
			break;
		case LoopType.ingame:
			this.currentSound = this.ingame;
			break;
		case LoopType.topmenu:
			this.currentSound = this.topmenu;
			break;
		}
	}

	public void BackgroundSoundChange(bool stop = false)
	{
		if (stop)
		{
			this.currentSound.Stop();
		}
		else
		{
			this.currentSound.Play();
		}
	}

	public static SoundManager Instance;

	private List<SoundInfo> _soundList;

	public int initialCapacity = 5;

	public int maxCapacity = 50;

	private FlybackLoop flyback;

	private MagnetLoop magnet;

	private ReviveLoop revive;

	private ChestOpenSound chestOpen;

	private UnlockSound unlock;

	private StartSound start;

	private ChestShowSound chestShow;

	private TopMenuLoop topmenu;

	public IngameLoop ingame;

	private SoundLoop currentSound;
}
