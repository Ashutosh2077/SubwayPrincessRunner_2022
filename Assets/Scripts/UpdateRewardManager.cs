using System;
using UnityEngine;

public class UpdateRewardManager : MonoBehaviour
{
	public static UpdateRewardManager Instance
	{
		get
		{
			if (UpdateRewardManager._instance == null)
			{
				UpdateRewardManager._instance = Utils.FindObject<UpdateRewardManager>();
			}
			return UpdateRewardManager._instance;
		}
	}

	public int GetUpdateRewardInfo(ref UpdateReward one, ref UpdateReward two)
	{
		int num = PlayerInfo.Instance.updateRewardIndex;
		num %= this.updateRewards.Length;
		one = this.updateRewards[num];
		num = (num + 1) % this.updateRewards.Length;
		two = this.updateRewards[num];
		return num;
	}

	public void GetReward(UpdateReward reward, int multiple)
	{
		if (reward == null)
		{
			return;
		}
		int num = reward.number * multiple;
		switch (reward.rewardType)
		{
		case UpdateRewardType.Coins:
			PlayerInfo.Instance.amountOfCoins += num;
			break;
		case UpdateRewardType.Keys:
			PlayerInfo.Instance.amountOfKeys += num;
			break;
		case UpdateRewardType.HeadSprint:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, num);
			break;
		case UpdateRewardType.ScoreBooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, num);
			break;
		case UpdateRewardType.Helmet:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, num);
			break;
		case UpdateRewardType.Chest:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.chest, num);
			break;
		case UpdateRewardType.LeeToken:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.lee, num);
			break;
		case UpdateRewardType.TurtlefokToken:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.turtlefok, num);
			break;
		}
	}

	private static UpdateRewardManager _instance;

	[SerializeField]
	private UpdateReward[] updateRewards;
}
