using System;
using UnityEngine;

[ExecuteInEditMode]
public class UITextShadowHelper : UIShadowHelper<UILabel>
{
	protected override void withUpdate()
	{
		if (this._front.color != this.frontColor)
		{
			this._front.color = this.frontColor;
		}
		if (this._front.effectColor != this.shadowColor)
		{
			this._front.effectColor = this.shadowColor;
		}
		if (this.shadow.text != this._front.text)
		{
			this.shadow.text = this._front.text;
		}
		if (this.shadow.color != this.shadowColor)
		{
			this.shadow.color = this.shadowColor;
		}
	}

	public Color frontColor = Color.white;

	public Color shadowColor = Color.magenta;
}
