using System;
using UnityEngine;

public class SmoothDampVector3
{
	public SmoothDampVector3(Vector3 value, float smoothTime)
	{
		this.smoothTime = smoothTime;
		this.value = value;
		this.target = value;
	}

	public void Update()
	{
		float num = Mathf.SmoothDamp(this.value.x, this.target.x, ref this.velocity.x, this.smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num))
		{
			this.value.x = num;
		}
		float num2 = Mathf.SmoothDamp(this.value.y, this.target.y, ref this.velocity.y, this.smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num2))
		{
			this.value.y = num2;
		}
		float num3 = Mathf.SmoothDamp(this.value.z, this.target.z, ref this.velocity.z, this.smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num3))
		{
			this.value.z = num3;
		}
	}

	public float SmoothTime
	{
		get
		{
			return this.smoothTime;
		}
		set
		{
			this.smoothTime = value;
		}
	}

	public Vector3 Target
	{
		get
		{
			return this.target;
		}
		set
		{
			this.target = value;
		}
	}

	public Vector3 Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return this.velocity;
		}
		set
		{
			this.velocity = value;
		}
	}

	private float smoothTime;

	private Vector3 target;

	private Vector3 value;

	private Vector3 velocity = Vector3.zero;
}
