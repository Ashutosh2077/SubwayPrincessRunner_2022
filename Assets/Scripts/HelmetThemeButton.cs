using System;
using UnityEngine;

public class HelmetThemeButton : MonoBehaviour
{
	public event HelmetThemeButton.OnPressEvent OnPress;

	private void ButtonPressed()
	{
		HelmetThemeButton.OnPressEvent onPress = this.OnPress;
		if (onPress != null)
		{
			onPress(this._index);
		}
	}

	public void InitButton(string iconName, Color32 bgColor, Color32 iconColor, int index)
	{
		if (!string.IsNullOrEmpty(iconName))
		{
			this._icon.spriteName = iconName;
		}
		this._icon.color = iconColor;
		this._background.color = bgColor;
		this._index = index;
	}

	public void SetButtonState(int newState)
	{
		if (newState != 0)
		{
			if (newState != 1)
			{
				if (newState == 2)
				{
					this.selectedSprite.SetActive(true);
					this.ownedSprite.SetActive(false);
				}
			}
			else
			{
				this.selectedSprite.SetActive(false);
				this.ownedSprite.SetActive(true);
			}
		}
		else
		{
			this.selectedSprite.SetActive(false);
			this.ownedSprite.SetActive(false);
		}
	}

	public int ThemeIndex
	{
		get
		{
			return this._index;
		}
	}

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private GameObject selectedSprite;

	[SerializeField]
	private GameObject ownedSprite;

	private int _index;

	public delegate void OnPressEvent(int index);
}
