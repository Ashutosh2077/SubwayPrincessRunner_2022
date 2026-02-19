using System;
using UnityEngine;

public class AvatarEyeAnimation : MonoBehaviour
{
	public bool IsAnimating()
	{
		return this.animating;
	}

	public void ForceBlink()
	{
		this.waitForBlinkEndTime = Time.time;
		this.blinkEndTime = this.waitForBlinkEndTime + this.blinkTime;
	}

	public void ChangeMat(Material mat)
	{
		this.closedEyes.material = mat;
	}

	public void StartAnimatingEyes()
	{
		this.waitForBlinkEndTime = UnityEngine.Random.Range(this.blinkWaitTimeMin, this.blinkWaitTimeMax);
		this.blinkEndTime = this.waitForBlinkEndTime + this.blinkTime;
		this.animating = true;
	}

	public void StopAnimatingEyes()
	{
		this.animating = false;
		this.closedEyes.enabled = false;
	}

	private void Update()
	{
		if (this.animating)
		{
			this.UpdateEyeBlinking();
		}
		else
		{
			this.closedEyes.enabled = false;
		}
	}

	private void UpdateEyeBlinking()
	{
		if (this.closedEyes != null)
		{
			if (this.blinkWaitTimeMax >= this.blinkWaitTimeMin)
			{
				if (Time.time >= this.waitForBlinkEndTime)
				{
					if (Time.time < this.blinkEndTime)
					{
						if (!this.closedEyes.enabled)
						{
							this.closedEyes.enabled = true;
						}
					}
					else
					{
						this.closedEyes.enabled = false;
						this.waitForBlinkEndTime = Time.time + UnityEngine.Random.Range(this.blinkWaitTimeMin, this.blinkWaitTimeMax);
						this.blinkEndTime = this.waitForBlinkEndTime + this.blinkTime;
					}
				}
			}
			else
			{
				UnityEngine.Debug.Log("AvatarEyeAnimation: You need to make blinkWaitTimeMax larger then or equal blinkWaitTimeMin", this);
			}
		}
	}

	private bool animating;

	public Renderer closedEyes;

	public float blinkTime = 0.1f;

	public float blinkWaitTimeMax = 5.7f;

	public float blinkWaitTimeMin = 0.6f;

	private float waitForBlinkEndTime;

	private float blinkEndTime;
}
