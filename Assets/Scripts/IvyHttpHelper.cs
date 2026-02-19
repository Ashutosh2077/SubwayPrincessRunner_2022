using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class IvyHttpHelper : MonoBehaviour
{
	public static IvyHttpHelper Instance
	{
		get
		{
			IvyHttpHelper result;
			if ((result = IvyHttpHelper._Instance) == null)
			{
				result = (IvyHttpHelper._Instance = (UnityEngine.Object.FindObjectOfType<IvyHttpHelper>() ?? new GameObject("HttpHelper").AddComponent<IvyHttpHelper>()));
			}
			return result;
		}
	}

	private void Awake()
	{
		if (IvyHttpHelper._Instance == null)
		{
			IvyHttpHelper._Instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public Coroutine MakeRequest(string url, Action<string> callback, Dictionary<string, string> param)
	{
		Coroutine result;
		try
		{
			result = base.StartCoroutine("PostRequest", new IvyHttpHelper.PostRequestData(url, param, callback));
		}
		catch
		{
			result = null;
		}
		return result;
	}

	private IEnumerator PostRequest(IvyHttpHelper.PostRequestData data)
	{
		WWWForm form = null;
		if (data != null && data.post != null && data.post.Count > 0)
		{
			form = new WWWForm();
			foreach (KeyValuePair<string, string> keyValuePair in data.post)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					form.AddField(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
		WWW www = null;
		if (form == null)
		{
			www = new WWW(data.url);
		}
		else
		{
			www = new WWW(data.url, form);
		}
		yield return www;
		if (data.callback != null)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				data.callback(www.text);
			}
			else
			{
				data.callback(null);
			}
		}
		yield break;
	}

	public Coroutine GetRequest(string url, Action<string> callback, Dictionary<string, string> param)
	{
		Coroutine result;
		try
		{
			result = base.StartCoroutine("GetRequest_C", new IvyHttpHelper.PostRequestData(url, param, callback));
		}
		catch
		{
			result = null;
		}
		return result;
	}

	private IEnumerator GetRequest_C(IvyHttpHelper.PostRequestData data)
	{
		StringBuilder parameters = new StringBuilder(data.url);
		if (data != null && data.post != null && data.post.Count > 0 && data.post != null && data.post.Count > 0)
		{
			parameters.Append("?");
			foreach (KeyValuePair<string, string> keyValuePair in data.post)
			{
				parameters.Append(keyValuePair.Key).Append('=').Append(keyValuePair.Value).Append('&');
			}
			parameters.Remove(parameters.Length - 1, 1);
		}
		WWW www = new WWW(parameters.ToString());
		while (!www.isDone)
		{
			yield return null;
		}
		if (data.callback != null)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				data.callback(www.text);
			}
			else
			{
				data.callback(null);
			}
		}
		yield break;
	}

	public float GetProgress(WWW www)
	{
		float result;
		if (www != null && www.isDone && string.IsNullOrEmpty(www.error))
		{
			result = 1f;
		}
		else if (www != null && www.isDone)
		{
			result = 0.9f;
		}
		else if (www != null)
		{
			result = www.progress;
		}
		else
		{
			result = 0f;
		}
		return result;
	}

	private static IvyHttpHelper _Instance;

	private class PostRequestData
	{
		public PostRequestData(string url, Dictionary<string, string> post, Action<string> callback)
		{
			this.url = url;
			this.post = post;
			this.callback = callback;
		}

		public string url;

		public Dictionary<string, string> post;

		public Action<string> callback;
	}
}
