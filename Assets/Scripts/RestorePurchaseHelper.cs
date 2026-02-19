using System;
using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	private void Awake()
	{
		WatchFreeViewSystem.Instance.AddNewTime("FreeViewKeys", 180);
	}

	private void OnEnable()
	{
		this.freeLbl.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS);
		this.getLbl.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_GET);
	}

	private void Update()
	{
		if (!WatchFreeViewSystem.Instance.IsCoolingDownOver("FreeViewKeys"))
		{
			this.FreeDelayTime.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_CD) + WatchFreeViewSystem.Instance.GetCoolingDownTime("FreeViewKeys");
			this.btnCollider.enabled = false;
			this.getLabel.color = Color.gray;
			this.fillSpr.color = Color.cyan;
			for (int i = 0; i < this.images.Length; i++)
			{
				this.images[i].color = new Color(0f, 255f, 255f, 255f);
			}
		}
		else
		{
			this.FreeDelayTime.text = string.Empty;
			this.btnCollider.enabled = true;
			this.getLabel.color = this.getlabelOriColor;
			this.fillSpr.color = Color.white;
			for (int j = 0; j < this.images.Length; j++)
			{
				this.images[j].color = new Color(255f, 255f, 255f, 255f);
			}
		}
	}

	public void OnFreeRewardClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_shop_gem", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_shop_gem", "default,default");
		if (!WatchFreeViewSystem.Instance.IsCoolingDownOver("FreeViewKeys"))
		{
			return;
		}
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_shop_gem", 0, null);
				RiseSdk.Instance.ShowRewardAd(1);
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

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UILabel FreeDelayTime;

	[SerializeField]
	private UISprite[] images;

	[SerializeField]
	private UILabel getLabel;

	[SerializeField]
	private Color getlabelOriColor;

	[SerializeField]
	private BoxCollider btnCollider;

	public const string timeKey = "FreeViewKeys";
}
