using System;
using UnityEngine;

public class BuyButtonIngame : MonoBehaviour
{
	public void initBuyButton(PropType type)
	{
		this._type = type;
	}

	public void Reload(bool forceBuy)
	{
		this.freelbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		if (!forceBuy && PlayerInfo.Instance.CheckIfFreeUpgrade() && RiseSdk.Instance.HasRewardAd())
		{
			this.buyButton.SetActive(false);
			this.freeButton.SetActive(true);
			this.fill.spriteName = "AX_greenBtn";
			this.inbuy = false;
		}
		else
		{
			this.buyButton.SetActive(true);
			this.freeButton.SetActive(false);
			this.fill.spriteName = "AX_yellowBtn";
			this.inbuy = true;
		}
	}

	private void OnClick()
	{
		if (!this._purchaseInProgress)
		{
			if (this.inbuy)
			{
				PurchaseHandler.Instance.PurchaseUpgrade(this._type, this);
			}
			else
			{
				this.FreeRewardClick();
			}
		}
	}

	private void FreeRewardClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_upgrades", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_upgrades", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(8);
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

	public void PurchaseFailure()
	{
		this._purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		this._purchaseInProgress = false;
	}

	[SerializeField]
	private UILabel freelbl;

	[SerializeField]
	private GameObject buyButton;

	[SerializeField]
	private GameObject freeButton;

	[SerializeField]
	private UISprite fill;

	private bool _purchaseInProgress;

	private PropType _type;

	private bool inbuy;
}
