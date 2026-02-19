using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class CountryCode : StringKeyValue
	{
		public CountryCode()
		{
			this._key = "countryCode";
			this._playerPrefsKey = "Network_CountryCode";
			this._value = PlayerPrefs.GetString(this._playerPrefsKey, "NotSet");
			if ("NotSet".Equals(this._value))
			{
				base.GetStringValue();
			}
		}

		protected override void UploadValueBeforeGetValue()
		{
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetCountryCode, null, new Action<string>(this.GetCountryCodeListener));
		}

		private void GetCountryCodeListener(string s)
		{
			UnityEngine.Debug.Log("GetCountryCodeListener:" + s);
			if (string.IsNullOrEmpty(s))
			{
				return;
			}
			IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
			if (dictionary == null || !dictionary.ContainsKey("countryCode"))
			{
				return;
			}
			base.UploadKeyValue_Force((string)dictionary["countryCode"]);
		}

		public override string Value
		{
			get
			{
				if ("NotSet".Equals(this._value))
				{
					base.GetStringValue();
				}
				return this._value;
			}
			protected set
			{
				base.Value = value;
			}
		}
	}
}
