using System;
using UnityEngine;

public class ShopBackgroundScreen : UIBaseScreen
{
	public override void Hide()
	{
		base.Hide();
		this.HideBackground();
	}

	public void HideBackground()
	{
		this.background.enabled = false;
	}

	public override void Show()
	{
		base.Show();
		this.ShowBackground();
	}

	public void ShowBackground()
	{
		this.background.enabled = true;
	}

	[SerializeField]
	private UITexture background;
}
