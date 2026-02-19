using System;
using UnityEngine;

public class Sample : Point
{
	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
	}

	public override void OnInit(PointsManager manager)
	{
		if (this.idelClip != null)
		{
			manager.TargetAnim.AddClip(this.idelClip);
		}
	}

	public override void OnStart(PointsManager manager)
	{
		if (this.idelClip != null)
		{
			string name = this.idelClip.name;
			AnimationState animationState = manager.TargetAnim[name];
			animationState.enabled = true;
			animationState.speed = 0f;
			animationState.normalizedTime = (float)this.frame / (this.idelClip.frameRate * this.idelClip.length);
			manager.TargetAnim.anim.Sample();
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.GoToNextPoint();
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast && this.idelClip != null)
		{
			manager.TargetAnim.RemoveClip(this.idelClip);
		}
	}

	[SerializeField]
	private AnimationClip idelClip;

	[SerializeField]
	private int frame;

	[SerializeField]
	private bool removeClipAtLast;
}
