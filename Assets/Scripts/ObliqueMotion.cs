using System;
using UnityEngine;

public class ObliqueMotion
{
	public static float CalcHeight(float t)
	{
		return 4f * t * (1f - t);
	}

	public static float CalcTA(float h)
	{
		return Mathf.Sqrt(1f - h) * 0.5f + 0.5f;
	}

	public static float CalcTB(float h)
	{
		return 0.5f - Mathf.Sqrt(1f - h) * 0.5f;
	}
}
