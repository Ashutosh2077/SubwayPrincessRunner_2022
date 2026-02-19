using System;
using UnityEngine;

[Serializable]
public class AchievementInfo
{
	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public AchievementInfo Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}

	public string icon;

	public RewardType rewardType;

	public int rewardAmount;
}
