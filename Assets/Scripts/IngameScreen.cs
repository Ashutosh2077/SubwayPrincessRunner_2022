using System;
using System.Collections;
using UnityEngine;

public class IngameScreen : UIBaseScreen
{
	public IngameScreen()
	{
		this.score = -1;
	}

	private IEnumerator AnimateColor(UISprite sprite, Color32 startValue, Color32 endValue, float speedFactor)
	{
		yield return new WaitForSeconds(0.2f);
		float Factor = 0f;
		while (Factor < 1f)
		{
			Factor += RealTimeTracker.deltaTime * speedFactor;
			sprite.color = Color.Lerp(startValue, endValue, Factor);
			yield return null;
		}
		yield break;
	}

	private IEnumerator AnimateSize(UISprite sprite, int startHeightValue, int endHeightValue, int startWidthValue, int endWidthValue, float speedFactor)
	{
		float Factor = 0f;
		while (Factor < 1f)
		{
			Factor += RealTimeTracker.deltaTime * speedFactor;
			sprite.width = Mathf.CeilToInt(Mathf.Lerp((float)startWidthValue, (float)endWidthValue, Factor));
			sprite.height = Mathf.CeilToInt(Mathf.Lerp((float)startHeightValue, (float)endHeightValue, Factor));
			yield return null;
		}
		yield break;
	}

	private void FadeFromWhiteToDarkBlue()
	{
		this.multiplierLabel.color = Color.Lerp(this.SCOREBOOSTER_WHITE, this.SCOREBOOSTER_ELECTRICBLUE, this.lerpFactor);
		if (this.multiplierLabel.text.Contains(string.Empty + PlayerInfo.Instance.scoreMultiplier) && this.doLastIteration)
		{
			this.multiplierLabel.color = Color.Lerp(this.SCOREBOOSTER_WHITE, this.SCOREBOOSTER_ACTIVE_COLOR, this.lerpFactor);
		}
	}

	public override void Hide()
	{
		base.Hide();
		this.resetMultiplierLabel();
		this._countingDown = false;
		this.countdownStartingLabel.text = string.Empty;
		this.countdownLabel.text = string.Empty;
		if (UIScreenController.Instance.curDeviceType != UIScreenController.DeviceType.iPhoneX)
		{
			RiseSdk.Instance.CloseBanner();
		}
	}

	public override void Init()
	{
		base.Init();
		this.resetMultiplierLabel();
		this.scoreLabel.text = GameStats.Instance.score.ToString();
		this.countdownStartingLabel.text = string.Empty;
		this.countdownLabel.text = string.Empty;
		if (Application.internetReachability != NetworkReachability.NotReachable && UIScreenController.Instance.curDeviceType != UIScreenController.DeviceType.iPhoneX && !PlayerInfo.Instance.hasSubscribed)
		{
			this.topLeft.pixelOffset = new Vector2(0f, -UIScreenController.Instance.bannerHeight);
			this.topRight.pixelOffset = new Vector2(0f, -UIScreenController.Instance.bannerHeight);
		}
		else if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			this.topLeft.pixelOffset = new Vector2(0f, -132f);
			this.topRight.pixelOffset = new Vector2(0f, -132f);
		}
		else
		{
			this.topLeft.pixelOffset = Vector3.zero;
			this.topRight.pixelOffset = Vector3.zero;
		}
		this.helmetBtnHelp.Init();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onScoreMultiplierChanged = (Action)Delegate.Combine(instance.onScoreMultiplierChanged, new Action(this.resetMultiplierLabel));
		GameStats instance2 = GameStats.Instance;
		instance2.OnCoinsChanged = (Action)Delegate.Combine(instance2.OnCoinsChanged, new Action(this.OnCoinsChanged));
		instance2.OnCoinsWithHelmetChanged = (Action)Delegate.Combine(instance2.OnCoinsWithHelmetChanged, new Action(this.OnCoinsWithHelmetChanged));
		Game.Instance.OnGameStarted = (Action)Delegate.Combine(Game.Instance.OnGameStarted, new Action(this.OnGameStarted));
	}

	private void OnCoinsChanged()
	{
		this.coinLabel.text = GameStats.Instance.coins.ToString();
		this.ResizeCoinBox();
	}

	private void OnCoinsWithHelmetChanged()
	{
		if (!this.trialCharge && TrialManager.Instance.IsTestHelm)
		{
			this.progress = GameStats.Instance.coinsWithHelmet + PlayerInfo.Instance.CurrentTrialInfoProgress();
			this.value = (float)this.progress / (float)this.info.taskAim;
			if (this.value >= 1f)
			{
				this.slider.value = 1f;
				this.sliderLbl.text = this.info.taskAim + "/" + this.info.taskAim;
				this.slider.gameObject.SetActive(false);
				UISliderInController.Instance.OnTrialFinished();
				this.trialCharge = true;
			}
			else
			{
				this.slider.value = this.value;
				this.sliderLbl.text = this.progress + "/" + this.info.taskAim;
			}
		}
	}

	private void OnEnable()
	{
		this.mTimeStarted = true;
		this.mTimeDelta = 0f;
		this.mTimeStart = Time.realtimeSinceStartup;
	}

	private void OnGameStarted()
	{
		this.panel.alpha = 1f;
		this.start = true;
		this.lastKnowDigits = 0;
		this.digits = 0;
		Game instance = Game.Instance;
		if (!instance.isReadyForSlideinPowerups)
		{
			this.slideinPowerupHelper.HidePowerups(true);
		}
		this.trialCharge = true;
		if (TrialManager.Instance.IsInTest())
		{
			this.info = TrialManager.Instance.currentTrialInfo;
			if (this.info != null)
			{
				int num = PlayerInfo.Instance.CurrentTrialInfoProgress();
				if (this.info.type == TrialType.Character)
				{
					this.trialCharge = (num >= this.info.taskAim * 1000);
					this.slider.value = (float)num * 0.001f / (float)this.info.taskAim;
					this.sliderLbl.text = num / 1000 + "/" + this.info.taskAim;
				}
				else if (this.info.type == TrialType.Helmet)
				{
					this.trialCharge = (num >= this.info.taskAim);
					this.slider.value = (float)num / (float)this.info.taskAim;
					this.sliderLbl.text = num + "/" + this.info.taskAim;
				}
				this.slider.gameObject.SetActive(true);
				this.sliderIcon.spriteName = this.info.icon;
			}
		}
		else
		{
			this.slider.gameObject.SetActive(false);
			this.info = null;
		}
		if ((TrialManager.Instance.IsTestHelm || Game.Instance.IsTestHelm) && !instance.Attachment.IsActive(instance.Attachment.Helmet))
		{
			instance.Attachment.Add(instance.Attachment.Helmet);
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, 1);
		}
	}

	private void resetMultiplierLabel()
	{
		if (GameStats.Instance.scoreBooster5Activated && !PlayerInfo.Instance.doubleScore)
		{
			int num = int.Parse(this.multiplierLabel.text.Substring(this.multiplierLabel.text.IndexOf("x") + 1));
			if (num < PlayerInfo.Instance.scoreMultiplier)
			{
				int num2 = num + 1;
				this._isMultiplierLabelUpdated = false;
				this.doLastIteration = false;
				this.multiplierLabel.text = "x" + num2;
			}
			else
			{
				this.multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
				if (this.doLastIteration)
				{
					this._isMultiplierLabelUpdated = true;
				}
				this.doLastIteration = true;
			}
		}
		else
		{
			this.multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
		}
	}

	public void ResetMultiplierLabelColour()
	{
		if (GameStats.Instance.scoreBooster5Activated)
		{
			this.multiplierLabel.color = this.SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (this.multiplierLabel.color != this.MULTIPLIER_LABEL_ORIGINAL_COLOR)
		{
			this.multiplierLabel.color = this.MULTIPLIER_LABEL_ORIGINAL_COLOR;
		}
	}

	private void ResizeCoinBox()
	{
		int length = this.coinLabel.text.Length;
		float num = 64f;
		if (length > 1)
		{
			num += (float)(12 * (length - 1));
		}
	}

	private void ResizeMultiplierBox()
	{
		int length = this.multiplierLabel.text.Length;
		float num = 50f;
		if (length > 2)
		{
			num += (float)(10 * (length - 2));
		}
		if (this._multiplier.transform.localScale.x != num)
		{
			this._multiplier.transform.localScale = new Vector3(num, this._multiplier.transform.localScale.y, this._multiplier.transform.localScale.z);
		}
	}

	public void SetPauseButtonVisibility(bool isActive)
	{
		this.pauseButton.SetActive(isActive);
	}

	private void SetScoreLabel()
	{
		this.score = GameStats.Instance.score;
		this.digits = Utility.NumberOfDigits(this.score);
		string str;
		switch (this.digits)
		{
		case 1:
			str = "00000";
			break;
		case 2:
			str = "0000";
			break;
		case 3:
			str = "000";
			break;
		case 4:
			str = "00";
			break;
		case 5:
			str = "0";
			break;
		default:
			str = string.Empty;
			break;
		}
		this.scoreLabel.text = str + this.score.ToString();
		if (this.lastKnowDigits != this.digits)
		{
			this.lastKnowDigits = this.digits;
		}
	}

	public void CountDown()
	{
		this._countdownSeconds = 3f;
		this._countingDown = true;
		this.mTimeStarted = false;
		Game.Instance.TriggerPause(true);
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance == null)
		{
			UnityEngine.Debug.LogError("You must be running the wrong scene");
		}
		else
		{
			if (Game.Instance.isPaused && UIScreenController.Instance.GameIsFocused)
			{
				this._countdownSeconds = 3f;
				this._countingDown = true;
			}
			if (!this._countingDown)
			{
				TasksManager.Instance.inRun = true;
				if (this.multiplierLabel.color != this.MULTIPLIER_LABEL_ORIGINAL_COLOR)
				{
					this.multiplierLabel.color = this.MULTIPLIER_LABEL_ORIGINAL_COLOR;
				}
				this.panel.alpha = 0f;
				this.start = false;
			}
			this.SetPauseButtonVisibility(true);
			if (UIScreenController.Instance.curDeviceType != UIScreenController.DeviceType.iPhoneX && !PlayerInfo.Instance.hasSubscribed)
			{
				RiseSdk.Instance.ShowBanner(3);
			}
		}
	}

	private void Update()
	{
		this.UpdateMultiplierLable();
		if (!this.start)
		{
			return;
		}
		GameStats.Instance.CalculateScore();
		if (this.score != GameStats.Instance.score)
		{
			this.SetScoreLabel();
		}
		if (Game.Instance.isReadyForSlideinPowerups && !Game.Instance.trackController.IsRunningOnTutorialTrack)
		{
			Game.Instance.isReadyForSlideinPowerups = false;
			this.helmetBtnHelp.Show();
			this.slideinPowerupHelper.ShowPowerups();
		}
		if (!this.trialCharge && TrialManager.Instance.IsTestChar)
		{
			this.progress = (int)(Game.Instance.GetDuration() * 1000f) + PlayerInfo.Instance.CurrentTrialInfoProgress();
			this.value = (float)this.progress * 0.001f / (float)this.info.taskAim;
			if (this.value >= 1f)
			{
				this.slider.value = 1f;
				this.sliderLbl.text = this.info.taskAim + "/" + this.info.taskAim;
				this.slider.gameObject.SetActive(false);
				UISliderInController.Instance.OnTrialFinished();
				this.trialCharge = true;
			}
			else
			{
				this.slider.value = this.value;
				this.sliderLbl.text = this.progress / 1000 + "/" + this.info.taskAim;
			}
		}
	}

	private void UpdateMultiplierLable()
	{
		if (GameStats.Instance.scoreBooster5Activated && !this._isMultiplierLabelUpdated && !PlayerInfo.Instance.doubleScore)
		{
			this.lerpFactor += Time.deltaTime * 2f;
			if (this.lerpFactor > 1f)
			{
				this.resetMultiplierLabel();
				this.lerpFactor = 0f;
			}
			else
			{
				this.FadeFromWhiteToDarkBlue();
			}
		}
		if (this._countingDown && UIScreenController.Instance.GameIsFocused)
		{
			float num = this.UpdateRealTimeDelta() * 1.75f;
			this._countdownSeconds -= num;
			this.countdownStartingLabel.text = Strings.Get(LanguageKey.INGAME_UI_COUNTDOWN_STARTING);
			this.countdownLabel.text = Mathf.CeilToInt(this._countdownSeconds).ToString();
			if (!this.countdownLabel.enabled)
			{
				this.countdownStartingLabel.enabled = true;
				this.countdownLabel.enabled = true;
			}
			if (this._cachedCountdownLabelScale == Vector3.zero)
			{
				this._cachedCountdownLabelScale = this.countdownLabel.cachedTransform.localScale;
			}
			this.countdownLabel.cachedTransform.localScale = this._cachedCountdownLabelScale * ((1f - this._countdownSeconds % 1f) * 0.5f + 1f);
			if (this._countdownSeconds < 0f)
			{
				this._countingDown = false;
				this.countdownStartingLabel.text = string.Empty;
				this.countdownLabel.text = string.Empty;
				this.countdownStartingLabel.enabled = false;
				this.countdownLabel.enabled = false;
				if (Game.Instance != null)
				{
					Game.Instance.TriggerPause(false);
					Character.Instance.sameLaneTimeStamp = Time.time;
				}
			}
		}
	}

	private float UpdateRealTimeDelta()
	{
		if (this.mTimeStarted)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float b = realtimeSinceStartup - this.mTimeStart;
			this.mActual += Mathf.Max(0f, b);
			this.mTimeDelta = 0.001f * Mathf.Round(this.mActual * 1000f);
			this.mActual -= this.mTimeDelta;
			this.mTimeStart = realtimeSinceStartup;
		}
		else
		{
			this.mTimeStarted = true;
			this.mTimeStart = Time.realtimeSinceStartup;
			this.mTimeDelta = 0f;
		}
		return this.mTimeDelta;
	}

	private int extraNumberOfChars
	{
		get
		{
			return Mathf.Clamp(this.digits - 6, 0, 28);
		}
	}

	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UILabel sliderLbl;

	[SerializeField]
	private UISprite sliderIcon;

	[SerializeField]
	private UILabel scoreLabel;

	public UILabel multiplierLabel;

	[SerializeField]
	private UILabel coinLabel;

	[SerializeField]
	private Transform _multiplier;

	[SerializeField]
	private SlideinPowerupHelper slideinPowerupHelper;

	[SerializeField]
	private UILabel countdownStartingLabel;

	[SerializeField]
	private UILabel countdownLabel;

	public Color SCOREBOOSTER_ACTIVE_COLOR = new Color(0.7490196f, 0.9137255f, 0.9921569f);

	public Color MULTIPLIER_LABEL_ORIGINAL_COLOR = new Color(1f, 0.8588235f, 0f);

	[SerializeField]
	private GameObject pauseButton;

	[SerializeField]
	private UIAnchor topLeft;

	[SerializeField]
	private UIAnchor topRight;

	[SerializeField]
	private HelmetButtonHelp helmetBtnHelp;

	private TrialInfo info;

	private Vector3 _cachedCountdownLabelScale = Vector3.zero;

	private float _countdownSeconds;

	private bool _countingDown;

	private bool _isMultiplierLabelUpdated;

	private bool start;

	private int digits;

	private bool doLastIteration;

	private int lastKnowDigits;

	private float lerpFactor;

	private float mActual;

	private float mTimeDelta;

	private float mTimeStart;

	private bool mTimeStarted;

	private int score;

	private Color SCOREBOOSTER_ELECTRICBLUE = new Color(0f, 0.6117647f, 1f);

	private Color SCOREBOOSTER_WHITE = new Color(1f, 1f, 1f);

	private int progress;

	private float value;

	private bool trialCharge = true;
}
