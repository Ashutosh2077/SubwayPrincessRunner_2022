using System;
using UnityEngine;

public class HelmetSelectButton : MonoBehaviour
{
	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
		RiseSdkListener.OnAdEvent += this.OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
	}

	private void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 13)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_helmet", 0, null);
			if (this.tryGo.activeSelf)
			{
				this.tryGo.SetActive(false);
			}
			Game.Instance.TestHelmet(this.currentHelmtype);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	public void OnClick()
	{
		if (!this.isInited)
		{
			return;
		}
		if (this.activeState == HelmetSelectButton.State.buy)
		{
			this.PurchaseHelmetTheme();
		}
		else if (this.activeState == HelmetSelectButton.State.select && this.selectInt == 1)
		{
			PlayerInfo.Instance.currentHelmet = this.currentHelmtype;
			this.SetToSelectState();
		}
	}

	public void OnTryClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_try_helmet", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_helmet", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(13);
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

	private void PurchaseHelmetTheme()
	{
		if (!this._purchaseInProgress)
		{
			this._purchaseInProgress = true;
			PurchaseHandler.Instance.PurchaseHelmetTheme(this.currentHelmtype, this);
		}
	}

	public void PurchaseHelmetFailure()
	{
		this._purchaseInProgress = false;
	}

	public void PurchaseHelmetSuccess()
	{
		switch (Helmets.helmOrder.IndexOf(this.currentHelmtype))
		{
		case 1:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet2nd", 0, null);
			break;
		case 2:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet3rd", 0, null);
			break;
		case 3:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet4th", 0, null);
			break;
		case 4:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet5th", 0, null);
			break;
		case 5:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet6th", 0, null);
			break;
		case 6:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet7th", 0, null);
			break;
		}
		this._purchaseInProgress = false;
		if (TrialManager.Instance.IsCurrentHelmetTrial(this.currentHelmtype))
		{
			TrialManager.Instance.currentTrialInfo = null;
		}
		UIScreenController.Instance.ShowUnlockAnimationForHelmet(this.currentHelmtype);
	}

	public void InitButton(HelmScreen screen)
	{
		this.isInited = true;
		this.col = base.GetComponent<BoxCollider>();
		this.UpdateSelectState(PlayerInfo.Instance.currentHelmet);
	}

	public void UpdateSelectState(Helmets.HelmType helmtype)
	{
		if (!this.isInited)
		{
			return;
		}
		this.helmunlocked = HelmetManager.Instance.isHelmetUnlocked(helmtype);
		this.currentHelm = Helmets.helmData[helmtype];
		this.currentHelmtype = helmtype;
		this.unlocktype = this.currentHelm.unlockType;
		this.price = this.currentHelm.price;
		if (this.helmunlocked)
		{
			this.SetState(HelmetSelectButton.State.select);
			this.SetToSelectState();
		}
		else
		{
			this.SetState(HelmetSelectButton.State.buy);
			this.SetToBuyState();
		}
		this.UpdateTryButton();
	}

	private void SetState(HelmetSelectButton.State newState)
	{
		this.activeState = newState;
		if (newState != HelmetSelectButton.State.buy)
		{
			if (newState == HelmetSelectButton.State.select)
			{
				this.select.SetActive(true);
				this.buyGo.SetActive(false);
			}
		}
		else
		{
			this.buyGo.SetActive(true);
			this.select.SetActive(false);
		}
	}

	private void SetFillToGreen()
	{
		this.fillSprite.spriteName = this.greenBtnSpriteName;
		this.col.enabled = true;
	}

	private void SetFillToBlue()
	{
		this.fillSprite.spriteName = this.blueBtnSpriteName;
		this.col.enabled = false;
	}

	private void SetToBuyState()
	{
		this.SetFillToGreen();
		this.buycoinSprite.enabled = (this.unlocktype == Helmets.UnlockType.coins);
		this.buykeySprite.enabled = (this.unlocktype == Helmets.UnlockType.keys);
		this.buypriceSprite.text = this.price.ToString();
	}

	private void SetToSelectState()
	{
		if (this.currentHelmtype == PlayerInfo.Instance.currentHelmet)
		{
			this.selectInt = 0;
			this.SetFillToBlue();
			this.selectLabel.text = Strings.Get(LanguageKey.HOVERBOARD_SELCTECT_BUTTON_SELECTED);
		}
		else if (this.currentHelmtype != PlayerInfo.Instance.currentHelmet)
		{
			this.selectInt = 1;
			this.SetFillToGreen();
			this.selectLabel.text = Strings.Get(LanguageKey.HOVERBOARD_SELCTECT_BUTTON_SELECT);
		}
	}

	private void UpdateTryButton()
	{
		bool flag = TrialManager.Instance.HasHelmetTrial(this.currentHelmtype);
		if (flag && !this.helmunlocked)
		{
			if (!this.tryGo.activeSelf)
			{
				this.tryGo.SetActive(true);
			}
		}
		else if (this.tryGo.activeSelf)
		{
			this.tryGo.SetActive(false);
		}
		this.watchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_WATCH);
		this.tryLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_TRY);
	}

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UISprite fillSprite;

	[SerializeField]
	private UISprite buycoinSprite;

	[SerializeField]
	private UISprite buykeySprite;

	[SerializeField]
	private UILabel buypriceSprite;

	[SerializeField]
	private UILabel selectLabel;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject select;

	[SerializeField]
	private GameObject tryGo;

	[SerializeField]
	private string greenBtnSpriteName;

	[SerializeField]
	private string blueBtnSpriteName;

	private BoxCollider col;

	private Helmets.Helm currentHelm;

	private Helmets.HelmType currentHelmtype;

	private bool helmunlocked;

	private Helmets.UnlockType unlocktype;

	private int price;

	private HelmetSelectButton.State activeState;

	private bool isInited;

	private int selectInt = -1;

	private bool _purchaseInProgress;

	private enum State
	{
		buy,
		select,
		freeView
	}
}
