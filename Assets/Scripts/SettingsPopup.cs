using System;
using Network;
using UnityEngine;

public class SettingsPopup : UIBaseScreen
{
	private void Start()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.changeNameBtn.gameObject);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnChangeNameClick);
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_TITLE);
		this.nameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_NAME);
		this.redeemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_REDEEM);
		this.followLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_FOLLOW);
		this.enterNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_TITLE);
		this.submitLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_SUBMIT);
		this.redeemCodeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_TITLE);
		this.followUsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_FOLLOW_US);
		this.followUsRedeemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_FOLLOW_REDEEM);
		this.okLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_OK);
		this.connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_CONNECT);
		this.loginedLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
	}

	public override void Show()
	{
		base.Show();
		this.RefreshPlayerNameUI();
		this.RefreshLabel();
		if (FacebookManger.Instance.facebookHasLogin)
		{
			if (this.loginFacebookGo.activeInHierarchy)
			{
				this.loginFacebookGo.SetActive(false);
			}
			if (!this.hasLoginFacebookGo.activeInHierarchy)
			{
				this.hasLoginFacebookGo.SetActive(true);
			}
		}
		else
		{
			if (!this.loginFacebookGo.activeInHierarchy)
			{
				this.loginFacebookGo.SetActive(true);
			}
			if (this.hasLoginFacebookGo.activeInHierarchy)
			{
				this.hasLoginFacebookGo.SetActive(false);
			}
		}
	}

	public void OnSendGiftClick()
	{
		this.sendRecodePanel.SetActive(true);
		this.inputRecodeField.value = string.Empty;
	}

	public void CloseSendRecodePanel()
	{
		this.sendRecodePanel.SetActive(false);
	}

	public void SendGiftRecode()
	{
		this.sendRecodePanel.SetActive(false);
		RecodeManager.Instance.SendRecode(this.inputRecodeField.value.ToString());
	}

	public void OnChangeNameClick(GameObject go)
	{
		this.changeNamePanel.SetActive(true);
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null)
		{
			this.inputField.value = playerName.Value;
		}
	}

	public void OnChangeNameCloseClick()
	{
		this.changeNamePanel.SetActive(false);
	}

	public void SubmitNameClick()
	{
		this.changeNamePanel.SetActive(false);
		this.inputName = this.inputField.value;
		this.inputName = this.inputName.Trim();
		if (!string.IsNullOrEmpty(this.inputName))
		{
			ServerManager.Instance.UploadPlayerName(this.inputName);
		}
	}

	private void OnEnable()
	{
		ServerManager.Instance.RegisterOnPlayerNameChange(new Action(this.RefreshPlayerNameUI));
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPlayerNameChange(new Action(this.RefreshPlayerNameUI));
	}

	private void RefreshPlayerNameUI()
	{
		if (SecondManager.Instance.facebook)
		{
			this.playerNameLbl.text = FacebookManger.Instance.me.name;
		}
		else
		{
			PlayerName playerName = ServerManager.Instance.PlayerName;
			if (playerName != null)
			{
				this.playerNameLbl.text = playerName.Value;
			}
		}
		if (ServerManager.Instance.CanUploadPlayerName())
		{
			this.changeNameBtn.color = Color.white;
			this.changeNameCollider.enabled = true;
		}
		else
		{
			this.changeNameBtn.color = Color.cyan;
			this.changeNameCollider.enabled = false;
		}
	}

	public void URL()
	{
		FacebookManger.Instance.URL();
	}

	public void OnFacebookLoginClick()
	{
		FacebookManger instance = FacebookManger.Instance;
		instance.OnFacebookLoginResult = (Action<bool>)Delegate.Remove(instance.OnFacebookLoginResult, new Action<bool>(this.OnLoginFacebookResult));
		FacebookManger instance2 = FacebookManger.Instance;
		instance2.OnFacebookLoginResult = (Action<bool>)Delegate.Combine(instance2.OnFacebookLoginResult, new Action<bool>(this.OnLoginFacebookResult));
		FacebookManger.Instance.LoginFacebook();
	}

	private void OnLoginFacebookResult(bool result)
	{
		FacebookManger instance = FacebookManger.Instance;
		instance.OnFacebookLoginResult = (Action<bool>)Delegate.Remove(instance.OnFacebookLoginResult, new Action<bool>(this.OnLoginFacebookResult));
		if (result)
		{
			if (this.loginFacebookGo.activeInHierarchy)
			{
				this.loginFacebookGo.SetActive(false);
			}
			if (!this.hasLoginFacebookGo.activeInHierarchy)
			{
				this.hasLoginFacebookGo.SetActive(true);
			}
			this.changeNameBtn.color = Color.cyan;
			this.changeNameCollider.enabled = false;
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel nameLbl;

	[SerializeField]
	private UILabel redeemLbl;

	[SerializeField]
	private UILabel followLbl;

	[SerializeField]
	private UILabel enterNameLbl;

	[SerializeField]
	private UILabel submitLbl;

	[SerializeField]
	private UILabel redeemCodeLbl;

	[SerializeField]
	private UILabel connectLbl;

	[SerializeField]
	private UILabel loginedLbl;

	[SerializeField]
	private UILabel followUsLbl;

	[SerializeField]
	private UILabel followUsRedeemLbl;

	[SerializeField]
	private UILabel okLbl;

	[SerializeField]
	private UILabel playerNameLbl;

	[SerializeField]
	private UISprite changeNameBtn;

	[SerializeField]
	private BoxCollider changeNameCollider;

	[SerializeField]
	private GameObject changeNamePanel;

	[SerializeField]
	private UIInput inputField;

	[SerializeField]
	private GameObject sendRecodePanel;

	[SerializeField]
	private UIInput inputRecodeField;

	[SerializeField]
	private GameObject loginFacebookGo;

	[SerializeField]
	private GameObject hasLoginFacebookGo;

	private string inputName;

	public static bool IsInGameOver;

	public static bool IsInPause;
}
