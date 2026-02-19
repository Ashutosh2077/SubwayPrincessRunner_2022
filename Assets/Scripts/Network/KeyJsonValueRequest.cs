using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class KeyJsonValueRequest
	{
		private KeyJsonValueRequest()
		{
			this.jsonKeyValueList = new Dictionary<string, KeyJsonValueRequest.JsonData>();
		}

		public static KeyJsonValueRequest Instance
		{
			get
			{
				if (KeyJsonValueRequest._instance == null)
				{
					KeyJsonValueRequest._instance = new KeyJsonValueRequest();
				}
				return KeyJsonValueRequest._instance;
			}
		}

		public void UploadJsonData(string userId, string key, string json, Action<int, object> handle = null)
		{
			if (!this.jsonKeyValueList.ContainsKey(key))
			{
				this.jsonKeyValueList.Add(key, new KeyJsonValueRequest.JsonData(userId, key, json, handle));
			}
			KeyJsonValueRequest.JsonData @object = new KeyJsonValueRequest.JsonData(userId, key, json, handle);
			Dictionary<string, string> uploadJsonDataDict = NetworkConnect.Instance.GetUploadJsonDataDict(userId, key, json);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadJsonData, uploadJsonDataDict, new Action<string>(@object.UploadDataListener));
		}

		public void GetJsonData(string userId, string key, string jsonKey, Action<int, object> handle = null)
		{
			if (!this.jsonKeyValueList.ContainsKey(key))
			{
				this.jsonKeyValueList.Add(key, new KeyJsonValueRequest.JsonData(userId, key, null, handle));
			}
			KeyJsonValueRequest.JsonData @object = this.jsonKeyValueList[key];
			Dictionary<string, string> requestJsonDataDict = NetworkConnect.Instance.GetRequestJsonDataDict(userId, key, jsonKey);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetJsonData, requestJsonDataDict, new Action<string>(@object.GetDataListener));
		}

		public void GetAllJsonData(string userId, string key, Action<int, object> handle = null)
		{
			if (!this.jsonKeyValueList.ContainsKey(key))
			{
				this.jsonKeyValueList.Add(key, new KeyJsonValueRequest.JsonData(userId, key, null, handle));
			}
			KeyJsonValueRequest.JsonData @object = this.jsonKeyValueList[key];
			Dictionary<string, string> requestAllJsonDataDict = NetworkConnect.Instance.GetRequestAllJsonDataDict(userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetAllInnerJsonData, requestAllJsonDataDict, new Action<string>(@object.GetDataListener));
		}

		private static KeyJsonValueRequest _instance;

		private Dictionary<string, KeyJsonValueRequest.JsonData> jsonKeyValueList;

		public class JsonData
		{
			public JsonData(string userId, string key, string json, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				this.json = json;
				this.onRespondResult = onSuccess;
			}

			public void UploadDataListener(string s)
			{
				UnityEngine.Debug.Log("Key:" + this.key + ", UploadDataListener:" + s);
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

			public void GetDataListener(string s)
			{
				UnityEngine.Debug.Log("Key:" + this.key + ", GetDataListener:" + s);
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
						this.onRespondResult(-1, "Is not contains data!");
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

			public string json;

			public Action<int, object> onRespondResult;
		}
	}
}
