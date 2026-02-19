using System;
using UnityEngine;

[Serializable]
public class TopRunData
{
	public TopRunData()
	{
		UnityEngine.Random.InitState(DateTime.UtcNow.Millisecond);
		this.playerName = "Player" + UnityEngine.Random.Range(1000, 9999) + UnityEngine.Random.Range(10, 99);
		this.country = "NOTSET";
		this.weekstring = "201904";
		this.hasGoldMedal = 0;
		this.hasSliverMedal = 0;
		this.hasBronzeMedal = 0;
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public TopRunData Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}

	public string playerName;

	public string country;

	public string weekstring;

	public int hasGoldMedal;

	public int hasSliverMedal;

	public int hasBronzeMedal;
}
