using System;
using System.Collections;
using UnityEngine;

public class CelebrationPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		this._timeScaleBeforeCelebrationPopup = Time.timeScale;
		Time.timeScale = 1f;
		this.GlowEffect.enabled = false;
		this.SetupCelebrationScreen();
	}

	private void _FinishOpening()
	{
		this.celebrationHasStarted = false;
		this._isFingerPressed = false;
		if (this.anotherReward)
		{
			base.StartCoroutine(this.MoveNextBoxToFront());
		}
		else
		{
			int i = 0;
			int num = this.slots.Length;
			while (i < num)
			{
				UnityEngine.Object.Destroy(this.slots[i]);
				i++;
			}
			this.ResetBackgroundToNormal();
			this.continueButton.enabled = true;
			this.openButton.enabled = false;
			UnityEngine.Object.Destroy(this.tapToStartGo);
			if (RewardManager.rewardsToUnlockCount > 0 && RewardManager.canShowMultipleQueuedCelebrations)
			{
				this.SetupCelebrationScreen();
			}
			else
			{
				if (RewardManager.canShowMultipleQueuedCelebrations)
				{
					RewardManager.canShowMultipleQueuedCelebrations = false;
				}
				UIScreenController.Instance.ClosePopup(null);
			}
		}
	}

	private IEnumerator _ShowReward(int currentRewardIndex)
	{
		CelebrationReward reward = this.rewardsToUnlock[currentRewardIndex];
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest)
		{
			Statistics stats= PlayerInfo.Instance.stats;
			(stats )[Stat.ChestesOpened] = stats[Stat.ChestesOpened] + 1;
		}
		GameObject rewardGo = null;
		bool flag = false;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			this.ShowModel(reward.characterType, reward.characterThemeIndex, UIModelController.ModelScreen.CelebrationCharacterUnlock);
			AudioPlayer.Instance.PlaySound("leyou_Hr_unlock", 0.5f, 0.5f, 1f);
			flag = true;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock)
		{
			this.ShowBackgroundWithStripes(reward.helmType, UIModelController.ModelScreen.CelebrationHelmUnlock);
			flag = true;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			this.ShowBackgroundWithStripes(PlayerInfo.Instance.currentHelmet, UIModelController.ModelScreen.CelebrationHighScore);
			AudioPlayer.Instance.PlaySound("leyou_Hr_unlock", 0.5f, 0.5f, 1f);
			flag = true;
		}
		if (!flag)
		{
			this.GlowEffect.enabled = true;
			GameObject rewardPrefab = this.ChooseRewardPrefab(reward);
			rewardGo = NGUITools.AddChild(this.slots[0], rewardPrefab);
			rewardGo.transform.localPosition = this._rewardLocalPosition;
			rewardGo.transform.localScale = this._rewardStartScale;
			rewardGo.transform.localRotation = Quaternion.Euler(this._rewardStartRotation);
			Utility.SetLayerRecursively(rewardGo.transform, this.boxParent.layer);
			AudioPlayer.Instance.PlaySound("leyou_Hr_ChestOpen", 0.5f, 0.5f, 1f);
			Animation animation = null;
			if (this.IsTwoStepsCelebration(currentRewardIndex))
			{
				animation = this._celebrationRewardContainer[currentRewardIndex].GetComponentInChildren<Animation>();
				if (!(animation == null) && !(animation["up"] == null))
				{
					animation.Play("up");
					while (animation["up"].normalizedTime < 0.5f)
					{
						yield return null;
					}
				}
			}
			this._maySetTimeScale = true;
			base.StartCoroutine(this.ScaleGameObject(rewardGo.transform, 2f, this._rewardEndScale));
			if (this.IsTwoStepsCelebration(currentRewardIndex))
			{
				base.StartCoroutine(this.MoveGameObject(this._celebrationRewardContainer[currentRewardIndex].transform, 0.7f, this._outOfScreenPosition));
			}
			base.StartCoroutine(this.RotateGameObject(rewardGo.transform, 4f, new Vector3(0f, 1500f, 0f)));
			yield return new WaitForSeconds(0.5f);
			base.StartCoroutine(this.AnimateColor(this.GlowEffect.material, 1.5f, Color.white));
			base.StartCoroutine(this.RotateGameObject(this.GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
			yield return new WaitForSeconds(1.3f);
		}
		CelebrationPopupLabelTemplate template = this.InitRewardLabelTemplate(reward);
		this.celPopLabelTemple = template;
		if (reward.rewardType != CelebrationRewardType.highscore)
		{
			base.StartCoroutine(this.AnimateAlpha(template, 0.2f, 1f));
		}
		yield return new WaitForSeconds(0.5f);
		if (reward.rewardType != CelebrationRewardType.coins)
		{
			if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.HelmetUnlock)
			{
				yield return new WaitForSeconds(1f);
			}
		}
		else
		{
			base.StartCoroutine(this.CountUpCoins(reward.amount, template));
			yield return new WaitForSeconds(2.5f);
		}
		base.StartCoroutine(this.AnimateColor(this.GlowEffect.material, 0.5f, Color.black));
		this.PayoutReward(reward);
		if (UIScreenController.Instance.CheckNetwork() && UIScreenController.Instance.GetTopScreenName().Equals("GameoverUI") && reward.rewardType != CelebrationRewardType.highscore && reward.rewardType != CelebrationRewardType.character && reward.rewardType != CelebrationRewardType.specialHelm && reward.rewardType != CelebrationRewardType._notset)
		{
			this.canDoubleReward = reward;
			this.watchDoublePrefab.SetActive(true);
			this.openButton.enabled = false;
			this.continueButton.enabled = false;
			if (RiseSdk.Instance.HasRewardAd())
			{
				this.watchDoubleButton.enabled = true;
				for (int i = 0; i < this.watchLabels.Length; i++)
				{
					this.watchLabels[i].color = this.watchLabelOriColor;
				}
				for (int j = 0; j < this.watchSprites.Length; j++)
				{
					this.watchSprites[j].color = new Color(255f, 255f, 255f, 255f);
				}
			}
			else
			{
				this.watchDoubleButton.enabled = false;
				for (int k = 0; k < this.watchLabels.Length; k++)
				{
					this.watchLabels[k].color = Color.gray;
				}
				for (int l = 0; l < this.watchSprites.Length; l++)
				{
					this.watchSprites[l].color = new Color(0f, 255f, 255f, 255f);
				}
			}
		}
		else
		{
			this.openButton.enabled = true;
			this.continueButton.enabled = true;
			this.watchDoublePrefab.SetActive(false);
			this.canDoubleReward = null;
		}
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.HelmetUnlock)
		{
			yield return new WaitForSeconds(1f);
		}
		this.tapToStartLabel.text = Strings.Get(LanguageKey.CELEBRATION_POPUP_CONTINUE);
		base.StartCoroutine(this.AnimateAlpha(this.tapToStartLabel, 0.5f, 1f));
		this._maySetTimeScale = false;
		Time.timeScale = 1f;
		this.isWaitingForInput = true;
		while (this.ShouldWaitForInput())
		{
			yield return null;
		}
		this.isWaitingForInput = false;
		if (this.watchDoublePrefab.activeInHierarchy)
		{
			this.watchDoublePrefab.SetActive(false);
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			this.HideModel();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock)
		{
			this.HideSpecialHelm();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			this.HideModel();
		}
		UnityEngine.Object.Destroy(rewardGo);
		UnityEngine.Object.Destroy(template.gameObject);
		UnityEngine.Object.Destroy(this._celebrationRewardContainer[currentRewardIndex]);
		if (this._currentNewHighScoreHandler != null)
		{
			UnityEngine.Object.Destroy(this._currentNewHighScoreHandler.gameObject);
			this._currentNewHighScoreHandler = null;
		}
		this._FinishOpening();
		yield break;
	}

	public void OnDoubleClick()
	{
		Game.Instance.ShowRewarAd(20, Game.Instance.rewardAdCallback = delegate(bool success)
		{
			if (success)
			{
				if (this.canDoubleReward.rewardType == CelebrationRewardType.coins)
				{
					PlayerInfo.Instance.amountOfCoins += this.canDoubleReward.amount;
					this.celPopLabelTemple.SetDoubleCions(this.canDoubleReward.amount * 2);
				}
				else if (this.canDoubleReward.rewardType == CelebrationRewardType.powerup)
				{
					this.celPopLabelTemple.SetDoublePowerup(this.canDoubleReward.powerupType, this.canDoubleReward.amount * 2);
				}
				else if (this.canDoubleReward.rewardType == CelebrationRewardType.symbol)
				{
					this.celPopLabelTemple.SetupDoubleSymbol(this.canDoubleReward.characterType, this.canDoubleReward.amount);
					TasksManager.Instance.PlayerDidThis(TaskTarget.Symbols, this.canDoubleReward.amount, -1);
				}
				else if (this.canDoubleReward.rewardType == CelebrationRewardType.keys)
				{
					this.celPopLabelTemple.SetupDoubleKeys(this.canDoubleReward.amount * 2);
				}
				this.PayoutReward(this.canDoubleReward);
				this.watchDoublePrefab.SetActive(false);
			}
			this.canDoubleReward = null;
		});
	}

	private CelebrationPopupLabelTemplate InitRewardLabelTemplate(CelebrationReward reward)
	{
		GameObject gameObject = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, this.rewardLabelTemplate);
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			gameObject.transform.localPosition = this._labelPositionUnlock;
		}
		else
		{
			gameObject.transform.localPosition = this._labelPosition;
		}
		CelebrationPopupLabelTemplate component = gameObject.GetComponent<CelebrationPopupLabelTemplate>();
		component.Init(this._backgroundPanel.depth);
		if (this._stripeCoroutineRunning)
		{
			component.bigLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.subLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.bigLabel.effectColor = new Color32(250, 187, 231, byte.MaxValue);
			component.subLabel.effectColor = new Color32(250, 187, 231, byte.MaxValue);
		}
		else
		{
			component.bigLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.subLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.bigLabel.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			component.subLabel.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		if (reward.rewardType == CelebrationRewardType.coins)
		{
			component.SetupCoins();
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			component.SetupPowerup(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.symbol)
		{
			component.SetupSymbol(reward.characterType, reward.amount);
			TasksManager.Instance.PlayerDidThis(TaskTarget.Symbols, reward.amount, -1);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			component.SetupKeys(reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.character)
		{
			CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(reward.characterType, reward.characterThemeIndex);
			if (themeForCharacter != null)
			{
				component.SetupCharacter(Strings.Get(themeForCharacter.title));
			}
			else
			{
				Characters.Model model = Characters.characterData[reward.characterType];
				component.SetupCharacter(Strings.Get(model.name));
			}
		}
		else if (reward.rewardType == CelebrationRewardType.specialHelm)
		{
			component.SetupEventSpecialHelm(Strings.Get(Helmets.helmData[reward.helmType].name));
		}
		else if (reward.rewardType == CelebrationRewardType.highscore)
		{
			gameObject.SetActive(false);
		}
		return component;
	}

	private IEnumerator AnimateAlpha(CelebrationPopupLabelTemplate template, float duration, float toAlpha)
	{
		float fromAlpha = template.Alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			template.Alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
		yield break;
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
		yield break;
	}

	private IEnumerator AnimateColor(Material material, float duration, Color toColor)
	{
		Color fromColor = material.GetColor(Shaders.Instance.MainColor);
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			material.SetColor(Shaders.Instance.MainColor, Color.Lerp(fromColor, toColor, factor));
			yield return null;
		}
		yield break;
	}

	private void ResetBackgroundToNormal()
	{
		this._stripeCoroutineRunning = false;
		base.StopCoroutine("AnimateStripes");
		if (this._stripeTextures != null)
		{
			for (int i = 0; i < this._stripeTextures.Length; i++)
			{
				this._stripeTextures[i].gameObject.SetActive(false);
				UnityEngine.Object.Destroy(this._stripeTextures[i].gameObject);
			}
		}
		this._stripeTextures = null;
	}

	private IEnumerator AnimateStripes()
	{
		this._stripeCoroutineRunning = true;
		Color32 colorOfStripe = new Color32(233, 132, 196, byte.MaxValue);
		int minSpeed = 600;
		int maxSpeed = 2000;
		float minAlpha = 0.3f;
		float maxAlpha = 0.9f;
		float minScale = 0.5f;
		float maxScale = 1f;
		int numberOfStripes = 12;
		Vector2 minimumThreshold = new Vector2(-150f, -225f);
		Vector2 maximumThreshold = new Vector2(150f, 700f);
		if (this._stripeTextures != null)
		{
			for (int k = 0; k < this._stripeTextures.Length; k++)
			{
				this._stripeTextures[k].gameObject.SetActive(false);
				UnityEngine.Object.Destroy(this._stripeTextures[k].gameObject);
			}
		}
		this._stripeTextures = new UITexture[numberOfStripes];
		float[] stripeSpeeds = new float[numberOfStripes];
		for (int i = 0; i < numberOfStripes; i++)
		{
			stripeSpeeds[i] = (float)UnityEngine.Random.Range(minSpeed, maxSpeed);
			this._stripeTextures[i] = NGUITools.AddWidget<UITexture>(this.stripePanel);
			this._stripeTextures[i].mainTexture = this.itemHuntStripe;
			this._stripeTextures[i].shader = Shader.Find("Unlit/Transparent Colored");
			this._stripeTextures[i].MakePixelPerfect();
			this._stripeTextures[i].color = colorOfStripe;
			this._stripeTextures[i].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[i] / (float)maxSpeed);
			Vector3 localScale = this._stripeTextures[i].cachedTransform.localScale;
			this._stripeTextures[i].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale.x * minScale, localScale.y * minScale, localScale.z), new Vector3(localScale.x * maxScale, localScale.y * maxScale, localScale.z), stripeSpeeds[i] / (float)maxSpeed);
			float x = UnityEngine.Random.Range(minimumThreshold.x, maximumThreshold.x);
			float y = UnityEngine.Random.Range(minimumThreshold.y, maximumThreshold.y);
			this._stripeTextures[i].cachedTransform.localPosition = new Vector3(x, y, 0f);
		}
		yield return null;
		for (;;)
		{
			for (int j = 0; j < this._stripeTextures.Length; j++)
			{
				Vector3 localPosition = this._stripeTextures[j].cachedTransform.localPosition;
				float num = stripeSpeeds[j] * Time.deltaTime;
				if (localPosition.y > maximumThreshold.y)
				{
					localPosition.x = UnityEngine.Random.Range(minimumThreshold.x, maximumThreshold.x);
					stripeSpeeds[j] = (float)UnityEngine.Random.Range(minSpeed, maxSpeed);
					num = stripeSpeeds[j] * Time.deltaTime;
					this._stripeTextures[j].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[j] / (float)maxSpeed);
					this._stripeTextures[j].MakePixelPerfect();
					Vector3 localScale2 = this._stripeTextures[j].cachedTransform.localScale;
					this._stripeTextures[j].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale2.x * minScale, localScale2.y * minScale, localScale2.z), new Vector3(localScale2.x * maxScale, localScale2.y * maxScale, localScale2.z), stripeSpeeds[j] / (float)maxSpeed);
					localPosition.y = minimumThreshold.y - num;
				}
				this._stripeTextures[j].cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y + num, localPosition.z);
			}
			yield return null;
		}
		yield break;
	}

	private void Awake()
	{
		if (Application.isEditor && this._NewHighScoreHandlerPrefab == null)
		{
			UnityEngine.Debug.LogError("NewHighScoreHandler not set in CelebrationPopup");
		}
	}

	private IEnumerator BoxIdleAnimCoroutine(Transform rewardTrans)
	{
		Vector3 baseLocalPos = rewardTrans.parent.localPosition;
		this.stopIdleAnim = false;
		float t = UnityEngine.Random.Range(0f, 6f);
		Vector3 newLocalPos = baseLocalPos;
		while (!this.stopIdleAnim)
		{
			t += Time.deltaTime;
			newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
		bool doneResetting = false;
		while (!doneResetting)
		{
			newLocalPos.y = Mathf.MoveTowards(newLocalPos.y, baseLocalPos.y, Time.deltaTime * 20f);
			if (Mathf.Approximately(newLocalPos.y, baseLocalPos.y))
			{
				doneResetting = true;
			}
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
		yield break;
	}

	private IEnumerator CharacterIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		while (!this.stopIdleAnim)
		{
			yield return null;
		}
		yield break;
	}

	private GameObject ChooseRewardPrefab(CelebrationReward reward)
	{
		GameObject result = this.rewardCoins;
		switch (reward.rewardType)
		{
		case CelebrationRewardType.coins:
			return this.rewardCoins;
		case CelebrationRewardType.powerup:
		{
			PropType powerupType = reward.powerupType;
			if (powerupType == PropType.headstart2000)
			{
				return this.rewardPowerupHeadstart2000;
			}
			if (powerupType != PropType.scorebooster)
			{
				return result;
			}
			return this.rewardPowerupScoreBooster;
		}
		case CelebrationRewardType.symbol:
		{
			Characters.CharacterType characterType = reward.characterType;
			if (characterType == Characters.CharacterType.lee)
			{
				return this.rewardSymbolLee;
			}
			if (characterType != Characters.CharacterType.turtlefok)
			{
				return result;
			}
			return this.rewardSymbolTurtlefok;
		}
		case CelebrationRewardType.keys:
			return this.rewardKey;
		}
		return result;
	}

	private IEnumerator CountUpCoins(int amount, CelebrationPopupLabelTemplate rewardTemplate)
	{
		PlayerInfo.Instance.amountOfCoins += amount;
		float countFactor = 0f;
		float countTime = Mathf.Lerp(1.5f, 4f, (float)amount / 100000f);
		int rewardLabelFrom = 0;
		yield return new WaitForSeconds(0.5f);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			rewardTemplate.UpdateCoins(Mathf.RoundToInt(Mathf.SmoothStep((float)rewardLabelFrom, (float)amount, countFactor)));
			yield return null;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, amount, -1);
		yield break;
	}

	private void InitSlotsAndRewardContainers(int length)
	{
		this.slots = new GameObject[length];
		this._celebrationRewardContainer = new GameObject[length];
		for (int i = 0; i < this._numberOfRewards; i++)
		{
			this.slots[i] = NGUITools.AddChild(this.boxParent);
			this.slots[i].transform.localPosition = CelebrationPopup.FIRST_SLOT_POSITION + CelebrationPopup.OTHER_SLOT_OFFSET * (float)((i > 0) ? 1 : 0);
		}
	}

	private void FillOutAllTheSlotsWithRewardContainers()
	{
		for (int i = 0; i < this._numberOfRewards; i++)
		{
			GameObject gameObject = null;
			CelebrationRewardOrigin celebrationRewardOrigin = this.rewardsToUnlock[i].CelebrationRewardOrigin;
			bool flag = true;
			if (celebrationRewardOrigin != CelebrationRewardOrigin.Chest)
			{
				if (celebrationRewardOrigin != CelebrationRewardOrigin.SuperChest)
				{
					if (celebrationRewardOrigin == CelebrationRewardOrigin.ChestMini)
					{
						gameObject = NGUITools.AddChild(this.slots[i], this.miniChestPrefab);
						flag = false;
					}
				}
				else
				{
					gameObject = NGUITools.AddChild(this.slots[i], this.superChestPrefab);
					flag = false;
				}
			}
			else
			{
				gameObject = NGUITools.AddChild(this.slots[i], this.chestPrefab);
				flag = false;
			}
			if (!flag)
			{
				gameObject.transform.localScale = this._boxScale;
				gameObject.transform.localRotation = Quaternion.Euler(this._boxRotation);
				Utility.SetLayerRecursively(gameObject.transform, this.boxParent.layer);
				this._celebrationRewardContainer[i] = gameObject;
			}
			else
			{
				this._celebrationRewardContainer[i] = null;
			}
		}
		this.GlowEffect.material.SetColor(Shaders.Instance.MainColor, Color.black);
	}

	public void OnPressed()
	{
		if (!this._isFingerPressed)
		{
			this._isFingerPressed = true;
			this.stopIdleAnim = true;
			if (this.IsTwoStepsCelebration(this._currentReward))
			{
				Animation componentInChildren = this._celebrationRewardContainer[this._currentReward].GetComponentInChildren<Animation>();
				if (componentInChildren != null && componentInChildren["down"] != null)
				{
					componentInChildren.Play("down");
				}
			}
		}
	}

	public void OnReleased()
	{
		if (!this.celebrationHasStarted)
		{
			this.openButton.enabled = false;
			if (this._containerEffect != null)
			{
				this._containerEffect.StopEffect();
			}
			base.StartCoroutine(this.AnimateAlpha(this.tapToStartLabel, 0.5f, 0f));
			base.StartCoroutine(this._ShowReward(this._currentReward));
			this.celebrationHasStarted = true;
		}
	}

	public override void Hide()
	{
		base.Hide();
		Time.timeScale = this._timeScaleBeforeCelebrationPopup;
		base.StopCoroutine("AnimateStripes");
	}

	private void HideModel()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private void HideSpecialHelm()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private bool IsTwoStepsCelebration(int rewardIndex)
	{
		return !(this._celebrationRewardContainer[rewardIndex] == null);
	}

	private IEnumerator MoveGameObject(Transform trans, float duration, Vector3 toPos)
	{
		if (trans == null)
		{
			yield break;
		}
		Vector3 fromPos = trans.localPosition;
		float factor = 0f;
		while (factor < 1f && trans != null)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, 0.5f * (Mathf.Sin((factor - 0.5f) * 3.141593f) + 1f));
			yield return null;
		}
		if (trans != null)
		{
			trans.localPosition = toPos;
		}
		yield break;
	}

	private IEnumerator MoveNextBoxToFront()
	{
		Time.timeScale = 1f;
		this._currentReward++;
		if (this._currentReward >= this._numberOfRewards - 1)
		{
			this.anotherReward = false;
		}
		this.UpdateGui(this.rewardsToUnlock[this._currentReward]);
		this.GlowEffect.enabled = false;
		this.GlowEffect.material.SetColor(Shaders.Instance.MainColor, Color.black);
		this.ToggleSuperChestEffect(this.rewardsToUnlock[this._currentReward].CelebrationRewardOrigin);
		this.openButton.enabled = false;
		if (!this.IsTwoStepsCelebration(this._currentReward))
		{
			this.OnReleased();
		}
		else
		{
			this._celebrationRewardContainer[this._currentReward].transform.parent = this.slots[0].transform;
			base.StartCoroutine(this.MoveGameObject(this._celebrationRewardContainer[this._currentReward].transform, 0.35f, Vector3.zero));
			this.StartIdleAnimCoroutine(this._celebrationRewardContainer[this._currentReward].transform, this.rewardsToUnlock[this._currentReward]);
			yield return new WaitForSeconds(0.35f);
			this.openButton.enabled = true;
		}
		yield break;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && this._isFingerPressed)
		{
			this.OnReleased();
		}
	}

	private void PayoutReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.topRun)
		{
			UIScreenController.Instance.PayoutCelebrationReward(reward);
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.symbol)
		{
			PlayerInfo.Instance.CollectSymbol(reward.characterType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.amountOfKeys += reward.amount;
		}
		RewardManager.RewardPayedOut(reward);
		PlayerInfo.Instance.SaveIfDirty();
	}

	private IEnumerator RotateGameObject(Transform trans, float duration, Vector3 angleToRotate)
	{
		if (trans == null)
		{
			yield break;
		}
		Quaternion fromRotation = trans.localRotation;
		float factor = 0f;
		while (factor < 1f && trans != null)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			float angle = 0f;
			angle = Mathf.Lerp(4.712389f, 6.283185f, factor);
			float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
			trans.localRotation = fromRotation;
			trans.Rotate(angleToRotate * cosFactor, Space.World);
			yield return null;
		}
		yield break;
	}

	private IEnumerator ScaleGameObject(Transform trans, float duration, Vector3 toScale)
	{
		if (trans == null)
		{
			yield break;
		}
		float factor = 0f;
		Vector3 fromScale = trans.localScale;
		while (factor < 1f && trans != null)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			float angle = 0f;
			angle = Mathf.Lerp(3.141593f, 6.283185f, factor);
			float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
			trans.localScale = Vector3.Lerp(fromScale, toScale, cosFactor);
			yield return null;
		}
		yield break;
	}

	private void SetupCelebrationScreen()
	{
		this.boxParent.transform.position = UIModelController.Instance.CelebrationPopupAnchor.transform.position;
		this.rewardsToUnlock = RewardManager.GetRewardsToUnlockForCelebration();
		this.doubleLabe.text = Strings.Get(LanguageKey.UI_POPUP_CELEBRATION_DOUBLE);
		this.doubleScriptLabel.text = Strings.Get(LanguageKey.UI_POPUP_CELEBRATION_DOUBLE_DESCRIPT);
		this._numberOfRewards = this.rewardsToUnlock.Length;
		if (this._numberOfRewards <= 0)
		{
			this.anotherReward = false;
			UIScreenController.Instance.ClosePopup(null);
			this.openButton.enabled = false;
		}
		else
		{
			if (this._numberOfRewards == 1)
			{
				this.anotherReward = false;
			}
			else
			{
				this.anotherReward = true;
			}
			this.InitTapToStart();
			this.InitSlotsAndRewardContainers(this._numberOfRewards);
			this.FillOutAllTheSlotsWithRewardContainers();
			this._currentReward = 0;
			this.InitEffect();
			this.UpdateGui(this.rewardsToUnlock[this._currentReward]);
			if (this.IsTwoStepsCelebration(this._currentReward))
			{
				this.StartIdleAnimCoroutine(this._celebrationRewardContainer[this._currentReward].transform, this.rewardsToUnlock[this._currentReward]);
			}
			else
			{
				this.StartOneStepUnlock();
			}
		}
	}

	private void InitEffect()
	{
		if (this.superChestEffectPrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(this.slots[this._currentReward], this.superChestEffectPrefab);
			gameObject.transform.transform.position = this.SUPERBOX_EFFECT_POSITION;
			this._containerEffect = gameObject.GetComponent<SuperChestEffect>();
			if (this._containerEffect == null)
			{
				UnityEngine.Debug.LogError("CelebrationPopup ERROR: unable to find SuperChestEffect component on superChestEffectPrefab", this);
			}
			else
			{
				this.ToggleSuperChestEffect(this.rewardsToUnlock[this._currentReward].CelebrationRewardOrigin);
			}
		}
		else
		{
			UnityEngine.Debug.LogError("CelebrationPopup ERROR: superChestEffectPrefab is not assigned", this);
		}
	}

	private void InitTapToStart()
	{
		this.openButton.enabled = true;
		this.continueButton.enabled = true;
		this.watchDoublePrefab.SetActive(false);
		this.tapToStartGo = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, this.tapToStartLabelPrefab);
		this.tapToStartGo.transform.localPosition = this._continueLabelPosition;
		this.tapToStartLabel = this.tapToStartGo.GetComponentInChildren<UILabel>();
		this.tapToStartLabel.alpha = 0f;
		this.tapToStartGo.GetComponent<UIPanel>().depth = this._backgroundPanel.depth + 1;
	}

	private bool ShouldWaitForInput()
	{
		if (this.skipWaitBackButtonPressed)
		{
			this.skipWaitBackButtonPressed = false;
			return false;
		}
		return !Input.GetMouseButtonUp(0);
	}

	private void ShowBackgroundWithStripes(Helmets.HelmType helmType, UIModelController.ModelScreen modelScreen)
	{
		UIModelController.Instance.ActivateCelebrationHelmWithStripes(helmType, modelScreen);
	}

	private GameObject ShowModel(Characters.CharacterType charType, int themeIndex, UIModelController.ModelScreen modelScreen)
	{
		return UIModelController.Instance.ShowCharacterInCelebration(charType, themeIndex, modelScreen);
	}

	public void SkipNow()
	{
		if (this._maySetTimeScale)
		{
			Time.timeScale = 4f;
		}
	}

	private void StartIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		this.tapToStartLabel.text = Strings.Get(LanguageKey.CELEBRATION_POPUP_OPEN);
		this.tapToStartLabel.alpha = 1f;
		base.StartCoroutine(this.BoxIdleAnimCoroutine(rewardTrans));
	}

	private void StartOneStepUnlock()
	{
		this.OnReleased();
	}

	private void ToggleSuperChestEffect(CelebrationRewardOrigin origin)
	{
		if (this._containerEffect != null)
		{
			if (origin == CelebrationRewardOrigin.SuperChest)
			{
				this._containerEffect.FastForwardEffect(3);
				this._containerEffect.StartEffect();
			}
			else
			{
				this._containerEffect.StopEffect();
			}
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			if (this.openButton.enabled)
			{
				this.OnPressed();
				this.OnReleased();
			}
			if (this.continueButton.enabled)
			{
				this.SkipNow();
			}
			if (this.isWaitingForInput)
			{
				this.skipWaitBackButtonPressed = true;
			}
		}
	}

	private void UpdateGui(CelebrationReward reward)
	{
		switch (reward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.Chest:
		case CelebrationRewardOrigin.SuperChest:
		case CelebrationRewardOrigin.ChestMini:
			this.ResetBackgroundToNormal();
			this.openButton.enabled = true;
			break;
		case CelebrationRewardOrigin.CharacterUnlock:
		case CelebrationRewardOrigin.HelmetUnlock:
			this.ResetBackgroundToNormal();
			this.openButton.enabled = false;
			break;
		case CelebrationRewardOrigin.NewHighScore:
			this.ResetBackgroundToNormal();
			this.openButton.enabled = false;
			this._currentNewHighScoreHandler = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, this._NewHighScoreHandlerPrefab.gameObject).GetComponent<NewHighScoreHandler>();
			Utility.SetLayerRecursively(this._currentNewHighScoreHandler.gameObject.transform, UIScreenController.Instance.CameraOverlay2d.gameObject.layer);
			break;
		}
	}

	public GameObject boxParent;

	public GameObject rewardLabelTemplate;

	[SerializeField]
	private Collider openButton;

	[SerializeField]
	private Collider continueButton;

	[SerializeField]
	private Collider skipDoubleButton;

	[SerializeField]
	private Collider watchDoubleButton;

	[SerializeField]
	private Color watchLabelOriColor;

	[SerializeField]
	private UILabel[] watchLabels;

	[SerializeField]
	private UISprite[] watchSprites;

	[SerializeField]
	private GameObject tapToStartLabelPrefab;

	[SerializeField]
	private GameObject watchDoublePrefab;

	[SerializeField]
	private GameObject chestPrefab;

	[SerializeField]
	private GameObject superChestPrefab;

	[SerializeField]
	private GameObject miniChestPrefab;

	[SerializeField]
	private GameObject superChestEffectPrefab;

	[SerializeField]
	private GameObject rewardCoins;

	[SerializeField]
	private GameObject rewardSymbolLee;

	[SerializeField]
	private GameObject rewardSymbolTurtlefok;

	[SerializeField]
	private GameObject rewardPowerupHeadstart2000;

	[SerializeField]
	private GameObject rewardPowerupScoreBooster;

	[SerializeField]
	private GameObject rewardKey;

	[SerializeField]
	private MeshRenderer GlowEffect;

	[SerializeField]
	private UIPanel _backgroundPanel;

	[SerializeField]
	private Texture2D itemHuntStripe;

	[SerializeField]
	private GameObject stripePanel;

	[SerializeField]
	private NewHighScoreHandler _NewHighScoreHandlerPrefab;

	[SerializeField]
	private UILabel doubleLabe;

	[SerializeField]
	private UILabel doubleScriptLabel;

	[SerializeField]
	private Vector3 _boxRotation = new Vector3(0f, 250f, 20f);

	[SerializeField]
	private Vector3 _boxScale = new Vector3(6f, 6f, 6f);

	[SerializeField]
	private Vector3 _continueLabelPosition = new Vector3(0f, -330f, -5f);

	[SerializeField]
	private Vector3 _continueDoubleLabelPosition = new Vector3(0f, -400f, -5f);

	[SerializeField]
	private Vector3 _labelPosition = new Vector3(0f, -110f, -5f);

	[SerializeField]
	private Vector3 _labelPositionUnlock = new Vector3(0f, -160f, -5f);

	[SerializeField]
	private Vector3 _outOfScreenPosition = new Vector3(0f, -500f, 0f);

	[SerializeField]
	private Vector3 _rewardLocalPosition = new Vector3(0f, 20f, 0f);

	[SerializeField]
	private Vector3 _rewardStartScale = new Vector3(4f, 4f, 4f);

	[SerializeField]
	private Vector3 _rewardStartRotation = new Vector3(0f, -10.5f, 0f);

	[SerializeField]
	private Vector3 _rewardEndScale = new Vector3(10f, 10f, 10f);

	private float _animationLerpFactor;

	private GameObject[] _celebrationRewardContainer;

	private NewHighScoreHandler _currentNewHighScoreHandler;

	private int _currentReward;

	private bool _isFingerPressed;

	private bool _maySetTimeScale;

	private int _numberOfRewards;

	private bool _stripeCoroutineRunning;

	private UITexture[] _stripeTextures;

	private SuperChestEffect _containerEffect;

	private float _timeScaleBeforeCelebrationPopup = 1f;

	private bool anotherReward = true;

	private bool celebrationHasStarted;

	private static readonly Vector3 FIRST_SLOT_POSITION = new Vector3(0f, 15f, -50f);

	private bool isWaitingForInput;

	private static readonly Vector3 OTHER_SLOT_OFFSET = new Vector3(0f, 0f, 9370f);

	private CelebrationReward[] rewardsToUnlock;

	private bool skipWaitBackButtonPressed;

	private GameObject[] slots;

	private bool stopIdleAnim;

	private Vector3 SUPERBOX_EFFECT_POSITION = new Vector3(0f, 0f, -180f);

	private GameObject tapToStartGo;

	private UILabel tapToStartLabel;

	private CelebrationReward canDoubleReward;

	private CelebrationPopupLabelTemplate celPopLabelTemple;
}
