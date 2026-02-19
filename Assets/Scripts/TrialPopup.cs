using System;
using UnityEngine;

public class TrialPopup : UIBaseScreen
{
	public override void Init()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.tryGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnTryClick);
		this._tryLocalPos = this.tryGo.transform.localPosition;
		this._buyLocalPos = this.buyGo.transform.localPosition;
		base.Init();
	}

	public override void Show()
	{
		base.Show();
		if (TrialPopup.startNewGame)
		{
			this.tryGo.transform.localPosition = new Vector3(0f, this._tryLocalPos.y, 0f);
			this.buyGo.SetActive(false);
		}
		else
		{
			this.tryGo.transform.localPosition = this._tryLocalPos;
			this.buyGo.transform.localPosition = this._buyLocalPos;
			this.buyGo.SetActive(true);
		}
		this.trialInfo = TrialManager.Instance.currentTrialInfo;
		if (this.trialInfo.type == TrialType.Character)
		{
			UIModelController.Instance.ActivateTrailRoleModel(this.trialInfo.characterType, this.trialInfo.characterThemeId);
			Characters.Model model = Characters.characterData[this.trialInfo.characterType];
			CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(this.trialInfo.characterType, this.trialInfo.characterThemeId);
			Characters.UnlockType unlockType = model.unlockType;
			int price = model.Price;
			if (themeForCharacter != null)
			{
				unlockType = themeForCharacter.unlockType;
				price = themeForCharacter.price;
			}
			if (unlockType == Characters.UnlockType.coins)
			{
				this.buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			if (unlockType == Characters.UnlockType.keys)
			{
				this.buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
			}
			this.priceLbl.text = price.ToString();
		}
		else if (this.trialInfo.type == TrialType.Helmet)
		{
			UIModelController.Instance.ActivateTrailHelmetModel(this.trialInfo.helmetType);
			Helmets.Helm helm = Helmets.helmData[this.trialInfo.helmetType];
			if (helm.unlockType == Helmets.UnlockType.coins)
			{
				this.buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			if (helm.unlockType == Helmets.UnlockType.keys)
			{
				this.buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
			}
			this.priceLbl.text = helm.price.ToString();
		}
		this.ReloadUI();
		this.ReloadTime();
	}

	private void ReloadUI()
	{
		if (this.trialInfo.type == TrialType.Character)
		{
			this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TITLE_R);
			this.skillLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_R);
			Characters.Model model = Characters.characterData[this.trialInfo.characterType];
			if (model.freeReviveCount > 0)
			{
				this.skill1Icon.enabled = true;
				this.skill1Icon.spriteName = "Try_r_skill_icon1";
				this.skill1Lbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_R), model.freeReviveCount);
			}
			else
			{
				this.skill1Icon.enabled = false;
				this.skill1Lbl.text = string.Empty;
			}
			this.skill2Lbl.text = string.Empty;
			this.skill2Icon.enabled = false;
			this.tipTitleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_TITLE);
			this.tipOneLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT1);
			this.tipTwoLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT2);
			this.tipThreeLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT3);
		}
		else if (this.trialInfo.type == TrialType.Helmet)
		{
			this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TITLE_H);
			this.skillLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_H);
			this.skill1Lbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_H);
			this.skill2Lbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL2_H);
			this.skill1Icon.enabled = true;
			this.skill2Icon.enabled = true;
			this.skill1Icon.spriteName = "Try_h_skill_icon1";
			this.skill2Icon.spriteName = "Try_h_skill_icon2";
			this.tipTitleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_TITLE);
			this.tipOneLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT1);
			this.tipTwoLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT2);
			this.tipThreeLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT3);
		}
		this.tryLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FREE);
		this.messageLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_BOTTOM_INFO), this.trialInfo.aim);
		this.RefreshSlots(PlayerInfo.Instance.CurrentTrialInfoLevel(), this.trialInfo.aim);
		if (RiseSdk.Instance.HasRewardAd())
		{
			this.tryFill.color = Color.white;
		}
		else
		{
			this.tryFill.color = Color.cyan;
		}
	}

	private void RefreshSlots(int cur, int aim)
	{
		int i = 0;
		int num = this.slots.Length;
		while (i < num)
		{
			if (i < cur)
			{
				this.slots[i].enabled = true;
				this.slots[i].spriteName = "Try_h_h_slot2";
			}
			else if (i < aim)
			{
				this.slots[i].enabled = true;
				this.slots[i].spriteName = "Try_h_h_slot1";
			}
			else
			{
				this.slots[i].enabled = false;
			}
			i++;
		}
		this.sliderLbl.text = cur + "/" + aim;
	}

	private void ReloadTime()
	{
		TimeSpan timeSpan = TrialManager.Instance.begainDateTime.AddDays((double)PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow;
		if (timeSpan.Ticks < 0L)
		{
			this.timeLbl.text = "----";
		}
		else
		{
			this.timeLbl.text = string.Concat(new object[]
			{
				timeSpan.Days,
				Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TIME_D),
				" ",
				timeSpan.Hours,
				Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TIME_H)
			});
		}
	}

	public override void Hide()
	{
		UIModelController.Instance.ClearModels();
		base.Hide();
		if (TrialPopup.startNewGame)
		{
			SaveMeManager.ResetSaveMeForNewRun();
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
		}
		TrialPopup.startNewGame = false;
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

	public void OnBuyClick()
	{
		if (this.trialInfo.type == TrialType.Character)
		{
			PurchaseHandler.Instance.PurchaseCharacter(this.trialInfo.characterType, this.trialInfo.characterThemeId, true, delegate
			{
				switch (Characters.characterOrder.IndexOf(this.trialInfo.characterType))
				{
				case 0:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles1st", 0, null);
					break;
				case 1:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles2nd", 0, null);
					break;
				case 2:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles3rd", 0, null);
					break;
				case 3:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles4th", 0, null);
					break;
				case 4:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles5th", 0, null);
					break;
				case 5:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles6th", 0, null);
					break;
				case 6:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles7th", 0, null);
					break;
				case 7:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles8th", 0, null);
					break;
				case 8:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles9th", 0, null);
					break;
				case 9:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles10th", 0, null);
					break;
				case 10:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles11th", 0, null);
					break;
				}
				UIScreenController.Instance.ClosePopup(null);
				UIScreenController.Instance.ShowUnlockAnimationForCharacter(this.trialInfo.characterType, this.trialInfo.characterThemeId);
				UIModelController.Instance.SelectCharacterForPlay(this.trialInfo.characterType, this.trialInfo.characterThemeId);
				TrialManager.Instance.currentTrialInfo = null;
			});
		}
		else if (this.trialInfo.type == TrialType.Helmet)
		{
			PurchaseHandler.Instance.PurchaseHelmet(this.trialInfo.helmetType, delegate()
			{
				switch (Helmets.helmOrder.IndexOf(this.trialInfo.helmetType))
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
				UIScreenController.Instance.ClosePopup(null);
				UIScreenController.Instance.ShowUnlockAnimationForHelmet(this.trialInfo.helmetType);
				PlayerInfo.Instance.currentHelmet = this.trialInfo.helmetType;
				TrialManager.Instance.currentTrialInfo = null;
			});
		}
	}

	public void OnTryClick(GameObject go)
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_Try_Popup", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_Try_Popup", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(11);
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

	private void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 11)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_Try_Popup", 0, null);
			TrialManager.Instance.Begin();
			TrialPopup.startNewGame = false;
			UIScreenController.Instance.ClosePopup(null);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	public void OnQuestionClick()
	{
		UIModelController.Instance.ActivateTutorialPopup(false);
		if (!this.tipGo.activeInHierarchy)
		{
			this.tipGo.SetActive(true);
		}
	}

	public void OnQuestionClose()
	{
		UIModelController.Instance.ActivateTutorialPopup(true);
		if (this.tipGo.activeInHierarchy)
		{
			this.tipGo.SetActive(false);
		}
	}

	public override void GainFocus()
	{
		UIModelController.Instance.ActivateTutorialPopup(true);
	}

	public override void LooseFocus()
	{
		UIModelController.Instance.ActivateTutorialPopup(false);
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UILabel messageLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private UILabel skillLbl;

	[SerializeField]
	private UILabel skill1Lbl;

	[SerializeField]
	private UILabel skill2Lbl;

	[SerializeField]
	private UILabel tipTitleLbl;

	[SerializeField]
	private UILabel tipOneLbl;

	[SerializeField]
	private UILabel tipTwoLbl;

	[SerializeField]
	private UILabel tipThreeLbl;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel sliderLbl;

	[SerializeField]
	private UISprite buyIcon;

	[SerializeField]
	private UISprite tryFill;

	[SerializeField]
	private UISprite skill1Icon;

	[SerializeField]
	private UISprite skill2Icon;

	[SerializeField]
	private GameObject tryGo;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject tipGo;

	[SerializeField]
	private UISprite[] slots;

	private TrialInfo trialInfo;

	private Vector3 _tryLocalPos;

	private Vector3 _buyLocalPos;

	public static bool startNewGame;
}
