using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSceneManager
{
	public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode, Action update, Action finish)
	{
		AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, mode);
		while (!ao.isDone)
		{
			if (update != null)
			{
				update();
			}
			yield return null;
		}
		if (finish != null)
		{
			finish();
		}
		ao = null;
		yield break;
	}

	public IEnumerator UnloadSceneAsync(string sceneName, Action update, Action finish)
	{
		AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneName);
		while (!ao.isDone)
		{
			if (update != null)
			{
				update();
			}
			yield return null;
		}
		if (finish != null)
		{
			finish();
		}
		ao = null;
		yield break;
	}
}
