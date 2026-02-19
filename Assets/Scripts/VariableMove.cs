using System;
using UnityEngine;

public class VariableMove : Point
{
	public override void OnInit(PointsManager manager)
	{
		if (this.animClip != null)
		{
			manager.TargetAnim.AddClip(this.animClip);
		}
		this.move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (this.removeClipAtLast && this.animClip != null)
		{
			manager.TargetAnim.RemoveClip(this.animClip);
		}
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.speed = this.initialSpeed;
		if (this.animClip != null)
		{
			manager.TargetAnim.SetSpeed(this.animClip.name, this.animSpeed);
			manager.TargetAnim.Play(this.animClip.name);
		}
		manager.PlaySound(this.audioClip, true);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.RotatoToTarget();
		this.speed += this.acc * Time.deltaTime;
		if (Mathf.Abs(this.speed - this.initialSpeed) > Mathf.Abs(this.maxSpeed - this.initialSpeed))
		{
			this.speed = this.maxSpeed;
		}
		manager.TargetAnim.SetSpeed(this.animClip.name, this.animSpeed * this.speed / this.initialSpeed);
		manager.Move(this.speed * Time.deltaTime);
		manager.Check();
		return true;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformToTarget();
		manager.StopSound();
	}

	[SerializeField]
	protected AnimationClip animClip;

	[SerializeField]
	protected float animSpeed;

	[SerializeField]
	private AudioClipInfo audioClip;

	[SerializeField]
	protected float initialSpeed;

	[SerializeField]
	protected float acc;

	[SerializeField]
	protected float maxSpeed;

	[SerializeField]
	private bool removeClipAtLast;

	private float speed;
}
