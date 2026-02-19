using System;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterThemeShifter : MonoBehaviour
{
	public void AddOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			this._onThemeButtonPressed = (Action<int>)Delegate.Combine(this._onThemeButtonPressed, handler);
		}
	}

	private void Awake()
	{
		Vector3 localPosition = this._defaultCharacterThemeButton.transform.localPosition;
		Vector3 localPosition2 = this._customCharacterThemeButton.transform.localPosition;
		this._distanceBetweenButtons = localPosition2 - localPosition;
		this._customButtons.Add(this._customCharacterThemeButton.GetComponent<CharacterThemeButton>());
		this._defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().AddOnPressListener(new Action<int>(this.ThemeButtonPressed));
		this._customButtons[0].AddOnPressListener(new Action<int>(this.ThemeButtonPressed));
		this._selectedMarker.transform.position = this._defaultCharacterThemeButton.transform.position;
	}

	public void InitValues(Characters.CharacterType charType, int index)
	{
		this._cachedCharacter = charType;
		this._activeTheme = index;
	}

	public void MakeCustomThemesAvailable(List<CharacterTheme> themes)
	{
		this._currentCharacterThemes = themes;
		if (themes.Count > 0)
		{
			this._sortedThemes = this.SortThemes(themes);
		}
		int i = 0;
		int count = this._customButtons.Count;
		while (i < count)
		{
			this._customButtons[i].gameObject.SetActive(false);
			i++;
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count > 3) ? 3 : themes.Count);
		}
		this._defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().SetColors(this.CHARACTERS_CUSTOMIZATION_BASE_SKIN_FILL, this.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON, 0);
		for (int j = 0; j < num; j++)
		{
			if (this._customButtons.Count > j)
			{
				CharacterThemeButton characterThemeButton = this._customButtons[j];
				if (this._sortedThemes != null && this._sortedThemes.Length > j)
				{
					CharacterTheme characterTheme = this._sortedThemes[j];
					characterThemeButton.SetColors(characterTheme.buttonBgSpriteName, characterTheme.buttonIconSpriteName, themes.IndexOf(characterTheme) + 1);
				}
				characterThemeButton.gameObject.SetActive(true);
			}
			else
			{
				CharacterThemeButton component = UnityEngine.Object.Instantiate<GameObject>(this._customCharacterThemeButton).GetComponent<CharacterThemeButton>();
				if (component != null)
				{
					component.transform.parent = base.gameObject.transform;
					component.transform.localPosition = this._customCharacterThemeButton.transform.localPosition + this._distanceBetweenButtons * (float)j;
					component.transform.localScale = Vector3.one;
					component.name = j + 1 + "CharThemeBtn";
					if (this._sortedThemes != null && this._sortedThemes.Length > j)
					{
						CharacterTheme characterTheme2 = this._sortedThemes[j];
						component.SetColors(characterTheme2.buttonBgSpriteName, characterTheme2.buttonIconSpriteName, themes.IndexOf(characterTheme2) + 1);
					}
					this._customButtons.Add(component);
					component.AddOnPressListener(new Action<int>(this.ThemeButtonPressed));
				}
			}
		}
	}

	private void OnDisable()
	{
		int i = 0;
		int count = this._customButtons.Count;
		while (i < count)
		{
			this._customButtons[i].gameObject.SetActive(false);
			i++;
		}
	}

	public void RemoveOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			this._onThemeButtonPressed = (Action<int>)Delegate.Remove(this._onThemeButtonPressed, handler);
		}
	}

	private void SelectButton(int index)
	{
		bool flag = index == 0;
		if (this._selectedMarker != null)
		{
			if (flag)
			{
				this._selectedMarker.transform.position = this._defaultCharacterThemeButton.transform.position;
			}
			else
			{
				this._selectedMarker.transform.position = this._customButtons[this._currentCharacterThemes.IndexOf(this._sortedThemes[index - 1])].transform.position;
			}
		}
	}

	private CharacterTheme[] SortThemes(List<CharacterTheme> themes)
	{
		CharacterTheme[] array = new CharacterTheme[themes.Count];
		List<CharacterTheme> list = new List<CharacterTheme>();
		list.AddRange(themes);
		int num = int.MaxValue;
		CharacterTheme characterTheme = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			int j = 0;
			int count = list.Count;
			while (j < count)
			{
				if (list[j].uiPriority < num)
				{
					num = list[j].uiPriority;
					characterTheme = list[j];
				}
				j++;
			}
			array[i] = characterTheme;
			list.Remove(characterTheme);
			num = int.MaxValue;
		}
		return array;
	}

	private void ThemeButtonPressed(int index)
	{
		if (this._onThemeButtonPressed != null)
		{
			this._onThemeButtonPressed(index);
		}
	}

	public void UpdateUIForCharacter(Characters.CharacterType charType, int outfitIndex)
	{
		if (this._activeTheme != outfitIndex || this._cachedCharacter != charType)
		{
			this._cachedCharacter = UIModelController.Instance.currentCharacterModelShown;
			this._activeTheme = outfitIndex;
			PlayerInfo.Instance.ThemeSeen(this._cachedCharacter, outfitIndex);
			this.SelectButton(outfitIndex);
		}
	}

	[SerializeField]
	private GameObject _defaultCharacterThemeButton;

	[SerializeField]
	private GameObject _customCharacterThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	private int _activeTheme;

	private Characters.CharacterType _cachedCharacter;

	private List<CharacterTheme> _currentCharacterThemes = new List<CharacterTheme>();

	private List<CharacterThemeButton> _customButtons = new List<CharacterThemeButton>();

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private Action<int> _onThemeButtonPressed;

	private CharacterTheme[] _sortedThemes;

	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	public string CHARACTERS_CUSTOMIZATION_BASE_SKIN_FILL = string.Empty;

	public string CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON = string.Empty;
}
