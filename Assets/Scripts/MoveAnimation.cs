using System;
using UnityEngine;

public class MoveAnimation : Point
{
	public override void OnInit(PointsManager manager)
	{
		manager.TargetAnim.AddClip(this.clip);
		this.move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast)
		{
			manager.TargetAnim.RemoveClip(this.clip);
		}
	}

	public override void OnStart(PointsManager manager)
	{
		manager.TargetAnim[this.clip.name].speed = this.speed;
		manager.TargetAnim.Play(this.clip.name);
		this.time = manager.TargetAnim[this.clip.name].length / this.speed;
		if (this.front)
		{
			this.moveTime = this.time - this.frame / this.clip.frameRate / this.speed;
			this.moveSpeed = manager.GetDistanceToTarget() / this.moveTime;
		}
		else
		{
			this.moveTime = this.frame / this.clip.frameRate / this.speed;
			this.moveSpeed = manager.GetDistanceToTarget() / this.moveTime;
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (this.time > 0f)
		{
			this.time -= Time.deltaTime;
			if (this.animationDrive)
			{
				return false;
			}
			if ((this.front && this.time < this.moveTime) || (!this.front && this.time > this.moveTime))
			{
				manager.RotatoToPoint();
				manager.Move(this.moveSpeed * Time.deltaTime);
			}
		}
		else
		{
			manager.GoToNextPoint();
		}
		return true;
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
	}

	[SerializeField]
	private AnimationClip clip;

	[SerializeField]
	private float speed;

	[SerializeField]
	private bool animationDrive;

	[SerializeField]
	private bool front;

	[SerializeField]
	private float frame;

	[SerializeField]
	private bool removeClipAtLast;

	[SerializeField]
	private bool rotate;

	private float moveSpeed;

	private float moveTime;

	private float time;
}
