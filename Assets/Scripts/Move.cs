using System;
using UnityEngine;

public class Move : Point
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

	public override void OnStart(PointsManager manager)
	{
		if (this.animClip != null)
		{
			manager.TargetAnim.SetSpeed(this.animClip.name, this.animSpeed);
			manager.TargetAnim.Play(this.animClip.name);
		}
		manager.PlaySound(this.audioClip, false);
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.StopSound();
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.RotatoToTarget();
		manager.Move(this.moveSpeed * (float)((!this.onStop) ? 1 : 2) * Time.deltaTime);
		manager.Check();
		return true;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
		manager.StopSound();
	}

	[SerializeField]
	protected AnimationClip animClip;

	[SerializeField]
	protected float animSpeed;

	[SerializeField]
	protected float moveSpeed;

	[SerializeField]
	private bool removeClipAtLast;

	[SerializeField]
	private AudioClipInfo audioClip;

	private bool onStop;
}
