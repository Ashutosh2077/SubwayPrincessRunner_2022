using System;
using UnityEngine;

public class MovingAnimal : MovingO
{
	protected override void Awake()
	{
		base.Awake();
		this.originLPosZForChild = this.child.localPosition.z;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		this.child.localPosition = new Vector3(0f, 0f, this.originLPosZForChild);
		this.moveFirstUpdate = true;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.anim.Stop();
		this.child.transform.localPosition = new Vector3(0f, 0f, this.originLPosZForChild);
	}

	protected override void Update()
	{
		if (this.Distance * this.Speed > this.originLPosZForChild)
		{
			return;
		}
		if (this.moveFirstUpdate)
		{
			this.anim.Play(this.runClip);
			this.moveFirstUpdate = false;
		}
		base.Update();
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
	private Animation anim;

	[SerializeField]
	private string runClip;

	private float originLPosZForChild;

	private bool moveFirstUpdate;
}
