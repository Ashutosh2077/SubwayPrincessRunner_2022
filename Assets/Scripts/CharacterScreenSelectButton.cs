using System;
using UnityEngine;

public class CharacterScreenSelectButton : MonoBehaviour
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

	public void InitButton()
	{
		this._hasInited = true;
		this._managerInstance = CharacterScreenManager.Instance;
		this.col = base.GetComponent<BoxCollider>();
		UIEventListener uieventListener = UIEventListener.Get(this.tryButton);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnTry);
		this.ReloadButton();
	}

	public void OnClick()
	{
		if (this._activeState == CharacterScreenSelectButton.ButtonStates.buy)
		{
			this.PurchaseCharacter();
		}
		else
		{
			this._managerInstance.SelectCharacter(this.currentlyShownModelType, this.currentlyShownThemeIndex);
		}
	}

	public void OnTry(GameObject go)
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_try_role", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_role", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(5);
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
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 5)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_role", 0, null);
			if (this.tryButton.activeSelf)
			{
				this.tryButton.SetActive(false);
			}
			Game.Instance.TestCharacter(this.currentlyShownModelType, this.currentlyShownThemeIndex);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	private void PurchaseCharacter()
	{
		this._managerInstance.PurchaseCharacter(this.currentlyShownModelType, this.currentlyShownThemeIndex);
	}

	public void ReloadButton()
	{
		if (!this._hasInited)
		{
			return;
		}
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		this.currentlyShownModelType = instance.currenCharacterShown;
		this.currentlyShownThemeIndex = instance.currentCThemeShownIndex;
		this.modelData = Characters.characterData[this.currentlyShownModelType];
		this.modelTheme = CharacterThemes.GetThemeForCharacter(this.currentlyShownModelType, this.currentlyShownThemeIndex);
		this.unlockType = this.modelData.unlockType;
		this.isCharacterOwned = PlayerInfo.Instance.IsCollectionComplete(this.currentlyShownModelType);
		this.unlockPrice = this.modelData.Price;
		if (this.modelTheme != null)
		{
			CharacterTheme characterTheme = this.modelTheme;
			this.unlockType = characterTheme.unlockType;
			this.isThemeOwned = PlayerInfo.Instance.IsThemeUnlockedForCharacter(this.currentlyShownModelType, this.currentlyShownThemeIndex);
			this.unlockPrice = characterTheme.price;
		}
		if (this.unlockType == Characters.UnlockType.free || (this.isCharacterOwned && this.isThemeOwned) || (this.isCharacterOwned && this.modelTheme == null))
		{
			this.SetButtonState(CharacterScreenSelectButton.ButtonStates.select);
			this.UpdateUIForSelectState();
		}
		else if (this.unlockType == Characters.UnlockType.subscription)
		{
			this.SetButtonState(CharacterScreenSelectButton.ButtonStates.exclusive);
			this.UpdateUIForExclusiveState();
		}
		else if (!this.isCharacterOwned && this.modelTheme != null)
		{
			this.SetButtonState(CharacterScreenSelectButton.ButtonStates.lockedTheme);
			this.UpdateUIForLockedThemeState();
		}
		else if (this.unlockType == Characters.UnlockType.symbols)
		{
			this.SetButtonState(CharacterScreenSelectButton.ButtonStates.lockedSymbol);
			this.UpdateUIForLockedSymbolState();
		}
		else
		{
			this.SetButtonState(CharacterScreenSelectButton.ButtonStates.buy);
			this.UpdateUIForBuyState();
		}
		this.UpdateTryButton();
		this.UpdateVIP();
	}

	public void SetButtonState(CharacterScreenSelectButton.ButtonStates state)
	{
		this._activeState = state;
		if (state == CharacterScreenSelectButton.ButtonStates.buy)
		{
			this.buy.SetActive(true);
			this.select.SetActive(false);
			this.lockedTheme.SetActive(false);
			this.lockedSymbol.SetActive(false);
			this.exclusive.SetActive(false);
		}
		else if (state == CharacterScreenSelectButton.ButtonStates.lockedSymbol)
		{
			this.lockedSymbol.SetActive(true);
			this.buy.SetActive(false);
			this.select.SetActive(false);
			this.lockedTheme.SetActive(false);
			this.exclusive.SetActive(false);
		}
		else if (state == CharacterScreenSelectButton.ButtonStates.lockedTheme)
		{
			this.lockedTheme.SetActive(true);
			this.buy.SetActive(false);
			this.select.SetActive(false);
			this.lockedSymbol.SetActive(false);
			this.exclusive.SetActive(false);
		}
		else if (state == CharacterScreenSelectButton.ButtonStates.select)
		{
			this.select.SetActive(true);
			this.buy.SetActive(false);
			this.lockedTheme.SetActive(false);
			this.lockedSymbol.SetActive(false);
			this.exclusive.SetActive(false);
		}
		else if (state == CharacterScreenSelectButton.ButtonStates.exclusive)
		{
			this.exclusive.SetActive(true);
			this.buy.SetActive(false);
			this.lockedTheme.SetActive(false);
			this.lockedSymbol.SetActive(false);
			this.select.SetActive(false);
		}
		else
		{
			UnityEngine.Debug.LogError("No handler for button state: " + state.ToString(), null);
		}
	}

	private void ChangeButtonBgSprite(string color)
	{
		this.fill.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.fillSpriteNameFormat, color);
	}

	private void SetUIToBlueState()
	{
		this.ChangeButtonBgSprite("blue");
		this.col.enabled = false;
	}

	private void SetUIToGreenState()
	{
		this.ChangeButtonBgSprite("green");
		this.col.enabled = true;
	}

	private void SetUIToGrayState()
	{
		this.ChangeButtonBgSprite("gray");
		this.col.enabled = false;
	}

	private void UpdateUIForBuyState()
	{
		this.SetUIToGreenState();
		this.buyPriceLabel.text = this.unlockPrice.ToString();
		this.buyCoinSprite.gameObject.SetActive(this.unlockType == Characters.UnlockType.coins);
		this.buyKeySprite.gameObject.SetActive(this.unlockType == Characters.UnlockType.keys);
	}

	private void UpdateUIForExclusiveState()
	{
		this.SetUIToGrayState();
		this.exclusiveLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_EXCLUSIVE);
	}

	private void UpdateUIForLockedSymbolState()
	{
		this.SetUIToGrayState();
		this.lockedSymbolProgress.text = PlayerInfo.Instance.GetCollectedSymbols(this.currentlyShownModelType).ToString() + "/" + this.unlockPrice;
		this.lockedSymbolToken.spriteName = this.modelData.symbolSprite2dName;
		this.findLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_FIND);
		this.inLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_IN);
	}

	private void UpdateUIForLockedThemeState()
	{
		this.SetUIToGrayState();
		CharacterTheme characterTheme = this.modelTheme;
		this.lockedThemeAmount.text = characterTheme.price.ToString();
		this.lockedThemeCoin.enabled = (this.unlockType == Characters.UnlockType.coins);
		this.lockedThemeKey.enabled = (this.unlockType == Characters.UnlockType.keys);
		string text = Strings.Get(characterTheme.unlockDescription);
		if (!string.IsNullOrEmpty(text))
		{
			this.lockedThemeFeedback.text = text.ToUpper();
		}
	}

	private void UpdateUIForSelectState()
	{
		bool flag = PlayerInfo.Instance.currentCharacter == (int)this.currentlyShownModelType && PlayerInfo.Instance.currentThemeIndex == UIModelController.Instance.currentCThemeShownIndex;
		if (flag)
		{
			this.SetUIToBlueState();
			this.selectedLabel.text = Strings.Get(LanguageKey.UICHARACTER_SELECT_BUTTON_SELECTED);
		}
		else
		{
			this.SetUIToGreenState();
			this.selectedLabel.text = Strings.Get(LanguageKey.UICHARACTER_SELECT_BUTTON_SELECT);
		}
	}

	private void UpdateTryButton()
	{
		bool flag = TrialManager.Instance.HasTrialCharacter(this.currentlyShownModelType);
		if (flag && ((!this.isCharacterOwned && this.modelTheme == null) || (this.modelTheme != null && !this.isThemeOwned)))
		{
			if (!this.tryButton.activeSelf)
			{
				this.tryButton.SetActive(true);
			}
		}
		else if (this.tryButton.activeSelf)
		{
			this.tryButton.SetActive(false);
		}
		this.watchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_WATCH);
		this.tryLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_TRY);
	}

	private void UpdateVIP()
	{
		if (this.currentlyShownModelType == Game.Instance.subscriptionCharacterType)
		{
			if (!this.vip.activeSelf)
			{
				this.vip.SetActive(true);
			}
		}
		else if (this.vip.activeSelf)
		{
			this.vip.SetActive(false);
		}
	}

	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UILabel selectedLabel;

	[SerializeField]
	private UILabel buyPriceLabel;

	[SerializeField]
	private UISprite buyKeySprite;

	[SerializeField]
	private UISprite buyCoinSprite;

	[SerializeField]
	private UISprite lockedThemeCoin;

	[SerializeField]
	private UISprite lockedThemeKey;

	[SerializeField]
	private UILabel lockedThemeAmount;

	[SerializeField]
	private UILabel lockedThemeFeedback;

	[SerializeField]
	private UISprite lockedSymbolToken;

	[SerializeField]
	private UILabel lockedSymbolProgress;

	[SerializeField]
	private UILabel exclusiveLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UILabel findLbl;

	[SerializeField]
	private UILabel inLbl;

	[SerializeField]
	private GameObject buy;

	[SerializeField]
	private GameObject select;

	[SerializeField]
	private GameObject exclusive;

	[SerializeField]
	private GameObject lockedTheme;

	[SerializeField]
	private GameObject lockedSymbol;

	[SerializeField]
	private GameObject tryButton;

	[SerializeField]
	private GameObject vip;

	private CharacterScreenSelectButton.ButtonStates _activeState;

	private bool _hasInited;

	private CharacterScreenManager _managerInstance;

	private BoxCollider col;

	private Characters.CharacterType currentlyShownModelType;

	private int currentlyShownThemeIndex;

	private bool isCharacterOwned;

	private bool isThemeOwned;

	private Characters.Model modelData;

	private CharacterTheme modelTheme;

	private int unlockPrice;

	private Characters.UnlockType unlockType;

	public enum ButtonStates
	{
		buy,
		select,
		lockedTheme,
		lockedSymbol,
		unselect,
		exclusive
	}
}
