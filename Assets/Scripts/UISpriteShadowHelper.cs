using System;
using UnityEngine;

[ExecuteInEditMode]
public class UISpriteShadowHelper : UIShadowHelper<UISprite>
{
	protected override void withUpdate()
	{
		if (this._front.color != this.frontColor)
		{
			this._front.color = this.frontColor;
		}
		if (this.shadow.color != this.shadowColor)
		{
			this.shadow.color = this.shadowColor;
		}
		if (this.shadow.spriteName != this._front.spriteName)
		{
			this.shadow.spriteName = this._front.spriteName;
		}
	}

	public Color frontColor = Color.white;

	public Color shadowColor = Color.magenta;
}
