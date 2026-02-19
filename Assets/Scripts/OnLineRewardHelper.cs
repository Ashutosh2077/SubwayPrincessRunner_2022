using System;
using UnityEngine;

public class OnLineRewardHelper : MonoBehaviour
{
	public void SetIcomAndNumber(string icon, int number)
	{
		this.icon.spriteName = icon;
		this.number.text = number.ToString();
	}

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel number;
}
