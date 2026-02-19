using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public abstract class StringKeyValue
	{
		public StringKeyValue()
		{
			this._rule = new SynchroniseRule(60f);
		}

		public void GetStringValue()
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			StringKeyValueRequest.Instance.GetStringData(SecondManager.Instance.userId, this._key, new Action<int, object>(this.GetValueCallback));
			this._rule.Request();
		}

		private void GetValueCallback(int status, object obj)
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
			if (this.forceUpload)
			{
				return;
			}
			string userId = SecondManager.Instance.userId;
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey(userId))
			{
				if (dictionary[userId] is string)
				{
					this._rule.hasGotInitValue = true;
					this.Value = (string)dictionary[userId];
				}
				else if (dictionary[userId] is bool)
				{
					this.UploadValueBeforeGetValue();
				}
			}
		}

		protected virtual void UploadValueBeforeGetValue()
		{
		}

		public void UploadKeyValue_Strict(string value)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!this._rule.hasGotInitValue)
			{
				return;
			}
			if (string.IsNullOrEmpty(value) || value.Equals(this._value))
			{
				return;
			}
			this._valueTmp = value;
			StringKeyValueRequest.Instance.UploadStringData(SecondManager.Instance.userId, this._key, this._valueTmp, new Action<int, object>(this.UploadCallback));
		}

		public void UploadKeyValue_Force(string value)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (string.IsNullOrEmpty(value) || value.Equals(this._value))
			{
				return;
			}
			this._valueTmp = value;
			StringKeyValueRequest.Instance.UploadStringData(SecondManager.Instance.userId, this._key, this._valueTmp, new Action<int, object>(this.UploadCallback));
		}

		protected virtual void UploadCallback(int status, object obj)
		{
			if (status == -1)
			{
				return;
			}
			if (status == 0)
			{
				return;
			}
			this._rule.hasGotInitValue = true;
			this.forceUpload = true;
			this.Value = this._valueTmp;
		}

		public void Synchronise(string value)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			this._rule.hasGotInitValue = true;
			this.Value = value;
		}

		public virtual void OnValueChange()
		{
			PlayerPrefs.SetString(this._playerPrefsKey, this._value);
			if (this.onValueChange != null)
			{
				this.onValueChange();
			}
		}

		public virtual string Value
		{
			get
			{
				return this._value;
			}
			protected set
			{
				if (value.Equals(this._value))
				{
					return;
				}
				this._value = value;
				this.OnValueChange();
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

		protected string _value;

		protected string _valueTmp;

		protected string _playerPrefsKey;

		protected SynchroniseRule _rule;

		public Action onValueChange;

		protected bool forceUpload;
	}
}
