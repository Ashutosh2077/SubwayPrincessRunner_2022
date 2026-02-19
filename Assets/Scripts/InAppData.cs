using System;
using System.Collections.Generic;
using UnityEngine;

public static class InAppData
{
	public static Dictionary<int, InAppProfile> inAppData = new Dictionary<int, InAppProfile>
	{
		{
			0,
			new InAppProfile
			{
				amountOfCoins = 7500,
				title = "Stack of Coins",
				iconName = "jinbi1",
				type = InAppData.DataType.Coin,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_COIN1),
				isConsumable = true
			}
		},
		{
			1,
			new InAppProfile
			{
				amountOfCoins = 18000,
				title = "Stack of Coins",
				iconName = "jinbi2",
				type = InAppData.DataType.Coin,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_COIN2),
				isConsumable = true
			}
		},
		{
			2,
			new InAppProfile
			{
				amountOfCoins = 30000,
				title = "Stack of Coins",
				iconName = "jinbi3",
				type = InAppData.DataType.Coin,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_COIN3),
				isConsumable = true
			}
		},
		{
			3,
			new InAppProfile
			{
				amountOfCoins = 45000,
				title = "Stack of Coins",
				iconName = "jinbi4",
				type = InAppData.DataType.Coin,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_COIN4),
				isConsumable = true
			}
		},
		{
			4,
			new InAppProfile
			{
				amountOfCoins = 100000,
				title = "Stack of Coins",
				iconName = "jinbi5",
				type = InAppData.DataType.Coin,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_COIN5),
				isConsumable = true
			}
		},
		{
			5,
			new InAppProfile
			{
				amountOfKeys = 10,
				title = "Sack of Gems",
				iconName = "diamond1",
				type = InAppData.DataType.Key,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_GEM1),
				isConsumable = true
			}
		},
		{
			6,
			new InAppProfile
			{
				amountOfKeys = 25,
				title = "Sack of Gems",
				iconName = "diamond2",
				type = InAppData.DataType.Key,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_GEM2),
				isConsumable = true
			}
		},
		{
			7,
			new InAppProfile
			{
				amountOfKeys = 80,
				title = "Sack of Gems",
				iconName = "diamond3",
				type = InAppData.DataType.Key,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_GEM3),
				isConsumable = true
			}
		},
		{
			8,
			new InAppProfile
			{
				amountOfKeys = 300,
				title = "Sack of Gems",
				iconName = "diamond4",
				type = InAppData.DataType.Key,
				price = Strings.Get(LanguageKey.UI_SCREEN_SHOP_PAY_PRICE_GEM4),
				isConsumable = true
			}
		}
	};

	public enum DataType
	{
		None,
		Coin,
		Key,
		Bundle
	}
}
