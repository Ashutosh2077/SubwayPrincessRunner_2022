using System;
using UnityEngine;

public class FreeRewardManager : MonoBehaviour
{
	public FreeRewardPopupData GetRewardPopupData()
	{
		return this._popupData;
	}

	public void SetFreeRewardType(RewardType type, Action callback = null, int rewardNum = 0)
	{
		this._popupData = new FreeRewardPopupData();
		this._popupData.rewardType = type;
		this._popupData.payReward = true;
		this._popupData.getCallback = callback;
		switch (type)
		{
		case RewardType.coins:
			this._popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward;
			this._popupData.popupDescription = string.Format("{0} Coins Reward", this._popupData.num);
			this._popupData.startGame = false;
			this._popupData.showAd = true;
			this._popupData.useCloseBtn = true;
			break;
		case RewardType.keys:
			this._popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward;
			this._popupData.popupDescription = string.Format("{0} Gems Reward", this._popupData.num);
			this._popupData.startGame = false;
			this._popupData.showAd = true;
			this._popupData.useCloseBtn = true;
			break;
		case RewardType.viewcoins:
			this._popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward;
			this._popupData.popupDescription = string.Format("{0} Coins Reward", this._popupData.num);
			this._popupData.startGame = false;
			this._popupData.showAd = false;
			this._popupData.useCloseBtn = false;
			break;
		case RewardType.viewkeys:
			this._popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward;
			this._popupData.popupDescription = string.Format("{0} Gems Reward", this._popupData.num);
			this._popupData.startGame = false;
			this._popupData.showAd = false;
			this._popupData.useCloseBtn = false;
			break;
		case RewardType.doublecoins:
			if (rewardNum != 0)
			{
				this._popupData.num = rewardNum;
			}
			else
			{
				this._popupData.num = GameStats.Instance.coins;
			}
			this._popupData.popupDescription = string.Format("{0} Coins Reward", GameStats.Instance.coins);
			this._popupData.startGame = false;
			this._popupData.showAd = false;
			this._popupData.useCloseBtn = false;
			break;
		case RewardType.headstart2000:
			this._popupData.popupDescription = "1 headstart2000 Reward";
			this._popupData.num = 1;
			this._popupData.startGame = true;
			this._popupData.showAd = false;
			this._popupData.useCloseBtn = true;
			break;
		case RewardType.scorebooster:
			this._popupData.popupDescription = "1 scorebooster Reward";
			this._popupData.num = 1;
			this._popupData.startGame = true;
			this._popupData.showAd = false;
			this._popupData.useCloseBtn = true;
			break;
		}
		if (this._popupData.showAd && !PlayerInfo.Instance.hasSubscribed && !Game.Instance.show20sAd && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.ShowAd("passlevel");
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(RewardType type, int amount, Action callback = null)
	{
		this._popupData = new FreeRewardPopupData();
		this._popupData.rewardType = type;
		this._popupData.payReward = true;
		this._popupData.startGame = false;
		this._popupData.showAd = false;
		this._popupData.useCloseBtn = false;
		this._popupData.getCallback = callback;
		switch (type)
		{
		case RewardType.coins:
		case RewardType.viewcoins:
			this._popupData.num = amount;
			this._popupData.popupDescription = string.Format("{0} Coins Reward", amount);
			break;
		case RewardType.keys:
		case RewardType.viewkeys:
			this._popupData.num = amount;
			this._popupData.popupDescription = string.Format("{0} Gems Reward", amount);
			break;
		case RewardType.headstart2000:
			this._popupData.popupDescription = string.Format("{0} headstart2000s Reward", amount);
			this._popupData.num = amount;
			break;
		case RewardType.scorebooster:
			this._popupData.popupDescription = string.Format("{0} scoreboosters Reward", amount);
			this._popupData.num = amount;
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(DailyLandingAward award, Action callback = null)
	{
		this._popupData = new FreeRewardPopupData();
		this._popupData.payReward = true;
		this._popupData.startGame = false;
		this._popupData.showAd = false;
		this._popupData.useCloseBtn = false;
		this._popupData.getCallback = callback;
		DailyLandingAward.DailyLandingRewardType type = award.type;
		if (type != DailyLandingAward.DailyLandingRewardType.Coins)
		{
			if (type == DailyLandingAward.DailyLandingRewardType.Keys)
			{
				this._popupData.rewardType = RewardType.keys;
				this._popupData.num = award.Amount;
				this._popupData.popupDescription = string.Format(" {0} Gems Reward", this._popupData.num);
			}
		}
		else
		{
			this._popupData.rewardType = RewardType.coins;
			this._popupData.num = award.Amount;
			this._popupData.popupDescription = string.Format(" {0} Coins Reward", this._popupData.num);
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(WheelReward reward, Action callback = null)
	{
		this._popupData = new FreeRewardPopupData();
		this._popupData.startGame = false;
		this._popupData.showAd = false;
		this._popupData.useCloseBtn = false;
		this._popupData.payReward = false;
		this._popupData.getCallback = callback;
		switch (reward.type)
		{
		case WheelRewardType.Coin:
			this._popupData.rewardType = RewardType.coins;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Coins Reward", reward.count);
			break;
		case WheelRewardType.Key:
			this._popupData.rewardType = RewardType.keys;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Gems Reward", reward.count);
			break;
		case WheelRewardType.Scorebooster:
			this._popupData.rewardType = RewardType.scorebooster;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Scoreboosters Reward", reward.count);
			break;
		case WheelRewardType.Headstart:
			this._popupData.rewardType = RewardType.headstart2000;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Headstarts Reward", reward.count);
			break;
		case WheelRewardType.LeeSymbol:
			this._popupData.rewardType = RewardType.leeSymbol;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Symbols Of Lee Reward", reward.count);
			break;
		case WheelRewardType.TurtlefokSymbol:
			this._popupData.rewardType = RewardType.turtlefokSymbol;
			this._popupData.num = reward.count;
			this._popupData.popupDescription = string.Format("{0} Symbols Of Turtlefok Reward", reward.count);
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public static FreeRewardManager Instance
	{
		get
		{
			if (FreeRewardManager._instance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "FreeRewardManager";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				FreeRewardManager._instance = gameObject.AddComponent<FreeRewardManager>();
			}
			return FreeRewardManager._instance;
		}
	}

	private static FreeRewardManager _instance;

	public FreeRewardPopupData _popupData;
}
