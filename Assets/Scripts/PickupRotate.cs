using System;
using UnityEngine;

public class PickupRotate : BaseO
{
	protected override void Awake()
	{
		base.Awake();
		base.enabled = false;
	}

	public override void OnActivate()
	{
		this.z = base.transform.position.z;
		CoinPool.Instance.AddActiveRotatePickups(this);
		base.enabled = true;
	}

	public override void OnDeactivate()
	{
		CoinPool.Instance.RemoveActiveRotatePickups(this);
		base.enabled = false;
	}

	public void PhasedRotate()
	{
		this.target.localRotation = Quaternion.AngleAxis(Time.time * this.speed + this.z * this.rotatePhase, Vector3.up);
	}

	public float Z
	{
		get
		{
			return this.z;
		}
	}

	public Transform target;

	public float speed = 180f;

	public float rotatePhase = 0.9f;

	private float z;
}
