using System;
using UnityEngine;

public class DailyLandingPopup : UIBaseScreen
{
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < this.helps.Length; i++)
		{
			this.helps[i].Init(i + 1);
		}
	}

	public override void Show()
	{
		base.Show();
		this.Refresh();
		this.RefreshLabel();
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_TITLE);
		this.getLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_BUTTON_GET);
		this.tomorrowLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_KEEP_CONTINUE);
	}

	private void Refresh()
	{
		bool flag;
		PlayerInfo.Instance.GetDailyLandingDaysInRow(out flag);
		for (int i = 0; i < this.helps.Length; i++)
		{
			this.helps[i].Refresh();
		}
		bool flag2;
		PlayerInfo.Instance.GetDailyLandingDaysInRow(out flag2);
		if (flag2)
		{
			if (this.viewGo.activeInHierarchy)
			{
				this.viewGo.SetActive(false);
			}
			if (this.getGo.activeInHierarchy)
			{
				this.getGo.SetActive(false);
			}
			this.tomorrowLbl.enabled = true;
		}
		else
		{
			if (!this.getGo.activeInHierarchy)
			{
				this.getGo.SetActive(true);
			}
			this.tomorrowLbl.enabled = false;
			if (UIScreenController.Instance.CheckNetwork() && RiseSdk.Instance.HasRewardAd())
			{
				if (!this.viewGo.activeInHierarchy)
				{
					this.viewGo.SetActive(true);
				}
				this.getGo.transform.localPosition = Vector3.up * -500f;
				this.viewGo.transform.localPosition = Vector3.up * -365f;
			}
			else
			{
				if (this.viewGo.activeInHierarchy)
				{
					this.viewGo.SetActive(false);
				}
				this.getGo.transform.localPosition = Vector3.up * -365f;
			}
		}
	}

	public void OnReceiceClick()
	{
		PlayerInfo.Instance.ReceiveDailyLandingPayout(1, new Action(this.Refresh));
	}

	public void OnDoubleClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_double_daily", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_double_daily", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(15);
			}
			else
			{
				UIScreenController.Instance.PushPopup("Videoloading");
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeView;
		RiseSdkListener.OnAdEvent += this.OnFreeView;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeView;
	}

	private void OnFreeView(RiseSdk.AdEventType aet, int id, string tag, int type)
	{
		if (aet == RiseSdk.AdEventType.RewardAdShowFinished && id == 15)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_double_daily", 0, null);
			PlayerInfo.Instance.ReceiveDailyLandingPayout(2, new Action(this.Refresh));
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UILabel tomorrowLbl;

	[SerializeField]
	private DailyLandingHelp[] helps;

	[SerializeField]
	private GameObject getGo;

	[SerializeField]
	private GameObject viewGo;
}
