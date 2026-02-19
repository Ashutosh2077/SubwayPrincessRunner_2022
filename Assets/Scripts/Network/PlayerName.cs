using System;
using UnityEngine;

namespace Network
{
	public class PlayerName : StringKeyValue
	{
		public PlayerName()
		{
			this._key = "playerName";
			this._playerPrefsKey = "Network_PlayerName";
			this._value = PlayerPrefs.GetString(this._playerPrefsKey, "player");
			if ("player".Equals(this._value))
			{
				base.GetStringValue();
			}
		}

		protected override void UploadValueBeforeGetValue()
		{
			base.UploadKeyValue_Force(PlayerInfo.Instance.TopRunData.playerName);
		}
	}
}
