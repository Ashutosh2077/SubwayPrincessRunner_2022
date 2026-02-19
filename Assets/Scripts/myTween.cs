using System;
using System.Collections;
using UnityEngine;

public class myTween
{
	public static IEnumerator RealtimeTo(float duration, float startValue, float endValue, Action<float> callback)
	{
		float start = Time.realtimeSinceStartup;
		float end = start + duration;
		float speed = 1f / duration;
		float startSpeed = start / duration;
		for (float time = Time.realtimeSinceStartup; time < end; time = Time.realtimeSinceStartup)
		{
			callback(Mathf.Lerp(startValue, endValue, time * speed - startSpeed));
			yield return null;
		}
		callback(endValue);
		yield break;
	}

	public static IEnumerator To(float duration, Action<float> callback)
	{
		return myTween.To(duration, 0f, 1f, callback);
	}

	public static IEnumerator To(float duration, float startValue, float endValue, Action<float> callback)
	{
		float start = Time.time;
		float end = start + duration;
		float durationInv = 1f / duration;
		float startMulDurationInv = start / duration;
		for (float t = Time.time; t < end; t = Time.time)
		{
			callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
			yield return null;
		}
		callback(endValue);
		if (myTween.complete != null)
		{
			myTween.complete();
		}
		yield break;
	}

	public static Action complete;
}
