using System;
using UnityEngine;

public static class SaveMeManager
{
	public static int GetNumberOfKeysToSaveMe()
	{
		return SaveMeManager._numberOfUsedKeysInCurrentRun;
	}

	public static void HasShownKeysPopup()
	{
		PlayerInfo.Instance.hasShownKeysPopup = true;
	}

	public static void IncrementNumberOfUsedKeys()
	{
		if (SaveMeManager._numberOfUsedKeysInCurrentRun <= 0)
		{
			SaveMeManager._numberOfUsedKeysInCurrentRun = 1;
		}
		else
		{
			SaveMeManager._numberOfUsedKeysInCurrentRun *= 2;
		}
	}

	public static void ResetSaveMeForNewRun()
	{
		SaveMeManager._numberOfUsedKeysInCurrentRun = 1;
	}

	public static void SendReviveIfPurchaseSucceeded()
	{
		if (UIScreenController.isInstanced)
		{
			if (UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
			{
				if (PlayerInfo.Instance.amountOfKeys - SaveMeManager.GetNumberOfKeysToSaveMe() >= 0)
				{
					PlayerInfo instance = PlayerInfo.Instance;
					instance.amountOfKeys -= SaveMeManager.GetNumberOfKeysToSaveMe();
					TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, SaveMeManager.GetNumberOfKeysToSaveMe(), -1);
				}
				SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
				SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance2 = Revive.Instance;
				if (instance2 != null)
				{
					instance2.SendRevive();
				}
				else
				{
					UnityEngine.Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
				SaveMeManager.IncrementNumberOfUsedKeys();
				UIScreenController instance3 = UIScreenController.Instance;
				IngameScreen ingameScreen = instance3.GetScreenFromCache(instance3.GetTopScreenName()) as IngameScreen;
				if (ingameScreen == null)
				{
					UnityEngine.Debug.LogError("IngameScreen == NULL");
				}
				else
				{
					ingameScreen.SetPauseButtonVisibility(true);
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}

	public static void SkipReviveIfPurchaseFailed()
	{
		if (UIScreenController.isInstanced)
		{
			if (UIScreenController.Instance.GetTopScreenName() == "IngameUI" && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
			{
				SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
				SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance = Revive.Instance;
				if (instance != null)
				{
					instance.SendSkipRevive();
				}
				else
				{
					UnityEngine.Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
			}
			Time.timeScale = 1f;
		}
		else
		{
			UnityEngine.Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}

	public static int _numberOfUsedKeysInCurrentRun = 1;

	public static bool IS_PURCHASE_MADE_FROM_INGAME;

	public static bool IS_PURCHASE_RUNNING_INGAME;

	private const string SAVE_ME_LOCALES_KEY = "save_me_locales";
}
