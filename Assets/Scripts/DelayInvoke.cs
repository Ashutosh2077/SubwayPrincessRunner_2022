using System;
using System.Collections;
using UnityEngine;

public class DelayInvoke : MonoBehaviour
{
	private void Awake()
	{
		base.enabled = false;
		DelayInvoke.instance = this;
	}

	private static void init()
	{
		if (!DelayInvoke.hasInit)
		{
			DelayInvoke.hasInit = true;
			GameObject gameObject = new GameObject("DelayInvoke");
			gameObject.AddComponent<DelayInvoke>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static Coroutine delayDo(Action action, float delayTime, bool ignoreTimeScale = false)
	{
		DelayInvoke.init();
		if (ignoreTimeScale)
		{
			return DelayInvoke.instance.StartCoroutine(DelayInvoke.startIgnoreTimeScale(action, delayTime));
		}
		return DelayInvoke.instance.StartCoroutine(DelayInvoke.start(action, delayTime));
	}

	public static IEnumerator start(Action action, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		if (action != null)
		{
			action();
		}
		yield break;
	}

	public static IEnumerator startIgnoreTimeScale(Action action, float delayTime)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + delayTime)
		{
			yield return null;
		}
		if (action != null)
		{
			action();
		}
		yield break;
	}

	public static void stopCoroutine(Coroutine cor)
	{
		DelayInvoke.init();
		if (cor != null)
		{
			DelayInvoke.instance.StopCoroutine(cor);
		}
	}

	private static bool hasInit;

	private static DelayInvoke instance;
}
