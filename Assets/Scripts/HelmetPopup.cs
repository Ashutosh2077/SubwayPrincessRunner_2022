using System;
using UnityEngine;

public class HelmetPopup : UIBaseScreen
{
	public override void Hide()
	{
		base.Hide();
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom", 20000);
		}
	}

	public override void Show()
	{
		base.Show();
		this.UpdateCoinsUI();
		this.UpdateGemsUI();
		this.UpdateLabels();
		RiseSdk.Instance.enableBackHomeAd(false, "custom", 20000);
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(this.UpdateCoinsUI));
		instance.onKeysChanged = (Action)Delegate.Combine(instance.onKeysChanged, new Action(this.UpdateGemsUI));
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(this.UpdateLabels));
		this.popupTitle.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP);
		this.helmetDescription1.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP_DESCRIPTION_1);
		this.helmetDescription2.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP_DESCRIPTION_2);
		this.getLbl.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_GET) + " 3";
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.onCoinsChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onCoinsChanged, new Action(this.UpdateCoinsUI));
		PlayerInfo.Instance.onKeysChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onKeysChanged, new Action(this.UpdateGemsUI));
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void UpdateCoinsUI()
	{
		this.coinsLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
	}

	public void UpdateGemsUI()
	{
		this.gemsLabel.text = PlayerInfo.Instance.amountOfKeys.ToString();
	}

	private void UpdateLabels()
	{
		this.helmetAmountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet).ToString();
	}

	public override void GainFocus()
	{
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	public override void LooseFocus()
	{
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel helmetAmountLabel;

	[SerializeField]
	private UILabel helmetDescription1;

	[SerializeField]
	private UILabel helmetDescription2;

	[SerializeField]
	private UILabel coinsLabel;

	[SerializeField]
	private UILabel gemsLabel;

	[SerializeField]
	private UILabel getLbl;
}
