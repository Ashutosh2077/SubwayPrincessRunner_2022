using System;
using System.Collections;
using UnityEngine;

public class GameOverNewUI : MonoBehaviour, IGameOverUI
{
	public void Init(GameOverScreen screen)
	{
		this.gameOverScreen = screen;
		UIEventListener uieventListener = UIEventListener.Get(this.claimGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnClaimClick);
		uieventListener = UIEventListener.Get(this.tryCharacterGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnTryClick);
		uieventListener = UIEventListener.Get(this.doubleViewGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnDoubleClick);
		uieventListener = UIEventListener.Get(this.lotteryGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnLotteryClick);
		this.width = (float)Screen.width * UIScreenController.Instance.root.pixelSizeAdjustment;
		this.showTryRole = false;
	}

	private void OnEnable()
	{
		GameStats.Instance.OnGameOverPlayLotteryCountIncreased = (Action)Delegate.Combine(GameStats.Instance.OnGameOverPlayLotteryCountIncreased, new Action(this.OnGameOverPlayLotteryCountIncreased));
	}

	private void OnDisable()
	{
		GameStats.Instance.OnGameOverPlayLotteryCountIncreased = (Action)Delegate.Remove(GameStats.Instance.OnGameOverPlayLotteryCountIncreased, new Action(this.OnGameOverPlayLotteryCountIncreased));
	}

	private void RefreshLabel()
	{
		this.claimLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_CLAIM);
		this.luckyWheelLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_LOTTERY);
		this.tryItWatchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_TRY);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		this.midGo.transform.localPosition = new Vector3(this.width, 0f, 0f);
		if (this.upGo.activeInHierarchy)
		{
			this.upGo.SetActive(false);
		}
		this.RefreshLabel();
		this.isAfterDoubleClick = false;
		GameStats.Instance.gameOverPlayLotteryCount = 0;
		this.showConfirmPopup = false;
		this.claimGo.transform.localPosition = new Vector3(180f, 60f, 0f);
		this.doubleViewGo.transform.localPosition = new Vector3(-180f, 60f, 0f);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void RefreshUI(bool showTryRole, int coins)
	{
		this.showTryRole = showTryRole;
		if (coins == 0)
		{
			this.gameOverScreen.AnimateFoot(delegate
			{
				this.ShowLotteryBtn(this.showTryRole);
			});
			return;
		}
		this.claimGo.SetActive(false);
		SpringPosition.Begin(this.midGo, Vector3.zero, this.midSpringStrength).onFinished = delegate()
		{
			base.StartCoroutine(this.ShowMid());
		};
		this.collider_claim.enabled = true;
		this.gameOverDoubleCoinViewRate = PlayerInfo.Instance.CheckGameOverDoubleCoinViewRate(coins);
		this.label_claim.text = coins.ToString();
		this.label_double.text = (coins * this.gameOverDoubleCoinViewRate).ToString();
		this.Sprite_doubleRate.enabled = false;
		this.Sprite_doubleRate.spriteName = "icon_X" + this.gameOverDoubleCoinViewRate;
		bool flag = RiseSdk.Instance.HasRewardAd();
		if (flag)
		{
			this.collider_double.enabled = true;
			this.doubleViewSpr.color = Color.white;
			if (this.remoteValue == 0)
			{
				this.ts_double.PlayForward();
			}
			else
			{
				this.ts_double.enabled = false;
			}
		}
		else
		{
			this.ts_double.enabled = false;
			this.collider_double.enabled = false;
			this.ts_double.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			this.doubleViewSpr.color = Color.cyan;
		}
	}

	private IEnumerator ShowMid()
	{
		this.Sprite_doubleRate.enabled = true;
		Animation anim = this.Sprite_doubleRate.GetComponent<Animation>();
		anim.Play("UI_chuo");
		this.scoreCounterSoundPlayer.PlayVipSound();
		float time = anim["UI_chuo"].length;
		float factor = 0f;
		while (factor < time)
		{
			factor += Time.deltaTime;
			yield return null;
		}
		this.scoreCounterSoundPlayer.StopVipSound();
		this.claimGo.SetActive(true);
		yield break;
	}

	public void AfterTryCharacter()
	{
		if (this.trialInfo.type == TrialType.Character)
		{
			Game.Instance.TestCharacter(this.trialInfo.characterType, this.trialInfo.characterThemeId);
		}
		else if (this.trialInfo.type == TrialType.Helmet)
		{
			Game.Instance.TestHelmet(this.trialInfo.helmetType);
		}
		if (this.showConfirmPopup)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		if (this.upGo.activeInHierarchy)
		{
			this.upGo.SetActive(false);
		}
	}

	private void OnDoubleClick(GameObject go)
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_double", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_double", 0, null);
		if (PlayerInfo.Instance.gameOverDoubleConfirmNoRemind)
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
			this.showConfirmPopup = false;
		}
		else
		{
			this.showConfirmPopup = true;
			UIScreenController.Instance.PushPopup("ConfirmPopup");
		}
	}

	private void OnTryClick(GameObject go)
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_try_endless", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_endless", 0, null);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(10);
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

	public void AfterDoubleCoins(int coins)
	{
		this.collider_double.enabled = false;
		this.collider_claim.enabled = false;
		this.isAfterDoubleClick = true;
		if (PlayerInfo.Instance.hasSubscribed)
		{
			if (coins > 0)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, new Action(this.HideMid), coins * this.gameOverDoubleCoinViewRate * 2);
			}
			PlayerInfo.Instance.amountOfCoins += coins * this.gameOverDoubleCoinViewRate * 2;
		}
		else
		{
			if (coins > 0)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, new Action(this.HideMid), coins * this.gameOverDoubleCoinViewRate);
			}
			PlayerInfo.Instance.amountOfCoins += coins * this.gameOverDoubleCoinViewRate;
		}
		PlayerInfo.Instance.ResetGameOverDoubleCoinViewRate();
	}

	private void HideMid()
	{
		if (this.showConfirmPopup)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		SpringPosition.Begin(this.midGo, new Vector3(-this.width, 0f, 0f), this.midSpringStrength).onFinished = delegate()
		{
			this.gameOverScreen.AnimateFoot(delegate
			{
				if (this.isAfterDoubleClick)
				{
					return;
				}
				this.ShowLotteryBtn(this.showTryRole);
			});
		};
	}

	private void ShowLotteryBtn(bool showTryRole)
	{
		this.trialInfo = TrialManager.Instance.SelectValidlyTrialInfo();
		if (this.trialInfo == null)
		{
			showTryRole = false;
		}
		else
		{
			this.tryIconSpr.spriteName = this.trialInfo.icon;
		}
		if (!this.upGo.activeInHierarchy)
		{
			this.upGo.SetActive(true);
		}
		if (this.tryCharacterGo.activeInHierarchy != showTryRole)
		{
			this.tryCharacterGo.SetActive(showTryRole);
		}
		if (this.lotteryGo.activeInHierarchy == showTryRole)
		{
			this.lotteryGo.SetActive(!showTryRole);
		}
		if (RiseSdk.Instance.HasRewardAd())
		{
			this.upFillSpr.color = Color.white;
			this.upIconSpr.color = Color.white;
			if (showTryRole)
			{
				this.collider_try.enabled = true;
				this.tryIconSpr.color = Color.white;
			}
			else
			{
				this.collider_lottery.enabled = true;
				this.lotteryIconSpr.color = Color.white;
			}
			this.ps_lotter_try.Play();
			this.ts_up.enabled = false;
			base.StartCoroutine(this.GameObjectScale(this.upGo.transform, Vector3.zero, this.ts_up.from, 0.5f, delegate
			{
				this.ts_up.PlayForward();
			}));
		}
		else
		{
			this.upFillSpr.color = Color.cyan;
			this.upIconSpr.color = Color.cyan;
			if (showTryRole)
			{
				this.collider_try.enabled = false;
				this.tryIconSpr.color = Color.cyan;
			}
			else
			{
				this.collider_lottery.enabled = false;
				this.lotteryIconSpr.color = Color.cyan;
			}
			this.ps_lotter_try.Stop();
			this.ts_up.enabled = false;
		}
	}

	private void OnClaimClick(GameObject go)
	{
		this.collider_double.enabled = false;
		this.collider_claim.enabled = false;
		this.isAfterDoubleClick = false;
		PlayerInfo.Instance.amountOfCoins += GameStats.Instance.coins;
		PlayerInfo.Instance.DonotClickDoubleCoinView();
		base.StartCoroutine(this.ClaimClickDelay());
	}

	private IEnumerator ClaimClickDelay()
	{
		this.ps_claim.Play();
		yield return new WaitForSeconds(this.ps_claim.main.startLifetimeMultiplier + 0.2f);
		this.HideMid();
		yield break;
	}

	private void OnLotteryClick(GameObject go)
	{
		WheelSurfPopup.screenUI = ScreenUI.GameOverUI;
		UIScreenController.Instance.PushPopup("LotteryPopup");
	}

	public void AfterLotteryClick()
	{
		WheelSurfPopup.screenUI = ScreenUI.GameOverUI;
		UIScreenController.Instance.PushPopup("LotteryPopup");
	}

	private void OnGameOverPlayLotteryCountIncreased()
	{
		base.StartCoroutine(this.GameObjectScale(this.upGo.transform, this.upGo.transform.localScale, Vector3.zero, 0.5f, delegate
		{
			if (this.upGo.activeInHierarchy)
			{
				this.upGo.SetActive(false);
			}
		}));
	}

	private IEnumerator GameObjectScale(Transform trans, Vector3 from, Vector3 to, float duration, Action onFinished)
	{
		trans.localScale = from;
		float countFactor = 0f;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / duration;
			trans.localScale = Vector3.Slerp(from, to, countFactor);
			yield return null;
		}
		trans.localScale = to;
		yield return null;
		if (onFinished != null)
		{
			onFinished();
		}
		yield break;
	}

	public bool FootAutoShow()
	{
		return false;
	}

	[SerializeField]
	private UILabel luckyWheelLbl;

	[SerializeField]
	private UILabel tryItWatchLbl;

	[SerializeField]
	private UILabel claimLbl;

	[SerializeField]
	private GameObject midGo;

	[SerializeField]
	private float midSpringStrength;

	[SerializeField]
	private GameObject claimGo;

	[SerializeField]
	private ParticleSystem ps_claim;

	[SerializeField]
	private ParticleSystem ps_lotter_try;

	[SerializeField]
	private UILabel label_claim;

	[SerializeField]
	private BoxCollider collider_claim;

	[SerializeField]
	private GameObject doubleViewGo;

	[SerializeField]
	private UILabel label_double;

	[SerializeField]
	private UISprite Sprite_doubleRate;

	[SerializeField]
	private TweenScale ts_double;

	[SerializeField]
	private BoxCollider collider_double;

	[SerializeField]
	private UISprite doubleViewSpr;

	[SerializeField]
	private GameObject lotteryGo;

	[SerializeField]
	private BoxCollider collider_lottery;

	[SerializeField]
	private UISprite lotteryIconSpr;

	[SerializeField]
	private UILabel lotteryLbl;

	[SerializeField]
	private GameObject tryCharacterGo;

	[SerializeField]
	private BoxCollider collider_try;

	[SerializeField]
	private UISprite tryIconSpr;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private GameObject upGo;

	[SerializeField]
	private UISprite upFillSpr;

	[SerializeField]
	private UISprite upIconSpr;

	[SerializeField]
	private TweenScale ts_up;

	[SerializeField]
	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	private int remoteValue;

	private bool showTryRole;

	private bool showConfirmPopup;

	private bool isAfterDoubleClick;

	private int gameOverDoubleCoinViewRate;

	private float width;

	private TrialInfo trialInfo;

	private GameOverScreen gameOverScreen;
}
