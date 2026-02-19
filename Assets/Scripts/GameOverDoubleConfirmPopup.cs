using System;
using UnityEngine;

public class GameOverDoubleConfirmPopup : UIBaseScreen
{
	public override void Init()
	{
		base.Init();
		UIEventListener uieventListener = UIEventListener.Get(this.confirmGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnDoubleClick);
	}

	public override void Show()
	{
		base.Show();
		if (PlayerInfo.Instance.firstGameOverNoRemind)
		{
			PlayerInfo.Instance.firstGameOverNoRemind = false;
			PlayerInfo.Instance.gameOverDoubleConfirmNoRemind = true;
		}
		this.toggle.value = PlayerInfo.Instance.gameOverDoubleConfirmNoRemind;
		this.RefreshLabel();
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_TITLE);
		this.descriptionLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_CONTENT);
		this.watchLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_BUTTON_WATCH);
		this.reminderLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_NO_REMINDER);
	}

	private void OnDoubleClick(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(4);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	public void OnToggleChange()
	{
		PlayerInfo.Instance.gameOverDoubleConfirmNoRemind = UIToggle.current.value;
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel descriptionLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel reminderLbl;

	[SerializeField]
	private GameObject confirmGo;

	[SerializeField]
	private UIToggle toggle;
}
