using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMode : StageMenuSequence
{
	private void Start()
	{
		if (this.anim == null)
		{
			this.anim = Character.Instance.characterModel.characterAnimation;
		}
	}

	public override void StartPlayIdleRummagesAnimation()
	{
		Character.Instance.transform.position = base.transform.position;
		Character.Instance.transform.rotation = base.transform.rotation;
		int i = 0;
		int num = this.idelRummages.Length;
		while (i < num)
		{
			if (this.anim[this.idelRummages[i].name] == null)
			{
				this.anim.AddClip(this.idelRummages[i], this.idelRummages[i].name);
			}
			i++;
		}
		this.selectClip = null;
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		this.isPlay = true;
		List<AnimationClip> temps = new List<AnimationClip>();
		while (this.isPlay)
		{
			int i = 0;
			int num = this.idelRummages.Length;
			while (i < num)
			{
				if (this.idelRummages[i] != this.selectClip)
				{
					temps.Add(this.idelRummages[i]);
				}
				i++;
			}
			int random = UnityEngine.Random.Range(0, temps.Count);
			this.selectClip = temps[random];
			this.anim.Play(this.selectClip.name);
			this.animationTime = this.selectClip.length;
			while (this.animationTime > 0f)
			{
				this.animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		this.isPlay = false;
		this.selectClip = null;
		this.animationRoutine = null;
		yield break;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		int i = 0;
		int num = this.idelRummages.Length;
		while (i < num)
		{
			if (this.anim[this.idelRummages[i].name] != null)
			{
				this.anim.RemoveClip(this.idelRummages[i]);
			}
			i++;
		}
		base.StopPlayIdleRummagesAnimation();
	}

	public AnimationClip[] idelRummages;

	private AnimationClip selectClip;
}
