using System;
using UnityEngine;

public class WatchFreeViewPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		this.RefreshLabel();
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_TITLE);
		this.contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_REWARD);
		this.freeRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_BUTTON_VIDEO);
		if (RiseSdk.Instance.HasRewardAd())
		{
			this.fillSpr.color = Color.white;
			this.btn_collider.enabled = true;
			this.btn_tween.PlayForward();
		}
		else
		{
			this.fillSpr.color = Color.cyan;
			this.btn_collider.enabled = false;
			this.btn_tween.enabled = false;
		}
	}

	public void OnFreeViewClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_box", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_box", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_box", 0, null);
				RiseSdk.Instance.ShowRewardAd(WatchFreeViewPopup.rewardId);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
				UIScreenController.Instance.ClosePopup(null);
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel contentLbl;

	[SerializeField]
	private UILabel freeRewardLbl;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private BoxCollider btn_collider;

	[SerializeField]
	private TweenScale btn_tween;

	public static int rewardId;
}
