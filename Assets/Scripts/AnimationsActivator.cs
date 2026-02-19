using System;
using UnityEngine;

public class AnimationsActivator : BaseO
{
	protected override void Awake()
	{
		this.animations = base.GetComponentsInChildren<Animation>();
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		int i = 0;
		int num = this.animations.Length;
		while (i < num)
		{
			if (this.animations[i] != null)
			{
				this.animations[i].enabled = true;
				this.animations[i].Play();
			}
			i++;
		}
	}

	public override void OnDeactivate()
	{
		int i = 0;
		int num = this.animations.Length;
		while (i < num)
		{
			if (this.animations[i] != null)
			{
				this.animations[i].Stop();
				this.animations[i].enabled = false;
			}
			i++;
		}
	}

	private Animation[] animations;
}
