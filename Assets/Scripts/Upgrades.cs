using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Upgrades
{
	public static void LoadFile()
	{
		Upgrades.Load();
	}

	public static Dictionary<PropType, Upgrade> Load()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Upgrades");
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		Upgrades.upgrades = new Dictionary<PropType, Upgrade>();
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			Upgrades.upgrades.Add((PropType)Enum.Parse(typeof(PropType), keyValuePair.Key), Upgrade.Parse((string)keyValuePair.Value));
		}
		return Upgrades.upgrades;
	}

	public static void Save()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<PropType, Upgrade> keyValuePair in Upgrades.upgrades)
		{
			dictionary.Add(keyValuePair.Key.ToString(), Upgrade.ToJson(keyValuePair.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Upgrades.txt";
		try
		{
			using (StreamWriter streamWriter = File.CreateText(path))
			{
				streamWriter.WriteLine(value);
				streamWriter.Close();
			}
		}
		catch (Exception message)
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	public static Dictionary<PropType, Upgrade> upgrades;
}
