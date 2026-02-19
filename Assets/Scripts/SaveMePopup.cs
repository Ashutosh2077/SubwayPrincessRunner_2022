using System;
using System.Collections;
using UnityEngine;

public class SaveMePopup : UIBaseScreen
{
	private void Awake()
	{
		this.titleLabel.text = Strings.Get(LanguageKey.SAVE_ME_POPUP_BUTTON_LABEL);
		this.watchVideoOriginePos = this.WatchVideoBtn.localPosition;
		this.useKeyOriginePos = this.UseKeysBtn.localPosition;
	}

	public float getAnimationDuration()
	{
		return 10f;
	}

	public float getAnimationTimeLeft()
	{
		return this.timeLeft;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.startClockAnimation());
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			UnityEngine.Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(false);
		}
		if (GameStats.Instance.reviveCount > 0)
		{
			this.WatchVideoBtn.gameObject.SetActive(false);
			this.UseKeysBtn.gameObject.SetActive(false);
			this.FreeBtn.gameObject.SetActive(true);
			GameStats.Instance.reviveCount--;
		}
		else if (!RiseSdk.Instance.HasRewardAd() || !UIScreenController.Instance.CheckNetwork())
		{
			this.WatchVideoBtn.gameObject.SetActive(false);
			this.FreeBtn.gameObject.SetActive(false);
			this.UseKeysBtn.gameObject.SetActive(true);
			this.UseKeysBtn.localPosition = new Vector3(0f, this.useKeyOriginePos.y, 0f);
		}
		else if (SaveMeManager._numberOfUsedKeysInCurrentRun == 1)
		{
			this.UseKeysBtn.gameObject.SetActive(false);
			this.FreeBtn.gameObject.SetActive(false);
			this.WatchVideoBtn.gameObject.SetActive(true);
			this.WatchVideoBtn.localPosition = new Vector3(0f, this.watchVideoOriginePos.y, 0f);
		}
		else
		{
			this.FreeBtn.gameObject.SetActive(false);
			this.WatchVideoBtn.gameObject.SetActive(true);
			this.UseKeysBtn.gameObject.SetActive(true);
			this.WatchVideoBtn.localPosition = this.watchVideoOriginePos;
			this.UseKeysBtn.localPosition = this.useKeyOriginePos;
		}
	}

	private IEnumerator startClockAnimation()
	{
		this.timeLeft = 10f;
		while (this.timeLeft > -0.2f)
		{
			float spriteAmount = Mathf.Clamp01(this.timeLeft / 10f);
			this.saveMeAnimateClock.FillSpriteAmount(spriteAmount);
			this.timeLeft -= Time.deltaTime;
			yield return null;
		}
		UIScreenController.Instance.ClosePopup(null);
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
		yield break;
	}

	public override void GainFocus()
	{
		Time.timeScale = 1f;
	}

	public override void LooseFocus()
	{
		Time.timeScale = 0f;
	}

	public SaveMeAnimateClock saveMeAnimateClock;

	[SerializeField]
	private UILabel titleLabel;

	public Transform WatchVideoBtn;

	public Transform UseKeysBtn;

	public Transform FreeBtn;

	private const float ANIMATION_DURATION = 10f;

	private float timeLeft;

	private Vector3 watchVideoOriginePos;

	private Vector3 useKeyOriginePos;
}
