using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RewardManager
{
	private static void AddChestToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		PlayerInfo instance = PlayerInfo.Instance;
		bool flag = instance.pendingRewards.Exists((CelebrationReward mb) => mb.Uid == reward.Uid);
		if (reward.Uid <= 0L || !flag)
		{
			reward.Uid = DateTime.Now.Ticks + (long)UnityEngine.Random.Range(0, int.MaxValue);
		}
		if (!flag)
		{
			instance.AddPendingReward(reward);
			if (shouldSaveToDisk)
			{
				instance.SaveIfDirty();
			}
		}
	}

	public static void AddRewardToUnlock(CelebrationRewardOrigin origin)
	{
		CelebrationReward celebrationReward = null;
		if (origin == CelebrationRewardOrigin.Chest)
		{
			celebrationReward = Chest.Roll(Chest.Type.Normal);
		}
		else if (origin == CelebrationRewardOrigin.SuperChest)
		{
			celebrationReward = Chest.Roll(Chest.Type.Super);
		}
		else if (origin == CelebrationRewardOrigin.ChestMini)
		{
			celebrationReward = Chest.Roll(Chest.Type.Mini);
		}
		else
		{
			UnityEngine.Debug.LogWarning("RewardManger Use this method for unknown prize only: MB, SMB,WCMB,WCSMB and MMB.");
		}
		if (celebrationReward != null)
		{
			celebrationReward.CelebrationRewardOrigin = origin;
			RewardManager.AddChestToUnlock(celebrationReward, true);
		}
	}

	public static void AddRewardToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Notset && reward.rewardType != CelebrationRewardType._notset)
		{
			RewardManager.AddChestToUnlock(reward, true);
		}
		else
		{
			UnityEngine.Debug.LogWarning("RewardManager You can't add a not set reward to unlock");
		}
	}

	public static CelebrationReward[] GetRewardsToUnlockForCelebration()
	{
		if (PlayerInfo.Instance.lastAddedReward != null && !RewardManager.canShowMultipleQueuedCelebrations)
		{
			return new CelebrationReward[]
			{
				PlayerInfo.Instance.lastAddedReward
			};
		}
		return PlayerInfo.Instance.pendingRewards.ToArray();
	}

	public static List<CelebrationReward> GetWeeklyHuntRewardsToUnlock()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		List<CelebrationReward> list = new List<CelebrationReward>();
		for (int i = 0; i < instance.pendingRewards.Count; i++)
		{
			CelebrationReward item = instance.pendingRewards[i];
			list.Add(item);
		}
		return list;
	}

	private static bool IsRewardChest(CelebrationReward reward)
	{
		return reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest;
	}

	public static void PayoutNonChestRewards()
	{
		List<CelebrationReward> list = new List<CelebrationReward>(PlayerInfo.Instance.pendingRewards);
		list = (from celebrationReward in list
		where !RewardManager.IsRewardChest(celebrationReward)
		select celebrationReward).ToList<CelebrationReward>();
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			UIScreenController.Instance.PayoutCelebrationReward(list[i]);
			i++;
		}
	}

	public static void RewardPayedOut(CelebrationReward celebrationReward)
	{
		PlayerInfo.Instance.RemovePendingReward(celebrationReward);
	}

	public static int rewardsToUnlockCount
	{
		get
		{
			return PlayerInfo.Instance.pendingRewards.Count;
		}
	}

	public static bool canShowMultipleQueuedCelebrations;
}
