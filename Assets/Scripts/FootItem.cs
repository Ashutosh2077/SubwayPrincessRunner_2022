using System;

[Serializable]
public class FootItem
{
	public void SetFill(bool isfill)
	{
		if (this.isFilled == isfill)
		{
			return;
		}
		this.isFilled = isfill;
		this.fill.enabled = isfill;
		this.fillIcon.enabled = isfill;
	}

	public UISprite fill;

	public UISprite fillIcon;

	private bool isFilled = true;
}
