using System;
using UnityEngine;

public class NotEnoughCurencyPopup : UIBaseScreen
{
	public override void Hide()
	{
		base.Hide();
		this._popupData = null;
	}

	public void OnFreeReward(RiseSdk.AdEventType b, int type, string tag, int d)
	{
		if (b == RiseSdk.AdEventType.RewardAdShowFinished && type == 6)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_not_enough", 0, null);
			if (this._popupData.isCoins)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewcoins, delegate()
				{
					UIScreenController.Instance.ClosePopup(null);
				}, 0);
			}
			else
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewkeys, delegate()
				{
					UIScreenController.Instance.ClosePopup(null);
				}, 0);
			}
		}
	}

	public void OnFreeViewClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_not_enough", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_not_enough", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(6);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
		RiseSdkListener.OnAdEvent += this.OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
	}

	public void OnBuyClicked(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			RiseSdk.Instance.Pay(5);
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
		UIScreenController.Instance.ClosePopup(null);
	}

	public void OnCancelClicked()
	{
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()) && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
			if (UIScreenController.isInstanced)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
		}
	}

	public void OnOkClicked(GameObject go)
	{
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushScreen("CoinsUI_shop");
			if (this._popupData != null && this._popupData.lastIsPopup)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public override void Show()
	{
		base.Show();
		this._popupData = InAppManager.instance.GetPopupData();
		this.buyLbl.text = Strings.Get(LanguageKey.UI_POPUP_NOT_ENOUGH_CURENCY_BUTTON_BUY);
		if (this._popupData != null)
		{
			this.popupTitle.text = this._popupData.popupTitle;
			this.popupDescription.text = this._popupData.popupDescription;
			this.freeLbl.text = Strings.Get(LanguageKey.UI_POPUP_NOT_ENOUGH_CURENCY_BUTTON_FREE);
		}
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()) && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			UIEventListener uieventListener = UIEventListener.Get(this.buy);
			uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnBuyClicked);
		}
		else
		{
			UIEventListener uieventListener2 = UIEventListener.Get(this.buy);
			uieventListener2.onClick = new UIEventListener.VoidDelegate(this.OnOkClicked);
		}
	}

	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel popupDescription;

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel buyLbl;

	[SerializeField]
	private GameObject buy;

	private InAppManagerPopupData _popupData;
}
