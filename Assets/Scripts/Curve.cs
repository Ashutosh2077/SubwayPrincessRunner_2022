using System;
using UnityEngine;

public class Curve
{
	public Curve()
	{
		this.curveX.postWrapMode = WrapMode.ClampForever;
		this.curveX.preWrapMode = WrapMode.ClampForever;
		this.curveY.postWrapMode = WrapMode.ClampForever;
		this.curveY.preWrapMode = WrapMode.ClampForever;
		this.curveZ.postWrapMode = WrapMode.ClampForever;
		this.curveZ.preWrapMode = WrapMode.ClampForever;
	}

	public void AddKey(float t, Vector3 value)
	{
		this.curveX.AddKey(t, value.x);
		this.curveY.AddKey(t, value.y);
		this.curveZ.AddKey(t, value.z);
		if (t < this.min)
		{
			this.min = t;
		}
		if (t > this.max)
		{
			this.max = t;
		}
	}

	public void AddKey(float t, Vector3 value, Vector3 inTangent, Vector3 outTangent)
	{
		this.curveX.AddKey(new Keyframe(t, value.x, inTangent.x, outTangent.x));
		this.curveY.AddKey(new Keyframe(t, value.y, inTangent.y, outTangent.y));
		this.curveZ.AddKey(new Keyframe(t, value.z, inTangent.z, outTangent.z));
		if (t < this.min)
		{
			this.min = t;
		}
		if (t > this.max)
		{
			this.max = t;
		}
	}

	public void DrawGizmos(Color color)
	{
		Gizmos.color = color;
		int num = 1000;
		Vector3 from = this.Evaluate(0f);
		for (int i = 0; i < num; i++)
		{
			float t = (this.max - this.min) * (float)i / (float)(num - 1);
			Vector3 vector = this.Evaluate(t);
			Gizmos.DrawLine(from, vector);
			from = vector;
		}
	}

	public Vector3 Evaluate(float t)
	{
		return new Vector3(this.curveX.Evaluate(t), this.curveY.Evaluate(t), this.curveZ.Evaluate(t));
	}

	public void MoveKey(int index, float t, Vector3 value)
	{
		this.curveX.MoveKey(index, new Keyframe(t, value.x));
		this.curveY.MoveKey(index, new Keyframe(t, value.y));
		this.curveZ.MoveKey(index, new Keyframe(t, value.z));
	}

	public void MoveKey(int index, float t, Vector3 value, Vector3 inTangent, Vector3 outTangent)
	{
		this.curveX.MoveKey(index, new Keyframe(t, value.x, inTangent.x, outTangent.x));
		this.curveY.MoveKey(index, new Keyframe(t, value.y, inTangent.y, outTangent.y));
		this.curveZ.MoveKey(index, new Keyframe(t, value.z, inTangent.z, outTangent.z));
	}

	public void SmoothTangents(int index, float weight)
	{
		this.curveX.SmoothTangents(index, weight);
		this.curveY.SmoothTangents(index, weight);
		this.curveZ.SmoothTangents(index, weight);
	}

	public AnimationCurve curveX = new AnimationCurve();

	public AnimationCurve curveY = new AnimationCurve();

	public AnimationCurve curveZ = new AnimationCurve();

	private float max = float.NegativeInfinity;

	private float min = float.PositiveInfinity;
}
