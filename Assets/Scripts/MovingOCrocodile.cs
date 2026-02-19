using System;
using UnityEngine;

public class MovingOCrocodile : MovingO
{
	protected override void Awake()
	{
		base.Awake();
		this.anim[this.attackClip].AddMixingTransform(this.neck);
		this.anim[this.attackClip].layer = 2;
		this.firstPlayAttackClip = false;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		this.anim.Play(this.runClip);
		this.anim[this.attackClip].enabled = false;
		this.firstPlayAttackClip = true;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.anim.Stop();
	}

	protected override void Update()
	{
		base.Update();
		this.temp = this.Distance;
		if (this.temp < this.trigger && this.firstPlayAttackClip)
		{
			this.anim[this.attackClip].enabled = true;
			this.anim.Play(this.attackClip);
			if (this.audioClip != null)
			{
				AudioPlayer.Instance.PlaySound(this.audioClip.name, true);
			}
			this.firstPlayAttackClip = false;
		}
	}

	protected override float Distance
	{
		get
		{
			return this.curTrans.position.z - MovingO.characterController.transform.position.z;
		}
	}

	protected override float Speed
	{
		get
		{
			return this.speed;
		}
	}

	[SerializeField]
	private float trigger;

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private string runClip;

	[SerializeField]
	private string attackClip;

	[SerializeField]
	private Transform neck;

	[SerializeField]
	private AudioClip audioClip;

	private bool firstPlayAttackClip;

	private float temp;
}
