using System;
using UnityEngine;

namespace Network
{
	public class Subscription : StringKeyValue
	{
		public Subscription()
		{
			this._key = "subscription";
			this._playerPrefsKey = "Network_Subscription";
			this._value = PlayerPrefs.GetString(this._playerPrefsKey, string.Empty);
			if (string.IsNullOrEmpty(this._value))
			{
				base.GetStringValue();
			}
		}

		protected override void UploadValueBeforeGetValue()
		{
			base.UploadKeyValue_Force("no");
		}
	}
}
