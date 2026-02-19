using System;
using UnityEngine;

public class CoinButtonHelper : DiscountButton
{
	private void _Setup()
	{
		if (InAppData.inAppData[this.index].type == InAppData.DataType.Coin)
		{
			this.ShowNoDiscount(this.index, string.Format(Strings.Get(LanguageKey.COIN_BUTTON_HELPER_NUMBER_OF_COINS), InAppData.inAppData[this.index].amountOfCoins));
		}
		else
		{
			this.ShowNoDiscount(this.index, string.Format(Strings.Get(LanguageKey.COIN_BUTTON_HELPER_NUMBER_OF_KEYS), InAppData.inAppData[this.index].amountOfKeys));
		}
		if (base.transform.parent == this.onDisplayParent)
		{
			base.transform.parent = this.onDisplayParent.parent;
		}
	}

	public void Init(int sendIndex)
	{
		this.index = sendIndex;
		foreach (object obj in base.transform.parent)
		{
			Transform transform = (Transform)obj;
			if (transform.name == "000")
			{
				this.onDisplayParent = transform;
			}
		}
		this._Setup();
	}

	public void OnClick()
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			RiseSdk.Instance.Pay(this.index);
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	private int index;

	private Transform onDisplayParent;
}
