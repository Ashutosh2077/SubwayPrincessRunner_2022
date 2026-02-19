using System;
using UnityEngine;

public class RealTimeTracker : MonoBehaviour
{
	private static void Spawn()
	{
		GameObject gameObject = new GameObject("_RealTimeTracker");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		RealTimeTracker.mInst = gameObject.AddComponent<RealTimeTracker>();
		RealTimeTracker.mInst.mRealTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		this.mRealDelta = Mathf.Clamp01(realtimeSinceStartup - this.mRealTime);
		this.mRealTime = realtimeSinceStartup;
	}

	public static float deltaTime
	{
		get
		{
			if (RealTimeTracker.mInst == null)
			{
				RealTimeTracker.Spawn();
			}
			return RealTimeTracker.mInst.mRealDelta;
		}
	}

	public static float time
	{
		get
		{
			if (RealTimeTracker.mInst == null)
			{
				RealTimeTracker.Spawn();
			}
			return RealTimeTracker.mInst.mRealTime;
		}
	}

	private static RealTimeTracker mInst;

	private float mRealDelta;

	private float mRealTime;
}
