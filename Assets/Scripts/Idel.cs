using System;
using UnityEngine;

public class Idel : Point
{
	public override void OnInit(PointsManager manager)
	{
		if (this.idelClip != null)
		{
			manager.TargetAnim.AddClip(this.idelClip);
		}
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast && this.idelClip != null)
		{
			manager.TargetAnim.RemoveClip(this.idelClip);
		}
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.time = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
		if (this.idelClip != null)
		{
			manager.TargetAnim.Play(this.idelClip.name);
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (this.time > 0f)
		{
			this.time -= Time.deltaTime;
		}
		else
		{
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
	}

	[SerializeField]
	private AnimationClip idelClip;

	[SerializeField]
	private float minInterval;

	[SerializeField]
	private float maxInterval;

	[SerializeField]
	private bool removeClipAtLast;

	private float time;
}
