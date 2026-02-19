using System;
using UnityEngine;

public class BuyHelmetButton : MonoBehaviour
{
	public void OnBuyClick()
	{
		if (!this._purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseHelmet(this);
		}
	}

	public void OnFreeViewClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_buy_helmet", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_buy_helmet", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(12);
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

	private void Start()
	{
		Upgrade upgrade = Upgrades.upgrades[PropType.helmet];
		this.priceLabel.text = (upgrade.getPrice(0) * this.number).ToString();
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

	public void PurchaseFailure()
	{
		this._purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		this._purchaseInProgress = false;
		PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, this.number);
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 12)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_buy_helmet", 0, null);
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, 3);
		}
	}

	public int number;

	private bool _purchaseInProgress;

	[SerializeField]
	private UILabel priceLabel;
}
