using System;
using UnityEngine;

public class NewUpdatePopup : UIBaseScreen
{
	public override void Init()
	{
		this.updateReward1 = new UpdateReward();
		this.updateReward2 = new UpdateReward();
		UpdateRewardManager.Instance.GetUpdateRewardInfo(ref this.updateReward1, ref this.updateReward2);
		base.Init();
	}

	private void RefreshLabel()
	{
		this.updateLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_BUTTON_UPDATEB);
		this.rewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_GET_FREE_REWARD_TITLE);
	}

	public override void Show()
	{
		base.Show();
		this.index = UpdateRewardManager.Instance.GetUpdateRewardInfo(ref this.updateReward1, ref this.updateReward2);
		this.uiReward1.RefreshUI(this.updateReward1);
		this.uiReward2.RefreshUI(this.updateReward2);
		if (NewUpdatePopup.ShowUpdate)
		{
			this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_TITLE);
			this.contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_EXPLAIN);
			this.updateGo.SetActive(true);
			this.getRewardGo.SetActive(false);
		}
		else
		{
			this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_REWARD_TITLE);
			this.contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_REWARD_EXPLAIN);
			this.updateGo.SetActive(false);
			this.getRewardGo.SetActive(true);
		}
		this.RefreshLabel();
	}

	public void GetApp()
	{
		RiseSdk.Instance.GetApp(RiseSdk.Instance.GetConfig(10));
	}

	public void GetReward()
	{
		UpdateRewardManager.Instance.GetReward(this.updateReward1, 1);
		UpdateRewardManager.Instance.GetReward(this.updateReward2, 1);
		PlayerInfo.Instance.updateRewardIndex = this.index;
		UIScreenController.Instance.ClosePopup(null);
	}

	public static bool ShowUpdate;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel contentLbl;

	[SerializeField]
	private UILabel updateLbl;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UIRewardHelper uiReward1;

	[SerializeField]
	private UIRewardHelper uiReward2;

	[SerializeField]
	private GameObject updateGo;

	[SerializeField]
	private GameObject getRewardGo;

	private int index;

	private UpdateReward updateReward1;

	private UpdateReward updateReward2;
}
