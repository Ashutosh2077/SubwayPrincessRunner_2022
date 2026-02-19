using System;
using Network;
using UnityEngine;

public class RankScreen : UIBaseScreen
{
	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_HIGHSCORE_TITLE);
		this.globalLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_GLOBAL_TITLE);
		this.vipLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_VIP_TITLE);
		this.friendLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_FRIEND_TITLE);
		this.changeNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_TITLE);
		this.submitLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_SUBMIT);
	}

	public override void Show()
	{
		base.Show();
		if (this.inputGo.activeInHierarchy)
		{
			this.inputGo.SetActive(false);
		}
		this.RefreshLabel();
		this.RefreshFacebook();
		this.RefreshMedals();
		if (Application.internetReachability == NetworkReachability.NotReachable || !SecondManager.Instance.hasInited || !ServeTimeUpdate.Instance.ServerTimeValid())
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
			return;
		}
		this.toggleScore.value = true;
		ServerManager.Instance.RegisterOnMedalsChange(new Action(this.RefreshMedals));
		ServerManager.Instance.RequestMedals();
		if (PlayerInfo.Instance.autoShowChangePlayerName && ServerManager.Instance.CanUploadPlayerName())
		{
			if (!this.inputGo.activeInHierarchy)
			{
				this.inputGo.SetActive(true);
			}
			PlayerName playerName = ServerManager.Instance.PlayerName;
			if (playerName != null)
			{
				this.inputField.value = playerName.Value;
			}
			PlayerInfo.Instance.autoShowChangePlayerName = false;
		}
	}

	public override void Hide()
	{
		if (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.CloseAllPopups();
		}
		ServerManager.Instance.UnregisterOnMedalsChange(new Action(this.RefreshMedals));
		this.highScore.Hide();
		this.vip.Hide();
		this.friends.Hide();
		base.Hide();
	}

	private void RefreshFacebook()
	{
		if (SecondManager.Instance.facebook)
		{
			this.connectCollider.enabled = false;
			this.connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
		}
		else
		{
			this.connectCollider.enabled = true;
			this.connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_CONNECT);
		}
	}

	private void RefreshMedals()
	{
		Medals medals = ServerManager.Instance.Medals;
		if (medals != null)
		{
			this.goldLbl.text = medals._gold.ToString();
			this.sliverLbl.text = medals._sliver.ToString();
			this.copperLbl.text = medals._copper.ToString();
		}
	}

	public void ChangeState(RankScreen.RankPopupType type)
	{
		this.currentType = type;
		if (type == RankScreen.RankPopupType.HighScore)
		{
			NGUITools.SetActive(this.highScore.gameObject, true);
			NGUITools.SetActive(this.vip.gameObject, false);
			NGUITools.SetActive(this.friends.gameObject, false);
			NGUITools.SetActive(this.timeLeft.gameObject, true);
			NGUITools.SetActive(this.myRankCell.gameObject, true);
			this.highScore.Show();
			this.myRankCell.transform.localPosition = Vector3.up * this.globalMyRankPosY;
		}
		else if (type == RankScreen.RankPopupType.VIP)
		{
			NGUITools.SetActive(this.highScore.gameObject, false);
			NGUITools.SetActive(this.vip.gameObject, true);
			NGUITools.SetActive(this.friends.gameObject, false);
			NGUITools.SetActive(this.timeLeft.gameObject, false);
			NGUITools.SetActive(this.myRankCell.gameObject, false);
			this.vip.Show();
		}
		else if (type == RankScreen.RankPopupType.Friends)
		{
			NGUITools.SetActive(this.highScore.gameObject, false);
			NGUITools.SetActive(this.vip.gameObject, false);
			NGUITools.SetActive(this.friends.gameObject, true);
			NGUITools.SetActive(this.timeLeft.gameObject, false);
			NGUITools.SetActive(this.myRankCell.gameObject, false);
			this.friends.Show();
		}
		else
		{
			NGUITools.SetActive(this.highScore.gameObject, false);
			NGUITools.SetActive(this.vip.gameObject, false);
			NGUITools.SetActive(this.friends.gameObject, false);
			NGUITools.SetActive(this.timeLeft.gameObject, false);
		}
		this.myRankCell.UpdateUI();
	}

	public void ToggleGlobalScrollView()
	{
		if (UIToggle.current.value)
		{
			this.ChangeState(RankScreen.RankPopupType.HighScore);
		}
	}

	public void ToggleVIPScrollView()
	{
		if (UIToggle.current.value)
		{
			this.ChangeState(RankScreen.RankPopupType.VIP);
		}
	}

	public void ToggleFriendScrollView()
	{
		if (UIToggle.current.value)
		{
			this.ChangeState(RankScreen.RankPopupType.Friends);
		}
	}

	public void OnChangeNameClick(GameObject go)
	{
		if (!this.inputGo.activeInHierarchy)
		{
			this.inputGo.SetActive(true);
		}
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null)
		{
			this.inputField.value = playerName.Value;
		}
	}

	public void OnConnectClick()
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
			this.connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
			this.connectCollider.enabled = false;
		}
	}

	public void SubmitNameClick()
	{
		if (this.inputGo.activeInHierarchy)
		{
			this.inputGo.SetActive(false);
		}
		string text = this.inputField.value.Trim();
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null && !string.IsNullOrEmpty(text) && !text.Equals(playerName.Value))
		{
			ServerManager.Instance.UploadPlayerName(text);
		}
	}

	public void OnChangeNameCloseClick()
	{
		if (this.inputGo.activeInHierarchy)
		{
			this.inputGo.SetActive(false);
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel globalLbl;

	[SerializeField]
	private UILabel vipLbl;

	[SerializeField]
	private UILabel friendLbl;

	[SerializeField]
	private UILabel changeNameLbl;

	[SerializeField]
	private UILabel submitLbl;

	[SerializeField]
	private UILabel connectLbl;

	[SerializeField]
	private float globalMyRankPosY;

	[SerializeField]
	private MyRankCell myRankCell;

	[SerializeField]
	private TimeLeft timeLeft;

	[SerializeField]
	private GlobalScrollView highScore;

	[SerializeField]
	private VipScrollView vip;

	[SerializeField]
	private FriendsScrollView friends;

	[SerializeField]
	private UIToggle toggleScore;

	[SerializeField]
	private UIToggle toggleVip;

	[SerializeField]
	private UIToggle toggleFriend;

	[SerializeField]
	private GameObject inputGo;

	[SerializeField]
	private UIInput inputField;

	[SerializeField]
	private BoxCollider connectCollider;

	[SerializeField]
	private UILabel goldLbl;

	[SerializeField]
	private UILabel sliverLbl;

	[SerializeField]
	private UILabel copperLbl;

	private string inputName;

	private Transform rankLoading;

	private RankScreen.RankPopupType currentType;

	public enum RankPopupType
	{
		HighScore,
		VIP,
		Friends,
		None
	}
}
