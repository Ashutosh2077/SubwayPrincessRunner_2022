using System;
using System.Collections;
using UnityEngine;

public class GameOverScreen : UIBaseScreen
{
	private IEnumerator CountUpCoins()
	{
		this.ClickCatcherBox.enabled = true;
		float countFactor = 0f;
		float countTime = Mathf.Lerp(0.3f, 3f, (float)this.collectedCoinsTo / 200f);
		this.countingUpCoins = true;
		this.UpdateDoubleCoinLabels();
		this.collectedCoinLabel.text = this.collectedCoinsFrom.ToString();
		yield return new WaitForSeconds(1.2f);
		countFactor = 0f;
		this.scoreCounterSoundPlayer.PlayCoinSound(countFactor);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			this.scoreLabel.text = Mathf.Round(Mathf.SmoothStep((float)this.scoreFrom, (float)this.scoreTo, countFactor)).ToString();
			this.collectedCoinLabel.text = Mathf.Round(Mathf.SmoothStep((float)this.collectedCoinsFrom, (float)this.collectedCoinsTo, countFactor)).ToString();
			yield return null;
		}
		this.scoreCounterSoundPlayer.StopScoreSound();
		if (PlayerInfo.Instance.hasSubscribed)
		{
			this.vipTipAni.gameObject.SetActive(true);
			this.scoreCounterSoundPlayer.PlayVipSound();
			this.coinEffectAnr.gameObject.SetActive(true);
			this.coinEffectAnr.Play("ui_jiesuan", 0, 0f);
			float time = 0f;
			float alltime = this.vipTipAni.GetClip("VIP_TIP").length;
			this.vipTipAni.Play();
			while (time < alltime)
			{
				time += Time.deltaTime;
				yield return null;
			}
			this.scoreCounterSoundPlayer.StopVipSound();
			this.collectedCoinsFrom = this.collectedCoinsTo;
			this.collectedCoinsTo *= 2;
			countFactor = 0f;
			while (countFactor < 1f)
			{
				this.scoreCounterSoundPlayer.PlayCoinSound(countFactor);
				countFactor += Time.deltaTime / countTime;
				this.collectedCoinLabel.text = Mathf.Round(Mathf.SmoothStep((float)this.collectedCoinsFrom, (float)this.collectedCoinsTo, countFactor)).ToString();
				yield return null;
			}
		}
		this.scoreCounterSoundPlayer.StopScoreSound();
		if (this.selectUI.FootAutoShow())
		{
			countFactor = 0f;
			countTime = 0.6f;
			while (countFactor < 1f)
			{
				countFactor += Time.deltaTime / countTime;
				this.foot.localPosition = new Vector3(0f, Mathf.SmoothStep(-220f, 0f, countFactor), 0f);
				yield return null;
			}
		}
		this.scoreLabel.text = this.scoreTo.ToString();
		this.collectedCoinLabel.text = this.collectedCoinsTo.ToString();
		this.countingUpCoins = false;
		this.CountUpCompleted();
		this.selectUI.RefreshUI(this.showTryRole, this.collectedCoinsTo);
		yield return new WaitForSeconds(0.1f);
		this.ShowPassLevelAD();
		if (this.selectUI.FootAutoShow())
		{
			yield return new WaitForSeconds(0.5f);
			this.ClickCatcherBox.enabled = false;
		}
		yield break;
	}

	public void AnimateFoot(Action finished)
	{
		base.StartCoroutine(this.AnimateFoot_C(finished));
	}

	private IEnumerator AnimateFoot_C(Action finished)
	{
		float countFactor = 0f;
		float countTime = 0.6f;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			this.foot.localPosition = new Vector3(0f, Mathf.SmoothStep(-220f, 0f, countFactor), 0f);
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		if (finished != null)
		{
			finished();
		}
		this.ClickCatcherBox.enabled = false;
		yield break;
	}

	private void ShowPassLevelAD()
	{
		if (PlayerInfo.Instance.numberOfRuns == 2)
		{
			UIScreenController.Instance.PushPopup("RatingPopup");
		}
	}

	private void CountUpCompleted()
	{
	}

	public override void Init()
	{
		base.Init();
		this.scoreCounterSoundPlayer = base.GetComponent<ScoreCounterSoundPlayer>();
		this.vipTipAni.gameObject.SetActive(false);
		this.newUI.Init(this);
		this.selectUI = this.newUI;
	}

	public bool IsCountingUpCoins()
	{
		return this.countingUpCoins;
	}

	private void PushNewHighScoreCelebrationScreen(int newHighSchore)
	{
		RewardManager.AddRewardToUnlock(new CelebrationReward
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.NewHighScore,
			rewardType = CelebrationRewardType.highscore,
			characterType = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter,
			characterThemeIndex = PlayerInfo.Instance.currentThemeIndex
		}, true);
	}

	public void SetupAfterChest()
	{
		if (this.hasBeenSetupAfterAGame)
		{
			if (PlayerInfo.Instance.numberOfRuns != 2 && this.hasNative)
			{
				float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
				if (Mathf.Abs(num - 0.5625f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
				}
				else if (Mathf.Abs(num - 0.6667f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
				}
				else if (Mathf.Abs(num - 0.75f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
				}
				else
				{
					RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
				}
			}
			TrialManager.Instance.End();
			Game.Instance.ResetTest(false);
			int coins = GameStats.Instance.coins;
			this.collectedCoinsFrom = 0;
			this.collectedCoinsTo = coins;
			this.scoreTo = this.scoreFrom + GameStats.CoinToScoreConversion(coins);
			TasksManager.Instance.PlayerDidThis(TaskTarget.GetExactlyAmountOfCoins, coins, -1);
			base.StartCoroutine("CountUpCoins");
			this.hasBeenSetupAfterAGame = false;
		}
	}

	public void SetupBeforeChest()
	{
		this.hasBeenSetupAfterAGame = true;
		this.scoreLabel.text = string.Empty + GameStats.Instance.score;
		this.scoreFrom = GameStats.Instance.score;
	}

	private void CheckShowNative()
	{
		if (!UIScreenController.Instance.CheckNetwork() || !RiseSdk.Instance.HasNativeAd("loading") || PlayerInfo.Instance.numberOfRuns == 2 || PlayerInfo.Instance.hasSubscribed)
		{
			this.hasNative = false;
			this.showScoreBg.localPosition = new Vector3(0f, 655f, 0f);
		}
		else
		{
			this.hasNative = true;
			this.showScoreBg.localPosition = new Vector3(0f, 395f, 0f);
		}
	}

	private void OnEnable()
	{
		coinEffectAnr.gameObject.SetActive(value: false);
		coinEffectAnr.enabled = false;//();
		RiseSdkListener.OnAdEvent -= RewardAdSuc;
		RiseSdkListener.OnAdEvent += RewardAdSuc;
		CheckShowNative();
	}

	private void OnDisable()
	{
		this.vipTipAni.gameObject.SetActive(false);
		RiseSdkListener.OnAdEvent -= this.RewardAdSuc;
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void GoToMainMenu()
	{
		UIScreenController.Instance.PushScreen("FrontUI");
	}

	public void RewardAdSuc(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished)
		{
			if (id == 4)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_double", 0, null);
				this.DoubleCoinsSet();
				this.selectUI.AfterDoubleCoins(GameStats.Instance.coins);
				PlayerInfo.Instance.gameOverDoubleCoinsShowCountLastDay++;
			}
			if (id == 10)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_endless", 0, null);
				this.selectUI.AfterTryCharacter();
				SaveMeManager.ResetSaveMeForNewRun();
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
			}
		}
	}

	public void DoubleCoinsSet()
	{
		int coins = GameStats.Instance.coins;
		int num = PlayerInfo.Instance.CheckGameOverDoubleCoinViewRate(coins);
		this.collectedCoinLabel.color = this.yellow;
		if (PlayerInfo.Instance.hasSubscribed)
		{
			this.collectedCoinLabel.text = string.Empty + coins * num * 2;
		}
		else
		{
			this.collectedCoinLabel.text = string.Empty + coins * num;
		}
	}

	public override void GainFocus()
	{
		if (this.hasNative)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	public override void LooseFocus()
	{
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.gameOverFullAdCount++;
		this.selectUI.Show();
		this.scoreTitleLabel.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_TITLE);
		this.vipSpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_ICON_SUBSCRIBE_SPRITE);
		this.hightScoreTip.enabled = false;
		UISliderInController.Instance.Stop = true;
		this.foot.localPosition = Vector3.down * 220f;
		int num = GameStats.Instance.score + GameStats.CoinToScoreConversion(GameStats.Instance.coins);
		bool flag = num > PlayerInfo.Instance.highestScore;
		PlayerInfo.Instance.highestScore = num;
		if (flag)
		{
			this.hightScoreTip.enabled = true;
			this.PushNewHighScoreCelebrationScreen(num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.BeatOwnHighscore, 1, -1);
		}
		if (!TrialManager.Instance.IsInTest() && !Game.Instance.IsInTest() && Game.Instance.GetDuration() > 120f && PlayerInfo.Instance.CheckGameoverUITry())
		{
			this.showTryRole = true;
			PlayerInfo.Instance.ShowGameoverUITry();
		}
		else
		{
			this.showTryRole = false;
		}
		if (TrialManager.Instance.IsTestHelm)
		{
			PlayerInfo.Instance.IncreaseTrialProgress(GameStats.Instance.coinsWithHelmet);
		}
		else if (TrialManager.Instance.IsTestChar)
		{
			PlayerInfo.Instance.IncreaseTrialProgress((int)(Game.Instance.GetDuration() * 1000f));
		}
		RiseSdk.Instance.enableBackHomeAd(false, "custom", 20000);
	}

	public override void Hide()
	{
		base.Hide();
		UISliderInController.Instance.Stop = false;
		this.selectUI.Hide();
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom", 20000);
		}
	}

	public void StopCoinsCountUp()
	{
		if (this.countingUpCoins)
		{
			base.StopCoroutine("CountUpCoins");
			this.scoreCounterSoundPlayer.StopScoreSound();
			this.scoreLabel.text = string.Empty + this.scoreTo;
			this.collectedCoinLabel.text = string.Empty + this.collectedCoinsTo;
			this.countingUpCoins = false;
			this.CountUpCompleted();
		}
	}

	private void UpdateDoubleCoinLabels()
	{
		this.collectedCoinLabel.color = Color.white;
	}

	private IEnumerator SplashLabelEffect(UILabel label, int expander)
	{
		label.gameObject.SetActive(true);
		float countFactor = 0f;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime;
			Transform cachedTransform = label.cachedTransform;
			cachedTransform.localScale += new Vector3((float)expander * Time.deltaTime * 34f, (float)(34 * expander) * Time.deltaTime, 0f);
			label.alpha = 0.1f - countFactor * 0.1f;
			yield return null;
		}
		yield break;
	}

	[SerializeField]
	private UILabel scoreTitleLabel;

	[SerializeField]
	private UISprite vipSpr;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel collectedCoinLabel;

	[SerializeField]
	private Transform foot;

	[SerializeField]
	private float footY;

	[SerializeField]
	private Transform showScoreBg;

	[SerializeField]
	private GameOverNewUI newUI;

	private IGameOverUI selectUI;

	private bool hasNative;

	public BoxCollider ClickCatcherBox;

	private int collectedCoinsFrom;

	private int collectedCoinsTo;

	private bool countingUpCoins;

	private bool hasBeenSetupAfterAGame;

	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	private int scoreFrom;

	private int scoreTo;

	private Color yellow = new Color(1f, 0.8745098f, 0.04705882f);

	[SerializeField]
	private Animation vipTipAni;

	[SerializeField]
	private Animator coinEffectAnr;

	[SerializeField]
	private UISprite hightScoreTip;

	private bool showTryRole;
}
