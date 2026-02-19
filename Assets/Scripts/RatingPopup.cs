using System;
using UnityEngine;

public class RatingPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		this.RefreshLabel();
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_TITLE);
		this.descripeLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_CONTENT);
		this.rateLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_BUTTON_GOOD);
	}

	public void OkClicked()
	{
		RiseSdk.Instance.Rate();
		UIScreenController.Instance.ClosePopup(null);
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel descripeLbl;

	[SerializeField]
	private UILabel rateLbl;
}
