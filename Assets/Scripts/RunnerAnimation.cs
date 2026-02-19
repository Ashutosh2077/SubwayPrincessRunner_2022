using System;
using UnityEngine;

public class RunnerAnimation : MonoBehaviour
{
	public void SetAnimationSpeedEvent(AnimationEvent animEvent)
	{
		if (!RunnerAnimation.addedListeners)
		{
			RunnerAnimation.addedListeners = true;
			animEvent.animationState.speed = 1f + (Game.Instance.NormalizedGameSpeed - 1f) * this.AnimationSpeedUpFactor;
		}
	}

	private static bool addedListeners;

	public float AnimationSpeedUpFactor = 0.5f;
}
