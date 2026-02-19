using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;

public class DailyLandingAward
{
	public DailyLandingAward()
	{
		this.type = DailyLandingAward.DailyLandingRewardType.Coins;
		this.Amount = 1;
		this.chestType = Chest.Type.Normal;
	}

	public DailyLandingAward(DailyLandingAward award)
	{
		this.type = award.type;
		this.Amount = award.Amount;
		this.chestType = award.chestType;
	}

	public string ToJson()
	{
		return Json.Serialize(new Dictionary<string, object>
		{
			{
				"type",
				this.type.ToString()
			},
			{
				"Amount",
				this.Amount
			},
			{
				"chestType",
				this.chestType.ToString()
			}
		});
	}

	public DailyLandingAward Parse(string json)
	{
		IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
		if (dictionary.ContainsKey("type"))
		{
			try
			{
				this.type = (DailyLandingAward.DailyLandingRewardType)Enum.Parse(typeof(DailyLandingAward.DailyLandingRewardType), (string)dictionary["type"]);
			}
			catch (ArgumentException)
			{
				UnityEngine.Debug.Log(json + "Parse Error.");
			}
		}
		if (dictionary.ContainsKey("Amount"))
		{
			this.Amount = (int)((long)dictionary["Amount"]);
		}
		if (dictionary.ContainsKey("chestType"))
		{
			try
			{
				this.chestType = (Chest.Type)Enum.Parse(typeof(Chest.Type), (string)dictionary["chestType"]);
			}
			catch (ArgumentException)
			{
				UnityEngine.Debug.Log(json + "Parse Error.");
			}
		}
		return this;
	}

	public DailyLandingAward.DailyLandingRewardType type;

	public int Amount;

	public Chest.Type chestType;

	public enum DailyLandingRewardType
	{
		Coins,
		Chest,
		Keys
	}
}
