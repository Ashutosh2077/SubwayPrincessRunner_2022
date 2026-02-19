using System;
using UnityEngine;

public class CharacterThemeButton : MonoBehaviour
{
	public void AddOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			this._onPress = (Action<int>)Delegate.Combine(this._onPress, handler);
		}
	}

	private void ButtonPressed()
	{
		Action<int> onPress = this._onPress;
		if (onPress != null)
		{
			onPress(this._index);
		}
	}

	public void RemoveOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			this._onPress = (Action<int>)Delegate.Remove(this._onPress, handler);
		}
	}

	public void SetColors(string bgColor, string iconColor, int index)
	{
		this._background.spriteName = bgColor;
		this._icon.spriteName = iconColor;
		this._index = index;
	}

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	private int _index;

	private Action<int> _onPress;
}
