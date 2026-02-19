using System;
using UnityEngine;

public class OnlineRewardsPopup : UIBaseScreen
{
	public override void Init()
	{
		int i = 0;
		int num = this.rewardLines.Length;
		while (i < num)
		{
			this.rewardLines[i].Init();
			i++;
		}
		this.lastIndex = 0;
		for (int j = 0; j < OnlineRewardManager.Instance.Zones.Length; j++)
		{
			if (this.onlineTime >= OnlineRewardManager.Instance.Zones[j].deadline)
			{
				this.lastIndex++;
			}
		}
		base.Init();
	}

	private void OnEnable()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_TITLE);
		this.timeLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_ONLINE_TIME);
	}

	public override void Show()
	{
		base.Show();
		int i = 0;
		int num = this.rewardLines.Length;
		while (i < num)
		{
			this.rewardLines[i].Show();
			i++;
		}
	}

	private void Update()
	{
		this.onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (this.onlineTime > 3600)
		{
			this.time.text = string.Format("{0:D2}:{1:D2}:{2:D2}", this.onlineTime / 3600, this.onlineTime / 60 % 60, this.onlineTime % 60);
		}
		else
		{
			this.time.text = string.Format("{0:D2}:{1:D2}", this.onlineTime / 60, this.onlineTime % 60);
		}
		if (this.lastIndex >= this.rewardLines.Length)
		{
			return;
		}
		this.line = this.rewardLines[this.lastIndex];
		this.line.RefreshSlider();
		if (this.onlineTime >= OnlineRewardManager.Instance.Zones[this.lastIndex].deadline)
		{
			this.line.RefreshButton();
			this.line.RefreshSign();
			this.lastIndex++;
		}
	}

	[SerializeField]
	private OnlineRewardLine[] rewardLines;

	[SerializeField]
	private UILabel time;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel timeLbl;

	private int onlineTime;

	private OnlineRewardLine line;

	private int lastIndex;
}
