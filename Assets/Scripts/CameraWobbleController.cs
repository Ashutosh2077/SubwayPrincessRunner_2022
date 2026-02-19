using System;
using UnityEngine;

public class CameraWobbleController : MonoBehaviour
{
	public void Wobble()
	{
		this.AnimationTime = 0f;
	}

	public Vector3 UpdateWobbleController()
	{
		Vector3 result = new Vector3(-this.WobbleX_AC.Evaluate(this.AnimationTime), this.WobbleY_AC.Evaluate(this.AnimationTime), 0f) * this.WobbleIntensity;
		this.AnimationTime = Mathf.Min(this.AnimationTime + Time.deltaTime / this.WobbleLength, 1f);
		return result;
	}

	private float AnimationTime = 1f;

	[SerializeField]
	private float WobbleIntensity = 1f;

	[SerializeField]
	private float WobbleLength = 0.5f;

	[SerializeField]
	private AnimationCurve WobbleX_AC;

	[SerializeField]
	private AnimationCurve WobbleY_AC;
}
