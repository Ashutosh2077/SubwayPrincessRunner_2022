using System;
using System.Collections;
using UnityEngine;

public class OrderMode : StageMenuSequence
{
	public override void StartPlayIdleRummagesAnimation()
	{
		int i = 0;
		int num = this.orders.Length;
		while (i < num)
		{
			if (this.anim[this.orders[i].name] == null)
			{
				this.anim.AddClip(this.orders[i], this.orders[i].name);
			}
			i++;
		}
		this.selectIndex = -1;
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		this.isPlay = true;
		while (this.isPlay)
		{
			this.selectIndex = Mathf.Clamp(this.selectIndex + 1, 0, this.orders.Length);
			AnimationClip clip = this.orders[this.selectIndex];
			this.anim.Play(clip.name);
			this.animationTime = clip.length;
			while (this.animationTime > 0f)
			{
				this.animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		this.isPlay = false;
		this.selectIndex = -1;
		this.animationRoutine = null;
		yield break;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		int i = 0;
		int num = this.orders.Length;
		while (i < num)
		{
			if (this.anim[this.orders[i].name] != null)
			{
				this.anim.RemoveClip(this.orders[i]);
			}
			i++;
		}
		base.StopPlayIdleRummagesAnimation();
	}

	public AnimationClip[] orders;

	private int selectIndex;
}
