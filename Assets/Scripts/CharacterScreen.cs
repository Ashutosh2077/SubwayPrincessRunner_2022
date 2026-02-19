using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreen : UIBaseScreen, IScrollClick
{
	private void CenterScrollOnCharacterType(Characters.CharacterType charType)
	{
		int num = 0;
		int i = 0;
		int count = this._characterList.Count;
		while (i < count)
		{
			if (charType == this._characterList[i].Key)
			{
				break;
			}
			num++;
			i++;
		}
		if (num >= this.characterIndices.Count)
		{
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				"CharacterScreen: Index of character: ",
				num,
				" - is bigger or equal than character indices count: ",
				this.characterIndices.Count
			}));
			num = this.characterIndices.Count - 1;
		}
		bool flag = false;
		if (this._centerer.centeredObject != null)
		{
			flag = true;
		}
		if (flag)
		{
			this.ChangeCurrentCharacterShow(this._screenManagerInstance.currenCharacterShown, this._screenManagerInstance.currentCThemeShownIndex);
		}
		else
		{
			this._centerer.CenterOnTransform(this.characterIndices[num].transform, true);
			this._centerer.Recenter();
		}
	}

	private void ChangeCurrentCharacterShow(Characters.CharacterType charType, int index)
	{
		this._screenManagerInstance.currenCharacterShown = charType;
		this._screenManagerInstance.currentCThemeShownIndex = index;
		this.DisplayCharacter3dModel(charType, index);
		this.UpdateUIElements();
		List<CharacterTheme> themes = CharacterThemes.TryGetCustomThemesForChar(charType);
		bool flag = CharacterThemes.characterCustomThemes.ContainsKey(charType);
		this.characterThemeShifter.gameObject.SetActive(flag);
		if (flag)
		{
			this.characterThemeShifter.MakeCustomThemesAvailable(themes);
			this.characterThemeShifter.UpdateUIForCharacter(charType, index);
		}
	}

	private void ClearArraysAndDestroyCachedGameObjects()
	{
		int i = 0;
		int count = this.characterModels.Count;
		while (i < count)
		{
			UnityEngine.Object.Destroy(this.characterModels[i]);
			i++;
		}
		this.characterModels.Clear();
		int j = 0;
		int count2 = this.characterIndices.Count;
		while (j < count2)
		{
			UnityEngine.Object.Destroy(this.characterIndices[j].gameObject);
			j++;
		}
		this.characterIndices.Clear();
		foreach (object obj in this.characterGrid.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(transform);
		}
		if (!this.characterAnchor.activeSelf)
		{
			this.characterAnchor.SetActive(true);
		}
		this.characterAnchor.transform.localPosition = new Vector3(0f, this.characterAnchor.transform.localPosition.y, this.characterAnchor.transform.localPosition.z);
		this.scrollPanel.cachedTransform.localPosition = Vector3.zero;
		this.scrollPanel.clipOffset = new Vector2(0f, 0f);
		this._centerer.ClearCenterObject();
		SpringPanel component = this.scrollPanel.GetComponent<SpringPanel>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private void DisplayCharacter3dModel(Characters.CharacterType charType, int themeIndex)
	{
		UIModelController.Instance.ShowMenuCharacterModel(charType, themeIndex);
	}

	public override void Hide()
	{
		base.Hide();
		this.characterThemeShifter.RemoveOnChangeThemeListener(new Action<int>(this.ThemeButtonClicked));
		UIModelController.Instance.ClearModels();
		CharacterScreenManager.Instance.RemoveOnCharacterUnlockedListener(new Action<Characters.CharacterType, int>(this.OnCharacterUnlocked));
		CharacterScreenManager.Instance.RemoveOnShownCharacterSelectedListener(new Action(this.UpdateSelectedCharacter));
	}

	public override void Init()
	{
		base.Init();
		this._screenManagerInstance = CharacterScreenManager.Instance;
		this._cellWidth = this.characterGrid.cellWidth;
		this._centerer = this.characterGrid.GetComponent<CenterOnChild>();
		this.InitScrollWithCharacterModels();
		this.InitializeSelectButton();
		base.InitializeCoinbox(true, true, true, 0f, 0f, 0f);
		this._hasInited = true;
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(this.centerAnchor, this.selectButtonPrefab);
		this._characterSelectButton = gameObject.GetComponent<CharacterScreenSelectButton>();
		gameObject.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.characterScreenSelectedButtonPos;
		this.characterThemeShifter.InitValues(this._screenManagerInstance.currenCharacterShown, this._screenManagerInstance.currentCThemeShownIndex);
		this._characterSelectButton.InitButton();
	}

	private void InitScrollWithCharacterModels()
	{
		this._screenManagerInstance.InitCharacters();
		this._characterList = this._screenManagerInstance.GetCharacterList();
		int num = 0;
		int i = 0;
		int count = this._characterList.Count;
		while (i < count)
		{
			Characters.CharacterType key = this._characterList[i].Key;
			int lastSelectedThemeForCharacterType = this._screenManagerInstance.GetLastSelectedThemeForCharacterType(key);
			GameObject characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(key.ToString(), lastSelectedThemeForCharacterType);
			if (characterModelSample == null)
			{
				UnityEngine.Debug.LogError("Character Screen: GetCharacterModelPreview for character " + key + " has failed. Creating default model.");
				characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(Characters.CharacterType.frank.ToString(), 0);
			}
			characterModelSample.name = string.Format("{0:000}{1}", num, key.ToString());
			this.characterModels.Add(characterModelSample);
			Transform transform = characterModelSample.transform;
			transform.parent = this.characterAnchor.transform;
			transform.localPosition = new Vector3((float)num * this._cellWidth, 0f, 50f);
			transform.localScale = Vector3.one * (float)this.minScale;
			transform.localEulerAngles = new Vector3(53f, 183f, 360f);
			transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			GameObject gameObject = NGUITools.AddChild(this.characterGrid.gameObject, this.dummyObject);
			this.characterIndices.Add(gameObject.AddComponent<OverlayIndex>());
			this.characterIndices[num].index = num;
			gameObject.name = string.Format("{0:000}{1}", num, key.ToString());
			num++;
			i++;
		}
		Utility.SetLayerRecursively(this.characterAnchor.transform, 20);
		this.characterGrid.Reposition();
		this.CenterScrollOnCharacterType(this._screenManagerInstance.currenCharacterShown);
		this.ScaleScrollModels();
	}

	private void OnCharacterUnlocked(Characters.CharacterType charType, int version)
	{
		this.DisplayCharacter3dModel(charType, version);
		this.UpdateUIElements();
	}

	private void ScaleScrollModels()
	{
		float num = Mathf.Abs(this.characterAnchor.transform.localPosition.x);
		for (int i = 0; i < this.characterModels.Count; i++)
		{
			float num2 = Mathf.Abs(num - (float)i * this._cellWidth);
			float num3 = 1.5f * this._cellWidth;
			float d = Mathf.SmoothStep((float)this.maxScale, (float)this.minScale, num2 / num3);
			this.characterModels[i].transform.localScale = Vector3.one * d;
		}
	}

	public void ScrollClicked(Vector2 pos)
	{
		Characters.CharacterType currenCharacterShown = this._screenManagerInstance.currenCharacterShown;
		bool flag = this._centerer.CenterOnClosestChildAtPosition(pos);
		bool flag2 = PlayerInfo.Instance.IsCollectionComplete(currenCharacterShown);
		int indexForLastSelectedTheme = PlayerInfo.Instance.GetIndexForLastSelectedTheme(currenCharacterShown);
		bool flag3 = indexForLastSelectedTheme == this._screenManagerInstance.currentCThemeShownIndex;
		if (flag && flag2 && flag3)
		{
			UIModelController.Instance.SelectThemeForCurrentCharacterModel(indexForLastSelectedTheme);
			this.UpdateSelectedCharacter();
			NGUITools.PlaySound(this.selectSound);
		}
	}

	public override void Show()
	{
		base.Show();
		this.characterThemeShifter.AddOnChangeThemeListener(new Action<int>(this.ThemeButtonClicked));
		CharacterScreenManager.Instance.AddOnCharacterUnlockedListener(new Action<Characters.CharacterType, int>(this.OnCharacterUnlocked));
		CharacterScreenManager.Instance.AddOnShownCharacterSelectedListener(new Action(this.UpdateSelectedCharacter));
		if (CharacterScreen.forceCenterOnCurrentlySelectedCharacter)
		{
			CharacterScreen.forceCenterOnCurrentlySelectedCharacter = false;
			this.CenterScrollOnCharacterType((Characters.CharacterType)PlayerInfo.Instance.currentCharacter);
		}
		else if (this._hasShownModel)
		{
			this.ChangeCurrentCharacterShow(this._screenManagerInstance.currenCharacterShown, this._screenManagerInstance.currentCThemeShownIndex);
		}
		else
		{
			this.Update();
		}
		this.characterThemeShifter.gameObject.SetActive(CharacterThemes.characterCustomThemes.ContainsKey(this._screenManagerInstance.currenCharacterShown));
	}

	private void ThemeButtonClicked(int index)
	{
		this._screenManagerInstance.currentCThemeShownIndex = index;
		this.DisplayCharacter3dModel(this._screenManagerInstance.currenCharacterShown, index);
		this.UpdateUIElements();
		this.characterThemeShifter.UpdateUIForCharacter(this._screenManagerInstance.currenCharacterShown, index);
	}

	private void Update()
	{
		if (this._hasInited)
		{
			if (this._centerer.centeredObject != null)
			{
				int index = this._centerer.centeredObject.GetComponent<OverlayIndex>().index;
				Characters.CharacterType key = this._characterList[index].Key;
				if (!this._hasShownModel)
				{
					this._hasShownModel = true;
					this.ChangeCurrentCharacterShow(key, PlayerInfo.Instance.GetIndexForLastSelectedTheme(key));
				}
				Characters.CharacterType currenCharacterShown = this._screenManagerInstance.currenCharacterShown;
				if (currenCharacterShown != key)
				{
					int index2 = PlayerInfo.Instance.GetIndexForLastSelectedTheme(key);
					if (key == (Characters.CharacterType)PlayerInfo.Instance.currentCharacter)
					{
						index2 = PlayerInfo.Instance.currentThemeIndex;
					}
					this.ChangeCurrentCharacterShow(key, index2);
				}
			}
			this.ScaleScrollModels();
			if (UIScreenController.Instance.isShowingPopup)
			{
				if (!this._popupActive || !this._inappOverlayActive)
				{
					this._popupActive = true;
				}
			}
			else if (this._popupActive && !this._inappOverlayActive)
			{
				this._popupActive = false;
			}
			if (this._inappOverlayActive)
			{
				this._inappOverlayActive = false;
			}
			if (this._inappOverlayActive || this._popupActive)
			{
				if (this._charactersEnabled)
				{
					this.characterAnchor.SetActive(false);
					this._charactersEnabled = false;
				}
			}
			else if (!this._charactersEnabled)
			{
				this.characterAnchor.SetActive(true);
				this._charactersEnabled = true;
			}
		}
	}

	private void UpdateSelectedCharacter()
	{
		Characters.CharacterType currentCharacterModelShown = UIModelController.Instance.currentCharacterModelShown;
		int currentCThemeShownIndex = UIModelController.Instance.currentCThemeShownIndex;
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentCharacterModelShown);
		bool flag2 = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentCharacterModelShown, currentCThemeShownIndex);
		if (flag && flag2)
		{
			PlayerInfo.Instance.SetLastSelectedTheme(currentCharacterModelShown, currentCThemeShownIndex);
			this.characterThemeShifter.UpdateUIForCharacter(currentCharacterModelShown, currentCThemeShownIndex);
			this._characterSelectButton.ReloadButton();
			this.RefreshCharacterModelInScrollList(currentCharacterModelShown, currentCThemeShownIndex);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Trying to select a character or theme we don't currently own.");
		}
	}

	private void RefreshCharacterModelInScrollList(Characters.CharacterType ctype, int themeIndex)
	{
		GameObject gameObject = this.characterModels.Find((GameObject g) => g.name.Substring(3).Equals(ctype.ToString()));
		int index = this.characterModels.IndexOf(gameObject);
		this.characterModels.Remove(gameObject);
		GameObject characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(ctype.ToString(), themeIndex);
		this.characterModels.Insert(index, characterModelSample);
		characterModelSample.name = gameObject.name;
		Transform transform = characterModelSample.transform;
		Transform transform2 = gameObject.transform;
		if (!this.characterAnchor.activeSelf)
		{
			this.characterAnchor.SetActive(true);
		}
		transform.parent = this.characterAnchor.transform;
		transform.localPosition = transform2.localPosition;
		transform.localScale = transform2.localScale;
		transform.localEulerAngles = transform2.localEulerAngles;
		Utility.SetLayerRecursively(transform, 20);
	}

	private void UpdateUIElements()
	{
		Characters.Model model = Characters.characterData[this._screenManagerInstance.currenCharacterShown];
		if (model.freeReviveCount > 0)
		{
			this.skillLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_R), model.freeReviveCount);
			this.saveMeSkillGo.SetActive(true);
		}
		else
		{
			this.saveMeSkillGo.SetActive(false);
		}
		this._characterSelectButton.ReloadButton();
	}

	[SerializeField]
	private GameObject characterAnchor;

	[SerializeField]
	private GameObject saveMeSkillGo;

	[SerializeField]
	private UILabel skillLbl;

	[SerializeField]
	private UIGrid characterGrid;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private UICharacterThemeShifter characterThemeShifter;

	[SerializeField]
	private GameObject selectButtonPrefab;

	[SerializeField]
	private GameObject centerAnchor;

	private float _cellWidth;

	private CenterOnChild _centerer;

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private CharacterScreenSelectButton _characterSelectButton;

	private bool _charactersEnabled;

	private bool _hasInited;

	private bool _hasShownModel;

	private bool _inappOverlayActive;

	private bool _popupActive;

	private CharacterScreenManager _screenManagerInstance;

	private List<OverlayIndex> characterIndices = new List<OverlayIndex>();

	private List<GameObject> characterModels = new List<GameObject>();

	public static bool forceCenterOnCurrentlySelectedCharacter;

	public static bool forceRefreshOnCharacterScreen;

	[SerializeField]
	private int minScale = 12;

	[SerializeField]
	private int maxScale = 16;
}
