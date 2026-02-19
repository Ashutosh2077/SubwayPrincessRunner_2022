using System;
using System.Collections.Generic;

public class CharacterScreenManager
{
	private CharacterScreenManager()
	{
	}

	public void AddOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			this._onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Combine(this._onCharacterUnlocked, handler);
		}
	}

	public void AddOnShownCharacterSelectedListener(Action handler)
	{
		if (handler != null)
		{
			this._onCharacterSelected = (Action)Delegate.Combine(this._onCharacterSelected, handler);
		}
	}

	public void CharacterPurchaseFailure()
	{
		if (this._purchaseInProgress)
		{
			this._purchaseInProgress = false;
		}
	}

	public void CharacterPurchaseSuccessful(Characters.CharacterType purchasedCharacter, int themeIndex)
	{
		if (this._purchaseInProgress)
		{
			switch (Characters.characterOrder.IndexOf(purchasedCharacter))
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
			this._purchaseInProgress = false;
			this.OnCharacterUnlocked(purchasedCharacter, themeIndex);
			UIScreenController.Instance.ShowUnlockAnimationForCharacter(purchasedCharacter, themeIndex);
			if (TrialManager.Instance.IsCurrentCharacterTrial(purchasedCharacter, themeIndex))
			{
				TrialManager.Instance.currentTrialInfo = null;
			}
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public List<KeyValuePair<Characters.CharacterType, Characters.Model>> GetCharacterList()
	{
		return this._characterList;
	}

	public int GetLastSelectedThemeForCharacterType(Characters.CharacterType charType)
	{
		int result = 0;
		if (CharacterThemes.GetThemeForCharacter(charType, this._playerInfo.GetIndexForLastSelectedTheme(charType)) != null)
		{
			result = this._playerInfo.GetIndexForLastSelectedTheme(charType);
		}
		return result;
	}

	public void InitCharacters()
	{
		this._characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		int i = 0;
		int count = Characters.characterOrder.Count;
		while (i < count)
		{
			Characters.Model value = Characters.characterData[Characters.characterOrder[i]];
			if (this._playerInfo.IsCollectionComplete(Characters.characterOrder[i]) || this._playerInfo.isCharacterActive(Characters.characterOrder[i]))
			{
				this._characterList.Add(new KeyValuePair<Characters.CharacterType, Characters.Model>(Characters.characterOrder[i], value));
			}
			i++;
		}
	}

	private void OnCharacterUnlocked(Characters.CharacterType character, int version)
	{
		if (this._onCharacterUnlocked != null)
		{
			this.SelectCharacter(character, version);
			this._onCharacterUnlocked(character, version);
		}
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, int themeIndex)
	{
		if (!this._purchaseInProgress)
		{
			this._purchaseInProgress = true;
			PurchaseHandler.Instance.PurchaseCharacter(characterType, themeIndex, false, null);
		}
	}

	public void RemoveOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			this._onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Remove(this._onCharacterUnlocked, handler);
		}
	}

	public void RemoveOnShownCharacterSelectedListener(Action handler)
	{
		if (handler != null)
		{
			this._onCharacterSelected = (Action)Delegate.Remove(this._onCharacterSelected, handler);
		}
	}

	public void SelectCharacter(Characters.CharacterType charType, int themeIndex)
	{
		UIModelController.Instance.SelectCharacterForPlay(charType, themeIndex);
		CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
		if (this._onCharacterSelected != null)
		{
			this._onCharacterSelected();
		}
	}

	public Characters.CharacterType currenCharacterShown
	{
		get
		{
			if (!this._hasCenteredOnCharacter)
			{
				this._hasCenteredOnCharacter = true;
				this._currentlyShownCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
			}
			return this._currentlyShownCharacter;
		}
		set
		{
			this._currentlyShownCharacter = value;
		}
	}

	public int currentCThemeShownIndex { get; set; }

	public static CharacterScreenManager Instance
	{
		get
		{
			if (CharacterScreenManager._instance == null)
			{
				CharacterScreenManager._instance = new CharacterScreenManager();
			}
			return CharacterScreenManager._instance;
		}
	}

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private Characters.CharacterType _currentlyShownCharacter;

	private bool _hasCenteredOnCharacter;

	private static CharacterScreenManager _instance;

	private Action _onCharacterSelected;

	private Action<Characters.CharacterType, int> _onCharacterUnlocked;

	private PlayerInfo _playerInfo = PlayerInfo.Instance;

	private bool _purchaseInProgress;
}
