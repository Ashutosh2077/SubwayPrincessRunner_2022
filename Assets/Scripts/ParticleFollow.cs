using System;
using UnityEngine;

public class ParticleFollow : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	private void LateUpdate()
	{
		Vector3 position = this.Target.position;
		position.x = base.transform.position.x;
		float num = Mathf.SmoothDamp(position.x, this.Target.position.x, ref this.tweenVelocity, this.TweenTime);
		if (!float.IsNaN(num))
		{
			position.x = num;
		}
		else
		{
			this.tweenVelocity = 0f;
		}
		base.transform.position = position;
	}

	private void OnDisable()
	{
		this.tweenVelocity = 0f;
	}

	private void OnEnable()
	{
		this.tweenVelocity = 0f;
	}

	public Transform Target;

	public float TweenTime;

	private float tweenVelocity;
}
