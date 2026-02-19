using System;
using UnityEngine;

public class TriggerO : BaseO
{
	protected override void Awake()
	{
		if (this.onTrigger != null)
		{
			this.onTrigger.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(this.onTrigger.OnEnter, new OnTriggerObject.OnEnterDelegate(this.TriggerOnEnter));
		}
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		if (this.onTrigger != null)
		{
			float num = Game.Instance.currentSpeed;
			if (num <= 0f)
			{
				num = Game.Instance.speed.min;
			}
			this.onTrigger.transform.localPosition = new Vector3(0f, 0f, -num * this.interval);
			if (!string.IsNullOrEmpty(this.idleClip) && this.anim[this.idleClip] != null)
			{
				this.anim.enabled = true;
				this.anim.Play(this.idleClip);
			}
		}
	}

	public override void OnDeactivate()
	{
		if (this.onTrigger != null)
		{
			this.anim.Stop();
			this.anim.enabled = false;
		}
	}

	public virtual void TriggerOnEnter(Collider collider)
	{
	}

	[SerializeField]
	protected OnTriggerObject onTrigger;

	[SerializeField]
	protected Animation anim;

	[SerializeField]
	protected string idleClip;

	[SerializeField]
	protected float interval;
}
