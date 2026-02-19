using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class StringKeyValueRequest
	{
		public static StringKeyValueRequest Instance
		{
			get
			{
				if (StringKeyValueRequest._instance == null)
				{
					StringKeyValueRequest._instance = new StringKeyValueRequest();
				}
				return StringKeyValueRequest._instance;
			}
		}

		public void UploadStringData(string userId, string key, string value, Action<int, object> handle = null)
		{
			StringKeyValueRequest.StringData @object = new StringKeyValueRequest.StringData(userId, key, value, handle);
			Dictionary<string, string> uploadStringDataDict = NetworkConnect.Instance.GetUploadStringDataDict(userId, key, value);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadStringData, uploadStringDataDict, new Action<string>(@object.UploadStringDataListener));
		}

		public void GetStringData(string userId, string key, Action<int, object> handle = null)
		{
			StringKeyValueRequest.StringData @object = new StringKeyValueRequest.StringData(userId, key, null, handle);
			Dictionary<string, string> requestStringDataDict = NetworkConnect.Instance.GetRequestStringDataDict(userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetStringData, requestStringDataDict, new Action<string>(@object.GetStringDataListener));
		}

		private static StringKeyValueRequest _instance;

		public class StringData
		{
			public StringData(string userId, string key, string value, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				this.value = value;
				this.onRespondResult = onSuccess;
			}

			public void UploadStringDataListener(string s)
			{
				UnityEngine.Debug.Log("Key:" + this.key + ", UploadStringDataListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (this.onRespondResult != null)
					{
						this.onRespondResult(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)((long)dictionary["status"]) == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						UnityEngine.Debug.Log(dictionary["msg"]);
					}
					if (this.onRespondResult != null)
					{
						this.onRespondResult(0, dictionary["msg"]);
					}
					return;
				}
				if (this.onRespondResult != null)
				{
					this.onRespondResult(1, null);
				}
			}

			public void GetStringDataListener(string s)
			{
				UnityEngine.Debug.Log("Key:" + this.key + ", GetStringDataListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (this.onRespondResult != null)
					{
						this.onRespondResult(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)((long)dictionary["status"]) == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						UnityEngine.Debug.Log(dictionary["msg"]);
					}
					if (this.onRespondResult != null)
					{
						this.onRespondResult(0, dictionary["msg"]);
					}
					return;
				}
				if (!dictionary.ContainsKey("data"))
				{
					if (this.onRespondResult != null)
					{
						this.onRespondResult(-1, "error message");
					}
					return;
				}
				if (this.onRespondResult != null)
				{
					this.onRespondResult(1, dictionary["data"]);
				}
			}

			public string userId;

			public string key;

			public string value;

			private Action<int, object> onRespondResult;
		}
	}
}
