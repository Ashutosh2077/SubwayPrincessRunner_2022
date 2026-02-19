using System;
using UnityEngine;

public class MoveRandomAnimation : Point
{
	public override void OnInit(PointsManager manager)
	{
		int i = 0;
		int num = this.clipNames.Length;
		while (i < num)
		{
			manager.TargetAnim.AddClip(this.clipNames[i]);
			i++;
		}
		this.move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast)
		{
			int i = 0;
			int num = this.clipNames.Length;
			while (i < num)
			{
				manager.TargetAnim.RemoveClip(this.clipNames[i]);
				i++;
			}
		}
	}

	public override void OnStart(PointsManager manager)
	{
		this.initPos = manager.transform.position;
		this.CrossFadeRandomClip(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.SetPositionAccordingRefrence(this.initPos);
		manager.RotatoToPoint();
		if (this.time > 0f)
		{
			this.time -= Time.deltaTime;
		}
		else
		{
			manager.GoToNextPoint();
		}
		return true;
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.Refrence.localPosition = Vector3.zero;
	}

	private void CrossFadeRandomClip(PointsManager manager)
	{
		int num = UnityEngine.Random.Range(0, this.clipNames.Length);
		this.clip = this.clipNames[num];
		manager.TargetAnim.CrossFade(this.clip.name, 0.2f);
		this.time = manager.TargetAnim[this.clip.name].length;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.Refrence.localPosition = Vector3.zero;
	}

	[SerializeField]
	private AnimationClip[] clipNames;

	[SerializeField]
	private bool removeClipAtLast;

	private Vector3 initPos;

	private AnimationClip clip;

	private float time;
}
