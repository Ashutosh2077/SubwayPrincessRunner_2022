using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class NetworkRequest : MonoBehaviour
	{
		public static NetworkRequest Instance
		{
			get
			{
				if (NetworkRequest._instance == null)
				{
					NetworkRequest._instance = new GameObject("NetworkRequest").AddComponent<NetworkRequest>();
				}
				return NetworkRequest._instance;
			}
		}

		private void Start()
		{
			this.commandQueue = new Dictionary<int, Queue<NetworkRequest.RequestData>>();
		}

		public void Request(NetworkConnect.RequestCommand command, Dictionary<string, string> dict, Action<string> onCompleted)
		{
			try
			{
				if (command == NetworkConnect.RequestCommand.GetCountryCode)
				{
					base.StartCoroutine(this.RequestCountryCode(null, onCompleted));
				}
				else if (command != NetworkConnect.RequestCommand.UploadFile && command != NetworkConnect.RequestCommand.GetFileUrl)
				{
					string url = NetworkConnect.Instance.Url(command);
					base.StartCoroutine(this.PostRequest(url, dict, onCompleted));
				}
			}
			catch
			{
				UnityEngine.Debug.Log("Request Error!");
			}
		}

		private IEnumerator PostRequest(string url, Dictionary<string, string> dict, Action<string> onCompleted)
		{
			WWWForm form = null;
			if (dict != null && dict.Count > 0)
			{
				form = new WWWForm();
				foreach (KeyValuePair<string, string> keyValuePair in dict)
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
				www = new WWW(url);
			}
			else
			{
				www = new WWW(url, form);
			}
			yield return www;
			if (onCompleted != null)
			{
				if (string.IsNullOrEmpty(www.error))
				{
					onCompleted(www.text);
				}
				else
				{
					onCompleted(null);
				}
			}
			yield break;
		}

		public IEnumerator RequestCountryCode(string ipStr, Action<string> listener)
		{
			WWW www = new WWW("http://ip-api.com/json/" + ipStr);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && listener != null)
			{
				listener(www.text);
			}
			yield break;
		}

		public IEnumerator RequestIpAddress(Action<string> listener)
		{
			WWW www = new WWW("http://whatismyip.akamai.com/");
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				string obj = null;
				try
				{
					obj = www.text;
				}
				finally
				{
					if (listener != null)
					{
						listener(obj);
					}
				}
			}
			yield break;
		}

		private static NetworkRequest _instance;

		public Dictionary<int, Queue<NetworkRequest.RequestData>> commandQueue;

		[Serializable]
		public class RequestData
		{
			public NetworkConnect.RequestCommand command;

			public Dictionary<string, string> dataDic;

			public Action<string> listener;

			public int order;
		}
	}
}
