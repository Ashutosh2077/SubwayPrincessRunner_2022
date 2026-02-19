using System;
using System.Collections.Generic;

namespace Network
{
	public abstract class KeyJsonValue
	{
		public KeyJsonValue()
		{
			this._rule = new SynchroniseRule(60f);
		}

		public virtual void Reset()
		{
			this._rule.Reset();
		}

		public void GetJsonData()
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			KeyJsonValueRequest.Instance.GetAllJsonData(SecondManager.Instance.userId, this._key, new Action<int, object>(this.GetJsonDataCallback));
			this._rule.Request();
		}

		private void GetJsonDataCallback(int status, object obj)
		{
			this._rule.Respond();
			if (status == -1)
			{
				return;
			}
			if (status == 0)
			{
				return;
			}
			this._rule.hasGotInitValue = true;
			string userId = SecondManager.Instance.userId;
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey(userId))
			{
				if (dictionary[userId] is bool)
				{
					this.Parse(null);
					return;
				}
				this.Parse(dictionary[userId]);
			}
		}

		protected virtual void Parse(object obj)
		{
		}

		protected virtual string ToJson()
		{
			return null;
		}

		public void UploadJson(string json)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			KeyJsonValueRequest.Instance.UploadJsonData(SecondManager.Instance.userId, this._key, json, new Action<int, object>(this.UploadCallback));
		}

		protected virtual void UploadCallback(int status, object obj)
		{
		}

		protected virtual void OnValueChange()
		{
			if (this.onValueChange != null)
			{
				this.onValueChange();
			}
		}

		public SynchroniseRule rule
		{
			get
			{
				return this._rule;
			}
		}

		protected string _key;

		protected SynchroniseRule _rule;

		public Action onValueChange;
	}
}
