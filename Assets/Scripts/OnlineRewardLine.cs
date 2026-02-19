using System;
using UnityEngine;

public class OnlineRewardLine : MonoBehaviour
{
	public void Init()
	{
		this.zone = OnlineRewardManager.Instance.Zones[this.index];
		if (this.index - 1 >= 0 && this.index < OnlineRewardManager.Instance.Zones.Length)
		{
			this.lastZone = OnlineRewardManager.Instance.Zones[this.index - 1];
		}
		else
		{
			this.lastZone = null;
		}
		int num = this.zone.rewards.Length;
		for (int i = 0; i < num; i++)
		{
			OnlineReward onlineReward = this.zone.rewards[i];
			this.rewardHelpers[i].SetIcomAndNumber(this.zone.rewards[i].icon, this.zone.rewards[i].number);
		}
	}

	public void Show()
	{
		this.getLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_BUTTON_GET);
		this.timeLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_INTERVAL_TIME), this.zone.deadline / 60);
		this.RefreshSign();
		this.RefreshSlider();
		this.RefreshButton();
	}

	public void RefreshSlider()
	{
		int onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (this.zone.deadline > onlineTime)
		{
			if (this.lastZone == null)
			{
				this.slider.value = (float)onlineTime / (float)this.zone.deadline;
			}
			else if (onlineTime < this.lastZone.deadline)
			{
				this.slider.value = 0f;
			}
			else
			{
				this.slider.value = (float)(onlineTime - this.lastZone.deadline) / (float)(this.zone.deadline - this.lastZone.deadline);
			}
		}
		else
		{
			this.slider.value = 1f;
		}
	}

	public void RefreshSign()
	{
		if (this.zone.deadline > PlayerInfo.Instance.GetOnlineTime())
		{
			this.lockedSpr.enabled = true;
			this.gotSpr.enabled = false;
			this.openningSpr.enabled = false;
		}
		else if (!PlayerInfo.Instance.GetOnlineZonePayedOut(this.index))
		{
			this.gotSpr.enabled = false;
			this.openningSpr.enabled = true;
			this.lockedSpr.enabled = false;
		}
		else
		{
			this.gotSpr.enabled = true;
			this.openningSpr.enabled = false;
			this.lockedSpr.enabled = false;
		}
	}

	public void RefreshButton()
	{
		if (this.zone.deadline > PlayerInfo.Instance.GetOnlineTime())
		{
			this.maskSpr.enabled = false;
			this.getBtn.SetActive(false);
			this.unGetBtn.SetActive(true);
		}
		else if (!PlayerInfo.Instance.GetOnlineZonePayedOut(this.index))
		{
			this.maskSpr.enabled = false;
			this.getBtn.SetActive(true);
			this.unGetBtn.SetActive(false);
		}
		else
		{
			this.maskSpr.enabled = true;
			this.getBtn.SetActive(false);
			this.unGetBtn.SetActive(false);
		}
	}

	public void OnGetClick()
	{
		if (this.zone == null)
		{
			return;
		}
		int i = 0;
		int num = this.zone.rewards.Length;
		while (i < num)
		{
			OnlineReward onlineReward = this.zone.rewards[i];
			if (onlineReward != null)
			{
				switch (onlineReward.rewardType)
				{
				case OnlineRewardType.Coins:
					PlayerInfo.Instance.amountOfCoins += onlineReward.number;
					break;
				case OnlineRewardType.Keys:
					PlayerInfo.Instance.amountOfKeys += onlineReward.number;
					break;
				case OnlineRewardType.HeadSprint:
					PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, onlineReward.number);
					break;
				case OnlineRewardType.ScoreBooster:
					PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, onlineReward.number);
					break;
				}
			}
			i++;
		}
		PlayerInfo.Instance.SetOnlineZonePayedOut(this.index, true);
		OnlineRewardManager.Instance.PayedOut--;
		this.RefreshButton();
		this.RefreshSign();
	}

	private OnlineZone zone;

	private OnlineZone lastZone;

	[SerializeField]
	private int index;

	[SerializeField]
	private UISprite lockedSpr;

	[SerializeField]
	private UISprite openningSpr;

	[SerializeField]
	private UISprite gotSpr;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private OnLineRewardHelper[] rewardHelpers;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private UISprite maskSpr;

	[SerializeField]
	private GameObject getBtn;

	[SerializeField]
	private GameObject unGetBtn;
}
