using System;
using UnityEngine;

public class RedeemPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		this.RefreshLabel();
		this.coinAmount = 0;
		this.gemAmount = 0;
		int i = 0;
		int num = RecodeManager.Instance.GetRecodes().Length;
		while (i < num)
		{
			RecodeManager.Good good = RecodeManager.Instance.GetRecodes()[i];
			int num2;
			if (int.TryParse(good._num, out num2))
			{
				string id = good._id;
				if (id != null)
				{
					if (!(id == "1"))
					{
						if (!(id == "2"))
						{
							if (!(id == "3"))
							{
								if (!(id == "4"))
								{
									if (!(id == "5"))
									{
										if (!(id == "6"))
										{
										}
									}
								}
							}
						}
						else
						{
							this.gemAmount += num2;
						}
					}
					else
					{
						this.coinAmount += num2;
					}
				}
			}
			i++;
		}
		this.coinNum.text = "X" + this.coinAmount;
		this.gemNum.text = "X" + this.gemAmount;
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_REDEEM_TITLE);
		this.getLbl.text = Strings.Get(LanguageKey.UI_POPUP_REDEEM_BUTTON_GET);
	}

	public void GetRecodeReward()
	{
		RecodeManager.Instance.GetRecodeGoods();
		UIScreenController.Instance.ClosePopup(null);
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UISprite keyIcon;

	[SerializeField]
	private UILabel coinNum;

	[SerializeField]
	private UILabel gemNum;

	private int coinAmount;

	private int gemAmount;
}
