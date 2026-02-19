using System;
using UnityEngine;

public class CalcCircularPoint
{
	public CalcCircularPoint(int count, CalcCircularPoint.Axis axis, float radius)
	{
		this._copyCount = count;
		this._radius = radius;
		this._axis = axis;
	}

	public Vector3 CalcCenterOffset(int index)
	{
		Vector3 zero = Vector3.zero;
		float f = (float)index * 3.141593f * 2f / (float)this._copyCount;
		CalcCircularPoint.Axis axis = this._axis;
		if (axis == CalcCircularPoint.Axis.X)
		{
			return new Vector3(0f, Mathf.Cos(f) * this._radius, Mathf.Sin(f) * this._radius);
		}
		if (axis == CalcCircularPoint.Axis.Y)
		{
			return new Vector3(Mathf.Cos(f) * this._radius, 0f, Mathf.Sin(f) * this._radius);
		}
		if (axis != CalcCircularPoint.Axis.Z)
		{
			return zero;
		}
		return new Vector3(Mathf.Cos(f) * this._radius, Mathf.Sin(f) * this._radius, 0f);
	}

	private int _copyCount;

	private float _radius = 1f;

	private CalcCircularPoint.Axis _axis = CalcCircularPoint.Axis.Z;

	public enum Axis
	{
		X,
		Y,
		Z
	}
}
