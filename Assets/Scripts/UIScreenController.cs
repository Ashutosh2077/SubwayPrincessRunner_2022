using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenController : MonoBehaviour
{
	public UIScreenController()
	{
		this._screenNamesWithoutBackground = new List<string>
		{
			"FrontUI",
			"IngameUI"
		};
		this._screenNamesWithCycleDebug = new List<string>
		{
			"Task_popup",
			"DailyRewardsPopup",
			"SettingsPopup",
			"HelmetPopup",
			"SettingsPopup",
			"DailyRewardsPopup",
			"Task_popup"
		};
		this._screenNamesWithCycleAd = new List<string>
		{
			"SettingsPopup",
			"Task_popup",
			"DailyRewardsPopup",
			"HelmetPopup",
			"CoinsUI_shop",
			"UpgradesUI_shop",
			"CharacterScreen",
			"HelmScreen"
		};
		this.text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		this.size = 16;
		DebugShow.SetWillShowDebugInfoByGUI(false);
	}

	public static event Action OnApplicationResumed;

	public event Action OnLastPopupClosed;

	public event Action<string> OnPopupClosed;

	public event Action OnPopupShown;

	private void Awake()
	{
		this.OnChangedScreen = (Action<string>)Delegate.Combine(this.OnChangedScreen, new Action<string>(this.UM_PageChange));
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		if (this.lilitaRegularFont != null)
		{
			this.lilitaRegularFont.RequestCharactersInTexture(this.text, this.size, this.style);
		}
		if (this.titanRegularFont != null)
		{
			this.titanRegularFont.RequestCharactersInTexture(this.text, this.size, this.style);
		}
		this.currentCycleCount = UnityEngine.Random.Range(this.minCycleCount, this.maxCycleCount + 1);
		if (this.curDeviceType == UIScreenController.DeviceType.iPad)
		{
			this.bannerHeight = 110f * (Screen.dpi / 160f);
		}
		else
		{
			this.bannerHeight = 50f * (Screen.dpi / 160f);
		}
		this.curDeviceType = UIScreenController.DeviceType.Android;
	}

	public bool CheckNetwork()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	private void _ActivateNextPopup(string oldPopupName = "")
	{
		if (this.OnPopupShown != null)
		{
			this.OnPopupShown();
		}
		if (this._popupQueue.Count > 0)
		{
			string text = this.QueuePeek(this._popupQueue);
			int i = 0;
			int count = this._popupQueue.Count;
			while (i < count)
			{
				if (this._cachedScreens.ContainsKey(this._popupQueue[i]))
				{
					UIBaseScreen uibaseScreen = this._cachedScreens[this._popupQueue[i]];
					if (uibaseScreen.isActive)
					{
						if (this._popupQueue[i] != text)
						{
							uibaseScreen.LooseFocus();
						}
						else
						{
							uibaseScreen.GainFocus();
						}
					}
				}
				i++;
			}
			this.ActivateAnchors(false);
			if (!this._cachedScreens.ContainsKey(text))
			{
				this._LoadScreenToCache(text, true);
			}
			if (text == "CelebrationPopup" && this._screenStack.Count > 0)
			{
				if (this._screenStack[this._screenStack.Count - 1] == "GameoverUI")
				{
					RewardManager.canShowMultipleQueuedCelebrations = true;
				}
				if (this._screenStack[this._screenStack.Count - 1] != "GameoverUI")
				{
					this._cachedScreens[this._screenStack[this._screenStack.Count - 1]].Hide();
				}
			}
			if (this._popupQueue.Count == 1)
			{
				this._cachedScreens[this._screenStack[this._screenStack.Count - 1]].LooseFocus();
			}
			if (this._screenNamesWithCycleDebug.Contains(text) && this.lastPopupNameIndex < this._screenNamesWithCycleDebug.Count - 1 && text == this._screenNamesWithCycleDebug[++this.lastPopupNameIndex] && this.lastPopupNameIndex == this._screenNamesWithCycleDebug.Count - 1)
			{
				DebugShow.SetWillShowDebugInfoByGUI(true);
			}
			this._SetCycleAd(text);
			UIBaseScreen uibaseScreen2 = this._cachedScreens[text];
			if (uibaseScreen2 != null && !uibaseScreen2.isActive)
			{
				if (oldPopupName != string.Empty)
				{
					uibaseScreen2.parentScreen = oldPopupName;
				}
				uibaseScreen2.Show();
			}
			this._popupActive = true;
			if (this.OnChangedScreen != null)
			{
				this.OnChangedScreen(text);
			}
		}
		else
		{
			this.ActivateAnchors(true);
		}
	}

	private void _PushPopup(string name)
	{
		int num = -1;
		string text = this.QueuePeek(this._popupQueue);
		if (!string.IsNullOrEmpty(text) && this._cachedScreens.ContainsKey(text))
		{
			UIPanel[] componentsInChildren = this._cachedScreens[text].GetComponentsInChildren<UIPanel>(true);
			int i = 0;
			int num2 = componentsInChildren.Length;
			while (i < num2)
			{
				if (componentsInChildren[i].depth > num)
				{
					num = componentsInChildren[i].depth;
				}
				i++;
			}
		}
		this._popupQueue.Insert(0, name);
		this._ActivateNextPopup(text);
		if (this._cachedScreens.ContainsKey(name))
		{
			UIPanel[] componentsInChildren2 = this._cachedScreens[name].GetComponentsInChildren<UIPanel>(true);
			int num3 = -1;
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				UIPanel uipanel = componentsInChildren2[0];
				int j = 0;
				int num4 = componentsInChildren2.Length;
				while (j < num4)
				{
					if (componentsInChildren2[j].depth < uipanel.depth)
					{
						uipanel = componentsInChildren2[j];
					}
					j++;
				}
				if (uipanel != null && num > -1)
				{
					num3 = num + 1 - uipanel.depth;
				}
				if (num3 > -1)
				{
					int k = 0;
					int num5 = componentsInChildren2.Length;
					while (k < num5)
					{
						componentsInChildren2[k].depth += num3;
						k++;
					}
				}
			}
		}
	}

	private void _QueuePopup(string name)
	{
		this.messageHelper.DisableShowLabel();
		this._popupQueue.Add(name);
		if (!this._popupActive)
		{
			this._ActivateNextPopup(string.Empty);
		}
	}

	private void _RemovePopup(string screenName = "")
	{
		if (this._popupQueue.Count >= 1)
		{
			if (screenName == string.Empty)
			{
				screenName = this.Dequeue(this._popupQueue);
			}
			else
			{
				this._popupQueue.Remove(screenName);
			}
			UIBaseScreen uibaseScreen = this._cachedScreens[screenName];
			uibaseScreen.Hide();
			uibaseScreen.parentScreen = string.Empty;
			if (this._cachedScreens.ContainsKey("HelmScreen") && this._popupQueue.Count == 0)
			{
				((HelmScreen)this._cachedScreens["HelmScreen"]).resetHelmAnimation = true;
			}
			if (this._popupQueue.Count == 0)
			{
				this._cachedScreens[this._screenStack[this._screenStack.Count - 1]].GainFocus();
			}
			Action<string> onPopupClosed = this.OnPopupClosed;
			if (onPopupClosed != null)
			{
				onPopupClosed(screenName);
			}
			this._popupActive = false;
			if (this._popupQueue.Count == 0)
			{
				Action<string> onChangedScreen = this.OnChangedScreen;
				if (onChangedScreen != null)
				{
					onChangedScreen(this.GetTopScreenName());
				}
			}
			if (this.doSaveAndDelayRemainingPopups)
			{
				this.SaveAndDelayPopups();
			}
			this._ActivateNextPopup(string.Empty);
			if (screenName == "CelebrationPopup")
			{
				if (this._screenStack.Count > 0 && this.GetTopScreenName() != "GameoverUI")
				{
					this._cachedScreens[this._screenStack[this._screenStack.Count - 1]].Show();
				}
				if (this.GetTopScreenName() == "GameoverUI")
				{
					this._cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().SetupAfterChest();
				}
			}
			if (this._popupQueue.Count == 0 && this.OnLastPopupClosed != null)
			{
				if (!this.ignoreLastPopupClosed)
				{
					this.OnLastPopupClosed();
				}
				else
				{
					this.ignoreLastPopupClosed = false;
				}
			}
		}
	}

	public void ClosePopup(string screenName)
	{
		this._RemovePopup(string.Empty);
	}

	private UIBaseScreen _ActivateScreen(string screenName)
	{
		bool flag = true;
		UIBaseScreen uibaseScreen;
		if (this._cachedScreens.ContainsKey(screenName))
		{
			if (this._screenStack.Count > 0)
			{
				int num = this._screenStack.LastIndexOf(screenName);
				if (num >= 0)
				{
					flag = false;
					if (num < this._screenStack.Count - 1)
					{
						int num2 = num + 1;
						int num3 = this._screenStack.Count - num2;
						for (int i = num2; i < num2 + num3; i++)
						{
							this._cachedScreens[this._screenStack[i]].Hide();
						}
						this._screenStack.RemoveRange(num2, num3);
					}
				}
			}
			uibaseScreen = this._cachedScreens[screenName];
		}
		else
		{
			uibaseScreen = this._LoadScreenToCache(screenName, false);
		}
		if (flag)
		{
			if (this._screenStack.Count > 0)
			{
				this._cachedScreens[this._screenStack[this._screenStack.Count - 1]].Hide();
			}
			this._screenStack.Add(screenName);
		}
		this._ShowScreen(screenName, uibaseScreen);
		return uibaseScreen;
	}

	private void _BackToPreviousScreen()
	{
		if (this._screenStack.Count > 1)
		{
			string text = this.Pop(this._screenStack);
			this._cachedScreens[text].Hide();
			text = this.Peek(this._screenStack);
			this._cachedScreens[text].Show();
			this._SetBackground(!this._screenNamesWithoutBackground.Contains(text));
			this.ScreenDidChange(text);
		}
		else
		{
			UIScreenController.LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
	}

	private UIBaseScreen _LoadScreenToCache(string screenName, bool isPopup = false)
	{
		GameObject gameObject;
		if (!isPopup)
		{
			GameObject prefab = Resources.Load("Prefabs/screens/" + screenName + this.uiPrefabNameSuffix, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(this.screenAnchor, prefab);
		}
		else
		{
			GameObject prefab2 = Resources.Load("Prefabs/popups/" + screenName + this.uiPrefabNameSuffix, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(this.popupAnchor, prefab2);
		}
		UIBaseScreen component = gameObject.GetComponent<UIBaseScreen>();
		this._cachedScreens.Add(screenName, component);
		component.Init();
		return component;
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform trans2 = (Transform)obj;
				this._PauseAnimations(pause, trans2);
			}
		}
		finally
		{
		}
		CharacterModel component = trans.GetComponent<CharacterModel>();
		if (component != null)
		{
			if (pause)
			{
				component.StopIdleAnimations();
			}
			else if (this.Peek(this._screenStack) != "HelmScreen")
			{
				component.StartIdleAnimations();
			}
		}
	}

	private void _PauseApplication(bool paused)
	{
		if (paused)
		{
			PlayerInfo.Instance.SaveIfDirty();
		}
		else
		{
			this._isApplicationResuming = true;
		}
	}

	private void _SetBackground(bool state)
	{
		string text = "NotebookPanel";
		if (state)
		{
			if (!this._cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
				this._cachedScreens.Add(text, NGUITools.AddChild(this.backgroundAnchor, prefab).GetComponent<UIBaseScreen>());
				this._cachedScreens[text].Init();
			}
			this._cachedScreens[text].Show();
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.enabled = false;
		}
		else
		{
			if (this._cachedScreens.ContainsKey(text))
			{
				this._cachedScreens[text].Hide();
			}
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.enabled = true;
		}
	}

	private void _SetCycleAd(string screenName)
	{
		if (this._screenNamesWithCycleAd.Contains(screenName) && !screenName.Equals(this.lastAdScreenName))
		{
			this.curAdCount++;
			if (this.curAdCount >= this.currentCycleCount)
			{
				this.currentCycleCount = UnityEngine.Random.Range(this.minCycleCount, this.maxCycleCount + 1);
				this.curAdCount = 0;
				if (!PlayerInfo.Instance.hasSubscribed && !Game.Instance.show20sAd && Game.Instance.GetNextAdDuration() > 20f)
				{
					Game.Instance.showAdTime = Time.time;
					RiseSdk.Instance.TrackEvent("interstitial_betweenUI", "default,default");
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_betweenUI", 0, null);
					RiseSdk.Instance.ShowAd("custom");
				}
			}
			this.lastAdScreenName = screenName;
		}
	}

	private void _ShowScreen(string screenName, UIBaseScreen screen)
	{
		this._SetBackground(!this._screenNamesWithoutBackground.Contains(screenName));
		this._SetCycleAd(screenName);
		screen.Show();
		if (screenName == "GameoverUI")
		{
			this._cachedScreens[screenName].GetComponent<GameOverScreen>().SetupBeforeChest();
			if (RewardManager.rewardsToUnlockCount > 0)
			{
				this._QueuePopup("CelebrationPopup");
			}
			else
			{
				this._cachedScreens[screenName].GetComponent<GameOverScreen>().SetupAfterChest();
			}
		}
		Action<string> onChangedScreen = this.OnChangedScreen;
		if (onChangedScreen != null)
		{
			onChangedScreen(screenName);
		}
		this.ScreenDidChange(screenName);
	}

	private void _SwitchScreen(string screenName)
	{
		string key = this.Pop(this._screenStack);
		this._cachedScreens[key].Hide();
		this._ActivateScreen(screenName);
	}

	public void ActivateAnchors(bool active)
	{
		NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.CelebrationPopupAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.PauseScreenAnchor, active);
		this._PauseAnimations(!active, this.MenuElements3D.transform);
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

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return base.StartCoroutine(this.AnimateAlpha(collectText, 0.1f, 1f));
		base.StartCoroutine(this.MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		base.StartCoroutine(this.AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
		yield break;
	}

	public void BackToPrevious()
	{
		this._BackToPreviousScreen();
	}

	public void CloseActivePopups(string popupToStopAt)
	{
		List<string> list = new List<string>();
		if (this._popupQueue.Contains(popupToStopAt))
		{
			int i = 0;
			int count = this._popupQueue.Count;
			while (i < count)
			{
				UIBaseScreen uibaseScreen = this._cachedScreens[this._popupQueue[i]];
				if (uibaseScreen.isActive)
				{
					list.Add(this._popupQueue[i]);
					if (this._popupQueue[i] == popupToStopAt)
					{
						break;
					}
				}
				i++;
			}
		}
		int j = 0;
		int count2 = list.Count;
		while (j < count2)
		{
			this._RemovePopup(list[j]);
			j++;
		}
	}

	public void CloseAllPopups()
	{
		int i = 0;
		int count = this._popupQueue.Count;
		while (i < count)
		{
			this._RemovePopup(this._popupQueue[i]);
			i++;
		}
	}

	private string Dequeue(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
			list.RemoveAt(0);
		}
		return result;
	}

	public void GameOverTriggered()
	{
		if (!this.GetTopScreenName().Equals("GameoverUI"))
		{
			PlayerInfo.Instance.RunCompleted();
			TasksManager.Instance.inRun = false;
			this._ActivateScreen("GameoverUI");
		}
	}

	public string GetCurrentPopupName()
	{
		return this.QueuePeek(this._popupQueue);
	}

	public UIBaseScreen GetScreenFromCache(string screenName)
	{
		if (this._cachedScreens.ContainsKey(screenName))
		{
			return this._cachedScreens[screenName];
		}
		return null;
	}

	public string GetTopScreenName()
	{
		if (this._screenStack != null && this._screenStack.Count > 0)
		{
			return this.Peek(this._screenStack);
		}
		return null;
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		base.StartCoroutine(this.GoToMainMenuFromGame());
	}

	public IEnumerator GoToMainMenuFromGame()
	{
		if (Game.Instance != null)
		{
			TrialManager.Instance.End();
			Game.Instance.ResetTest(false);
			TasksManager.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		yield return null;
		yield return null;
		this._ActivateScreen("FrontUI");
		yield break;
	}

	public bool IsPopupAlreadyQueued(string popupName)
	{
		int i = 0;
		int count = this._popupQueue.Count;
		while (i < count)
		{
			if (this._popupQueue[i] == popupName)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	public bool IsPopupQueueEmpty()
	{
		return this._popupQueue.Count <= 0;
	}

	private void LateUpdate()
	{
		if (this._isApplicationResuming)
		{
			this._isApplicationResuming = false;
			if (this._screenStack.Count > 0 && this.Peek(this._screenStack) == "IngameUI" && (Game.Instance == null || !Game.Instance.isDead))
			{
				this.PushScreen("PauseUI");
			}
			if (UIScreenController.OnApplicationResumed != null)
			{
				UIScreenController.OnApplicationResumed();
			}
		}
	}

	public static void LogError(string msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(msg, context);
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos__0 = trans.localPosition;
		float factor__ = 0f;
		while (factor__ < 1f)
		{
			factor__ += Time.deltaTime / duration;
			factor__ = Mathf.Clamp01(factor__);
			trans.localPosition = Vector3.Lerp(fromPos__0, toPos, factor__);
			yield return null;
		}
		yield break;
	}

	private void OnApplicationFocus(bool focus)
	{
		this._gameIsFocused = focus;
		if (UIScreenController.isGetBtnPressed)
		{
			return;
		}
		this._PauseApplication(!focus);
		if (Application.platform == RuntimePlatform.Android && !focus && this.Peek(this._screenStack) == "IngameUI" && !Game.Instance.isDead)
		{
			Time.timeScale = 0f;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (UIScreenController.isGetBtnPressed)
		{
			return;
		}
		this._PauseApplication(paused);
		if (Application.platform == RuntimePlatform.WP8Player && paused && this.Peek(this._screenStack) == "IngameUI" && !Game.Instance.isDead)
		{
			Time.timeScale = 0f;
		}
	}

	private void OnDestroy()
	{
		if (!this.stoppingFromEditor)
		{
			this.OnChangedScreen = (Action<string>)Delegate.Remove(this.OnChangedScreen, new Action<string>(this.UM_PageChange));
		}
	}

	public void PayoutCelebrationReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			PlayerInfo.Instance.amountOfCoins += reward.amount;
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo.Instance.amountOfKeys += reward.amount;
		}
		RewardManager.RewardPayedOut(reward);
		PlayerInfo.Instance.SaveIfDirty();
	}

	private string Peek(List<string> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return string.Empty;
	}

	private string Pop(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		return result;
	}

	public void PushPopup(string name)
	{
		this._PushPopup(name);
	}

	public void PushScreen(string screenName)
	{
		this._PushScreen(screenName);
	}

	private void _PushScreen(string screenOverride)
	{
		if (string.IsNullOrEmpty(screenOverride))
		{
			return;
		}
		if ((screenOverride != "PauseUI" || (Game.Instance != null && !Game.Instance.isDead)) && (this.Peek(this._screenStack) != "PauseUI" || screenOverride != "IngameUI"))
		{
			this._ActivateScreen(screenOverride);
		}
	}

	public void QueueChest()
	{
		if (this._popupQueue.Count <= 0 || this.QueuePeek(this._popupQueue) != "CelebrationPopup")
		{
			if (this._popupQueue.Count > 0)
			{
				this.PushPopup("CelebrationPopup");
			}
			else
			{
				this._QueuePopup("CelebrationPopup");
			}
		}
	}

	private string QueuePeek(List<string> list)
	{
		string result = string.Empty;
		if (list != null && list.Count > 0)
		{
			result = list[0];
		}
		return result;
	}

	public void QueuePopup(string popupName)
	{
		this._QueuePopup(popupName);
	}

	public void QueuePopupRightAfterCurrentActivePopup(string popupName)
	{
		if (!string.IsNullOrEmpty(this.QueuePeek(this._popupQueue)))
		{
			this._popupQueue.Insert(1, popupName);
		}
		else
		{
			this._QueuePopup(popupName);
		}
	}

	public void RecalculateViewportRectForClipCamera(Camera camera, bool popupSizedClip)
	{
		if (camera != null)
		{
			UICameraScreenClipping component = camera.GetComponent<UICameraScreenClipping>();
			if (component != null)
			{
				component.CalculateClipping(popupSizedClip);
			}
		}
	}

	public void RequeueDelayedPopups()
	{
		int i = 0;
		int count = this._delayedPopups.Count;
		while (i < count)
		{
			this.QueuePopup(this._delayedPopups[i]);
			i++;
		}
		this._delayedPopups.Clear();
	}

	public void SaveAndDelayPopups()
	{
		this.doSaveAndDelayRemainingPopups = false;
		int i = 0;
		int count = this._popupQueue.Count;
		while (i < count)
		{
			this._delayedPopups.Add(this._popupQueue[i]);
			i++;
		}
		this._popupQueue.Clear();
	}

	private void ScreenDidChange(string newScreenName)
	{
		if (newScreenName != "IngameUI")
		{
			this.messageHelper.DisableShowLabel();
		}
	}

	public void UM_PageChange(string screenName)
	{
	}

	public void ShowMainMenu()
	{
		base.StartCoroutine(this.ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		while (!LoadScene.finished)
		{
			yield return null;
		}
		this._ActivateScreen("FrontUI");
		yield break;
	}

	public void ShowUnlockAnimationForCharacter(Characters.CharacterType ctype, int themeIndex = 0)
	{
		RewardManager.AddRewardToUnlock(new CelebrationReward
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock,
			rewardType = CelebrationRewardType.character,
			characterType = ctype,
			characterThemeIndex = themeIndex
		}, true);
		this.QueueChest();
	}

	public void ShowUnlockAnimationForHelmet(Helmets.HelmType helmType)
	{
		RewardManager.AddRewardToUnlock(new CelebrationReward
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.HelmetUnlock,
			rewardType = CelebrationRewardType.specialHelm,
			helmType = helmType
		}, true);
		this.QueueChest();
	}

	public void AddUnlockForCharacterToReward(Characters.CharacterType ctype, int themeIndex = 0)
	{
		RewardManager.AddRewardToUnlock(new CelebrationReward
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock,
			rewardType = CelebrationRewardType.character,
			characterType = ctype,
			characterThemeIndex = themeIndex
		}, true);
	}

	public void AddUnlockForHelmetToReward(Helmets.HelmType helmType)
	{
		RewardManager.AddRewardToUnlock(new CelebrationReward
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.HelmetUnlock,
			rewardType = CelebrationRewardType.specialHelm,
			helmType = helmType
		}, true);
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uilabel = NGUITools.AddWidget<UILabel>(this.superPopupAnchor);
		Utility.SetLayerRecursively(uilabel.gameObject.transform, this.superPopupAnchor.layer);
		uilabel.text = text;
		uilabel.transform.position = new Vector3(startPosition.x, startPosition.y, uilabel.cachedTransform.position.z);
		uilabel.trueTypeFont = this.FloatingTextFont;
		uilabel.fontSize = 20;
		uilabel.supportEncoding = false;
		uilabel.multiLine = false;
		uilabel.keepCrispWhenShrunk = UILabel.Crispness.Always;
		uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
		uilabel.MakePixelPerfect();
		uilabel.color = new Color(0.9803922f, 0.7764706f, 0.2352941f, 0f);
		uilabel.gameObject.AddComponent<UIPanel>().depth = 11;
		base.StartCoroutine(this.AnimateCollectText(uilabel));
	}

	private void Start()
	{
		if (this.LoadMenuOnStart)
		{
			this.ShowMainMenu();
		}
		PlayerInfo.Instance.BragCompleted();
		TaskInfo[] taskInfo = TasksManager.Instance.GetTaskInfo();
		if (taskInfo[0].complete && taskInfo[1].complete && taskInfo[2].complete)
		{
			TasksManager.Instance.currentTaskSet++;
		}
		this.UpdateNguiTouchThresholds();
	}

	public void SwitchScreen(string screenName)
	{
		this._SwitchScreen(screenName);
	}

	private void UpdateNguiTouchThresholds()
	{
		float touchDragThreshold = (Screen.dpi > 0f) ? (0.1f * Screen.dpi) : 30f;
		float touchClickThreshold = (Screen.dpi > 0f) ? (0.1f * Screen.dpi) : 30f;
		this.nguiCamera.touchDragThreshold = touchDragThreshold;
		this.nguiCamera.touchClickThreshold = touchClickThreshold;
	}

	public bool GameIsFocused
	{
		get
		{
			return this._gameIsFocused;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			if (UIScreenController._instance == null)
			{
				UIScreenController._instance = (UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
			}
			return UIScreenController._instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			if (UIScreenController._instance == null)
			{
				UIScreenController._instance = (UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
			}
			return UIScreenController._instance != null;
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return this._popupQueue != null && this._popupQueue.Count > 0;
		}
	}

	public GameObject backgroundAnchor;

	public GameObject screenAnchor;

	public GameObject popupAnchor;

	public GameObject superPopupAnchor;

	public UICamera nguiCamera;

	public Camera CameraOverlay2d;

	public UIRoot root;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public Font FloatingTextFont;

	public Action<string> OnChangedScreen;

	[SerializeField]
	private Font lilitaRegularFont;

	[SerializeField]
	private Font titanRegularFont;

	public string text;

	public int size;

	public FontStyle style;

	public bool stoppingFromEditor;

	public bool doSaveAndDelayRemainingPopups;

	public bool ignoreLastPopupClosed;

	public UIMessageHelper messageHelper;

	public static bool isGetBtnPressed;

	[SerializeField]
	private int minCycleCount = 6;

	[SerializeField]
	private int maxCycleCount = 9;

	private Dictionary<string, UIBaseScreen> _cachedScreens = new Dictionary<string, UIBaseScreen>();

	private List<string> _delayedPopups = new List<string>();

	private bool _gameIsFocused = true;

	private static UIScreenController _instance;

	private bool _isApplicationResuming;

	private bool _popupActive;

	private List<string> _popupQueue = new List<string>();

	private List<string> _screenNamesWithoutBackground;

	private List<string> _screenNamesWithCycleDebug;

	private List<string> _screenNamesWithCycleAd;

	private List<string> _screenStack = new List<string>();

	private string lastAdScreenName;

	private int curAdCount;

	private int currentCycleCount;

	private int lastPopupNameIndex = -1;

	private Camera mainCamera;

	public string uiPrefabNameSuffix = string.Empty;

	public const string iPadNameSuffix = "_ipad";

	public const string iPhoneXNameSuffix = "_iphoneX";

	public float bannerHeight;

	public UIScreenController.DeviceType curDeviceType;

	public enum DeviceType
	{
		Android,
		iPhone,
		iPad,
		iPhoneX
	}
}
