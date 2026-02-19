using System;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
	public void Shake()
	{
		this.AnimationTime = 0f;
		this.diff = Vector3.zero;
	}

	public Vector3 UpdateShakeController()
	{
		this.diff += UnityEngine.Random.insideUnitSphere * this.ShakeIntensity;
		Vector3 result = (1f - this.AnimationTime) * this.diff * Time.deltaTime;
		this.AnimationTime = Mathf.Min(this.AnimationTime + Time.deltaTime / this.ShakeLength, 1f);
		return result;
	}

	private float AnimationTime = 1f;

	[SerializeField]
	private float ShakeIntensity = 1f;

	[SerializeField]
	private float ShakeLength = 0.5f;

	private Vector3 diff;
}
