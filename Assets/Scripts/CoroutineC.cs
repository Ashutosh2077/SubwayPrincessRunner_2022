using System;
using System.Collections;
using UnityEngine;

public class CoroutineC : MonoBehaviour
{
	public static CoroutineC Instance
	{
		get
		{
			if (CoroutineC._instance == null)
			{
				GameObject gameObject = new GameObject("CoroutineC");
				CoroutineC._instance = gameObject.AddComponent<CoroutineC>();
			}
			return CoroutineC._instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public Coroutine StartCoroutineC(IEnumerator coroutine)
	{
		return base.StartCoroutine(coroutine);
	}

	private static CoroutineC _instance;
}
