using System;
using UnityEngine;

public class SaveMeAnimateClock : MonoBehaviour
{
	public void FillSpriteAmount(float amount)
	{
		if (this.clockSprite != null)
		{
			this.clockSprite.fillAmount = amount;
			this.timeLabel.text = ((int)(amount * 10f)).ToString();
		}
		else
		{
			UnityEngine.Debug.Log("ClockSprite == NULL");
		}
	}

	public UISprite clockSprite;

	public UILabel timeLabel;
}
