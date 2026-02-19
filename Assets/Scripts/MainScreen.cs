using System;
using UnityEngine;

public class MainScreen : UIBaseScreen
{
	private void Awake()
	{
		foreach (GameObject gameObject in this.gameobjectsToTween)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (!PlayerInfo.Instance.tutorialCompleted)
		{
			this.triggerTween(false);
		}
		else
		{
			this.triggerTween(true);
		}
		PlayerInfo.Instance.onPowerupAmountChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onPowerupAmountChanged, new Action(this.UpdatehelmLabel));
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.onPowerupAmountChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onPowerupAmountChanged, new Action(this.UpdatehelmLabel));
	}

	public void TapStartOnClick()
	{
		Game.Instance.StartGame();
	}

	public void FreeViewOnClick()
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(0);
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

	private void RefreshLabel()
	{
		this.luckySpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_LOTTERY_BUTTON_SPRITE);
		this.rolesSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_ROLE_BUTTON_SPRITE);
		this.goSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_PLAY_BUTTON_SPRITE);
		this.vipSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_VIP_BUTTON_SPRITE);
		this.rankSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_RANK_BUTTON_SPRITE);
		this.trySpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_TRY_BUTTON_NEW);
		this.onlineSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_ONLINE_BUTTON_ONLINE);
	}

	public override void Show()
	{
		base.Show();
		this.RefreshLabel();
		this.ShowDeliciousIcon();
		NotificationsObserver.Instance.NotifyNotificationDataChange();
		if (Characters.characterOrder.IndexOf((Characters.CharacterType)PlayerInfo.Instance.currentCharacter) == 8 && !PlayerInfo.Instance.hasSubscribed)
		{
			CharacterScreenManager.Instance.SelectCharacter(Characters.CharacterType.slick, 0);
		}
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			bool flag = false;
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow.Date - PlayerInfo.Instance.gameOverFullAdDate).Days != 0)
			{
				PlayerInfo.Instance.gameOverFullAdDate = utcNow.Date;
				PlayerInfo.Instance.showTrialPopupCount = 0;
				PlayerInfo.Instance.showWatchVideoPopupCount = 0;
				PlayerInfo.Instance.gameOverFullAdCount = 0;
				this._lastGameoverAdCountForTrailRole = -1;
				flag = true;
			}
			if (!PlayerInfo.Instance.hasSubscribed && Game.Instance.show20sAd && !this.autoShowPopup && Game.Instance.GetNextAdDuration() > 20f)
			{
				Game.Instance.showAdTime = Time.time;
				RiseSdk.Instance.ShowAd("passlevel1");
				RiseSdk.Instance.TrackEvent("interstitial_start20s", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_start20s", 0, null);
				Game.Instance.show20sAd = false;
			}
			bool flag2 = false;
			if (PlayerInfo.Instance.ignoreSubscriptionPopup && utcNow > PlayerInfo.Instance.ignoreSubscriptionNextTime)
			{
				PlayerInfo.Instance.ignoreSubscriptionPopup = false;
			}
			if (!PlayerPrefs.HasKey("NewPlayerShowSubscribeFirst") && PlayerInfo.Instance.isNewPlayer)
			{
				this.autoShowPopup = true;
				UIScreenController.Instance.QueuePopup("SubscribePopup");
				PlayerPrefs.SetInt("NewPlayerShowSubscribeFirst", 1);
			}
			bool flag3 = false;
			bool flag4 = false;
			if (!TrialManager.Instance.nothingElse && TrialManager.Instance.currentTrialInfo != null && TrialManager.Instance.CheckOnMainScreen())
			{
				if (!PlayerInfo.Instance.isNewPlayer && flag)
				{
					flag2 = true;
					this.autoShowPopup = true;
					PlayerInfo.Instance.showTrialPopupCount++;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "Try_popup_auto", 0, null);
					UIScreenController.Instance.QueuePopup("TryHoverboardPopup");
				}
				else
				{
					flag3 = true;
				}
			}
			if (PlayerInfo.Instance.showWatchVideoPopupCount == 0 && RiseSdk.Instance.HasRewardAd())
			{
				flag4 = true;
			}
			if (PlayerInfo.Instance.showTrialPopupCount <= 4 && this._lastGameoverAdCountForTrailRole != PlayerInfo.Instance.gameOverFullAdCount && PlayerInfo.Instance.gameOverFullAdCount >= 4)
			{
				bool flag5 = UnityEngine.Random.value < 0.3f;
				if (flag4 && (flag5 || (!flag5 && !flag3)))
				{
					flag2 = true;
					this.autoShowPopup = true;
					UIScreenController.Instance.QueuePopup("WatchVideoPopup");
					PlayerInfo.Instance.showWatchVideoPopupCount++;
					PlayerInfo.Instance.gameOverFullAdCount = 0;
				}
				else if (flag3 && (!flag5 || (flag5 && !flag4)))
				{
					flag2 = true;
					this.autoShowPopup = true;
					PlayerInfo.Instance.showTrialPopupCount++;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "Try_popup_auto", 0, null);
					UIScreenController.Instance.QueuePopup("TryHoverboardPopup");
					PlayerInfo.Instance.gameOverFullAdCount = 0;
				}
			}
			TrialManager.Instance.preUseTryRole = false;
			this._lastGameoverAdCountForTrailRole = PlayerInfo.Instance.gameOverFullAdCount;
			if (!flag2 && PlayerInfo.Instance.shouldShowPlayerMenuPopup && !PlayerInfo.Instance.hasShownPlayerMenuPopup)
			{
				UIScreenController.Instance.PushScreen("CharacterScreen");
				PlayerInfo.Instance.hasShownPlayerMenuPopup = true;
				PlayerInfo.Instance.shouldShowPlayerMenuPopup = false;
				return;
			}
		}
		this.UpdatehelmLabel();
		this._showTimes++;
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public override void Hide()
	{
		base.Hide();
		RiseSdk.Instance.CloseDeliciousIconAd();
	}

	public void UpdatehelmLabel()
	{
		this.helmetNumLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet).ToString();
	}

	private void HandlePopups()
	{
		UIScreenController.Instance.RequeueDelayedPopups();
	}

	private void triggerTween(bool active)
	{
		int i = 0;
		int num = this.gameobjectsToTween.Length;
		while (i < num)
		{
			this.gameobjectsToTween[i].SetActive(active);
			i++;
		}
	}

	public void ShowDeliciousIcon()
	{
		if (RiseSdk.Instance.HasDeliciousAd())
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (num < 0.6f)
			{
				float num2 = (float)Screen.height / 1280f;
				if ((float)Screen.width > 800f)
				{
					RiseSdk.Instance.ShowDeliciousIconAd(20f * num2, 250f * num2, 130f * num2, 130f * num2, "delicious9-16");
				}
				else
				{
					RiseSdk.Instance.ShowDeliciousIconAd(20f * num2, 250f * num2, 130f * num2, 130f * num2, "delicious9-16");
				}
			}
			else if (num < 0.7f)
			{
				float num3 = (float)Screen.height / 1280f;
				RiseSdk.Instance.ShowDeliciousIconAd(20f * num3, 250f * num3, 130f * num3, 130f * num3, "delicious2-3");
			}
			else
			{
				float num4 = (float)Screen.height / 1280f;
				RiseSdk.Instance.ShowDeliciousIconAd(20f * num4, 250f * num4, 130f * num4, 130f * num4, "delicious3-4");
			}
			return;
		}
	}

	public override void GainFocus()
	{
		if (Characters.characterOrder.IndexOf((Characters.CharacterType)PlayerInfo.Instance.currentCharacter) == 8 && !PlayerInfo.Instance.hasSubscribed)
		{
			CharacterScreenManager.Instance.SelectCharacter(Characters.CharacterType.slick, 0);
		}
		this.ShowDeliciousIcon();
		if (!PlayerInfo.Instance.hasSubscribed && Game.Instance.show20sAd && PlayerInfo.Instance.tutorialCompleted && !this.autoShowPopup && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.ShowAd("passlevel1");
			RiseSdk.Instance.TrackEvent("interstitial_start20s", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_start20s", 0, null);
			Game.Instance.show20sAd = false;
		}
		this.autoShowPopup = false;
	}

	public override void LooseFocus()
	{
		RiseSdk.Instance.CloseDeliciousIconAd();
	}

	public override void Init()
	{
		base.Init();
		this._lastGameoverAdCountForTrailRole = PlayerInfo.Instance.gameOverFullAdCount;
		NotificationsObserver.Instance.RegisterNotificationAction(base.gameObject);
	}

	[SerializeField]
	private UISprite luckySpr;

	[SerializeField]
	private UISprite rolesSpr;

	[SerializeField]
	private UISprite goSpr;

	[SerializeField]
	private UISprite vipSpr;

	[SerializeField]
	private UISprite rankSpr;

	[SerializeField]
	private UISprite trySpr;

	[SerializeField]
	private UISprite onlineSpr;

	[SerializeField]
	private GameObject[] gameobjectsToTween;

	[SerializeField]
	private UILabel helmetNumLabel;

	[SerializeField]
	private OnlineButton onlineButton;

	private int _showTimes;

	private int _lastGameoverAdCountForTrailRole;

	private bool autoShowPopup;
}
