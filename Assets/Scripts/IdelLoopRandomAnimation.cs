using System;
using UnityEngine;

public class IdelLoopRandomAnimation : Point
{
	private void Awake()
	{
		this.randomClipName = new string[this.randomClip.Length];
		int i = 0;
		int num = this.randomClip.Length;
		while (i < num)
		{
			this.randomClipName[i] = this.randomClip[i].name;
			i++;
		}
	}

	public override void OnInit(PointsManager manager)
	{
		int i = 0;
		int num = this.randomClip.Length;
		while (i < num)
		{
			manager.TargetAnim.AddClip(this.randomClip[i]);
			i++;
		}
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast)
		{
			int i = 0;
			int num = this.randomClip.Length;
			while (i < num)
			{
				manager.TargetAnim.RemoveClip(this.randomClip[i]);
				i++;
			}
		}
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.CrossFadeRandomClip(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (this.time > 0f)
		{
			this.time -= Time.deltaTime;
		}
		else
		{
			this.CrossFadeRandomClip(manager);
		}
		return false;
	}

	private void CrossFadeRandomClip(PointsManager manager)
	{
		int num = UnityEngine.Random.Range(0, this.randomClipName.Length);
		if (this.randomClipName[num].Equals(this.clip))
		{
			num = (num + 1) % this.randomClipName.Length;
		}
		this.clip = this.randomClipName[num];
		manager.TargetAnim.Play(this.clip);
		this.time = manager.TargetAnim[this.clip].length;
	}

	public override void OnImility(PointsManager manager)
	{
	}

	[SerializeField]
	private AnimationClip[] randomClip;

	[SerializeField]
	private bool removeClipAtLast;

	private string[] randomClipName;

	private string clip;

	private float time;
}
