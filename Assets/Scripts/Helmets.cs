using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Helmets
{
	public static bool Save()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (Helmets.helmData == null)
		{
			return false;
		}
		foreach (KeyValuePair<Helmets.HelmType, Helmets.Helm> keyValuePair in Helmets.helmData)
		{
			dictionary.Add(keyValuePair.Key.ToString(), Helmets.Helm.ToJson(keyValuePair.Value));
		}
		string value = Json.Serialize(dictionary);
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		using (StreamWriter streamWriter = File.CreateText(Application.dataPath + "/Resources/" + Helmets.path + ".txt"))
		{
			streamWriter.WriteLine(value);
		}
		List<object> list = new List<object>();
		foreach (Helmets.HelmType helmType in Helmets.helmOrder)
		{
			list.Add(helmType);
		}
		string value2 = Json.Serialize(list);
		if (string.IsNullOrEmpty(value2))
		{
			return false;
		}
		using (StreamWriter streamWriter2 = File.CreateText(Application.dataPath + "/Resources/" + Helmets.path + "Order.txt"))
		{
			streamWriter2.WriteLine(value2);
		}
		return true;
	}

	public static bool Load()
	{
		Helmets.helmData = new Dictionary<Helmets.HelmType, Helmets.Helm>();
		TextAsset textAsset = Resources.Load<TextAsset>(Helmets.path);
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			try
			{
				Helmets.HelmType key = (Helmets.HelmType)Enum.Parse(typeof(Helmets.HelmType), keyValuePair.Key);
				Helmets.Helm value = Helmets.Helm.Parse((string)keyValuePair.Value);
				if (Helmets.helmData.ContainsKey(key))
				{
					UnityEngine.Debug.LogError(keyValuePair.Key + " has exist!");
					return false;
				}
				Helmets.helmData.Add(key, value);
			}
			catch (ArgumentException)
			{
				UnityEngine.Debug.LogError(keyValuePair.Key + " is not a membor of HelmType!");
				return false;
			}
		}
		Helmets.helmOrder = new List<Helmets.HelmType>(Helmets.helmData.Count);
		textAsset = Resources.Load<TextAsset>(Helmets.path + "Order");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IList<object> list = Json.Deserialize(textAsset.text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		foreach (object obj in list)
		{
			try
			{
				Helmets.HelmType item = (Helmets.HelmType)Enum.Parse(typeof(Helmets.HelmType), (string)obj);
				if (Helmets.helmOrder.Contains(item))
				{
					UnityEngine.Debug.LogError((string)obj + " has exist!");
					return false;
				}
				Helmets.helmOrder.Add(item);
			}
			catch (ArgumentException)
			{
				UnityEngine.Debug.LogError((string)obj + " is not a membor of HelmType!");
				return false;
			}
		}
		return true;
	}

	private static string path = "Text/Helm";

	public static Dictionary<Helmets.HelmType, Helmets.Helm> helmData;

	public static List<Helmets.HelmType> helmOrder;

	public struct Helm
	{
		public static string ToJson(Helmets.Helm helm)
		{
			return Json.Serialize(new Dictionary<string, object>
			{
				{
					"name",
					helm.name.ToString()
				},
				{
					"helmModelName",
					helm.helmModelName
				},
				{
					"price",
					helm.price
				},
				{
					"unlockType",
					helm.unlockType
				},
				{
					"description",
					helm.description.ToString()
				},
				{
					"useMagent",
					helm.useMagent
				},
				{
					"useMutiplier",
					helm.useMutiplier
				}
			});
		}

		public static Helmets.Helm Parse(string json)
		{
			Helmets.Helm result = default(Helmets.Helm);
			IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
			if (dictionary.ContainsKey("name"))
			{
				try
				{
					result.name = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["name"]);
				}
				catch (ArgumentException)
				{
					UnityEngine.Debug.LogError((string)dictionary["name"] + " is not one of StringID!");
				}
			}
			if (dictionary.ContainsKey("helmModelName"))
			{
				result.helmModelName = (string)dictionary["helmModelName"];
			}
			if (dictionary.ContainsKey("price"))
			{
				result.price = (int)((long)dictionary["price"]);
			}
			if (dictionary.ContainsKey("unlockType"))
			{
				try
				{
					result.unlockType = (Helmets.UnlockType)Enum.Parse(typeof(Helmets.UnlockType), (string)dictionary["unlockType"]);
				}
				catch (ArgumentException)
				{
					UnityEngine.Debug.LogError((string)dictionary["unlockType"] + " is not one of UnlockType!");
				}
			}
			if (dictionary.ContainsKey("description"))
			{
				try
				{
					result.description = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["description"]);
				}
				catch (ArgumentException)
				{
					UnityEngine.Debug.LogError((string)dictionary["description"] + " is not one of StringID!");
				}
			}
			if (dictionary.ContainsKey("useMagent"))
			{
				result.useMagent = (bool)dictionary["useMagent"];
			}
			if (dictionary.ContainsKey("useMutiplier"))
			{
				result.useMutiplier = (bool)dictionary["useMutiplier"];
			}
			return result;
		}

		public LanguageKey name;

		public string helmModelName;

		public int price;

		public Helmets.UnlockType unlockType;

		public LanguageKey description;

		public bool useMagent;

		public bool useMutiplier;
	}

	public enum HelmType
	{
		notset,
		normal,
		bouncer,
		snowhelm,
		miami,
		monster,
		rome,
		star
	}

	public enum UnlockType
	{
		alwaysUnlocked,
		free,
		coins,
		hiddenUntillUnlocked,
		keys
	}
}
