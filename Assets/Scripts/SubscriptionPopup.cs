using System;
using UnityEngine;

public class SubscriptionPopup : UIBaseScreen
{
	private void Awake()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.purchaseBtn);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.Pay);
	}

	public override void Show()
	{
		base.Show();
		this.Refresh();
		this.RefreshLabel();
	}

	private void Refresh()
	{
		if (PlayerInfo.Instance.hasSubscribed)
		{
			this.purchaseBtn.GetComponent<TweenScale>().enabled = false;
			this.purchaseBtn.GetComponent<BoxCollider>().enabled = false;
			this.btnNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_RESTORE);
			this.trialDescripeLbl.enabled = false;
		}
		else
		{
			this.purchaseBtn.GetComponent<TweenScale>().enabled = true;
			this.purchaseBtn.GetComponent<BoxCollider>().enabled = true;
			this.btnNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_TRIAL);
			this.trialDescripeLbl.enabled = true;
		}
		this.toggle.value = PlayerInfo.Instance.ignoreSubscriptionPopup;
	}

	private void RefreshLabel()
	{
		this.titleSpr.spriteName = Strings.Get(LanguageKey.ATLAS_SUBSCRIBE_TITLE_SPRITE);
		this.trialDescripeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_TRIAL_TIP);
		this.reminderLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_NO_REMINDER);
		this.removeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_REMOVE);
		this.adsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_ADS);
		this.gemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_DISCOUNT_GEMS);
		this.roleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_EXCLUSIVE_ROLE);
		this.roleTypeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_MONK);
		this.doubleCoinsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_DOUBLE_COINS);
		this.tipLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_SUBSCRIBE_INFO);
		this.termsLbl.text = string.Format("[url=https://sites.google.com/site/huskaimmcomtermsofuse/][u]{0}[/u][/url]", Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_URL_TERMS_OF_USE));
		this.policyLbl.text = string.Format("[url=https://sites.google.com/site/riooprivacypolicy/][u]{0}[/u][/url]", Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_URL_PRIVACY_POLICY));
	}

	public void Pay(GameObject go)
	{
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.Pay(13);
		}
	}

	public void OnToggleValue()
	{
		if (UIToggle.current.value)
		{
			PlayerInfo.Instance.ignoreSubscriptionNextTime = DateTime.UtcNow.AddDays(1.0);
			PlayerInfo.Instance.ignoreSubscriptionPopup = true;
		}
		else
		{
			PlayerInfo.Instance.ignoreSubscriptionPopup = false;
		}
	}

	[SerializeField]
	private UISprite titleSpr;

	[SerializeField]
	private UILabel trialLbl;

	[SerializeField]
	private UILabel trialDescripeLbl;

	[SerializeField]
	private UILabel reminderLbl;

	[SerializeField]
	private UILabel subscribeInfoLbl;

	[SerializeField]
	private UILabel removeLbl;

	[SerializeField]
	private UILabel adsLbl;

	[SerializeField]
	private UILabel gemLbl;

	[SerializeField]
	private UILabel roleLbl;

	[SerializeField]
	private UILabel roleTypeLbl;

	[SerializeField]
	private UILabel doubleCoinsLbl;

	[SerializeField]
	private UILabel tipLbl;

	[SerializeField]
	private UILabel termsLbl;

	[SerializeField]
	private UILabel policyLbl;

	[SerializeField]
	private GameObject purchaseBtn;

	[SerializeField]
	private UIToggle toggle;

	[SerializeField]
	private UILabel btnNameLbl;

	private const string termsUrlFormat = "[url=https://sites.google.com/site/huskaimmcomtermsofuse/][u]{0}[/u][/url]";

	private const string policyUrlFormat = "[url=https://sites.google.com/site/riooprivacypolicy/][u]{0}[/u][/url]";
}
