using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class Game : MonoBehaviour
{
	public event Game.OnSpeedChangedDelegate OnSpeedChanged;

	public event Game.OnTurboHeadstartInputDelegate OnTurboHeadstartInput;

	private SwipeDir AnalyzeSwipe(Swipe swipe)
	{
		Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(swipe.start.x, swipe.start.y, 2f));
		if (Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(swipe.end.x, swipe.end.y, 2f)), b) < this.swipe.distanceMin)
		{
			return SwipeDir.None;
		}
		Vector3 lhs = swipe.end - swipe.start;
		SwipeDir result = SwipeDir.None;
		float num = 0f;
		float num2 = Vector3.Dot(lhs, Vector3.up);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Up;
		}
		num2 = Vector3.Dot(lhs, Vector3.down);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Down;
		}
		num2 = Vector3.Dot(lhs, Vector3.left);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Left;
		}
		num2 = Vector3.Dot(lhs, Vector3.right);
		if (num2 > num)
		{
			result = SwipeDir.Right;
		}
		return result;
	}

	public void Awake()
	{
		this.character = Character.Instance;
		this.character.Initialize();
		this.characterRendering = CharacterRendering.Instance;
		this.characterAnimation = this.characterRendering.characterAnimation;
		this.trackController = TrackController.Instance;
		this.characterCamera = CharacterCamera.Instance;
		this.running = Running.Instance;
		this.flypack = Flypack.Instance;
		this.raft = Raft.Instance;
		this.die = Die.Instance;
		this.wallWalking = WallWalking.Instance;
		this.springJump = SpringJump.Instance;
		this.boundJump = BoundJump.Instance;
		this.speedup = SpeedUp.Instance;
		this.traversingCity = TraversingCity.Instance;
		this.boss = Boss.Instance;
		this.boss.Initialize();
		this.attachment = new CharacterAttachmentCollection();
		this.character.OnStumble += this.OnStumble;
		this.character.OnCriticalHit += this.OnCriticalHit;
		this.currentLevelSpeed = this.Speed(0f, this.speed);
		this.stats = GameStats.Instance;
		this.show20sAd = true;
		this.showAdTime = 0f;
		this.startGameTime = Time.time;
		ImageManager.Instance.Load();
		if (PlayerInfo.Instance.hasFacebookLogin)
		{
			FacebookManger.Instance.LoginFacebook();
		}
		this.awakeDone = true;
	}

	private void OnEnable()
	{
		RiseSdkListener.OnPaymentEvent -= this.PayResult;
		RiseSdkListener.OnPaymentEvent += this.PayResult;
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
		RiseSdkListener.OnAdEvent += this.OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnPaymentEvent -= this.PayResult;
		RiseSdkListener.OnAdEvent -= this.OnFreeReward;
		if (TrialManager.Instance != null)
		{
			TrialManager.Instance.End();
		}
		this.ResetTest(true);
		if (PlayerInfo.Instance != null)
		{
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void ResetTest(bool quit = false)
	{
		if (this.IsTestChar)
		{
			this.IsTestChar = false;
			if (!quit)
			{
				CharacterScreenManager.Instance.SelectCharacter((Characters.CharacterType)this.preCharacter, this.preCharacterSkin);
			}
			else
			{
				PlayerInfo.Instance.currentCharacter = this.preCharacter;
				PlayerInfo.Instance.currentThemeIndex = this.preCharacterSkin;
			}
		}
		if (this.IsTestHelm)
		{
			this.IsTestHelm = false;
			PlayerInfo.Instance.currentHelmet = (Helmets.HelmType)this.preHelmet;
		}
	}

	public void TestCharacter(Characters.CharacterType characterType, int themeId)
	{
		this.IsTestChar = true;
		this.preCharacter = PlayerInfo.Instance.currentCharacter;
		this.preCharacterSkin = PlayerInfo.Instance.currentThemeIndex;
		CharacterScreenManager.Instance.SelectCharacter(characterType, themeId);
	}

	public void TestHelmet(Helmets.HelmType helmType)
	{
		this.IsTestHelm = true;
		this.preHelmet = (int)PlayerInfo.Instance.currentHelmet;
		PlayerInfo.Instance.currentHelmet = helmType;
	}

	public bool IsInTest()
	{
		return this.IsTestChar || this.IsTestHelm;
	}

	public void Start()
	{
		base.StartCoroutine(this.GameIntro());
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom", 20000);
		}
	}

	public void ShowRewarAd(int rewardId, Action<bool> rewardAdBack)
	{
		this.rewardAdCallback = rewardAdBack;
		RiseSdk.Instance.ShowRewardAd(rewardId);
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		DebugShow.Log(string.Concat(new object[]
		{
			"TestSuoyin; ",
			type,
			",Id:",
			id,
			",AdType: ",
			eventType
		}));
		if (eventType != 1 && eventType != 2)
		{
			return;
		}
		if (type == RiseSdk.AdEventType.FullAdClosed)
		{
			RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0, null);
		}
		if (type == RiseSdk.AdEventType.RewardAdShowFinished)
		{
			if (this.rewardAdCallback != null)
			{
				this.rewardAdCallback(true);
				this.rewardAdCallback = null;
			}
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_all_success", 0, null);
			PlayerInfo.Instance.WatchVideoSuccessNum++;
			if (id == 0)
			{
				int num = (UnityEngine.Random.value >= 0.5f) ? 2 : 1;
				if (num == 1)
				{
					FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewcoins, delegate()
					{
						UIScreenController.Instance.ClosePopup(null);
					}, 0);
				}
				else
				{
					FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewkeys, delegate()
					{
						UIScreenController.Instance.ClosePopup(null);
					}, 0);
				}
			}
			else if (id == 1)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewkeys, delegate()
				{
					WatchFreeViewSystem.Instance.SetFreeTime("FreeViewKeys");
				}, 0);
			}
			else if (id == 2)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.viewcoins, delegate()
				{
					WatchFreeViewSystem.Instance.SetFreeTime("FreeViewCoins");
				}, 0);
			}
			else if (id == 3)
			{
				UIScreenController.Instance.ClosePopup(null);
				SaveMeButton.ReviveGameSuc();
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_saveme", 0, null);
			}
			if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
			{
				this.Pause(false);
			}
			else
			{
				AudioListener.volume = (float)((!Settings.optionSound) ? 0 : 1);
			}
		}
		else if (type == RiseSdk.AdEventType.RewardAdShowFailed)
		{
			if (this.rewardAdCallback != null)
			{
				this.rewardAdCallback(false);
				this.rewardAdCallback = null;
			}
		}
		else if (type == RiseSdk.AdEventType.AdClosed || type == RiseSdk.AdEventType.FullAdClosed)
		{
			if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
			{
				this.Pause(false);
			}
			else
			{
				AudioListener.volume = (float)((!Settings.optionSound) ? 0 : 1);
			}
		}
		else if (type == RiseSdk.AdEventType.AdShown)
		{
			this.Pause(true);
		}
	}

	public void PayResult(RiseSdk.PaymentResult result, int billId)
	{
		if (result == RiseSdk.PaymentResult.Success)
		{
			switch (billId)
			{
			case 0:
				PlayerInfo.Instance.amountOfCoins += 7500;
				break;
			case 1:
				PlayerInfo.Instance.amountOfCoins += 18000;
				break;
			case 2:
				PlayerInfo.Instance.amountOfCoins += 30000;
				break;
			case 3:
				PlayerInfo.Instance.amountOfCoins += 45000;
				break;
			case 4:
				PlayerInfo.Instance.amountOfCoins += 100000;
				break;
			case 5:
				PlayerInfo.Instance.amountOfKeys += ((!PlayerInfo.Instance.hasSubscribed) ? 10 : 15);
				if (UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
				{
					SaveMeManager.SendReviveIfPurchaseSucceeded();
					UIScreenController.Instance.ClosePopup(null);
				}
				break;
			case 6:
				PlayerInfo.Instance.amountOfKeys += ((!PlayerInfo.Instance.hasSubscribed) ? 25 : 38);
				break;
			case 7:
				PlayerInfo.Instance.amountOfKeys += ((!PlayerInfo.Instance.hasSubscribed) ? 80 : 120);
				break;
			case 8:
				PlayerInfo.Instance.amountOfKeys += ((!PlayerInfo.Instance.hasSubscribed) ? 300 : 450);
				break;
			case 13:
			{
				PlayerInfo.Instance.hasSubscribed = true;
				ServerManager.Instance.UploadSubscription("yes");
				UIModelController.Instance.SelectCharacterForPlay(this.subscriptionCharacterType, 0);
				UISliderInController.SlideIn slideIn = new UISliderInController.SlideIn(UISliderInController.SlideInType.Unlock, Strings.Get(LanguageKey.UI_TOP_TIP_SUBSCRIBE_TO_SUCCESS));
				UISliderInController.Instance.QueueSlideIn(slideIn);
				UIScreenController.Instance.ClosePopup(null);
				break;
			}
			}
		}
		else if (billId == 5 && UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public void Pause(bool active)
	{
		if (active)
		{
			this._paused = true;
			Time.timeScale = 0f;
			AudioListener.volume = 0f;
		}
		else
		{
			this._paused = false;
			Time.timeScale = 1f;
			AudioListener.volume = (float)((!Settings.optionSound) ? 0 : 1);
		}
	}

	public void ChangeCurrentSpeed(float timeOffset)
	{
		this.internalStartTime -= timeOffset;
	}

	public void ChangeState(CharacterState state)
	{
		this.characterState = state;
		if (state != null)
		{
			this.currentThread = state.Begin();
		}
	}

	public void GameStart()
	{
		if (this.OnGameStarted != null)
		{
			this.OnGameStarted();
		}
	}

	public float GetStartGameDuration()
	{
		return Time.time - this.startGameTime;
	}

	public float GetDuration()
	{
		return Time.time - this.startTime;
	}

	public float GetNextAdDuration()
	{
		return Time.time - this.showAdTime;
	}

	public void HandleControls()
	{
		if (this.doubleTapDelay <= 0f)
		{
			this.touchCount = 0;
			this.doubleTapDelay = 0f;
		}
		else
		{
			this.doubleTapDelay -= Time.deltaTime;
		}

		List<Touch> touches = InputHelper.GetTouches();
		if (!this._paused && touches.Count > 0)
		{
			Touch touch = touches[0];
			if (touch.phase == TouchPhase.Began)
			{
				this.currentSwipe = new Swipe();
				this.currentSwipe.start = touch.position;
				this.currentSwipe.startTime = Time.time;
				if (this.touchCount == 0)
				{
					this.doubleTapDelay = 0.25f;
				}
				this.touchCount++;
				if (this.touchCount == 2)
				{
					this.characterState.HandleDoubleTap();
					this.touchCount = 0;
				}
			}
			if (touch.phase == TouchPhase.Moved)
			{
				this.delta += touch.deltaPosition;
				if (Vector2.SqrMagnitude(this.delta) > 0.1f)
				{
					this.touchCount = 0;
				}
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && this.currentSwipe != null)
			{
				if (touch.phase == TouchPhase.Ended)
				{
					this.delta = Vector2.zero;
				}
				this.currentSwipe.endTime = Time.time;
				this.currentSwipe.end = touch.position;
				SwipeDir swipeDir = this.AnalyzeSwipe(this.currentSwipe);
				if (swipeDir != SwipeDir.None)
				{
					if (this.characterState != null)
					{
						this.characterState.HandleSwipe(swipeDir);
					}
					this.currentSwipe = null;
				}
			}
			if (touch.phase == TouchPhase.Ended && this.currentSwipe != null)
			{
				this.currentSwipe.endTime = Time.time;
				this.currentSwipe.end = touch.position;
				if (this.AnalyzeSwipe(this.currentSwipe) == SwipeDir.None && this.characterState != null)
				{
					this.HandleTap();
				}
			}
		}
	}

	private void HandleDebugControls()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.Chest);
			GameStats.Instance.chestPickups++;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.E))
		{
			PlayerInfo.Instance.NextTrialLevel();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			UnityEngine.Debug.Break();
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.N))
		{
			this.Megaheadstart();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.S))
		{
			GameStats gameStats = GameStats.Instance;
			gameStats.superShoesPickups++;
			this.attachment.Add(this.attachment.SuperShoes);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.M))
		{
			GameStats gameStats2 = GameStats.Instance;
			gameStats2.coinMagnetsPickups++;
			this.attachment.Add(this.attachment.CoinMagnet);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.X))
		{
			this.attachment.Stop();
		}
		if (this.characterState != null)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
			{
				this.characterState.HandleSwipe(SwipeDir.Up);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
			{
				this.characterState.HandleSwipe(SwipeDir.Down);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				this.characterState.HandleSwipe(SwipeDir.Left);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
			{
				this.characterState.HandleSwipe(SwipeDir.Right);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F12))
			{
				this.characterState.HandleDoubleTap();
			}
		}
	}

	private bool HandleTap()
	{
		bool result = false;
		this.wasButtonClicked = false;
		return result;
	}

	public void LayTrackChunks()
	{
		this.trackController.ChangeBackgroundMusic(this.character.z);
		this.trackController.LayTrackPieces(this.character.z);
	}

	private float Speed(float t, Game.SpeedInfo speedInfo)
	{
		if (t < speedInfo.rampUpDuration)
		{
			return t * (speedInfo.max - speedInfo.min) / speedInfo.rampUpDuration + speedInfo.min;
		}
		return speedInfo.max;
	}

	public void ResetEnemy()
	{
		this.boss.enabled = true;
		this.boss.MuteProximityLoop();
		this.boss.ResetCatchUp();
		this.boss.ResetModelRootPosition();
		this.boss.ShowEnemies(false);
	}

	public void NewPlayerRunDuration()
	{
		if (this.GetDuration() <= 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_0_30s", 0, null);
		}
		else if (this.GetDuration() <= 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_30_60s", 0, null);
		}
		else if (this.GetDuration() <= 90f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_60_90s", 0, null);
		}
		else if (this.GetDuration() <= 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_90_120s", 0, null);
		}
		else if (this.GetDuration() <= 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_120_180s", 0, null);
		}
		else if (this.GetDuration() <= 240f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_180_240s", 0, null);
		}
		else if (this.GetDuration() <= 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_240_300s", 0, null);
		}
		else if (this.GetDuration() > 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_300s_", 0, null);
		}
	}

	public void NormalPlayerRunDuration()
	{
		if (this.GetDuration() < 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time_0_30s", 0, null);
		}
		else if (this.GetDuration() < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time30_60s", 0, null);
		}
		else if (this.GetDuration() < 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time1_2min", 0, null);
		}
		else if (this.GetDuration() < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time2_3min", 0, null);
		}
		else if (this.GetDuration() < 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time3_5min", 0, null);
		}
		else if (this.GetDuration() < 360f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time5_6min", 0, null);
		}
		else if (this.GetDuration() < 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time6_7min", 0, null);
		}
		else if (this.GetDuration() >= 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time7_min", 0, null);
		}
	}

	public void UM_OnRunDuration()
	{
	}

	public void TriggerPause(bool pauseGame)
	{
		this._paused = pauseGame;
		if (pauseGame)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		if (this.OnPauseChange != null)
		{
			this.OnPauseChange(this._paused);
		}
	}

	public void Update()
	{
		if (!SaveMeManager.IS_PURCHASE_RUNNING_INGAME && !this.trackController.IsRunningOnTutorialTrack)
		{
			this.t = Time.time - this.internalStartTime;
			this.currentLevelSpeed = this.Speed(this.t, this.speed);
		}
		else
		{
			this.startTime += Time.deltaTime;
			this.internalStartTime += Time.deltaTime;
		}
		if (this.characterState != null && this.currentThread != null)
		{
			this.currentThread.MoveNext();
		}
		if (!this._paused && this.characterState != null)
		{
			this.attachment.Update();
		}
		GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.Home))
		{
			RiseSdk.Instance.OnExit();
		}
	}

	public void UpdateMeters()
	{
		this.stats.meters = (float)Mathf.RoundToInt(this.character.z / this.distancePerMeter);
	}

	private IEnumerator TopMenu()
	{
		this.IsInGame.Value = false;
		this.IsInTopMenu.Value = true;
		AudioPlayer.Instance.PlayMusic("Start_station_bg", 1f);
		this.followZ.enabled = false;
		this.boss.MuteProximityLoop();
		this.trackController.DeactivateTrackPieces();
		this.attachment.Resume();
		this.attachment.StopWithNoEnding();
		this.attachment.Update();
		GameStats.Instance.ClearPowerups();
		this.flypack.coinsManager.ReleaseCoins();
		this.springJump.coinsManager.ReleaseCoins();
		this.StageMenuSequence();
		this.characterCamera.SetCameraToTopMenu();
		if (this.firstStart)
		{
			this.characterCamera.Animation.Play("introPan");
			this.characterCamera.Animation["introPan"].speed = 1f;
			float time = this.characterCamera.Animation["introPan"].length;
			while (time > 0f)
			{
				time -= Time.deltaTime;
				if (Input.GetMouseButtonDown(0))
				{
					this.characterCamera.Animation["introPan"].speed *= 2f;
					time /= 2f;
				}
				yield return null;
			}
			this.firstStart = false;
			if (UIScreenController.isInstanced)
			{
				UIScreenController.Instance.ShowMainMenu();
			}
		}
		if (this.OnMenuScreenShown != null)
		{
			this.OnMenuScreenShown();
		}
		yield return null;
		yield break;
	}

	private IEnumerator Intro()
	{
		this.stats.Reset();
		this.attachment.Stop();
		this.attachment.Update();
		this.boss.MuteProximityLoop();
		this.isDead = false;
		this.character.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = PickupParticles.coinEfxOffset;
		this.boss.ShowEnemies(true);
		this.boss.PlayIntro();
		this.trackController.Restart();
		this.trackController.LayTrackPieces(0f);
		this.currentLevelSpeed = this.Speed(0f, this.speed);
		this.startTime = Time.time;
		this.internalStartTime = Time.time;
		this.character.Restart();
		SpawnUpgradeManager.Instance.Restart();
		this.characterCamera.Animation["startPan"].speed = 0.6f;
		this.characterCamera.Animation.Play("startPan");
		this.topMenu.OnNewGameStart();
		InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit(1f);
		if (this.OnIntroRun != null)
		{
			this.OnIntroRun();
		}
		float time = Time.time + this.characterCamera.Animation["startPan"].length / 0.6f;
		float fov_start = this.characterCamera.Camera.fieldOfView;
		float fov_end = this.characterCamera.CurrentCameraFollowPlayerConfig.NormalConfig.cameraFOV / this.characterCamera.Camera.aspect;
		while (time > Time.time)
		{
			this.characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_end, fov_start, (time - Time.time) / 0.6f);
			yield return null;
		}
		this.characterCamera.Camera.fieldOfView = fov_end;
		this.characterCamera.StartRun(this.character.transform.position, Quaternion.identity);
		this.boss.enabled = true;
		this.followZ.enabled = true;
		this.topMenu.Continue();
		if (this.trackController.IsRunningOnTutorialTrack)
		{
			this.boss.ResetCatchUp();
		}
		if (this.trackController.IsRunningOnTutorialTrack)
		{
			this.isReadyForSlideinPowerups = false;
		}
		else
		{
			this.isReadyForSlideinPowerups = true;
		}
		this.ChangeState(this.Running);
		UIScreenController.isGetBtnPressed = false;
		yield return null;
		yield break;
	}

	private void StageMenuSequence()
	{
		this.boss.ShowEnemies(false);
		this.boss.enemies[0].localPosition = Vector3.zero;
		this.boss.StopAllCoroutines();
		this.boss.enabled = false;
		this.character.StopAllCoroutines();
		CharacterModel characterModel = this.character.characterModel;
		characterModel.meshBlobShadow.enabled = true;
		this.characterAnimation = characterModel.characterAnimation;
		this.topMenu.StartPlayIdleRummagesAnimation();
		this.characterCamera.enabled = false;
		this.characterCamera.Camera.fieldOfView = 43f / this.characterCamera.Camera.aspect;
		if (this.OnStageMenuSequence != null)
		{
			this.OnStageMenuSequence();
		}
		if (this.character.superShoes.IsActive)
		{
			this.character.superShoes.StopUse();
			this.character.IsJumpingHigher = false;
		}
		Helmet.Instance.HardReset();
		this.Jetpack.ResetTurboHeadstart();
		GameStats.Instance.ClearPowerups();
		GameStats.Instance.PAUSEPOWERUPS = false;
	}

	private IEnumerator GameIntro()
	{
		this.firstStart = true;
		this.ChangeState(null);
		base.StartCoroutine(this.TopMenu());
		yield return null;
		yield break;
	}

	public void StartTopMenu()
	{
		this.ChangeState(null);
		base.StartCoroutine(this.TopMenu());
	}

	public void StartGame()
	{
		if (Game.Instance != null)
		{
			this.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
		else
		{
			UIScreenController.Instance.PushScreen("GameoverUI");
		}
	}

	public void StartNewRun(bool duel)
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
		this.IsInTopMenu.Value = false;
		this.IsInGame.Value = true;
		AudioPlayer.Instance.PlaySound("ChristmasGameStart", 0.5f, 1f, 1f);
		this.ChangeState(null);
		base.StartCoroutine(this.Intro());
		this.SendRunCharCount();
	}

	private void SendRunCharCount()
	{
		int num = 0;
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> keyValuePair in Characters.characterData)
		{
			if (PlayerInfo.Instance.IsCollectionComplete(keyValuePair.Key))
			{
				num++;
			}
		}
		if (num >= 2)
		{
			switch (Characters.characterOrder.IndexOf(CharacterScreenManager.Instance.currenCharacterShown))
			{
			case 0:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles01", 0, null);
				break;
			case 1:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles02", 0, null);
				break;
			case 2:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles03", 0, null);
				break;
			case 3:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles04", 0, null);
				break;
			case 4:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles05", 0, null);
				break;
			case 5:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles06", 0, null);
				break;
			case 6:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles07", 0, null);
				break;
			case 7:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles08", 0, null);
				break;
			case 8:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles09", 0, null);
				break;
			case 9:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles10", 0, null);
				break;
			case 10:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles11", 0, null);
				break;
			}
		}
	}

	private IEnumerator BackToCheckPointSequence()
	{
		this.goingBackToCheckpoint = true;
		this.ChangeState(null);
		yield return new WaitForSeconds(this.backToCheckpointDelayTime);
		if (this.IsInGame.Value)
		{
			this.character.SetBackToCheckPoint(this.backToCheckpointZoomTime);
			yield return new WaitForSeconds(this.backToCheckpointZoomTime);
		}
		this.goingBackToCheckpoint = false;
		yield break;
	}

	public void WillDie()
	{
		if (this.attachment.IsActive(this.attachment.Helmet))
		{
			this.boss.Restart(true);
			this.attachment.Helmet.Stop = StopFlag.STOP;
			this.boss.MuteProximityLoop();
			this.boss.ResetCatchUp();
			if (this.character.IsStumbling)
			{
				this.character.StopStumble();
			}
			GameStats.Instance.RemoveHoverHelmPowerup();
			if (this.type == Character.CriticalHitType.FallIntoWater)
			{
				this.character.SetFrontToCheckPoint();
			}
		}
		else if (this.trackController.IsRunningOnTutorialTrack)
		{
			if (!this.goingBackToCheckpoint)
			{
				base.StartCoroutine(this.BackToCheckPointSequence());
			}
		}
		else
		{
			GameStats.Instance.PAUSEPOWERUPS = true;
			MovingO.ActivateAutoPilot();
			this.boss.enabled = false;
			if (this.boss.isShowing)
			{
				if (this.characterAnimation["death_moving"].enabled)
				{
					this.boss.ShowEnemies(false);
				}
				else if (this.characterAnimation["intoWater"].enabled)
				{
					this.boss.FallIntoWater();
				}
				else
				{
					this.boss.CatchPlayer(this.characterAnimation);
				}
			}
			this.isDead = true;
			this.stats.duration = this.GetDuration();
			if (this.OnGameEnded != null)
			{
				this.OnGameEnded();
			}
			base.StopAllCoroutines();
			this.ChangeState(this.die);
		}
	}

	public void Revive()
	{
		this.ResetEnemy();
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.character.ResetWalls();
		GameStats.Instance.PAUSEPOWERUPS = false;
		this.IsInGame.Value = true;
		base.StopAllCoroutines();
		this.characterCamera.Revive();
		if (this.type == Character.CriticalHitType.FallIntoWater)
		{
			this.character.SetFrontToCheckPoint();
		}
		this.ChangeState(this.Running);
		this.isDead = false;
	}

	public IEnumerator SkipRevive()
	{
		yield return new WaitForSeconds(0.1f);
		this.die.SkipRevive = true;
		yield break;
	}

	public void OnCriticalHit(Character.CriticalHitType type)
	{
		if (this.characterState != null)
		{
			this.type = type;
			AudioPlayer.Instance.PlaySound("leyou_Hr_death", true);
			this.characterState.HandleCriticalHit(type != Character.CriticalHitType.FallIntoWater);
		}
	}

	public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.character.IsStumbling && this.characterState != null)
		{
			base.StartCoroutine(this.StumbleDeathSequence());
		}
	}

	private IEnumerator StumbleDeathSequence()
	{
		this.currentSpeed = this.speed.min;
		yield return new WaitForSeconds(0.2f);
		if (this.IsInGame.Value && !this.IsInFlypackMode && !this.IsInSpringJumpMode && !this.IsInBoundJumpMode)
		{
			this.characterAnimation.CrossFade("death_bounce", 0.2f);
			if (this.characterState != null)
			{
				this.characterState.HandleCriticalHit(true);
			}
		}
		yield break;
	}

	public void OnStartWall(SwipeDir dir, Wall wall)
	{
		this.wallWalking.SetData(dir, wall.Height, wall.Bounds.max.z);
		this.ChangeState(this.wallWalking);
	}

	public void Megaheadstart()
	{
		if (!this.flypack.ActivateTurboHeadstart)
		{
			if (this.isDead)
			{
				return;
			}
			this.Jetpack.headStart = true;
			this.Jetpack.powerType = PropType.headstart;
			this.ChangeState(this.Jetpack);
		}
		if (this.OnTurboHeadstartInput != null)
		{
			this.OnTurboHeadstartInput();
		}
	}

	public void PickupFlypack()
	{
		Game.Instance.StartFlypack();
		GameStats.Instance.flypackPickups++;
	}

	public void PickupPogostick(bool willShowPickup)
	{
		this.StartSpringJump(willShowPickup);
		GameStats.Instance.powerJumperPickups++;
	}

	public void PickupBound(float jumpHeight, float jumpDistance, float totalDistance, float startY)
	{
		this.StartBoundJump(jumpHeight, jumpDistance, totalDistance, new Vector3(this.character.transform.position.x, startY, this.character.transform.position.z));
		GameStats.Instance.powerJumperPickups++;
	}

	public void PickupTransition()
	{
		this.ChangeState(this.traversingCity);
	}

	public void StartFlypack()
	{
		this.flypack.headStart = false;
		this.flypack.powerType = PropType.flypack;
		this.ChangeState(this.Jetpack);
	}

	public void StartSpeedUp()
	{
		this.ChangeState(this.speedup);
	}

	public void StartBoundJump(float jumpHeight, float jumpDistance, float totalDistance, Vector3 position)
	{
		this.boundJump.SetBoundJumpData(jumpHeight, jumpDistance, totalDistance, position);
		this.ChangeState(this.boundJump);
	}

	public void StartSpringJump(bool willShowPickup)
	{
		this.SpringJump.powerType = PropType.springJump;
		this.SpringJump.WillShowPickup = willShowPickup;
		this.ChangeState(this.SpringJump);
	}

	public Character Character
	{
		get
		{
			return this.character;
		}
	}

	public static CharacterController Charactercontroller
	{
		get
		{
			if (Game.characterController == null)
			{
				Game.characterController = (UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController);
			}
			return Game.characterController;
		}
	}

	public Character.CriticalHitType HitType
	{
		get
		{
			return this.type;
		}
	}

	public bool HasSuperShoes
	{
		get
		{
			return this.attachment.SuperShoes.IsActive;
		}
	}

	public static Game Instance
	{
		get
		{
			if (Game.instance == null)
			{
				Game.instance = Utils.FindObject<Game>();
			}
			return Game.instance;
		}
	}

	public bool IsInFlypackMode
	{
		get
		{
			return this.characterState == this.flypack;
		}
	}

	public bool IsInSpringJumpMode
	{
		get
		{
			return this.characterState == this.springJump;
		}
	}

	public bool IsInBoundJumpMode
	{
		get
		{
			return this.characterState == this.boundJump;
		}
	}

	public bool isPaused
	{
		get
		{
			return this._paused;
		}
	}

	public Flypack Jetpack
	{
		get
		{
			return this.flypack;
		}
	}

	public CharacterState CharacterState
	{
		get
		{
			return this.characterState;
		}
	}

	public Raft Raft
	{
		get
		{
			return this.raft;
		}
	}

	public CharacterAttachmentCollection Attachment
	{
		get
		{
			return this.attachment;
		}
	}

	public float NormalizedGameSpeed
	{
		get
		{
			return this.currentSpeed / this.speed.min;
		}
	}

	public float DefaultSpeedForAnimation
	{
		get
		{
			return 1f;
		}
	}

	public SpringJump SpringJump
	{
		get
		{
			return this.springJump;
		}
	}

	public Running Running
	{
		get
		{
			return this.running;
		}
	}

	public bool IsInRunningMode
	{
		get
		{
			return this.characterState == this.running;
		}
	}

	public bool GoingBackToCheckpoint
	{
		get
		{
			return this.goingBackToCheckpoint;
		}
	}

	public bool isDead;

	public float currentSpeed;

	public float currentLevelSpeed = 30f;

	public float distancePerMeter = 8f;

	[SerializeField]
	private Game.SwipeInfo swipe;

	public Game.SpeedInfo speed;

	[SerializeField]
	private float backToCheckpointDelayTime = 0.7f;

	[SerializeField]
	private float backToCheckpointZoomTime = 1f;

	[SerializeField]
	private CharacterAttachmentCollection attachment;

	[HideInInspector]
	public Character character;

	[HideInInspector]
	public TrackController trackController;

	[SerializeField]
	private FollowZ followZ;

	private bool firstStart;

	public Game.OnPauseChangeDelegate OnPauseChange;

	public Game.OnStageMenuSequenceDelegate OnStageMenuSequence;

	public Game.OnIntroRunDelegate OnIntroRun;

	public Variable<bool> IsInGame = new Variable<bool>(false);

	public Variable<bool> IsInTopMenu = new Variable<bool>(false);

	[HideInInspector]
	public bool awakeDone;

	[HideInInspector]
	public bool isReadyForSlideinPowerups;

	[HideInInspector]
	public bool wasButtonClicked;

	public TopMenuAnimations topMenu;

	public Characters.CharacterType tryCharacterType;

	public int tryCharacterThemeIndex;

	public Characters.CharacterType subscriptionCharacterType;

	public Helmets.HelmType tryHelmetType;

	private bool _paused;

	private Animation characterAnimation;

	private CharacterCamera characterCamera;

	private static CharacterController characterController;

	private CharacterRendering characterRendering;

	private CharacterState characterState;

	private Swipe currentSwipe;

	private IEnumerator currentThread;

	private Boss boss;

	private bool goingBackToCheckpoint;

	public static bool HasLoaded;

	private static Game instance;

	private float internalStartTime;

	private float t;

	public Action OnGameEnded;

	public Action OnGameStarted;

	public Action OnMenuScreenShown;

	private Running running;

	private Flypack flypack;

	private SpringJump springJump;

	private BoundJump boundJump;

	private SpeedUp speedup;

	private TraversingCity traversingCity;

	private Raft raft;

	private Die die;

	private WallWalking wallWalking;

	private float startTime;

	private float startGameTime;

	private GameStats stats;

	public bool IsTestChar;

	public int preCharacter;

	public int preCharacterSkin;

	public bool IsTestHelm;

	public int preHelmet;

	[HideInInspector]
	public bool show20sAd = true;

	[HideInInspector]
	public float showAdTime = -20f;

	public Action<bool> rewardAdCallback;

	private int touchCount;

	private float doubleTapDelay = 0.5f;

	private Vector2 delta = Vector2.zero;

	private Character.CriticalHitType type;

	public delegate void OnGameOverDelegate(GameStats gameStats);

	public delegate void OnIntroRunDelegate();

	public delegate void OnPauseChangeDelegate(bool pause);

	public delegate void OnSpeedChangedDelegate(float speed);

	public delegate void OnStageMenuSequenceDelegate();

	public delegate void OnTopMenuDelegate();

	public delegate void OnTurboHeadstartInputDelegate();

	[Serializable]
	public class SpeedInfo
	{
		public float min = 110f;

		public float max = 220f;

		public float rampUpDuration = 200f;
	}

	[Serializable]
	public class SwipeInfo
	{
		public float distanceMin = 0.1f;

		public float doubleTapDuration = 0.3f;
	}
}
