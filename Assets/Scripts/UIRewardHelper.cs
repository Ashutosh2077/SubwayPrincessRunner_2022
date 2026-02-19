using System;
using UnityEngine;

public class UIRewardHelper : MonoBehaviour
{
	public void RefreshUI(UpdateReward reward)
	{
		if (reward == null)
		{
			return;
		}
		this.reward = reward;
		if (this.uiReward.Icon != null)
		{
			this.uiReward.Icon.spriteName = reward.icon;
		}
		if (this.uiReward.Number != null)
		{
			this.uiReward.Number.text = "X " + reward.number;
			this.uiReward.Number.color = reward.color;
		}
	}

	public void GetReward(int multiple = 1)
	{
		if (this.reward == null)
		{
			return;
		}
		UpdateRewardManager.Instance.GetReward(this.reward, multiple);
	}

	public UIReward uiReward;

	private UpdateReward reward;
}
