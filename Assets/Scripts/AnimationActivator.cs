using System;
using UnityEngine;

public class AnimationActivator : BaseO
{
	protected override void Awake()
	{
		this.anim = base.GetComponent<Animation>();
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		this.anim.enabled = true;
		this.anim.Play();
	}

	public override void OnDeactivate()
	{
		this.anim.Stop();
		this.anim.enabled = false;
	}

	private Animation anim;
}
