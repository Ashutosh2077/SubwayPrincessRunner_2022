using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public static class Chest
{
	public static void LoadFile()
	{
		Chest.LoadPrizeTables();
		Chest.LoadCharacterSymbolWeight();
	}

	public static bool SavePrizeTables()
	{
		if (Chest._prizeTables == null || Chest._prizeTables.Length <= 0)
		{
			return false;
		}
		string path = Application.dataPath + "/Resources/Text/Chest.txt";
		List<object> list = new List<object>();
		List<object> list2 = new List<object>();
		foreach (PrizeEntry[] array in Chest._prizeTables)
		{
			list.Clear();
			foreach (Chest.PrizeEntry entry in array)
			{
				list.Add(Chest.PrizeEntry.ToJson(entry));
			}
			list2.Add(Json.Serialize(list));
		}
		string value = Json.Serialize(list2);
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
		return true;
	}

	public static bool LoadPrizeTables()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Chest");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IList<object> list = Json.Deserialize(textAsset.text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		Chest._prizeTables = new Chest.PrizeEntry[list.Count][];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			IList<object> list2 = Json.Deserialize((string)list[i]) as IList<object>;
			Chest.PrizeEntry[] array = new Chest.PrizeEntry[list2.Count];
			int j = 0;
			int count2 = list2.Count;
			while (j < count2)
			{
				array[j] = Chest.PrizeEntry.Parse((string)list2[j]);
				j++;
			}
			Chest._prizeTables[i] = array;
			i++;
		}
		return true;
	}

	public static bool SaveCharacterSymbolWeight()
	{
		if (Chest.characterSymbolWeight == null || Chest.characterSymbolWeight.Count <= 0)
		{
			return false;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<Characters.CharacterType, int> keyValuePair in Chest.characterSymbolWeight)
		{
			dictionary.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/CharacterSymbolWeight.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
		return true;
	}

	public static bool LoadCharacterSymbolWeight()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/CharacterSymbolWeight");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Chest.InitCharacterSymbolWeight();
			Chest.SaveCharacterSymbolWeight();
			return true;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		Chest.characterSymbolWeight = new Dictionary<Characters.CharacterType, int>(dictionary.Count);
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			try
			{
				Characters.CharacterType key = (Characters.CharacterType)Enum.Parse(typeof(Characters.CharacterType), keyValuePair.Key);
				Chest.characterSymbolWeight.Add(key, (int)((long)keyValuePair.Value));
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
		if (Characters.characterData == null)
		{
			Characters.LoadFile();
		}
		bool flag = false;
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> keyValuePair2 in Characters.characterData)
		{
			if ((keyValuePair2.Value.unlockType != Characters.UnlockType.symbols || !Chest.characterSymbolWeight.ContainsKey(keyValuePair2.Key)) && (keyValuePair2.Value.unlockType == Characters.UnlockType.symbols || Chest.characterSymbolWeight.ContainsKey(keyValuePair2.Key)))
			{
				if (keyValuePair2.Value.unlockType == Characters.UnlockType.symbols && !Chest.characterSymbolWeight.ContainsKey(keyValuePair2.Key))
				{
					Chest.characterSymbolWeight.Add(keyValuePair2.Key, 1);
					flag = true;
				}
				else if (keyValuePair2.Value.unlockType != Characters.UnlockType.symbols && Chest.characterSymbolWeight.ContainsKey(keyValuePair2.Key))
				{
					Chest.characterSymbolWeight.Remove(keyValuePair2.Key);
					flag = true;
				}
			}
		}
		if (flag)
		{
			Chest.SaveCharacterSymbolWeight();
		}
		return true;
	}

	private static void InitCharacterSymbolWeight()
	{
		Chest.characterSymbolWeight = new Dictionary<Characters.CharacterType, int>();
		Characters.LoadFile();
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> keyValuePair in Characters.characterData)
		{
			if (keyValuePair.Value.unlockType == Characters.UnlockType.symbols)
			{
				Chest.characterSymbolWeight.Add(keyValuePair.Key, 1);
			}
		}
	}

	private static List<Characters.CharacterType> GetAccessibleCharacterSymbols()
	{
		List<Characters.CharacterType> list = new List<Characters.CharacterType>();
		foreach (Characters.CharacterType characterType in Chest.characterSymbolWeight.Keys)
		{
			if (PlayerInfo.Instance.IsSymbolUseful(characterType))
			{
				list.Add(characterType);
			}
		}
		return list;
	}

	private static Characters.CharacterType GetRandomCharacterSymbolType(List<Characters.CharacterType> validCharacters)
	{
		Characters.CharacterType result = Characters.CharacterType.darcy;
		int num = 0;
		int i = 0;
		int count = validCharacters.Count;
		while (i < count)
		{
			num += Chest.characterSymbolWeight[validCharacters[i]];
			i++;
		}
		int num2 = new System.Random().Next(0, num) + 1;
		int num3 = 0;
		for (int j = 0; j < validCharacters.Count; j++)
		{
			num3 += Chest.characterSymbolWeight[validCharacters[j]];
			if (num2 <= num3)
			{
				return validCharacters[j];
			}
		}
		return result;
	}

	private static Chest.PrizeEntry RollFromPrizeEntry(Chest.Type chestType)
	{
		float num = 0f;
		for (int i = 0; i < Chest._prizeTables[(int)chestType].Length; i++)
		{
			Chest.PrizeEntry prizeEntry = Chest._prizeTables[(int)chestType][i];
			num += prizeEntry.weight;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		for (int j = 0; j < Chest._prizeTables[(int)chestType].Length; j++)
		{
			Chest.PrizeEntry prizeEntry2 = Chest._prizeTables[(int)chestType][j];
			num3 += prizeEntry2.weight;
			if (num2 <= num3)
			{
				return prizeEntry2;
			}
		}
		return null;
	}

	public static CelebrationReward Roll(Chest.Type chestType)
	{
		CelebrationReward celebrationReward = Chest.RewardOfFirstOpen(chestType);
		if (celebrationReward != null)
		{
			return celebrationReward;
		}
		List<Characters.CharacterType> accessibleCharacterSymbols = Chest.GetAccessibleCharacterSymbols();
		celebrationReward = new CelebrationReward();
		Chest.PrizeEntry prizeEntry = Chest.RollFromPrizeEntry(chestType);
		while (prizeEntry.itemType == CelebrationRewardType.symbol && accessibleCharacterSymbols.Count <= 0)
		{
			prizeEntry = Chest.RollFromPrizeEntry(chestType);
		}
		if (prizeEntry == null)
		{
			UnityEngine.Debug.LogError("A reward was empty, how did this happen?");
		}
		if (prizeEntry.itemType == CelebrationRewardType.coins)
		{
			celebrationReward.rewardType = CelebrationRewardType.coins;
			celebrationReward.amount = UnityEngine.Random.Range(prizeEntry.min, prizeEntry.max);
			if (celebrationReward.amount >= 5000)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.ChestGrandPrize, 1, -1);
			}
			return celebrationReward;
		}
		if (prizeEntry.itemType == CelebrationRewardType.keys)
		{
			celebrationReward.rewardType = CelebrationRewardType.keys;
			celebrationReward.amount = UnityEngine.Random.Range(prizeEntry.min, prizeEntry.max);
			return celebrationReward;
		}
		if (prizeEntry.itemType == CelebrationRewardType.powerup)
		{
			celebrationReward.rewardType = CelebrationRewardType.powerup;
			celebrationReward.powerupType = prizeEntry.propType;
			celebrationReward.amount = UnityEngine.Random.Range(prizeEntry.min, prizeEntry.max);
			return celebrationReward;
		}
		if (prizeEntry.itemType != CelebrationRewardType.symbol)
		{
			return null;
		}
		if (accessibleCharacterSymbols.Count > 0)
		{
			celebrationReward.rewardType = CelebrationRewardType.symbol;
			celebrationReward.amount = UnityEngine.Random.Range(prizeEntry.min, prizeEntry.max);
			celebrationReward.characterType = Chest.GetRandomCharacterSymbolType(accessibleCharacterSymbols);
			return celebrationReward;
		}
		UnityEngine.Debug.Log("Mystery box consolation reward");
		return null;
	}

	private static CelebrationReward RewardOfFirstOpen(Chest.Type chestType)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		PlayerInfo instance = PlayerInfo.Instance;
		if (chestType == Chest.Type.Normal)
		{
			instance.amountOfChestesOpened++;
			if (instance.amountOfChestesOpened == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.coins;
				celebrationReward.amount = 1000;
				return celebrationReward;
			}
			if (instance.amountOfChestesOpened == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.symbol;
				celebrationReward.amount = 1;
				Characters.CharacterType characterType = Characters.CharacterType.lee;
				if (instance.IsCollectionComplete(characterType))
				{
					characterType = Characters.CharacterType.turtlefok;
				}
				celebrationReward.characterType = characterType;
				return celebrationReward;
			}
			if (instance.amountOfChestesOpened == 3)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.amount = 2;
				celebrationReward.powerupType = PropType.scorebooster;
				return celebrationReward;
			}
			if (!instance.hasReceivedFirstMBKey)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				instance.hasReceivedFirstMBKey = true;
				return celebrationReward;
			}
		}
		else if (chestType == Chest.Type.Super)
		{
			if (!instance.hasReceivedFirstSMBKey)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 2;
				instance.hasReceivedFirstSMBKey = true;
				return celebrationReward;
			}
			instance.amountOfSuperChestesOpened++;
		}
		else if (chestType == Chest.Type.Mini)
		{
			instance.amountOfMiniChestesOpened++;
			if (instance.amountOfMiniChestesOpened == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.powerupType = PropType.scorebooster;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			if (instance.amountOfMiniChestesOpened == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
		}
		return null;
	}

	public static Chest.PrizeEntry[][] _prizeTables;

	public static Dictionary<Characters.CharacterType, int> characterSymbolWeight;

	public class PrizeEntry
	{
		public static string ToJson(Chest.PrizeEntry entry)
		{
			return Json.Serialize(new Dictionary<string, object>
			{
				{
					"itemType",
					entry.itemType.ToString()
				},
				{
					"weight",
					entry.weight.ToString()
				},
				{
					"min",
					entry.min
				},
				{
					"max",
					entry.max
				},
				{
					"powerupType",
					entry.propType.ToString()
				}
			});
		}

		public static Chest.PrizeEntry Parse(string json)
		{
			Chest.PrizeEntry prizeEntry = new Chest.PrizeEntry();
			IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
			if (dictionary.ContainsKey("itemType"))
			{
				try
				{
					CelebrationRewardType celebrationRewardType = (CelebrationRewardType)Enum.Parse(typeof(CelebrationRewardType), (string)dictionary["itemType"]);
					prizeEntry.itemType = celebrationRewardType;
				}
				catch (ArgumentException)
				{
					prizeEntry.itemType = CelebrationRewardType._notset;
				}
			}
			if (dictionary.ContainsKey("weight"))
			{
				prizeEntry.weight = float.Parse((string)dictionary["weight"]);
			}
			if (dictionary.ContainsKey("min"))
			{
				prizeEntry.min = (int)((long)dictionary["min"]);
			}
			if (dictionary.ContainsKey("max"))
			{
				prizeEntry.max = (int)((long)dictionary["max"]);
			}
			if (dictionary.ContainsKey("powerupType"))
			{
				try
				{
					PropType propType = (PropType)Enum.Parse(typeof(PropType), (string)dictionary["powerupType"]);
					prizeEntry.propType = propType;
				}
				catch (ArgumentException)
				{
					prizeEntry.propType = PropType._notset;
				}
			}
			return prizeEntry;
		}

		public CelebrationRewardType itemType;

		public float weight;

		public int min = 1;

		public int max = 1;

		public PropType propType;
	}

	public enum Type
	{
		Normal,
		Super,
		Mini
	}
}
