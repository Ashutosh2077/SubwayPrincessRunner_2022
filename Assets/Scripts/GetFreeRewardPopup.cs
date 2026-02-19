using System;
using UnityEngine;

public class GetFreeRewardPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		this.ShowRewardPanel();
		this.RefreshLbal();
	}

	public override void Hide()
	{
		this.isRewardShowing = false;
		this.popupData = null;
		base.Hide();
	}

	private void ShowRewardPanel()
	{
		this.isRewardShowing = true;
		AudioPlayer.Instance.PlaySound("Get_reward_sfx", true);
		this.popupData = FreeRewardManager.Instance.GetRewardPopupData();
		this.closeBtn.gameObject.SetActive(this.popupData.useCloseBtn);
		switch (this.popupData.rewardType)
		{
		case RewardType.coins:
		case RewardType.viewcoins:
		case RewardType.doublecoins:
			this.powerParent.SetActive(false);
			this.itemParent.SetActive(true);
			this.coinIcon.enabled = true;
			this.keyIcon.enabled = false;
			this.amountOfItemLbl.text = "X" + this.popupData.num;
			break;
		case RewardType.keys:
		case RewardType.viewkeys:
			this.powerParent.SetActive(false);
			this.itemParent.SetActive(true);
			this.coinIcon.enabled = false;
			this.keyIcon.enabled = base.enabled;
			this.amountOfItemLbl.text = "X" + this.popupData.num;
			break;
		case RewardType.headstart2000:
			this.powerParent.SetActive(true);
			this.itemParent.SetActive(false);
			this.headstartIcon.enabled = true;
			this.scoreboosterIcon.enabled = false;
			this.leeIcon.enabled = false;
			this.turtlefokIcon.enabled = false;
			this.amountOfPowerLbl.text = "X" + this.popupData.num;
			this.powerTip.enabled = true;
			break;
		case RewardType.scorebooster:
			this.powerParent.SetActive(true);
			this.itemParent.SetActive(false);
			this.headstartIcon.enabled = false;
			this.scoreboosterIcon.enabled = true;
			this.leeIcon.enabled = false;
			this.turtlefokIcon.enabled = false;
			this.amountOfPowerLbl.text = "X" + this.popupData.num;
			this.powerTip.enabled = true;
			break;
		case RewardType.leeSymbol:
			this.powerParent.SetActive(true);
			this.itemParent.SetActive(false);
			this.headstartIcon.enabled = false;
			this.scoreboosterIcon.enabled = false;
			this.leeIcon.enabled = true;
			this.turtlefokIcon.enabled = false;
			this.amountOfPowerLbl.text = "X" + this.popupData.num;
			this.powerTip.enabled = true;
			break;
		case RewardType.turtlefokSymbol:
			this.powerParent.SetActive(true);
			this.itemParent.SetActive(false);
			this.headstartIcon.enabled = false;
			this.scoreboosterIcon.enabled = false;
			this.leeIcon.enabled = false;
			this.turtlefokIcon.enabled = true;
			this.amountOfPowerLbl.text = "X" + this.popupData.num;
			this.powerTip.enabled = true;
			break;
		}
	}

	private void RefreshLbal()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_GET_FREE_REWARD_TITLE);
		this.getLbl.text = Strings.Get(LanguageKey.UI_POPUP_GET_FREE_REWARD_BUTTON_GET);
	}

	public void CloseBtnOnClick()
	{
		if (!this.isRewardShowing)
		{
			return;
		}
		if (this.popupData.startGame)
		{
			Game.Instance.StartGame();
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public void ShowHLInterstitial()
	{
		if (!PlayerInfo.Instance.hasSubscribed && !Game.Instance.show20sAd && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.ShowAd("passlevel");
		}
	}

	public void GetBtnOnClick()
	{
		if (!this.isRewardShowing)
		{
			return;
		}
		if (this.popupData.payReward)
		{
			if (this.popupData.startGame)
			{
				if (UIScreenController.Instance.CheckNetwork())
				{
					this.ShowHLInterstitial();
					if (this.popupData.rewardType == RewardType.headstart2000)
					{
						PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, this.popupData.num);
					}
					if (this.popupData.rewardType == RewardType.scorebooster)
					{
						PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, this.popupData.num);
					}
				}
			}
			else
			{
				if (this.popupData.rewardType == RewardType.coins || this.popupData.rewardType == RewardType.viewcoins)
				{
					PlayerInfo.Instance.amountOfCoins += this.popupData.num;
				}
				if (this.popupData.rewardType == RewardType.keys || this.popupData.rewardType == RewardType.viewkeys)
				{
					PlayerInfo.Instance.amountOfKeys += this.popupData.num;
				}
			}
		}
		if (this.popupData.getCallback != null)
		{
			this.popupData.getCallback();
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private GameObject itemParent;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UISprite keyIcon;

	[SerializeField]
	private UILabel amountOfItemLbl;

	[SerializeField]
	private GameObject powerParent;

	[SerializeField]
	private UILabel powerTip;

	[SerializeField]
	private UISprite headstartIcon;

	[SerializeField]
	private UISprite scoreboosterIcon;

	[SerializeField]
	private UISprite leeIcon;

	[SerializeField]
	private UISprite turtlefokIcon;

	[SerializeField]
	private UILabel amountOfPowerLbl;

	[SerializeField]
	private Collider closeBtn;

	[SerializeField]
	private float delayGiftClick = 0.5f;

	private bool isRewardShowing;

	private FreeRewardPopupData popupData;
}
