using System;
using UnityEngine;

public class CelebrationBackground : MonoBehaviour
{
	public float GetCelebrationAnimationCurveValue(float pos)
	{
		return this._celebrationAnimationCurve.Evaluate(pos);
	}

	public void Hide()
	{
		this.visible = false;
	}

	public void InvertParticlesDirection(bool invert)
	{
		if (invert)
		{
			this.celebrationCloner.localPosition = new Vector3(0f, 100f, 0f);
			this.celebrationCloner.localRotation = new Quaternion(0f, 0f, 180f, 0f);
		}
		else
		{
			this.celebrationCloner.localPosition = new Vector3(0f, -20f, 0f);
			this.celebrationCloner.localRotation = Quaternion.identity;
		}
	}

	public void SetParticleAnimationSpeed(float speed)
	{
		foreach (ParticleSystem particleSystem in base.gameObject.GetComponentsInChildren<ParticleSystem>(false))
		{
			particleSystem.playbackSpeed = speed;
		}
	}

	public void Show(Quaternion rotation, bool shouldAnimateStripes, bool shouldRotateBackground)
	{
		if (!this.visible)
		{
			this.visible = true;
		}
		if (this.celebrationCloner.gameObject.activeSelf != shouldAnimateStripes)
		{
			this.celebrationCloner.gameObject.SetActive(shouldAnimateStripes);
		}
		this.shouldRotate = shouldRotateBackground;
		this.backgroundRoot.rotation = rotation;
		this.InvertParticlesDirection(false);
	}

	private void Update()
	{
		if (this.shouldRotate)
		{
			this.backgroundRoot.Rotate(0f, 0f, 45f * Time.deltaTime);
		}
	}

	public bool visible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			base.gameObject.SetActive(value);
		}
	}

	public Transform backgroundRoot;

	public Transform celebrationCloner;

	[SerializeField]
	private AnimationCurve _celebrationAnimationCurve;

	private bool shouldRotate;
}
