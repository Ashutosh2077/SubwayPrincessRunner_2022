using System;
using UnityEngine;

public class AudioRecycleItem : AbsRecycleItem
{
	public override bool IsUsing()
	{
		return this.isUsing;
	}

	public override void Release()
	{
		this.audio.Stop();
		this.isUsing = false;
	}

	public override void Retain()
	{
		this.audio.Play();
		this.isUsing = true;
	}

	public AudioSource audio;

	public bool isUsing;
}
