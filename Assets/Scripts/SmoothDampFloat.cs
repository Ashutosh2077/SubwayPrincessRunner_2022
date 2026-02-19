using System;
using UnityEngine;

public class SmoothDampFloat
{
	public SmoothDampFloat(float value, float smoothTime)
	{
		this.smoothTime = smoothTime;
		this.value = value;
		this.target = value;
	}

	public void Update()
	{
		float f = Mathf.SmoothDamp(this.value, this.target, ref this.valueSpeed, this.smoothTime);
		if (!float.IsNaN(f))
		{
			this.value = f;
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

	public float Target
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

	public float Value
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

	private float smoothTime;

	private float target;

	private float value;

	public float valueSpeed;
}
