using System;
using System.Collections;
using UnityEngine;

public class LoopMode : StageMenuSequence
{
	public override void StartPlayIdleRummagesAnimation()
	{
		if (this.anim[this.loopClip.name] == null)
		{
			this.anim.AddClip(this.loopClip, this.loopClip.name);
		}
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		this.isPlay = true;
		while (this.isPlay)
		{
			this.anim.Play(this.loopClip.name);
			this.animationTime = this.loopClip.length;
			while (this.animationTime > 0f)
			{
				this.animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		this.isPlay = false;
		this.animationRoutine = null;
		yield break;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		if (this.anim[this.loopClip.name] != null)
		{
			this.anim.RemoveClip(this.loopClip);
		}
		base.StopPlayIdleRummagesAnimation();
	}

	public AnimationClip loopClip;
}
