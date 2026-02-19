using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Kiloo.Common;
using Network;
using UnityEngine;

public class PlayerInfo
{
	private PlayerInfo()
	{
		this._upgradeAmounts = new Dictionary<PropType, int>
		{
			{
				PropType.helmet,
				0
			},
			{
				PropType.headstart500,
				0
			},
			{
				PropType.headstart2000,
				0
			},
			{
				PropType.chest,
				0
			},
			{
				PropType.scorebooster,
				0
			}
		};
		this._upgradeTiers = new Dictionary<PropType, int>
		{
			{
				PropType.flypack,
				0
			},
			{
				PropType.supershoes,
				0
			},
			{
				PropType.coinmagnet,
				0
			},
			{
				PropType.doubleMultiplier,
				0
			}
		};
		this._inappHistory = new Dictionary<string, string>();
		this._gameOverDoubleConfirmNoRemind = false;
		this._shouldShowDailyLandingPopup = false;
		this._hasShowDailyLandingPopup = false;
		this._collectedCharacterTokens = new int[Characters.characterData.Count];
		this._topRunData = new TopRunData();
		this.Load();
	}

	public void AddPendingReward(CelebrationReward reward)
	{
		this._lastAddedReward = reward;
		bool flag = false;
		int num = -1;
		for (int i = 0; i < this._pendingRewards.Count; i++)
		{
			if (this._pendingRewards[i].Uid == reward.Uid)
			{
				flag = true;
			}
			if (this._pendingRewards[i].CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore && num == -1)
			{
				num = i;
			}
		}
		if (!flag)
		{
			if (num > -1)
			{
				if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
				{
					this._pendingRewards[num] = reward;
				}
				else
				{
					this._pendingRewards.Insert(num, reward);
				}
			}
			else
			{
				this._pendingRewards.Add(reward);
			}
			this._dirty = true;
		}
	}

	public void RemovePendingReward(CelebrationReward reward)
	{
		CelebrationReward celebrationReward = this._pendingRewards.Find(new Predicate<CelebrationReward>(reward.Find));
		if (celebrationReward == null)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest)
			{
				celebrationReward = this._pendingRewards.Find(new Predicate<CelebrationReward>(this.FindChest));
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest)
			{
				celebrationReward = this._pendingRewards.Find(new Predicate<CelebrationReward>(this.FindSuperChest));
			}
		}
		if (celebrationReward != null)
		{
			this._pendingRewards.Remove(celebrationReward);
			if (this._lastAddedReward != null && celebrationReward.Uid == this._lastAddedReward.Uid)
			{
				this._lastAddedReward = null;
			}
			this._dirty = true;
		}
	}

	private bool FindChest(CelebrationReward mbr)
	{
		return mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest;
	}

	private bool FindSuperChest(CelebrationReward mbr)
	{
		return mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest;
	}

	public void AddSaveGemToUnlock()
	{
		this.amountOfKeys++;
	}

	public bool AddTransactionToHistory(string orderId, string itemId)
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WP8Player)
		{
			return true;
		}
		if (!this._inappHistory.ContainsKey(orderId))
		{
			this._inappHistory.Add(orderId, itemId);
			this._dirty = true;
			return true;
		}
		return false;
	}

	public void BragCompleted()
	{
		this.ResetHighestScoreTo(this.highestScore);
	}

	public void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
	{
		bool flag;
		if (PlayerInfo.Instance.GetDailyLandingDaysInRow(out flag) == 0 && !flag)
		{
			TasksManager.Instance.RemoveProgressForThis(TaskTarget.DailyQuestInRow);
		}
	}

	public void CollectSymbol(Characters.CharacterType characterType, int amount)
	{
		this._collectedCharacterTokens[(int)characterType] += amount;
		this._dirty = true;
		Action<Characters.CharacterType> onSymbolCollected = this.OnSymbolCollected;
		if (onSymbolCollected != null)
		{
			onSymbolCollected(characterType);
		}
	}

	public void DecreaseSymbol(Characters.CharacterType characterType, int amount)
	{
		this._collectedCharacterTokens[(int)characterType] -= amount;
		if (this._collectedCharacterTokens[(int)characterType] < 0)
		{
			this._collectedCharacterTokens[(int)characterType] = 0;
		}
	}

	public int GetCollectedSymbols(Characters.CharacterType ModelType)
	{
		return this._collectedCharacterTokens[(int)ModelType];
	}

	public int GetCurrentTaskProgress(int task)
	{
		if (task >= 3)
		{
			return this._achievementProgress[task - 3];
		}
		if (this._currentTaskProgress != null && task < this._currentTaskProgress.Length)
		{
			return this._currentTaskProgress[task];
		}
		return 0;
	}

	public int GetCurrentTier(PropType type)
	{
		if (!this._upgradeTiers.ContainsKey(type))
		{
			return 0;
		}
		return this._upgradeTiers[type];
	}

	public float GetHelmCoolDown()
	{
		return 5f;
	}

	public int GetIndexForLastSelectedTheme(Characters.CharacterType character)
	{
		int num;
		if (this._lastSelectedThemes.TryGetValue(character, out num))
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && list.Count >= num)
			{
				return num;
			}
		}
		return 0;
	}

	public float GetPowerupDuration(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.durations[this.GetCurrentTier(type)];
	}

	public float GetPowerupLandSpeed(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.landSpeed;
	}

	public float GetPowerupSpeed(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.speed;
	}

	private static List<CelebrationReward> GetRewardsFromString(string rewardsAsString)
	{
		List<CelebrationReward> list = new List<CelebrationReward>();
		if (!string.IsNullOrEmpty(rewardsAsString))
		{
			char[] separator = new char[]
			{
				';'
			};
			foreach (string text in rewardsAsString.Split(separator))
			{
				if (!string.IsNullOrEmpty(text))
				{
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.PopulateFromString(text);
					list.Add(celebrationReward);
				}
			}
		}
		return list;
	}

	private static string GetSavePath()
	{
		return Globals.GetUserDataPath() + "/playerdata";
	}

	private static string GetStringFromRewards(List<CelebrationReward> rewards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (CelebrationReward value in rewards)
		{
			stringBuilder.Append(value).Append(";");
		}
		int num = stringBuilder.ToString().LastIndexOf(';');
		if (num >= 0)
		{
			stringBuilder.Remove(num, 1);
		}
		return stringBuilder.ToString();
	}

	public int GetUpgradeAmount(PropType type)
	{
		return this._upgradeAmounts[type];
	}

	public void SetUpgradeAmount(PropType type, int num)
	{
		this._upgradeAmounts[type] = num;
	}

	public int GetUpgradeTierSum()
	{
		return this.GetCurrentTier(PropType.flypack) + this.GetCurrentTier(PropType.doubleMultiplier) + this.GetCurrentTier(PropType.coinmagnet) + this.GetCurrentTier(PropType.supershoes);
	}

	public bool HasHelmetBeenSeen(Helmets.HelmType helmType)
	{
		Helmets.Helm helm = Helmets.helmData[helmType];
		return this._hasHelmetBeenSeen.ContainsKey(helmType) && this._hasHelmetBeenSeen[helmType];
	}

	public void IncreasePowerupTier(PropType type)
	{
		if (this._upgradeTiers.ContainsKey(type))
		{
			Dictionary<PropType, int> upgradeTiers= this._upgradeTiers;
			(upgradeTiers )[type] = upgradeTiers[type] + 1;
			this._dirty = true;
		}
		else
		{
			PlayerInfo.LogError("Trying to increase tier for a non-tiered upgrade", null);
		}
	}

	public void IncreaseUpgradeAmount(PropType type, int amount = 1)
	{
		if (this._upgradeAmounts.ContainsKey(type))
		{
			Dictionary<PropType, int> upgradeAmounts= this._upgradeAmounts;
			(upgradeAmounts )[type] = upgradeAmounts[type] + amount;
			this._dirty = true;
			Action action = this.onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
		}
		else
		{
			PlayerInfo.LogError("Trying to increase upgrade amount for a non-consumable", null);
		}
	}

	public bool IncrementCurrentTaskProgress(int task, int target)
	{
		if (this._currentTaskProgress[task] < target)
		{
			this._currentTaskProgress[task]++;
			this._dirty = true;
			return this._currentTaskProgress[task] == target;
		}
		return false;
	}

	public void InitCurrentTaskSet(int taskSet, int taskCount, bool resetProgress)
	{
		if (taskSet != this._currentTaskSet)
		{
			this._currentTaskSet = taskSet;
			if (resetProgress)
			{
				this._currentTaskProgress = new int[taskCount];
				for (int i = 0; i < taskCount; i++)
				{
					this._currentTaskProgress[i] = 0;
				}
			}
			this._dirty = true;
			this.TriggerOnScoreMultiplierChanged();
		}
	}

	private void InitDailyLanding()
	{
		this._dailyLandingPayedOut = false;
	}

	public int GetDailyLandingDaysInRow(out bool Istoday)
	{
		int dayOfYear = DateTime.Now.DayOfYear;
		if (this._dailyLandingLastPayoutDayOfYear == dayOfYear)
		{
			Istoday = true;
			return this._dailyLandingInRow;
		}
		if (this._dailyLandingLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && DateTime.Now.AddDays(-1.0).DayOfYear == this._dailyLandingLastPayoutDayOfYear))
		{
			Istoday = false;
			this.InitDailyLanding();
			this._dirty = true;
			return this._dailyLandingInRow;
		}
		Istoday = false;
		this._dailyLandingInRow = 0;
		this.InitDailyLanding();
		this._dirty = true;
		return 0;
	}

	public void ReceiveDailyLandingPayout(int multiple, Action callback)
	{
		if (!this._dailyLandingPayedOut)
		{
			DailyLandingAward dailyLandingAward = DailyLandingAwards.awards[this._dailyLandingInRow];
			dailyLandingAward.Amount *= multiple;
			this._dailyLandingLastPayoutDayOfYear = DateTime.Now.DayOfYear;
			this._dailyLandingInRow++;
			this._dailyLandingPayedOut = true;
			FreeRewardManager.Instance.SetFreeRewardType(dailyLandingAward, callback);
			NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.DialyLandingReward);
			TasksManager.Instance.PlayerDidThis(TaskTarget.DailyQuests, 1, -1);
			TasksManager.Instance.PlayerDidThis(TaskTarget.DailyQuestInRow, 1, -1);
		}
	}

	public bool DailyLandingPayOut()
	{
		return !this._dailyLandingPayedOut;
	}

	public void InitNew()
	{
		this.amountOfCoins = 0;
		this._highestScore = 0;
		this._hasFacebookLogin = false;
		this._dailyLandingInRow = 0;
		this._dailyLandingLastPayoutDayOfYear = 0;
		this._dailyLandingPayedOut = false;
		this._amountOfChestesOpened = 0;
		this._amountOfMiniChestesOpened = 0;
		this._amountOfSuperChestesOpened = 0;
		this._hasReceivedFirstMBKey = false;
		this._hasReceivedFirstSMBKey = false;
		this._lotteryWatchViewRemainCount = 3;
		this._lastLotteryFreeViewDateTime = DateTime.UtcNow;
		this._lastPlayDate = DateTime.UtcNow;
		this._menuSliderShow = true;
		this.amountOfKeys = 5;
		this._currentCharacter = 0;
		this._currentTaskSet = -1;
		this._currentTaskProgress = null;
		this._achievementProgress = new int[Achievements.NUMBER_OF_ACHIEVEMENTS];
		this._achievementAwardPayedOut = new bool[Achievements.NUMBER_OF_ACHIEVEMENTS];
		this._onlineRewardPayedOut = new bool[4];
		this._onlineTotalSeconds = 0;
		this._openAppInNewDayStartDateTime = DateTime.UtcNow;
		this._tutorialCompleted = false;
		this._hasSubscribed = false;
		this._hasDoubleCoins = false;
		this._wallWalkTutorialCount = new Dictionary<string, int>();
		this._numberOfRunsSinceLastGuideline = 0;
		this._gameOverFullAdCount = 0;
		this._gameOverFullAdLastDate = DateTime.Today;
		this._ignoreTrialRoleNextTime = DateTime.UtcNow;
		this._autoShowChangePlayerName = true;
		this._firstGameOverNoRemind = true;
		this._shouldShownPlayerMenuPopup = false;
		this._hasShownTask1Popup = false;
		this._shouldShowTask1Popup = false;
		this._hasShownPlayerMenuPopup = false;
		this._ignoreTrailRolePopup = false;
		this._ignoreSubscriptionPopup = false;
		this._ignoreSubscriptionNextTime = DateTime.Today;
		this._gameoverUITryCount = 3;
		this._gameoverUITryNextTime = DateTime.UtcNow;
		this._numberOfRuns = 0;
		this._firstInstallDate = DateTime.UtcNow;
		this._updateFromLastApp = false;
		this._updateRewardIndex = 0;
		this._playOnceTimes = 0;
		this._playDailyTimes = 0;
		this._updateDateTime = DateTime.UtcNow;
		this._openAppCountDaily = 0;
		this._firstCheckVideoPlayerTime = DateTime.UtcNow;
		this._lastOpenGameDateTime = DateTime.UtcNow;
		this._watchVideoSuccessNum = 0;
		this._amountOfOpenGameApp = 0;
		this._isNewPlayer = false;
		this._currentTrialProgress = new int[3];
		this._currentTrialIndex = -1;
		this._totalTrialDays = 0;
		for (int i = 0; i < this._collectedCharacterTokens.Length; i++)
		{
			this._collectedCharacterTokens[i] = 0;
		}
		Dictionary<PropType, int> dictionary = new Dictionary<PropType, int>(this._upgradeAmounts.Count);
		foreach (PropType propType in this._upgradeAmounts.Keys)
		{
			if (propType == PropType.helmet)
			{
				dictionary[propType] = 3;
			}
			else
			{
				dictionary[propType] = 0;
			}
		}
		this._upgradeAmounts = dictionary;
		dictionary = new Dictionary<PropType, int>(this._upgradeTiers.Count);
		foreach (PropType key in this._upgradeTiers.Keys)
		{
			dictionary[key] = 0;
		}
		this._upgradeTiers = dictionary;
		this._taskCompletedSum = 0;
		this._hasHelmetBeenSeen = new Dictionary<Helmets.HelmType, bool>();
		this._helmetUnlockStatus = new Dictionary<Helmets.HelmType, bool>();
		this._inappHistory = new Dictionary<string, string>();
		this._currentHelmet = Helmets.HelmType.normal;
		this._hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();
		this._characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();
		this._lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();
		this._characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();
		this._pendingRewards = new List<CelebrationReward>();
		this._showTrialPopupCount = 0;
		this._gameOverDoubleConfirmNoRemind = false;
		this._showWatchVideoPopupCount = 0;
		this._freeUpgradeCount = 3;
		this._lastShowFreeUpgradeDate = DateTime.UtcNow;
		this._donotClickGameOverDoubleCoinViewCount = 0;
		this._gameOverDoubleCoinsShowCountOneDay = 0;
		this._gameOverDoubleCoinsShowCountTwoDay = 0;
		this._gameOverDoubleCoinsShowCountLastDay = 0;
		this._lastGameOverDoubleCoinsDateTime = DateTime.UtcNow;
		this._watchDoublePlayerLevel = 0;
		this._playerLevel = 0;
		this._gameOverDoubleCoinViewCycleCount = 0;
		this._lastGameOverDoubleCoinViewDate = DateTime.UtcNow;
		this._stats = new Statistics();
	}

	public bool isCharacterActive(Characters.CharacterType characterType)
	{
		return true;
	}

	public bool IsCollectionComplete(Characters.CharacterType characterType)
	{
		Characters.Model model = Characters.characterData[characterType];
		return model.unlockType == Characters.UnlockType.free || (model.unlockType == Characters.UnlockType.subscription && this._hasSubscribed) || (model.unlockType != Characters.UnlockType.subscription && this.GetCollectedSymbols(characterType) >= model.Price);
	}

	public bool CanIncreasePowerup()
	{
		bool flag = false;
		foreach (KeyValuePair<PropType, int> keyValuePair in this._upgradeTiers)
		{
			if (keyValuePair.Value < 3)
			{
				flag = true;
				break;
			}
		}
		if (flag && this.CheckIfFreeUpgrade())
		{
			return true;
		}
		foreach (KeyValuePair<PropType, int> keyValuePair2 in this._upgradeTiers)
		{
			if (Upgrades.upgrades[keyValuePair2.Key].getPrice(keyValuePair2.Value + 1) < this._amountOfCoins && Upgrades.upgrades[keyValuePair2.Key].getPrice(keyValuePair2.Value + 1) > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanUnlockHelm()
	{
		int i = 1;
		int count = Helmets.helmData.Count;
		while (i < count)
		{
			Helmets.HelmType helmType = (Helmets.HelmType)i;
			Helmets.Helm helm = Helmets.helmData[helmType];
			if (!HelmetManager.Instance.isHelmetUnlocked(helmType))
			{
				if (helm.unlockType == Helmets.UnlockType.coins && helm.price <= this._amountOfCoins)
				{
					return true;
				}
				if (helm.unlockType == Helmets.UnlockType.keys && helm.price <= this._amountOfKeys)
				{
					return true;
				}
			}
			i++;
		}
		return false;
	}

	public bool CanUnlockCharacter()
	{
		int i = 0;
		int count = Characters.characterData.Count;
		while (i < count)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)i;
			Characters.Model model = Characters.characterData[characterType];
			if (model.unlockType != Characters.UnlockType.free && model.unlockType != Characters.UnlockType.subscription && this.GetCollectedSymbols(characterType) < model.Price)
			{
				if (model.unlockType == Characters.UnlockType.coins && model.Price <= this._amountOfCoins)
				{
					return true;
				}
				if (model.unlockType == Characters.UnlockType.keys && model.Price <= this._amountOfKeys)
				{
					return true;
				}
			}
			i++;
		}
		return false;
	}

	public bool CanUnlockCharacterOrCTheme()
	{
		int i = 0;
		int count = Characters.characterData.Count;
		while (i < count)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)i;
			Characters.Model model = Characters.characterData[characterType];
			if (model.unlockType != Characters.UnlockType.free && model.unlockType != Characters.UnlockType.subscription && this.GetCollectedSymbols(characterType) < model.Price)
			{
				if (model.unlockType == Characters.UnlockType.coins && model.Price <= this._amountOfCoins)
				{
					return true;
				}
				if (model.unlockType == Characters.UnlockType.keys && model.Price <= this._amountOfKeys)
				{
					return true;
				}
			}
			i++;
		}
		int j = 0;
		int count2 = Characters.characterData.Count;
		while (j < count2)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)j;
			List<CharacterTheme> list = CharacterThemes.characterCustomThemes[characterType];
			if (list != null && list.Count > 0)
			{
				int[] array = this._characterThemesUnlocked[characterType];
				int k = 0;
				int count3 = list.Count;
				while (k < count3)
				{
					bool flag = false;
					int l = 0;
					int num = array.Length;
					while (l < num)
					{
						if (array[l] == k + 1)
						{
							flag = true;
							break;
						}
						l++;
					}
					if (!flag)
					{
						if (list[k].unlockType == Characters.UnlockType.coins && list[k].price <= this._amountOfCoins)
						{
							return true;
						}
						if (list[k].unlockType == Characters.UnlockType.keys && list[k].price <= this._amountOfKeys)
						{
							return true;
						}
					}
					k++;
				}
			}
			j++;
		}
		return false;
	}

	public bool IsNewUser()
	{
		return this.rawMultiplier <= 1;
	}

	public bool IsThemeSeenForCharacter(Characters.CharacterType character, int index)
	{
		if (index == 0)
		{
			return false;
		}
		int[] array;
		this._characterThemesSeen.TryGetValue(character, out array);
		if (array == null)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == index)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsThemeUnlockedForCharacter(Characters.CharacterType character, int index)
	{
		Characters.Model model = Characters.characterData[character];
		if (model.unlockType == Characters.UnlockType.subscription)
		{
			return this._hasSubscribed;
		}
		if (index == 0)
		{
			return true;
		}
		int[] array;
		this._characterThemesUnlocked.TryGetValue(character, out array);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == index)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int HasThemeUnlockedForCharactersNum()
	{
		int num = 0;
		if (this._characterThemesUnlocked.Count <= 0)
		{
			return num;
		}
		List<Characters.CharacterType> list = new List<Characters.CharacterType>(this._characterThemesUnlocked.Keys);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			Characters.CharacterType key = list[i];
			int[] array = this._characterThemesUnlocked[key];
			if (array != null)
			{
				num += array.Length;
			}
		}
		list = new List<Characters.CharacterType>(Characters.characterData.Keys);
		for (int j = list.Count - 1; j >= 0; j--)
		{
			Characters.CharacterType characterType = list[j];
			if (this.IsCollectionComplete(characterType))
			{
				num++;
			}
		}
		return num;
	}

	public bool IsSymbolUseful(Characters.CharacterType characterType)
	{
		Characters.Model model = Characters.characterData[characterType];
		return this.GetCollectedSymbols(characterType) < model.Price;
	}

	public void Load()
	{
		if ((Application.platform == RuntimePlatform.WebGLPlayer))
		{
			this.InitNew();
		}
		else
		{
			try
			{
				string path;
				string alternatePath;
				PlayerInfo.TryGetLoadPaths(out path, out alternatePath);
				MemoryStream memoryStream = new MemoryStream(FileUtil.Load(path, "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", alternatePath, true));
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				binaryReader.ReadInt32();
				Dictionary<PlayerInfo.Key, string> dict = FileUtil.ReadEnumStringDictionary<PlayerInfo.Key>(binaryReader);
				this.amountOfCoins = this.LoadInt(dict, PlayerInfo.Key.AmountOfCoins, 0);
				this.ResetHighestScoreTo(this.LoadInt(dict, PlayerInfo.Key.HighestScore, 0));
				this._dailyLandingPayedOut = this.LoadBool(dict, PlayerInfo.Key.DailyLandingPayedOut, false);
				this._currentCharacter = this.LoadInt(dict, PlayerInfo.Key.CurrentCharacter, 0);
				this._currentTaskSet = this.LoadInt(dict, PlayerInfo.Key.CurrentMissionSet, -1);
				this._amountOfChestesOpened = this.LoadInt(dict, PlayerInfo.Key.AmountOfMysteryBoxesOpened, 0);
				this._amountOfSuperChestesOpened = this.LoadInt(dict, PlayerInfo.Key.AmountOfSuperMysteryBoxesOpened, 0);
				this._hasReceivedFirstMBKey = this.LoadBool(dict, PlayerInfo.Key.HasFirstMBKey, false);
				this._lotteryWatchViewRemainCount = this.LoadInt(dict, PlayerInfo.Key.LotteryRemainCount, 3);
				this._lastLotteryFreeViewDateTime = this.LoadDateTime(dict, PlayerInfo.Key.LotteryLastDateTime, DateTime.UtcNow);
				this._lastPlayDate = this.LoadDateTime(dict, PlayerInfo.Key.LastPlayTime, DateTime.UtcNow);
				this._hasReceivedFirstSMBKey = this.LoadBool(dict, PlayerInfo.Key.HasFirstSMBKey, false);
				this._menuSliderShow = this.LoadBool(dict, PlayerInfo.Key.MenuSliderShow, true);
				this._gameOverFullAdCount = this.LoadInt(dict, PlayerInfo.Key.GameOverFullAdCount, 0);
				this._gameOverFullAdLastDate = this.LoadDateTime(dict, PlayerInfo.Key.GameOverFullAdLastDate, DateTime.Today);
				this._ignoreTrialRoleNextTime = this.LoadDateTime(dict, PlayerInfo.Key.IgnoreTrailRoleNextTime, DateTime.UtcNow);
				this._tutorialCompleted = this.LoadBool(dict, PlayerInfo.Key.TutorialCompleted, false);
				this._hasSubscribed = this.LoadBool(dict, PlayerInfo.Key.HasSubscribed, false);
				this._hasDoubleCoins = this.LoadBool(dict, PlayerInfo.Key.DoubleCoins, false);
				this._numberOfRunsSinceLastGuideline = this.LoadInt(dict, PlayerInfo.Key.NumberOfRunsSinceLastGuideline, 0);
				this._autoShowChangePlayerName = this.LoadBool(dict, PlayerInfo.Key.AutoShowChangePlayerName, true);
				this._hasFacebookLogin = this.LoadBool(dict, PlayerInfo.Key.HasFacebookLogin, false);
				this._firstGameOverNoRemind = this.LoadBool(dict, PlayerInfo.Key.FirstGameOverNoRemind, true);
				this._hasShownTask1Popup = this.LoadBool(dict, PlayerInfo.Key.HasShownMission1Popup, false);
				this._shouldShowTask1Popup = this.LoadBool(dict, PlayerInfo.Key.ShouldShowMission1Popup, false);
				this._shouldShownPlayerMenuPopup = this.LoadBool(dict, PlayerInfo.Key.ShouldShownPlayerMenuPopup, false);
				this._ignoreTrailRolePopup = this.LoadBool(dict, PlayerInfo.Key.IgnoreTrailRolePopup, false);
				this._ignoreSubscriptionPopup = this.LoadBool(dict, PlayerInfo.Key.IgnoreSubscriptionPopup, false);
				this._ignoreSubscriptionNextTime = this.LoadDateTime(dict, PlayerInfo.Key.IgnoreSubscriptionNextTime, DateTime.UtcNow);
				this._gameoverUITryCount = this.LoadInt(dict, PlayerInfo.Key.GameOverUITryCount, 3);
				this._gameoverUITryNextTime = this.LoadDateTime(dict, PlayerInfo.Key.GameOverUITryNextTime, DateTime.UtcNow);
				this._gameOverDoubleCoinsShowCountOneDay = this.LoadInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountOneDay, 0);
				this._gameOverDoubleCoinsShowCountTwoDay = this.LoadInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountTwoDay, 0);
				this._gameOverDoubleCoinsShowCountLastDay = this.LoadInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountLastDay, 0);
				this._watchDoublePlayerLevel = this.LoadInt(dict, PlayerInfo.Key.WatchDoublePlayerLevel, 0);
				this._playerLevel = this.LoadInt(dict, PlayerInfo.Key.PlayerLevel, 0);
				this._hasShownKeysPopup = this.LoadBool(dict, PlayerInfo.Key.HasShownKeysPopup, false);
				this._hasShownPlayerMenuPopup = this.LoadBool(dict, PlayerInfo.Key.HasShownPlayerMenuPopup, false);
				this.amountOfKeys = this.LoadInt(dict, PlayerInfo.Key.AmountOfKeys, 5);
				this._isNewPlayer = this.LoadBool(dict, PlayerInfo.Key.IsNewPlayer, false);
				this._gameOverDoubleCoinViewCycleCount = this.LoadInt(dict, PlayerInfo.Key.AchievementProgress, 0);
				this._donotClickGameOverDoubleCoinViewCount = this.LoadInt(dict, PlayerInfo.Key.GameOverDoubleCoinViewCycleCount, 0);
				this._topRunData.Parse(this.LoadString(dict, PlayerInfo.Key.TopRunData, this._topRunData.ToJson()));
				this._lastGameOverDoubleCoinViewDate = this.LoadDateTime(dict, PlayerInfo.Key.LastGameOverDoubleCoinViewDate, DateTime.UtcNow);
				this._pendingRewards = PlayerInfo.GetRewardsFromString(this.LoadString(dict, PlayerInfo.Key.PendingRewards, string.Empty));
				this._gameOverDoubleConfirmNoRemind = this.LoadBool(dict, PlayerInfo.Key.GameOverDoubleConfirnNoRemind, false);
				this._lastShowFreeUpgradeDate = this.LoadDateTime(dict, PlayerInfo.Key.LastShowFreeUpgradeDate, DateTime.UtcNow);
				this._firstCheckVideoPlayerTime = this.LoadDateTime(dict, PlayerInfo.Key.FirstCheckVideoPlayerTime, DateTime.UtcNow);
				this._lastOpenGameDateTime = this.LoadDateTime(dict, PlayerInfo.Key.LastOpenGameDateTime, DateTime.UtcNow);
				this._watchVideoSuccessNum = this.LoadInt(dict, PlayerInfo.Key.WatchVideoSuccessNum, 0);
				this._amountOfOpenGameApp = this.LoadInt(dict, PlayerInfo.Key.AmountOfOpenGameApp, 0);
				this._freeUpgradeCount = this.LoadInt(dict, PlayerInfo.Key.FreeUpgradeCount, 3);
				this._showTrialPopupCount = this.LoadInt(dict, PlayerInfo.Key.ShowTrialPopupCount, 0);
				this._firstInstallDate = this.LoadDateTime(dict, PlayerInfo.Key.FirstInstallDate, DateTime.UtcNow);
				this._updateRewardIndex = this.LoadInt(dict, PlayerInfo.Key.UpdateRewardIndex, 0);
				this._updateFromLastApp = this.LoadBool(dict, PlayerInfo.Key.UpdateFromLastApp, false);
				this._playOnceTimes = this.LoadInt(dict, PlayerInfo.Key.PlayOnceTimes, 0);
				this._playDailyTimes = this.LoadInt(dict, PlayerInfo.Key.PlayDailyTimes, 0);
				this._updateDateTime = this.LoadDateTime(dict, PlayerInfo.Key.UpdateDateTime, DateTime.UtcNow);
				this._openAppCountDaily = this.LoadInt(dict, PlayerInfo.Key.OpenAppCountDaily, 0);
				this._showWatchVideoPopupCount = this.LoadInt(dict, PlayerInfo.Key.ShowWatchVideoPopupCount, 0);
				this._numberOfRuns = this.LoadInt(dict, PlayerInfo.Key.NumberOfRuns, 0);
				this._stats = Statistics.Parse(this.LoadString(dict, PlayerInfo.Key.Stats, string.Empty));
				this._currentTaskProgress = this.LoadIntArray(dict, PlayerInfo.Key.CurrentMissionSetProgress, null);
				this._achievementProgress = this.LoadIntArray(dict, PlayerInfo.Key.AchievementProgress, new int[Achievements.NUMBER_OF_ACHIEVEMENTS]);
				this._achievementAwardPayedOut = this.LoadBoolArray(dict, PlayerInfo.Key.AchievementRewardPayedOut, new bool[Achievements.NUMBER_OF_ACHIEVEMENTS]);
				this._openAppInNewDayStartDateTime = this.LoadDateTime(dict, PlayerInfo.Key.OpenAppInNewDayStartDateTime, DateTime.UtcNow);
				this._onlineTotalSeconds = this.LoadInt(dict, PlayerInfo.Key.OnlineTotalSeconds, 0);
				this._onlineRewardPayedOut = this.LoadBoolArray(dict, PlayerInfo.Key.OnlineZonePayedOut, new bool[4]);
				this._currentTrialProgress = this.LoadIntArray(dict, PlayerInfo.Key.CurrentTrialProgress, new int[3]);
				this._currentTrialIndex = this.LoadInt(dict, PlayerInfo.Key.CurrentTrialIndex, -1);
				this._totalTrialDays = this.LoadInt(dict, PlayerInfo.Key.TotalTrialDays, 0);
				this._amountOfMiniChestesOpened = this.LoadInt(dict, PlayerInfo.Key.AmountOfMiniMysteryBoxesOpened, 0);
				this._dailyLandingInRow = this.LoadInt(dict, PlayerInfo.Key.DailyLandingInRow, 0);
				this._dailyLandingLastPayoutDayOfYear = this.LoadInt(dict, PlayerInfo.Key.DailyLandingLastPayoutDayOfYear, 0);
				this._taskCompletedSum = this.LoadInt(dict, PlayerInfo.Key.MissionCompletedSum, 0);
				int[] array = this.LoadIntArray(dict, PlayerInfo.Key.CollectedCharacterTokens, null);
				for (int i = 0; i < this._collectedCharacterTokens.Length; i++)
				{
					this._collectedCharacterTokens[i] = 0;
				}
				if (array != null)
				{
					int length = Mathf.Min(array.Length, this._collectedCharacterTokens.Length);
					Array.Copy(array, this._collectedCharacterTokens, length);
				}
				this._hasHelmetBeenSeen = Globals.convertStringToEnumBoolDictionary<Helmets.HelmType>(this.LoadString(dict, PlayerInfo.Key.HasHoverboardsBeenSeen, string.Empty));
				string sourceString = this.LoadString(dict, PlayerInfo.Key.UnlockedHoverboardTypes, string.Empty);
				this._helmetUnlockStatus = Globals.convertStringToEnumBoolDictionary<Helmets.HelmType>(sourceString);
				this._inappHistory = Globals.convertStringToStringStringDictionary(this.LoadString(dict, PlayerInfo.Key.HistoryOfInapp, string.Empty));
				this._currentHelmet = this.LoadEnum<Helmets.HelmType>(dict, PlayerInfo.Key.CurrentHoverboard, Helmets.HelmType.normal);
				this._hasCharacterBeenSeen = Globals.convertStringToEnumBoolDictionary<Characters.CharacterType>(this.LoadString(dict, PlayerInfo.Key.HasCharacterBeenSeen, string.Empty));
				this._characterThemesUnlocked = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(this.LoadString(dict, PlayerInfo.Key.CharacterThemesUnlocked, string.Empty));
				this._lastSelectedThemes = Globals.convertStringToEnumIntDictionary<Characters.CharacterType>(this.LoadString(dict, PlayerInfo.Key.CharacterLastSelectedThemes, string.Empty));
				this._characterThemesSeen = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(this.LoadString(dict, PlayerInfo.Key.CharacterThemesSeen, string.Empty));
				this._wallWalkTutorialCount = Globals.convertStringToStringIntDictionary(this.LoadString(dict, PlayerInfo.Key.WallWalkTutorialCount, string.Empty));
				foreach (KeyValuePair<PropType, int> keyValuePair in FileUtil.ReadEnumIntDictionary<PropType>(binaryReader))
				{
					this._upgradeAmounts[keyValuePair.Key] = keyValuePair.Value;
				}
				foreach (KeyValuePair<PropType, int> keyValuePair2 in FileUtil.ReadEnumIntDictionary<PropType>(binaryReader))
				{
					if (this._upgradeTiers.ContainsKey(keyValuePair2.Key))
					{
						this._upgradeTiers[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
				memoryStream.Close();
				this._dirty = false;
			}
			catch
			{
				this.InitNew();
			}
		}
	}

	private bool LoadBool(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, bool defaultValue)
	{
		string value;
		bool result;
		if (dict.TryGetValue(key, out value) && bool.TryParse(value, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private bool[] LoadBoolArray(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, bool[] defaultValue)
	{
		string text;
		if (dict.TryGetValue(key, out text) && !string.IsNullOrEmpty(text))
		{
			char[] separator = new char[]
			{
				','
			};
			return Globals.convertAllStringToBool(text.Split(separator));
		}
		return defaultValue;
	}

	private DateTime LoadDateTime(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, DateTime defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value))
		{
			return Utils.StringToDateTime(value, defaultValue);
		}
		return defaultValue;
	}

	private T LoadEnum<T>(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, T defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value))
		{
			try
			{
				return (T)((object)Enum.Parse(typeof(T), value));
			}
			catch (Exception)
			{
			}
			return defaultValue;
		}
		return defaultValue;
	}

	private int LoadInt(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, int defaultValue)
	{
		string s;
		int result;
		if (dict.TryGetValue(key, out s) && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private int[] LoadIntArray(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, int[] defaultValue)
	{
		string text;
		if (dict.TryGetValue(key, out text) && !string.IsNullOrEmpty(text))
		{
			char[] separator = new char[]
			{
				','
			};
			return Globals.convertAllStringToInt(text.Split(separator));
		}
		return defaultValue;
	}

	private long LoadLong(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, long defaultValue)
	{
		string s;
		long result;
		if (dict.TryGetValue(key, out s) && long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private string LoadString(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, string defaultValue)
	{
		string result;
		if (dict.TryGetValue(key, out result))
		{
			return result;
		}
		return defaultValue;
	}

	public void LockHelm(Helmets.HelmType helmType)
	{
		if (this._helmetUnlockStatus.ContainsKey(helmType))
		{
			this._helmetUnlockStatus[helmType] = false;
			this._dirty = true;
		}
	}

	private static void LogError(string msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(msg, context);
	}

	public void MarkHelmetAsSeen(Helmets.HelmType helmType)
	{
		if (this._hasHelmetBeenSeen.ContainsKey(helmType))
		{
			this._hasHelmetBeenSeen[helmType] = true;
		}
		else
		{
			this._hasHelmetBeenSeen.Add(helmType, true);
		}
		this._dirty = true;
	}

	public void ResetHighestScoreTo(int score)
	{
		if (score != this._highestScore)
		{
			this._highestScore = score;
			this._dirty = true;
		}
	}

	public void RunCompleted()
	{
		this._dirty = true;
		this._numberOfRunsSinceLastGuideline++;
		this._numberOfRuns++;
		if (!this._hasShowDailyLandingPopup)
		{
			bool flag;
			this.GetDailyLandingDaysInRow(out flag);
			if (flag)
			{
				this._shouldShowDailyLandingPopup = false;
				this._hasShowDailyLandingPopup = true;
			}
			else
			{
				this._shouldShowDailyLandingPopup = true;
			}
		}
		if (this._numberOfRunsSinceLastGuideline > 4)
		{
			this._numberOfRunsSinceLastGuideline = 0;
			if (!this._hasShownTask1Popup)
			{
				if (this.rawMultiplier < 5)
				{
					this._shouldShowTask1Popup = true;
					this._dirty = true;
					return;
				}
				this._hasShownTask1Popup = true;
				this._shouldShowTask1Popup = false;
				this._dirty = true;
			}
			if (!this._hasShownPlayerMenuPopup)
			{
				if (this.HasThemeUnlockedForCharactersNum() <= 1)
				{
					this._shouldShownPlayerMenuPopup = true;
					this._dirty = true;
					return;
				}
				this._hasShownPlayerMenuPopup = true;
				this._shouldShownPlayerMenuPopup = false;
				this._dirty = true;
			}
		}
	}

	private void Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			Dictionary<PlayerInfo.Key, string> dict = new Dictionary<PlayerInfo.Key, string>();
			this.SaveInt(dict, PlayerInfo.Key.AmountOfCoins, this.amountOfCoins);
			this.SaveInt(dict, PlayerInfo.Key.HighestScore, this._highestScore);
			this.SaveBool(dict, PlayerInfo.Key.DailyLandingPayedOut, this._dailyLandingPayedOut);
			this.SaveInt(dict, PlayerInfo.Key.CurrentCharacter, this._currentCharacter);
			this.SaveInt(dict, PlayerInfo.Key.CurrentMissionSet, this._currentTaskSet);
			this.SaveDateTime(dict, PlayerInfo.Key.LastShowFreeUpgradeDate, this._lastShowFreeUpgradeDate);
			this.SaveInt(dict, PlayerInfo.Key.FreeUpgradeCount, this._freeUpgradeCount);
			this.SaveInt(dict, PlayerInfo.Key.AmountOfMysteryBoxesOpened, this._amountOfChestesOpened);
			this.SaveInt(dict, PlayerInfo.Key.LotteryRemainCount, this._lotteryWatchViewRemainCount);
			this.SaveDateTime(dict, PlayerInfo.Key.LotteryLastDateTime, this._lastLotteryFreeViewDateTime);
			this.SaveDateTime(dict, PlayerInfo.Key.LastPlayTime, this._lastPlayDate);
			this.SaveInt(dict, PlayerInfo.Key.AmountOfSuperMysteryBoxesOpened, this._amountOfSuperChestesOpened);
			this.SaveBool(dict, PlayerInfo.Key.HasFirstMBKey, this._hasReceivedFirstMBKey);
			this.SaveBool(dict, PlayerInfo.Key.HasFirstSMBKey, this._hasReceivedFirstSMBKey);
			this.SaveBool(dict, PlayerInfo.Key.TutorialCompleted, this._tutorialCompleted);
			this.SaveBool(dict, PlayerInfo.Key.HasSubscribed, this._hasSubscribed);
			this.SaveBool(dict, PlayerInfo.Key.DoubleCoins, this._hasDoubleCoins);
			this.SaveDateTime(dict, PlayerInfo.Key.FirstInstallDate, this._firstInstallDate);
			this.SaveInt(dict, PlayerInfo.Key.ShowWatchVideoPopupCount, this._showWatchVideoPopupCount);
			this.SaveInt(dict, PlayerInfo.Key.ShowTrialPopupCount, this._showTrialPopupCount);
			this.SaveInt(dict, PlayerInfo.Key.NumberOfRunsSinceLastGuideline, this._numberOfRunsSinceLastGuideline);
			this.SaveBool(dict, PlayerInfo.Key.AutoShowChangePlayerName, this._autoShowChangePlayerName);
			this.SaveBool(dict, PlayerInfo.Key.HasFacebookLogin, this._hasFacebookLogin);
			this.SaveBool(dict, PlayerInfo.Key.MenuSliderShow, this._menuSliderShow);
			this.SaveBool(dict, PlayerInfo.Key.FirstGameOverNoRemind, this._firstGameOverNoRemind);
			this.SaveBool(dict, PlayerInfo.Key.HasShownMission1Popup, this._hasShownTask1Popup);
			this.SaveBool(dict, PlayerInfo.Key.ShouldShowMission1Popup, this._shouldShowTask1Popup);
			this.SaveBool(dict, PlayerInfo.Key.ShouldShownPlayerMenuPopup, this._shouldShownPlayerMenuPopup);
			this.SaveInt(dict, PlayerInfo.Key.GameOverFullAdCount, this._gameOverFullAdCount);
			this.SaveDateTime(dict, PlayerInfo.Key.GameOverFullAdLastDate, this._gameOverFullAdLastDate);
			this.SaveDateTime(dict, PlayerInfo.Key.IgnoreTrailRoleNextTime, this._ignoreTrialRoleNextTime);
			this.SaveBool(dict, PlayerInfo.Key.IgnoreSubscriptionPopup, this._ignoreSubscriptionPopup);
			this.SaveDateTime(dict, PlayerInfo.Key.IgnoreSubscriptionNextTime, this._ignoreSubscriptionNextTime);
			this.SaveInt(dict, PlayerInfo.Key.GameOverUITryCount, this._gameoverUITryCount);
			this.SaveDateTime(dict, PlayerInfo.Key.GameOverUITryNextTime, this._gameoverUITryNextTime);
			this.SaveBool(dict, PlayerInfo.Key.HasShownPlayerMenuPopup, this._hasShownPlayerMenuPopup);
			this.SaveInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountOneDay, this._gameOverDoubleCoinsShowCountOneDay);
			this.SaveInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountTwoDay, this._gameOverDoubleCoinsShowCountTwoDay);
			this.SaveInt(dict, PlayerInfo.Key.GameOverDoubleCoinsShowCountLastDay, this._gameOverDoubleCoinsShowCountLastDay);
			this.SaveInt(dict, PlayerInfo.Key.WatchDoublePlayerLevel, this._watchDoublePlayerLevel);
			this.SaveInt(dict, PlayerInfo.Key.PlayOnceTimes, this._playOnceTimes);
			this.SaveInt(dict, PlayerInfo.Key.PlayDailyTimes, this._playDailyTimes);
			this.SaveDateTime(dict, PlayerInfo.Key.UpdateDateTime, this._updateDateTime);
			this.SaveInt(dict, PlayerInfo.Key.OpenAppCountDaily, this._openAppCountDaily);
			this.SaveDateTime(dict, PlayerInfo.Key.LastGameOverDoubleCoinsDateTime, this._lastGameOverDoubleCoinsDateTime);
			this.SaveInt(dict, PlayerInfo.Key.PlayerLevel, this._playerLevel);
			this.SaveBool(dict, PlayerInfo.Key.IgnoreTrailRolePopup, this._ignoreTrailRolePopup);
			this.SaveBool(dict, PlayerInfo.Key.HasShownKeysPopup, this._hasShownKeysPopup);
			this.SaveInt(dict, PlayerInfo.Key.UpdateRewardIndex, this._updateRewardIndex);
			this.SaveBool(dict, PlayerInfo.Key.UpdateFromLastApp, this._updateFromLastApp);
			this.SaveDateTime(dict, PlayerInfo.Key.FirstCheckVideoPlayerTime, this._firstCheckVideoPlayerTime);
			this.SaveDateTime(dict, PlayerInfo.Key.LastOpenGameDateTime, this._lastOpenGameDateTime);
			this.SaveInt(dict, PlayerInfo.Key.WatchVideoSuccessNum, this._watchVideoSuccessNum);
			this.SaveInt(dict, PlayerInfo.Key.AmountOfOpenGameApp, this._amountOfOpenGameApp);
			this.SaveInt(dict, PlayerInfo.Key.GameOverDoubleCoinViewCycleCount, this._gameOverDoubleCoinViewCycleCount);
			this.SaveInt(dict, PlayerInfo.Key.DonotClickGameOverDoubleCoinViewCount, this._donotClickGameOverDoubleCoinViewCount);
			this.SaveBool(dict, PlayerInfo.Key.IsNewPlayer, this._isNewPlayer);
			this.SaveDateTime(dict, PlayerInfo.Key.LastGameOverDoubleCoinViewDate, this._lastGameOverDoubleCoinViewDate);
			this.SaveString(dict, PlayerInfo.Key.TopRunData, this._topRunData.ToJson());
			if (this._currentTaskSet >= 0)
			{
				this.SaveIntArray(dict, PlayerInfo.Key.CurrentMissionSetProgress, this._currentTaskProgress);
			}
			this.SaveIntArray(dict, PlayerInfo.Key.AchievementProgress, this._achievementProgress);
			this.SaveBoolArray(dict, PlayerInfo.Key.AchievementRewardPayedOut, this._achievementAwardPayedOut);
			this.SaveBoolArray(dict, PlayerInfo.Key.OnlineZonePayedOut, this._onlineRewardPayedOut);
			this.SaveInt(dict, PlayerInfo.Key.OnlineTotalSeconds, this._onlineTotalSeconds);
			this.SaveDateTime(dict, PlayerInfo.Key.OpenAppInNewDayStartDateTime, this._openAppInNewDayStartDateTime);
			this.SaveIntArray(dict, PlayerInfo.Key.CurrentTrialProgress, this._currentTrialProgress);
			this.SaveInt(dict, PlayerInfo.Key.CurrentTrialIndex, this._currentTrialIndex);
			this.SaveInt(dict, PlayerInfo.Key.TotalTrialDays, this._totalTrialDays);
			this.SaveIntArray(dict, PlayerInfo.Key.CollectedCharacterTokens, this._collectedCharacterTokens);
			this.SaveInt(dict, PlayerInfo.Key.DailyLandingInRow, this._dailyLandingInRow);
			this.SaveInt(dict, PlayerInfo.Key.DailyLandingLastPayoutDayOfYear, this._dailyLandingLastPayoutDayOfYear);
			this.SaveInt(dict, PlayerInfo.Key.MissionCompletedSum, this._taskCompletedSum);
			this.SaveString(dict, PlayerInfo.Key.HasHoverboardsBeenSeen, Globals.convertEnumBoolDictionaryToString<Helmets.HelmType>(this._hasHelmetBeenSeen));
			this.SaveString(dict, PlayerInfo.Key.UnlockedHoverboardTypes, Globals.convertEnumBoolDictionaryToString<Helmets.HelmType>(this._helmetUnlockStatus));
			this.SaveEnum<Helmets.HelmType>(dict, PlayerInfo.Key.CurrentHoverboard, this._currentHelmet);
			this.SaveString(dict, PlayerInfo.Key.HistoryOfInapp, Globals.convertStringStringDictionaryToString(this._inappHistory));
			this.SaveString(dict, PlayerInfo.Key.HasCharacterBeenSeen, Globals.convertEnumBoolDictionaryToString<Characters.CharacterType>(this._hasCharacterBeenSeen));
			this.SaveInt(dict, PlayerInfo.Key.AmountOfKeys, this.amountOfKeys);
			this.SaveString(dict, PlayerInfo.Key.CharacterThemesUnlocked, Globals.convertEnumIntArrayDictionaryToString<Characters.CharacterType>(this._characterThemesUnlocked));
			this.SaveString(dict, PlayerInfo.Key.CharacterLastSelectedThemes, Globals.convertEnumIntDictionaryToString<Characters.CharacterType>(this._lastSelectedThemes));
			this.SaveString(dict, PlayerInfo.Key.CharacterThemesSeen, Globals.convertEnumIntArrayDictionaryToString<Characters.CharacterType>(this._characterThemesSeen));
			this.SaveString(dict, PlayerInfo.Key.WallWalkTutorialCount, Globals.convertStringIntDictionaryToString(this._wallWalkTutorialCount));
			this.SaveString(dict, PlayerInfo.Key.PendingRewards, PlayerInfo.GetStringFromRewards(this._pendingRewards));
			this.SaveBool(dict, PlayerInfo.Key.GameOverDoubleConfirnNoRemind, this._gameOverDoubleConfirmNoRemind);
			this.SaveInt(dict, PlayerInfo.Key.NumberOfRuns, this._numberOfRuns);
			this.SaveString(dict, PlayerInfo.Key.Stats, this._stats.ToString());
			this.SaveInt(dict, PlayerInfo.Key.AmountOfMiniMysteryBoxesOpened, this._amountOfMiniChestesOpened);
			FileUtil.WriteEnumStringDictionary<PlayerInfo.Key>(binaryWriter, dict);
			FileUtil.WriteEnumIntDictionary<PropType>(binaryWriter, this._upgradeAmounts);
			FileUtil.WriteEnumIntDictionary<PropType>(binaryWriter, this._upgradeTiers);
			FileUtil.Save(PlayerInfo.GetSavePath(), memoryStream.GetBuffer(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", 0, (int)memoryStream.Length, 1, 5, null, 0, 0);
			memoryStream.Close();
			this._dirty = false;
		}
		catch (Exception arg)
		{
			PlayerInfo.LogError("Error saving player info: " + arg, null);
		}
	}

	private void SaveBool(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, bool value)
	{
		dict[key] = value.ToString();
	}

	private void SaveBoolArray(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, bool[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllBoolToString(value));
	}

	private void SaveDateTime(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, DateTime value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveEnum<T>(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, T value)
	{
		dict[key] = value.ToString();
	}

	public void SaveIfDirty()
	{
		if (this._dirty || this._stats.dirty)
		{
			this.Save();
		}
	}

	private void SaveInt(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, int value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveIntArray(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, int[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllIntToString(value));
	}

	private void SaveLong(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, long value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveString(Dictionary<PlayerInfo.Key, string> dict, PlayerInfo.Key key, string value)
	{
		dict[key] = value;
	}

	public void SetCurrentTaskProgress(int task, int progress)
	{
		if (task >= 3)
		{
			this._achievementProgress[task - 3] = progress;
		}
		else if (this._currentTaskProgress[task] != progress)
		{
			this._currentTaskProgress[task] = progress;
		}
		this._dirty = true;
	}

	public bool GetCurrentAchievementAward(int index)
	{
		return index >= 0 && index < this._achievementAwardPayedOut.Length && this._achievementAwardPayedOut[index];
	}

	public bool GetAllAchievementAward()
	{
		for (int i = 0; i < Achievements.NUMBER_OF_ACHIEVEMENTS; i++)
		{
			if (TasksManager.Instance.GetTaskInfo(i + 3).complete && !this._achievementAwardPayedOut[i])
			{
				return true;
			}
		}
		return false;
	}

	public void SetCurrentAchivementReward(int index, bool value)
	{
		if (index < 0 || index >= this._achievementAwardPayedOut.Length)
		{
			return;
		}
		this._achievementAwardPayedOut[index] = value;
		NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.AchiementFinished);
		this._dirty = true;
	}

	public void CheckOnlineNewDayOpen()
	{
		if ((DateTime.UtcNow.Date - this._openAppInNewDayStartDateTime.Date).Days == 0)
		{
			this._openAppInNewDayStartDateTime = DateTime.UtcNow;
			return;
		}
		this._openAppInNewDayStartDateTime = DateTime.UtcNow;
		this._onlineTotalSeconds = 0;
		int i = 0;
		int num = this._onlineRewardPayedOut.Length;
		while (i < num)
		{
			this._onlineRewardPayedOut[i] = false;
			i++;
		}
	}

	public void CalcOnlineTotalSeconds()
	{
		int num = (int)(DateTime.UtcNow - this._openAppInNewDayStartDateTime).TotalSeconds;
		if (num > 0)
		{
			this._onlineTotalSeconds += num;
			this._openAppInNewDayStartDateTime = this._openAppInNewDayStartDateTime.AddSeconds((double)num);
			this._dirty = true;
		}
	}

	public int GetOnlineTime()
	{
		this.CalcOnlineTotalSeconds();
		return this._onlineTotalSeconds;
	}

	public bool GetOnlineZonePayedOut(int index)
	{
		return index >= 0 && index < this._onlineRewardPayedOut.Length && this._onlineRewardPayedOut[index];
	}

	public void SetOnlineZonePayedOut(int index, bool value)
	{
		if (index < 0 || index >= this._onlineRewardPayedOut.Length)
		{
			return;
		}
		this._onlineRewardPayedOut[index] = value;
	}

	public bool AllOnlineZonePayedOut()
	{
		int i = 0;
		int num = this._onlineRewardPayedOut.Length;
		while (i < num)
		{
			if (!this._onlineRewardPayedOut[i])
			{
				return false;
			}
			i++;
		}
		return true;
	}

	public void SetLastSelectedTheme(Characters.CharacterType character, int themeIndex)
	{
		if (Application.isEditor)
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && (list.Count < themeIndex || themeIndex < 0))
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"The theme index is too great ",
					themeIndex,
					" for character: ",
					character.ToString()
				}));
			}
		}
		if (this._lastSelectedThemes.ContainsKey(character))
		{
			this._lastSelectedThemes.Remove(character);
		}
		if (themeIndex > 0)
		{
			this._lastSelectedThemes.Add(character, themeIndex);
		}
		this._dirty = true;
	}

	public bool CheckIfLotteryCanWatchFreeView()
	{
		if ((DateTime.UtcNow.Date - this._lastLotteryFreeViewDateTime.Date).Days != 0)
		{
			this._lastLotteryFreeViewDateTime = DateTime.UtcNow;
			this._lotteryWatchViewRemainCount = 3;
		}
		return this._lotteryWatchViewRemainCount > 0;
	}

	public string NextFreeLotteryTime()
	{
		TimeSpan timeSpan = this._lastLotteryFreeViewDateTime.Date.AddDays(1.0) - DateTime.UtcNow;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public void UseLotteryWatchView()
	{
		this._lotteryWatchViewRemainCount--;
	}

	public bool CheckIfFreeUpgrade()
	{
		if ((DateTime.UtcNow.Date - this._lastShowFreeUpgradeDate.Date).Days > 0)
		{
			this._lastShowFreeUpgradeDate = DateTime.UtcNow;
			this._freeUpgradeCount = 3;
		}
		return (DateTime.UtcNow - this._lastShowFreeUpgradeDate).TotalSeconds >= 0.0 && this._freeUpgradeCount > 0;
	}

	public void UseFreeUpgrade()
	{
		this._freeUpgradeCount--;
		this._lastShowFreeUpgradeDate = DateTime.UtcNow.AddSeconds((double)this._freeUpgradeInterval);
	}

	public void ThemeSeen(Characters.CharacterType character, int index)
	{
		int[] array;
		this._characterThemesSeen.TryGetValue(character, out array);
		if (array == null)
		{
			array = new int[]
			{
				index
			};
		}
		else
		{
			int[] array2 = new int[array.Length + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			array2[array.Length] = index;
			array = array2;
		}
		this._characterThemesSeen.Remove(character);
		this._characterThemesSeen.Add(character, array);
		this._dirty = true;
	}

	public void TriggerOnScoreMultiplierChanged()
	{
		Action action = this.onScoreMultiplierChanged;
		if (action != null)
		{
			action();
		}
	}

	private static void TryGetLoadPaths(out string path, out string externalPath)
	{
		path = Application.persistentDataPath + "/playerdata";
		externalPath = null;
	}

	public void UnlockHelmet(Helmets.HelmType helmType)
	{
		if (this._helmetUnlockStatus.ContainsKey(helmType))
		{
			this._helmetUnlockStatus[helmType] = true;
		}
		else
		{
			this._helmetUnlockStatus.Add(helmType, true);
		}
		Action<Helmets.HelmType> onHelmUnlocked = this.OnHelmUnlocked;
		if (onHelmUnlocked != null)
		{
			onHelmUnlocked(helmType);
		}
		this._dirty = true;
	}

	public void UnlockTheme(Characters.CharacterType character, int index)
	{
		int[] array;
		this._characterThemesUnlocked.TryGetValue(character, out array);
		if (array == null)
		{
			array = new int[]
			{
				index
			};
		}
		else
		{
			int[] array2 = new int[array.Length + 1];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == index)
				{
					return;
				}
				array2[i] = array[i];
			}
			array2[array.Length] = index;
			array = array2;
		}
		this._characterThemesUnlocked.Remove(character);
		this._characterThemesUnlocked.Add(character, array);
		this._dirty = true;
		Action<Characters.CharacterType> onCharacterOutfitUnlocked = this.OnCharacterOutfitUnlocked;
		if (onCharacterOutfitUnlocked != null)
		{
			onCharacterOutfitUnlocked(character);
		}
	}

	public void UseUpgrade(PropType type)
	{
		if (this._upgradeAmounts.ContainsKey(type))
		{
			Dictionary<PropType, int> upgradeAmounts= this._upgradeAmounts;
			(upgradeAmounts )[type] = upgradeAmounts[type] - 1;
			this._dirty = true;
			Action action = this.onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public bool CheckGameoverUITry()
	{
		if ((DateTime.UtcNow.Date - this._gameoverUITryNextTime.Date).Days > 0)
		{
			this._gameoverUITryCount = 3;
			return true;
		}
		return DateTime.UtcNow > this._gameoverUITryNextTime && this._gameoverUITryCount > 0;
	}

	public void ShowGameoverUITry()
	{
		this._gameoverUITryNextTime = DateTime.UtcNow.AddMinutes(5.0);
		this._gameoverUITryCount--;
		this._dirty = true;
	}

	public int CheckGameOverDoubleCoinViewRate(int coins)
	{
		int result = 2;
		if ((DateTime.UtcNow.Date - this._lastGameOverDoubleCoinViewDate.Date).Days != 0)
		{
			this._lastGameOverDoubleCoinViewDate = DateTime.UtcNow;
			this._gameOverDoubleCoinViewCycleCount = 0;
			this._donotClickGameOverDoubleCoinViewCount = 0;
			return result;
		}
		if (this._gameOverDoubleCoinViewCycleCount >= 2)
		{
			return result;
		}
		if (this._donotClickGameOverDoubleCoinViewCount == 1)
		{
			result = 3;
		}
		else if (this._donotClickGameOverDoubleCoinViewCount == 2)
		{
			if (coins < 500)
			{
				result = 5;
			}
			else
			{
				result = 3;
			}
		}
		else if (this._donotClickGameOverDoubleCoinViewCount >= 3)
		{
			if (coins <= 500)
			{
				result = 10;
			}
			else if (coins <= 1500)
			{
				result = 5;
			}
			else
			{
				result = 3;
			}
		}
		return result;
	}

	public void ResetGameOverDoubleCoinViewRate()
	{
		this._gameOverDoubleCoinViewCycleCount++;
		this._donotClickGameOverDoubleCoinViewCount = 0;
	}

	public void DonotClickDoubleCoinView()
	{
		this._donotClickGameOverDoubleCoinViewCount++;
	}

	public bool CheckWallWalkingTutorial(string cityName)
	{
		if (!this._wallWalkTutorialCount.ContainsKey(cityName))
		{
			this._wallWalkTutorialCount.Add(cityName, 0);
		}
		return this._wallWalkTutorialCount[cityName] < 10;
	}

	public void AddWallWalkingTutorial(string cityName)
	{
		if (!this._wallWalkTutorialCount.ContainsKey(cityName))
		{
			this._wallWalkTutorialCount.Add(cityName, 0);
		}
		Dictionary<string, int> wallWalkTutorialCount= this._wallWalkTutorialCount;
		(wallWalkTutorialCount )[cityName] = wallWalkTutorialCount[cityName] + 1;
	}

	public int UpdateSpanDays()
	{
		return (DateTime.UtcNow.Date - this._updateDateTime.Date).Days;
	}

	public void CheckGameOverDoubleCoinsSpanDays()
	{
		int days = (DateTime.UtcNow.Date - this._lastGameOverDoubleCoinsDateTime.Date).Days;
		this._lastGameOverDoubleCoinsDateTime = DateTime.UtcNow;
		if (days == 2)
		{
			this._gameOverDoubleCoinsShowCountOneDay = this._gameOverDoubleCoinsShowCountTwoDay;
			this._gameOverDoubleCoinsShowCountTwoDay = this._gameOverDoubleCoinsShowCountLastDay;
			this._gameOverDoubleCoinsShowCountLastDay = 0;
		}
		else if (days == 3)
		{
			this._gameOverDoubleCoinsShowCountOneDay = this._gameOverDoubleCoinsShowCountTwoDay;
			this._gameOverDoubleCoinsShowCountTwoDay = 0;
			this._gameOverDoubleCoinsShowCountLastDay = 0;
		}
		else if (days > 3)
		{
			this._gameOverDoubleCoinsShowCountOneDay = 0;
			this._gameOverDoubleCoinsShowCountTwoDay = 0;
			this._gameOverDoubleCoinsShowCountLastDay = 0;
		}
	}

	public int LastThreeDaysGameOverDoubleCoinsCount()
	{
		return this._gameOverDoubleCoinsShowCountOneDay + this._gameOverDoubleCoinsShowCountTwoDay + this._gameOverDoubleCoinsShowCountLastDay;
	}

	public void UpdateGameOverDoubleCoinsShowCount()
	{
		this._gameOverDoubleCoinsShowCountOneDay = this._gameOverDoubleCoinsShowCountTwoDay;
		this._gameOverDoubleCoinsShowCountTwoDay = this._gameOverDoubleCoinsShowCountLastDay;
		this._gameOverDoubleCoinsShowCountLastDay = 0;
	}

	public void NextTrialLevel()
	{
		int num = this._currentTrialProgress[this._currentTrialIndex] % 100 + 1;
		this._currentTrialProgress[this._currentTrialIndex] = num;
		TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
		if (num >= currentTrialInfo.aim)
		{
			if (currentTrialInfo.type == TrialType.Character)
			{
				if (currentTrialInfo.characterThemeId == 0)
				{
					this.CollectSymbol(currentTrialInfo.characterType, int.MaxValue);
					TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters, 1, -1);
				}
				else
				{
					this.UnlockTheme(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
				}
				NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
				UIScreenController.Instance.AddUnlockForCharacterToReward(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
				UIModelController.Instance.SelectCharacterForPlay(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
				this.SaveIfDirty();
			}
			else if (currentTrialInfo.type == TrialType.Helmet)
			{
				this.UnlockHelmet(currentTrialInfo.helmetType);
				UIScreenController.Instance.AddUnlockForHelmetToReward(currentTrialInfo.helmetType);
				this.currentHelmet = currentTrialInfo.helmetType;
			}
			TrialManager.Instance.currentTrialInfo = null;
		}
	}

	public void IncreaseTrialProgress(int number)
	{
		this._currentTrialProgress[this._currentTrialIndex] += number * 100;
		int num = this._currentTrialProgress[this._currentTrialIndex] / 100;
		TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
		if (currentTrialInfo != null)
		{
			int num2 = currentTrialInfo.taskAim;
			if (currentTrialInfo.type == TrialType.Character)
			{
				num2 *= 1000;
			}
			if (num >= num2)
			{
				int num3 = this._currentTrialProgress[this._currentTrialIndex] % 100 + 1;
				this._currentTrialProgress[this._currentTrialIndex] = num3;
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "try_role_hoverboard_fill", 0, null);
				if (num3 >= currentTrialInfo.aim)
				{
					if (currentTrialInfo.type == TrialType.Character)
					{
						if (currentTrialInfo.characterThemeId == 0)
						{
							this.CollectSymbol(currentTrialInfo.characterType, int.MaxValue);
							TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters, 1, -1);
						}
						else
						{
							this.UnlockTheme(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
						}
						IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_try_role_hoverboard", 0, null);
						NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
						switch (Characters.characterOrder.IndexOf(currentTrialInfo.characterType))
						{
						case 0:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles1st", 0, null);
							break;
						case 1:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles2nd", 0, null);
							break;
						case 2:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles3rd", 0, null);
							break;
						case 3:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles4th", 0, null);
							break;
						case 4:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles5th", 0, null);
							break;
						case 5:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles6th", 0, null);
							break;
						case 6:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles7th", 0, null);
							break;
						case 7:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles8th", 0, null);
							break;
						case 8:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles9th", 0, null);
							break;
						case 9:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles10th", 0, null);
							break;
						case 10:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles11th", 0, null);
							break;
						}
						UIScreenController.Instance.AddUnlockForCharacterToReward(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
						UIModelController.Instance.SelectCharacterForPlay(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
					}
					else if (currentTrialInfo.type == TrialType.Helmet)
					{
						this.UnlockHelmet(currentTrialInfo.helmetType);
						IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_try_role_hoverboard", 0, null);
						switch (Helmets.helmOrder.IndexOf(currentTrialInfo.helmetType))
						{
						case 1:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet2nd", 0, null);
							break;
						case 2:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet3rd", 0, null);
							break;
						case 3:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet4th", 0, null);
							break;
						case 4:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet5th", 0, null);
							break;
						case 5:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet6th", 0, null);
							break;
						case 6:
							IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet7th", 0, null);
							break;
						}
						UIScreenController.Instance.AddUnlockForHelmetToReward(currentTrialInfo.helmetType);
						this.currentHelmet = currentTrialInfo.helmetType;
					}
					this.SaveIfDirty();
					TrialManager.Instance.currentTrialInfo = null;
				}
			}
		}
	}

	public int CurrentTrialInfoProgress()
	{
		return this._currentTrialProgress[this._currentTrialIndex] / 100;
	}

	public int CurrentTrialInfoLevel()
	{
		return this._currentTrialProgress[this._currentTrialIndex] % 100;
	}

	public int NextTrial()
	{
		this._currentTrialIndex++;
		if (this._currentTrialIndex >= this._currentTrialProgress.Length)
		{
			this._currentTrialIndex = 0;
		}
		return this._currentTrialIndex;
	}

	public DateTime lastLotteryFreeViewDateTime
	{
		get
		{
			return this._lastLotteryFreeViewDateTime;
		}
		set
		{
			this._lastLotteryFreeViewDateTime = value;
			this._dirty = true;
		}
	}

	public int donotClickGameOverDoubleCoinViewCount
	{
		get
		{
			return this._donotClickGameOverDoubleCoinViewCount;
		}
		set
		{
			if (value != this._donotClickGameOverDoubleCoinViewCount)
			{
				this._donotClickGameOverDoubleCoinViewCount = value;
				this._dirty = true;
			}
		}
	}

	public int[] currentTrialProgress
	{
		get
		{
			return this._currentTrialProgress;
		}
		set
		{
			if (value != this._currentTrialProgress)
			{
				this._currentTrialProgress = value;
				this._dirty = true;
			}
		}
	}

	public int currentTrialIndex
	{
		get
		{
			return this._currentTrialIndex;
		}
		set
		{
			if (value != this._currentTrialIndex)
			{
				this._currentTrialIndex = value;
				this._dirty = true;
			}
		}
	}

	public int totalTrialDays
	{
		get
		{
			return this._totalTrialDays;
		}
		set
		{
			if (value != this._totalTrialDays)
			{
				this._totalTrialDays = value;
				this._dirty = true;
			}
		}
	}

	public int WatchVideoSuccessNum
	{
		get
		{
			return this._watchVideoSuccessNum;
		}
		set
		{
			this._watchVideoSuccessNum = value;
			this._dirty = true;
		}
	}

	public int OpenGameAppAmount
	{
		get
		{
			return this._amountOfOpenGameApp;
		}
		set
		{
			this._amountOfOpenGameApp = value;
			this._dirty = true;
		}
	}

	public int showTrialPopupCount
	{
		get
		{
			return this._showTrialPopupCount;
		}
		set
		{
			if (value != this._showTrialPopupCount)
			{
				this._showTrialPopupCount = value;
				this._dirty = true;
			}
		}
	}

	public int lotteryWatchViewRemainCount
	{
		get
		{
			return this._lotteryWatchViewRemainCount;
		}
		set
		{
			if (value != this._lotteryWatchViewRemainCount)
			{
				this._lotteryWatchViewRemainCount = value;
				this._dirty = true;
			}
		}
	}

	public int amountOfCoins
	{
		get
		{
			return this._amountOfCoins;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			this._dirty = true;
			this._amountOfCoins = value;
			Action action = this.onCoinsChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public int amountOfKeys
	{
		get
		{
			return this._amountOfKeys;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			this._amountOfKeys = value;
			this._dirty = true;
			Action action = this.onKeysChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public int amountOfMiniChestesOpened
	{
		get
		{
			return this._amountOfMiniChestesOpened;
		}
		set
		{
			if (this._amountOfMiniChestesOpened != value)
			{
				this._amountOfMiniChestesOpened = value;
				this._dirty = true;
			}
		}
	}

	public int amountOfChestesOpened
	{
		get
		{
			return this._amountOfChestesOpened;
		}
		set
		{
			if (this._amountOfChestesOpened != value)
			{
				this._amountOfChestesOpened = value;
				this._dirty = true;
			}
		}
	}

	public int amountOfSuperChestesOpened
	{
		get
		{
			return this._amountOfSuperChestesOpened;
		}
		set
		{
			if (this._amountOfSuperChestesOpened != value)
			{
				this._amountOfSuperChestesOpened = value;
				this._dirty = true;
			}
		}
	}

	public int currentCharacter
	{
		get
		{
			return this._currentCharacter;
		}
		set
		{
			if (value != this._currentCharacter)
			{
				this._currentCharacter = value;
				this._dirty = true;
			}
		}
	}

	public Helmets.HelmType currentHelmet
	{
		get
		{
			return this._currentHelmet;
		}
		set
		{
			if (value != this._currentHelmet)
			{
				this._currentHelmet = value;
				this._dirty = true;
			}
		}
	}

	public bool menuSliderShow
	{
		get
		{
			return this._menuSliderShow;
		}
		set
		{
			if (this._menuSliderShow != value)
			{
				this._menuSliderShow = value;
				this._dirty = true;
			}
		}
	}

	public int currentTaskSet
	{
		get
		{
			return this._currentTaskSet;
		}
	}

	public int currentThemeIndex
	{
		get
		{
			return this.GetIndexForLastSelectedTheme((Characters.CharacterType)this.currentCharacter);
		}
		set
		{
			this.SetLastSelectedTheme((Characters.CharacterType)this.currentCharacter, value);
		}
	}

	public bool doubleScore
	{
		get
		{
			return this._doubleScore;
		}
		set
		{
			if (value != this._doubleScore)
			{
				this._doubleScore = value;
				this.TriggerOnScoreMultiplierChanged();
			}
		}
	}

	public bool hasDoubleCoins
	{
		get
		{
			return this._hasDoubleCoins;
		}
		set
		{
			if (this._hasDoubleCoins != value)
			{
				this._hasDoubleCoins = value;
				this._dirty = true;
			}
		}
	}

	public DateTime firstInstallDate
	{
		get
		{
			return this._firstInstallDate;
		}
		set
		{
			this._firstInstallDate = value;
			this._dirty = true;
		}
	}

	public bool hasReceivedFirstMBKey
	{
		get
		{
			return this._hasReceivedFirstMBKey;
		}
		set
		{
			if (this._hasReceivedFirstMBKey != value)
			{
				this._hasReceivedFirstMBKey = value;
				this._dirty = true;
			}
		}
	}

	public bool hasReceivedFirstSMBKey
	{
		get
		{
			return this._hasReceivedFirstSMBKey;
		}
		set
		{
			if (this._hasReceivedFirstSMBKey != value)
			{
				this._hasReceivedFirstSMBKey = value;
				this._dirty = true;
			}
		}
	}

	public bool autoShowChangePlayerName
	{
		get
		{
			return this._autoShowChangePlayerName;
		}
		set
		{
			if (this._autoShowChangePlayerName != value)
			{
				this._autoShowChangePlayerName = value;
				this._dirty = true;
			}
		}
	}

	public bool hasShownKeysPopup
	{
		get
		{
			return this._hasShownKeysPopup;
		}
		set
		{
			if (this._hasShownKeysPopup != value)
			{
				this._hasShownKeysPopup = value;
				this._dirty = true;
			}
		}
	}

	public bool hasShownTask1Popup
	{
		get
		{
			return this._hasShownTask1Popup;
		}
		set
		{
			if (this._hasShownTask1Popup != value)
			{
				this._hasShownTask1Popup = value;
				this._dirty = true;
			}
		}
	}

	public bool hasShowDailyLandingPopup
	{
		get
		{
			return this._hasShowDailyLandingPopup;
		}
		set
		{
			if (this._hasShowDailyLandingPopup != value)
			{
				this._hasShowDailyLandingPopup = value;
			}
		}
	}

	public bool shouldShowDailyLandingPopup
	{
		get
		{
			return this._shouldShowDailyLandingPopup;
		}
		set
		{
			if (this._shouldShowDailyLandingPopup != value)
			{
				this._shouldShowDailyLandingPopup = value;
			}
		}
	}

	public bool hasShownPlayerMenuPopup
	{
		get
		{
			return this._hasShownPlayerMenuPopup;
		}
		set
		{
			if (this._hasShownPlayerMenuPopup != value)
			{
				this._hasShownPlayerMenuPopup = value;
				this._dirty = true;
			}
		}
	}

	public int gameOverDoubleCoinsShowCountLastDay
	{
		get
		{
			return this._gameOverDoubleCoinsShowCountLastDay;
		}
		set
		{
			if (value != this._gameOverDoubleCoinsShowCountLastDay)
			{
				this._gameOverDoubleCoinsShowCountLastDay = value;
				this._dirty = true;
			}
		}
	}

	public int watchDoublePlayerLevel
	{
		get
		{
			return this._watchDoublePlayerLevel;
		}
		set
		{
			if (value != this._watchDoublePlayerLevel)
			{
				this._watchDoublePlayerLevel = value;
				this._dirty = true;
			}
		}
	}

	public int playerLevel
	{
		get
		{
			return this._playerLevel;
		}
		set
		{
			if (value != this._playerLevel)
			{
				this._playerLevel = value;
				this._dirty = true;
			}
		}
	}

	public int highestScore
	{
		get
		{
			return this._highestScore;
		}
		set
		{
			if (value > this._highestScore)
			{
				this._highestScore = value;
				this._dirty = true;
			}
			if (value > 9999999)
			{
				return;
			}
			ServerManager.Instance.UploadScore(value);
			ServerManager.Instance.UploadScoreGlobal((float)value);
			if (this._hasSubscribed)
			{
				ServerManager.Instance.UploadScoreVip((float)value);
			}
		}
	}

	public int showWatchVideoPopupCount
	{
		get
		{
			return this._showWatchVideoPopupCount;
		}
		set
		{
			this._showWatchVideoPopupCount = value;
			this._dirty = true;
		}
	}

	public static PlayerInfo Instance
	{
		get
		{
			if (PlayerInfo._instance == null)
			{
				PlayerInfo._instance = new PlayerInfo();
			}
			return PlayerInfo._instance;
		}
	}

	public CelebrationReward lastAddedReward
	{
		get
		{
			return this._lastAddedReward;
		}
	}

	public int taskCompletedSum
	{
		get
		{
			if (this._taskCompletedSum == 0)
			{
				this._taskCompletedSum = this.currentTaskSet + 1;
			}
			return this._taskCompletedSum;
		}
		set
		{
			this._taskCompletedSum = value;
		}
	}

	public int numberOfRuns
	{
		get
		{
			return this._numberOfRuns;
		}
		set
		{
			if (this._numberOfRuns != value)
			{
				this._numberOfRuns = value;
				this._dirty = true;
			}
		}
	}

	public bool hasFacebookLogin
	{
		get
		{
			return this._hasFacebookLogin;
		}
		set
		{
			this._hasFacebookLogin = value;
			this._dirty = true;
		}
	}

	public List<CelebrationReward> pendingRewards
	{
		get
		{
			return this._pendingRewards;
		}
		set
		{
			this._pendingRewards = value;
			this._dirty = true;
		}
	}

	public TopRunData TopRunData
	{
		get
		{
			return this._topRunData;
		}
	}

	public int updateRewardIndex
	{
		get
		{
			return this._updateRewardIndex;
		}
		set
		{
			if (value != this._updateRewardIndex)
			{
				this._updateRewardIndex = value;
				this._dirty = true;
			}
		}
	}

	public bool updateFromLastApp
	{
		get
		{
			return this._updateFromLastApp;
		}
		set
		{
			if (value != this._updateFromLastApp)
			{
				this._updateFromLastApp = value;
				this._dirty = true;
			}
		}
	}

	public int rawMultiplier
	{
		get
		{
			return Mathf.Clamp(this._currentTaskSet + 1, 1, 30);
		}
	}

	public bool gameOverDoubleConfirmNoRemind
	{
		get
		{
			return this._gameOverDoubleConfirmNoRemind;
		}
		set
		{
			if (this._gameOverDoubleConfirmNoRemind != value)
			{
				this._gameOverDoubleConfirmNoRemind = value;
				this._dirty = true;
			}
		}
	}

	public int scoreMultiplier
	{
		get
		{
			int num = Mathf.Clamp(this._currentTaskSet + 1, 1, 30);
			if (GameStats.Instance.scoreBooster5Activated)
			{
				num += 5;
			}
			if (GameStats.Instance.scoreBooster10Activated)
			{
				num += 10;
			}
			if (this.doubleScore)
			{
				num *= 2;
			}
			return num;
		}
	}

	public bool firstGameOverNoRemind
	{
		get
		{
			return this._firstGameOverNoRemind;
		}
		set
		{
			if (this._firstGameOverNoRemind != value)
			{
				this._firstGameOverNoRemind = value;
				this._dirty = true;
			}
		}
	}

	public DateTime lastPlayDate
	{
		get
		{
			return this._lastPlayDate;
		}
		set
		{
			this._lastPlayDate = value;
			this._dirty = true;
		}
	}

	public int playOnceTimes
	{
		get
		{
			return this._playOnceTimes;
		}
		set
		{
			this._playOnceTimes = value;
			this._dirty = true;
		}
	}

	public int playDailyTimes
	{
		get
		{
			return this._playDailyTimes;
		}
		set
		{
			this._playDailyTimes = value;
			this._dirty = true;
		}
	}

	public int openAppCountDaily
	{
		get
		{
			return this._openAppCountDaily;
		}
		set
		{
			if (this._openAppCountDaily != value)
			{
				this._openAppCountDaily = value;
				this._dirty = true;
			}
		}
	}

	public bool shouldShowTask1Popup
	{
		get
		{
			return this._shouldShowTask1Popup;
		}
		set
		{
			if (this._shouldShowTask1Popup != value)
			{
				this._shouldShowTask1Popup = value;
				this._dirty = true;
			}
		}
	}

	public bool shouldShowPlayerMenuPopup
	{
		get
		{
			return this._shouldShownPlayerMenuPopup;
		}
		set
		{
			if (this._shouldShownPlayerMenuPopup != value)
			{
				this._shouldShownPlayerMenuPopup = value;
				this._dirty = true;
			}
		}
	}

	public bool ignoreTrailRolePopup
	{
		get
		{
			return this._ignoreTrailRolePopup;
		}
		set
		{
			if (this._ignoreTrailRolePopup != value)
			{
				this._ignoreTrailRolePopup = value;
				this._dirty = true;
			}
		}
	}

	public bool ignoreSubscriptionPopup
	{
		get
		{
			return this._ignoreSubscriptionPopup;
		}
		set
		{
			if (this._ignoreSubscriptionPopup != value)
			{
				this._ignoreSubscriptionPopup = value;
				this._dirty = true;
			}
		}
	}

	public DateTime ignoreSubscriptionNextTime
	{
		get
		{
			return this._ignoreSubscriptionNextTime;
		}
		set
		{
			if (this._ignoreSubscriptionNextTime != value)
			{
				this._ignoreSubscriptionNextTime = value;
				this._dirty = true;
			}
		}
	}

	public int gameoverUITryCount
	{
		get
		{
			return this._gameoverUITryCount;
		}
		set
		{
			if (this._gameoverUITryCount != value)
			{
				this._gameoverUITryCount = value;
				this._dirty = true;
			}
		}
	}

	public DateTime gameoverUITryNextTime
	{
		get
		{
			return this._gameoverUITryNextTime;
		}
		set
		{
			if (this._gameoverUITryNextTime != value)
			{
				this._gameoverUITryNextTime = value;
				this._dirty = true;
			}
		}
	}

	public bool isNewPlayer
	{
		get
		{
			return this._isNewPlayer;
		}
		set
		{
			if (this._isNewPlayer != value)
			{
				this._isNewPlayer = value;
				this._dirty = true;
			}
		}
	}

	public Statistics stats
	{
		get
		{
			return this._stats;
		}
	}

	public int gameOverFullAdCount
	{
		get
		{
			return this._gameOverFullAdCount;
		}
		set
		{
			if (this._gameOverFullAdCount != value)
			{
				this._gameOverFullAdCount = value;
				this._dirty = true;
			}
		}
	}

	public DateTime gameOverFullAdDate
	{
		get
		{
			return this._gameOverFullAdLastDate;
		}
		set
		{
			if (this._gameOverFullAdLastDate != value)
			{
				this._gameOverFullAdLastDate = value;
				this._dirty = true;
			}
		}
	}

	public DateTime ignoreTrialRoleNextTime
	{
		get
		{
			return this._ignoreTrialRoleNextTime;
		}
		set
		{
			if (this._ignoreTrialRoleNextTime != value)
			{
				this._ignoreTrialRoleNextTime = value;
				this._dirty = true;
			}
		}
	}

	public bool tutorialCompleted
	{
		get
		{
			return this._tutorialCompleted;
		}
		set
		{
			if (this._tutorialCompleted != value)
			{
				this._tutorialCompleted = value;
				this._dirty = true;
			}
		}
	}

	public bool hasSubscribed
	{
		get
		{
			return this._hasSubscribed;
		}
		set
		{
			if (this._hasSubscribed != value)
			{
				this._hasSubscribed = value;
				RiseSdk.Instance.enableBackHomeAd(false, "custom", 20000);
				if (this.OnSubscribed != null)
				{
					this.OnSubscribed();
				}
				this._dirty = true;
			}
		}
	}

	public Action onCoinsChanged;

	public Action onKeysChanged;

	public Action onScoreMultiplierChanged;

	public Action<Characters.CharacterType> OnCharacterOutfitUnlocked;

	public Action<Characters.CharacterType> OnSymbolCollected;

	public Action<Helmets.HelmType> OnHelmUnlocked;

	public Action OnSubscribed;

	public Action onPowerupAmountChanged;

	private int _amountOfMiniChestesOpened;

	private int _amountOfChestesOpened;

	private int _amountOfSuperChestesOpened;

	private Dictionary<Characters.CharacterType, int[]> _characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();

	private Dictionary<Characters.CharacterType, int[]> _characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();

	private int[] _collectedCharacterTokens;

	private int _currentCharacter;

	private Helmets.HelmType _currentHelmet;

	private int[] _currentTaskProgress;

	private int _currentTaskSet = -1;

	private int[] _achievementProgress;

	private bool[] _achievementAwardPayedOut;

	private int[] _currentTrialProgress;

	private int _currentTrialIndex;

	private int _totalTrialDays;

	private int _updateRewardIndex;

	private bool _updateFromLastApp;

	private int _watchVideoSuccessNum;

	private int _amountOfOpenGameApp;

	private int _dailyLandingInRow;

	private int _dailyLandingLastPayoutDayOfYear;

	private bool _dailyLandingPayedOut;

	private bool _dirty;

	private bool _doubleScore;

	private DateTime _openAppInNewDayStartDateTime;

	private int _onlineTotalSeconds;

	private bool[] _onlineRewardPayedOut;

	private int _openAppCountDaily;

	private int _playOnceTimes;

	private int _playDailyTimes;

	private DateTime _updateDateTime;

	private int _gameOverDoubleCoinsShowCountLastDay;

	private int _gameOverDoubleCoinsShowCountOneDay;

	private int _gameOverDoubleCoinsShowCountTwoDay;

	private DateTime _lastGameOverDoubleCoinsDateTime;

	private int _watchDoublePlayerLevel;

	private int _playerLevel;

	private DateTime _firstInstallDate;

	private DateTime _firstCheckVideoPlayerTime;

	private DateTime _lastOpenGameDateTime;

	private DateTime _lastLotteryFreeViewDateTime;

	private int _showWatchVideoPopupCount;

	private DateTime _lastShowFreeUpgradeDate;

	private DateTime _lastGameOverDoubleCoinViewDate;

	private DateTime _lastPlayDate;

	private int _showTrialPopupCount;

	private Dictionary<Characters.CharacterType, bool> _hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();

	private bool _hasDoubleCoins;

	private Dictionary<Helmets.HelmType, bool> _hasHelmetBeenSeen = new Dictionary<Helmets.HelmType, bool>();

	private bool _hasReceivedFirstMBKey;

	private bool _hasReceivedFirstSMBKey;

	private bool _menuSliderShow;

	private bool _autoShowChangePlayerName;

	private bool _hasShownKeysPopup;

	private bool _hasShownTask1Popup;

	private bool _hasShownPlayerMenuPopup;

	private int _highestScore;

	private int _lotteryWatchViewRemainCount;

	private int _gameOverDoubleCoinViewCycleCount;

	private int _donotClickGameOverDoubleCoinViewCount;

	private int _freeUpgradeCount;

	private int _freeUpgradeInterval = 120;

	public Dictionary<Helmets.HelmType, bool> _helmetUnlockStatus = new Dictionary<Helmets.HelmType, bool>();

	private Dictionary<string, string> _inappHistory;

	private static PlayerInfo _instance;

	private CelebrationReward _lastAddedReward;

	private Dictionary<Characters.CharacterType, int> _lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();

	private int _taskCompletedSum;

	private int _numberOfRuns;

	private int _numberOfRunsSinceLastGuideline;

	private bool _hasFacebookLogin;

	private List<CelebrationReward> _pendingRewards = new List<CelebrationReward>();

	private bool _gameOverDoubleConfirmNoRemind;

	private bool _shouldShownPlayerMenuPopup;

	private bool _firstGameOverNoRemind;

	private bool _shouldShowTask1Popup;

	private bool _shouldShowDailyLandingPopup;

	private bool _hasShowDailyLandingPopup;

	private Statistics _stats;

	private bool _hasSubscribed;

	private bool _tutorialCompleted;

	private Dictionary<PropType, int> _upgradeAmounts;

	private Dictionary<PropType, int> _upgradeTiers;

	private Dictionary<string, int> _wallWalkTutorialCount;

	private int _amountOfCoins;

	private int _amountOfKeys;

	private DateTime _gameOverFullAdLastDate;

	private int _gameOverFullAdCount;

	private DateTime _ignoreTrialRoleNextTime;

	private bool _ignoreTrailRolePopup;

	private DateTime _gameoverUITryNextTime;

	private int _gameoverUITryCount;

	private DateTime _ignoreSubscriptionNextTime;

	private bool _ignoreSubscriptionPopup;

	private bool _isNewPlayer;

	private TopRunData _topRunData;

	private enum Key
	{
		AmountOfCoins,
		HighestScore,
		HasSubscribed,
		DailyLandingPayedOut = 6,
		CurrentCharacter,
		CurrentMissionSet,
		CurrentMissionSetProgress,
		CollectedCharacterTokens,
		AmountOfMysteryBoxesOpened,
		TutorialCompleted,
		DoubleCoins = 15,
		DailyLandingInRow = 17,
		MissionCompletedSum,
		NumberOfRunsSinceLastGuideline,
		AutoShowChangePlayerName,
		HasFacebookLogin,
		HasShownPlayerMenuPopup,
		ShouldShownPlayerMenuPopup,
		FirstGameOverNoRemind,
		HasShownMission1Popup,
		ShouldShowMission1Popup,
		IgnoreTrailRolePopup,
		DailyLandingLastPayoutDayOfYear = 29,
		AmountOfSuperMysteryBoxesOpened,
		HasHoverboardsBeenSeen,
		UnlockedHoverboardTypes,
		CurrentHoverboard,
		HistoryOfInapp = 36,
		HasCharacterBeenSeen,
		AmountOfKeys = 39,
		HasFirstMBKey,
		HasFirstSMBKey,
		HasShownKeysPopup,
		CharacterThemesUnlocked,
		CharacterLastSelectedThemes,
		CharacterThemesSeen,
		PendingRewards,
		GameOverDoubleConfirnNoRemind,
		NumberOfRuns,
		Stats,
		FirstInstallDate = 51,
		ShowWatchVideoPopupCount,
		AmountOfMiniMysteryBoxesOpened,
		LotteryRemainCount,
		LotteryLastDateTime,
		ShowTrialPopupCount,
		LastShowFreeUpgradeDate,
		FreeUpgradeCount,
		MenuSliderShow,
		TopRunData,
		AchievementProgress,
		AchievementRewardPayedOut,
		GameOverFullAdCount,
		GameOverFullAdLastDate,
		IgnoreTrailRoleNextTime,
		GameOverUITryCount,
		GameOverUITryNextTime,
		IgnoreSubscriptionPopup,
		IgnoreSubscriptionNextTime,
		LastPlayTime,
		UpdateRewardIndex,
		UpdateFromLastApp,
		FirstCheckVideoPlayerTime,
		WatchVideoSuccessNum,
		AmountOfOpenGameApp,
		LastOpenGameDateTime,
		WallWalkTutorialCount,
		GameOverDoubleCoinViewCycleCount,
		DonotClickGameOverDoubleCoinViewCount,
		IsNewPlayer,
		LastGameOverDoubleCoinViewDate,
		LastClearSingleUserDateTime,
		LastSendSingerUserAverageTime,
		LastSendDayAveragePlayTime,
		OpenAppCountDaily,
		PlayOnceTimes,
		PlayDailyTimes,
		GameOverDoubleCoinsShowCountOneDay,
		GameOverDoubleCoinsShowCountTwoDay,
		GameOverDoubleCoinsShowCountLastDay,
		WatchDoublePlayerLevel,
		PlayerLevel,
		LastGameOverDoubleCoinsDateTime,
		UpdateDateTime,
		CurrentTrialProgress,
		CurrentTrialIndex,
		TotalTrialDays,
		OnlineZonePayedOut,
		OpenAppInNewDayStartDateTime,
		OnlineTotalSeconds
	}
}
