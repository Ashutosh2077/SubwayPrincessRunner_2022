using System;
using UnityEngine;

public class PurchaseHandler
{
	public void AddOnUpgradePurchase(Action handler)
	{
		this._onUpgradePurchase = (Action)Delegate.Combine(this._onUpgradePurchase, handler);
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, int themeIndex, bool isPopup, Action buySuccessCallBack = null)
	{
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		Characters.Model model = Characters.characterData[characterType];
		CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(characterType, themeIndex);
		bool flag = themeForCharacter != null;
		Characters.UnlockType unlockType = model.unlockType;
		int price = model.Price;
		if (flag)
		{
			CharacterTheme characterTheme = themeForCharacter;
			unlockType = characterTheme.unlockType;
			price = characterTheme.price;
		}
		if (unlockType != Characters.UnlockType.coins && unlockType != Characters.UnlockType.keys)
		{
			UnityEngine.Debug.LogWarning("Cannot buy character with unlocktype: " + model.unlockType.ToString());
			instance.CharacterPurchaseFailure();
		}
		else
		{
			int num = PlayerInfo.Instance.amountOfCoins;
			InAppData.DataType type = InAppData.DataType.Coin;
			if (unlockType == Characters.UnlockType.keys)
			{
				num = PlayerInfo.Instance.amountOfKeys;
				type = InAppData.DataType.Key;
			}
			if (num < price)
			{
				InAppManager.instance.SetupNativePopup(price, isPopup, type);
				instance.CharacterPurchaseFailure();
			}
			else
			{
				if (buySuccessCallBack != null)
				{
					buySuccessCallBack();
				}
				if (unlockType != Characters.UnlockType.coins)
				{
					if (unlockType == Characters.UnlockType.keys)
					{
						TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price, -1);
						PlayerInfo instance2 = PlayerInfo.Instance;
						instance2.amountOfKeys -= price;
					}
				}
				else
				{
					TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
					PlayerInfo instance3 = PlayerInfo.Instance;
					instance3.amountOfCoins -= price;
					Characters.Model model2 = Characters.characterData[characterType];
					if (model2.taskTargetKey != TaskTarget.none)
					{
						Characters.Model model3 = Characters.characterData[characterType];
						TasksManager.Instance.PlayerDidThis(model3.taskTargetKey, 1, -1);
					}
				}
				if (flag)
				{
					PlayerInfo.Instance.UnlockTheme(characterType, themeIndex);
				}
				else
				{
					PlayerInfo.Instance.CollectSymbol(characterType, price);
					TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters, 1, -1);
				}
				NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
				instance.CharacterPurchaseSuccessful(characterType, themeIndex);
			}
		}
	}

	public void PurchaseHelmetTheme(Helmets.HelmType helmetType, HelmetSelectButton btn)
	{
		Helmets.Helm helm = Helmets.helmData[helmetType];
		Helmets.UnlockType unlockType = helm.unlockType;
		int price = helm.price;
		if (unlockType != Helmets.UnlockType.coins && unlockType != Helmets.UnlockType.keys)
		{
			UnityEngine.Debug.LogWarning("Cannot buy helmet with unlocktype: " + helm.unlockType.ToString());
			btn.PurchaseHelmetFailure();
		}
		else
		{
			int num = PlayerInfo.Instance.amountOfCoins;
			InAppData.DataType type = InAppData.DataType.Coin;
			if (unlockType == Helmets.UnlockType.keys)
			{
				num = PlayerInfo.Instance.amountOfKeys;
				type = InAppData.DataType.Key;
			}
			if (num < price)
			{
				InAppManager.instance.SetupNativePopup(price, false, type);
				btn.PurchaseHelmetFailure();
			}
			else
			{
				if (unlockType != Helmets.UnlockType.coins)
				{
					if (unlockType == Helmets.UnlockType.keys)
					{
						TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price, -1);
						PlayerInfo.Instance.amountOfKeys -= price;
					}
				}
				else
				{
					TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
					PlayerInfo.Instance.amountOfCoins -= price;
				}
				PlayerInfo.Instance.UnlockHelmet(helmetType);
				btn.PurchaseHelmetSuccess();
			}
		}
	}

	public void PurchaseHelmet(BuyHelmetButton btn)
	{
		int num = Upgrades.upgrades[PropType.helmet].pricesRaw[0];
		int amountOfCoins = PlayerInfo.Instance.amountOfCoins;
		InAppData.DataType type = InAppData.DataType.Coin;
		if (amountOfCoins < btn.number * num)
		{
			btn.PurchaseFailure();
			InAppManager.instance.SetupNativePopup(btn.number * num, true, type);
		}
		else
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, btn.number * num, -1);
			PlayerInfo.Instance.amountOfCoins -= btn.number * num;
			btn.PurchaseSuccessful();
		}
	}

	public void PurchaseHelmet(Helmets.HelmType helmetType, Action buySuccessCallBack = null)
	{
		Helmets.Helm helm = Helmets.helmData[helmetType];
		Helmets.UnlockType unlockType = helm.unlockType;
		int price = helm.price;
		if (unlockType == Helmets.UnlockType.coins)
		{
			int amountOfCoins = PlayerInfo.Instance.amountOfCoins;
			if (amountOfCoins < price)
			{
				InAppManager.instance.SetupNativePopup(price, true, InAppData.DataType.Coin);
				return;
			}
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
			PlayerInfo.Instance.amountOfCoins -= price;
		}
		else if (unlockType == Helmets.UnlockType.keys)
		{
			int amountOfKeys = PlayerInfo.Instance.amountOfKeys;
			if (amountOfKeys < price)
			{
				InAppManager.instance.SetupNativePopup(price, true, InAppData.DataType.Key);
				return;
			}
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price, -1);
			PlayerInfo.Instance.amountOfKeys -= price;
		}
		PlayerInfo.Instance.UnlockHelmet(helmetType);
		if (buySuccessCallBack != null)
		{
			buySuccessCallBack();
		}
	}

	public void PurchaseKeysIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SetupNativePopup(amountToSaveMe, true, InAppData.DataType.Key);
	}

	public void PurchaseCoinsIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SetupNativePopup(amountToSaveMe, true, InAppData.DataType.Coin);
	}

	public void PurchaseUpgrade(PropType type, BuyButtonIngame sender)
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		int price;
		if (upgrade.numberOfTiers == 0)
		{
			price = Upgrades.upgrades[type].getPrice(0);
		}
		else
		{
			price = Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1);
		}
		if (PlayerInfo.Instance.amountOfCoins < price)
		{
			InAppManager.instance.SetupNativePopup(price, false, InAppData.DataType.Coin);
			sender.PurchaseFailure();
		}
		else
		{
			switch (type)
			{
			case PropType.helmet:
			case PropType.headstart500:
			case PropType.headstart2000:
			case PropType.scorebooster:
				PlayerInfo.Instance.IncreaseUpgradeAmount(type, 1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
				if (type == PropType.headstart2000)
				{
					TasksManager.Instance.PlayerDidThis(TaskTarget.HaveHeadStartLarge, 1, -1);
				}
				break;
			case PropType.chest:
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.Chest);
				UIScreenController.Instance.QueueChest();
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.BuyMysterybox, 1, -1);
				break;
			case PropType.flypack:
			case PropType.supershoes:
			case PropType.coinmagnet:
			case PropType.letters:
			case PropType.doubleMultiplier:
				PlayerInfo.Instance.IncreasePowerupTier(type);
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, -1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveUpgrades, 1, -1);
				break;
			case PropType.skiptask1:
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, 0);
				break;
			case PropType.skiptask2:
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, 1);
				break;
			case PropType.skiptask3:
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price, 2);
				break;
			}
			PlayerInfo.Instance.amountOfCoins -= price;
			sender.PurchaseSuccessful();
			Action onUpgradePurchase = this._onUpgradePurchase;
			if (onUpgradePurchase != null)
			{
				onUpgradePurchase();
			}
		}
	}

	public void PurchaseUpgradeFree(PropType type)
	{
		switch (type)
		{
		case PropType.helmet:
		case PropType.headstart500:
		case PropType.headstart2000:
		case PropType.scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(type, 1);
			if (type == PropType.headstart2000)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveHeadStartLarge, 1, -1);
			}
			break;
		case PropType.chest:
			TasksManager.Instance.PlayerDidThis(TaskTarget.BuyMysterybox, 1, -1);
			break;
		case PropType.flypack:
		case PropType.supershoes:
		case PropType.coinmagnet:
		case PropType.letters:
		case PropType.doubleMultiplier:
			PlayerInfo.Instance.IncreasePowerupTier(type);
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveUpgrades, 1, -1);
			break;
		}
		Action onUpgradePurchase = this._onUpgradePurchase;
		if (onUpgradePurchase != null)
		{
			onUpgradePurchase();
		}
	}

	public void RemoveOnUpgradePurchase(Action handler)
	{
		if (this._onUpgradePurchase != null)
		{
			this._onUpgradePurchase = (Action)Delegate.Remove(this._onUpgradePurchase, handler);
		}
	}

	public static PurchaseHandler Instance
	{
		get
		{
			if (PurchaseHandler._instance == null)
			{
				PurchaseHandler._instance = new PurchaseHandler();
			}
			return PurchaseHandler._instance;
		}
	}

	private static PurchaseHandler _instance;

	private Action _onUpgradePurchase;
}
