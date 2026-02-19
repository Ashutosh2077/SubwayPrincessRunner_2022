using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Characters
{
	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Characters/data");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		Characters.characterData = new Dictionary<Characters.CharacterType, Characters.Model>();
		Characters.characterOrder = new List<Characters.CharacterType>(Characters.characterData.Count);
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			Characters.characterData.Add((Characters.CharacterType)Enum.Parse(typeof(Characters.CharacterType), keyValuePair.Key), Characters.Model.Parse((string)keyValuePair.Value));
			Characters.characterOrder.Add(Characters.CharacterType.slick);
		}
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> keyValuePair2 in Characters.characterData)
		{
			Characters.characterOrder[keyValuePair2.Value.order] = keyValuePair2.Key;
		}
		return true;
	}

	public static bool SaveFile()
	{
		if (Characters.characterData == null || Characters.characterData.Count <= 0)
		{
			return false;
		}
		if (Characters.characterData.ContainsKey(Characters.CharacterType.none))
		{
			return false;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> keyValuePair in Characters.characterData)
		{
			dictionary.Add(keyValuePair.Key.ToString(), Characters.Model.ToJson(keyValuePair.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Characters/data.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
		return true;
	}

	public static Dictionary<Characters.CharacterType, Characters.Model> characterData;

	public static List<Characters.CharacterType> characterOrder;

	public enum CharacterType
	{
		slick,
		clown,
		strong,
		spike,
		yutani,
		frank,
		lee,
		turtlefok,
		nijia,
		darcy,
		venice,
		none
	}

	public class Model
	{
		public Model()
		{
		}

		public Model(Characters.Model model)
		{
			this.name = model.name;
			this.modelName = model.modelName;
			this.Price = model.Price;
			this.unlockType = model.unlockType;
			this.symbolName = model.symbolName;
			this.symbolSprite2dName = model.symbolSprite2dName;
			this.taskTargetKey = model.taskTargetKey;
			this.freeReviveCount = model.freeReviveCount;
			this.order = model.order;
		}

		public static string ToJson(Characters.Model model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			return Json.Serialize(new Dictionary<string, object>
			{
				{
					"name",
					model.name.ToString()
				},
				{
					"modelName",
					model.modelName
				},
				{
					"Price",
					model.Price
				},
				{
					"unlockType",
					model.unlockType.ToString()
				},
				{
					"symbolName",
					model.symbolName.ToString()
				},
				{
					"symbolSprite2dName",
					model.symbolSprite2dName
				},
				{
					"taskTargetKey",
					model.taskTargetKey.ToString()
				},
				{
					"freeReviveCount",
					model.freeReviveCount
				},
				{
					"order",
					model.order
				}
			});
		}

		public static Characters.Model Parse(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
			if (dictionary == null || dictionary.Count <= 0)
			{
				return null;
			}
			Characters.Model model = new Characters.Model();
			if (dictionary.ContainsKey("name"))
			{
				model.name = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["name"]);
			}
			if (dictionary.ContainsKey("modelName"))
			{
				model.modelName = (string)dictionary["modelName"];
			}
			if (dictionary.ContainsKey("Price"))
			{
				model.Price = (int)((long)dictionary["Price"]);
			}
			if (dictionary.ContainsKey("unlockType"))
			{
				model.unlockType = (Characters.UnlockType)Enum.Parse(typeof(Characters.UnlockType), (string)dictionary["unlockType"]);
			}
			if (dictionary.ContainsKey("symbolName"))
			{
				model.symbolName = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["symbolName"]);
			}
			if (dictionary.ContainsKey("symbolSprite2dName"))
			{
				model.symbolSprite2dName = (string)dictionary["symbolSprite2dName"];
			}
			if (dictionary.ContainsKey("taskTargetKey"))
			{
				model.taskTargetKey = (TaskTarget)Enum.Parse(typeof(TaskTarget), (string)dictionary["taskTargetKey"]);
			}
			if (dictionary.ContainsKey("freeReviveCount"))
			{
				model.freeReviveCount = (int)((long)dictionary["freeReviveCount"]);
			}
			if (dictionary.ContainsKey("order"))
			{
				model.order = (int)((long)dictionary["order"]);
			}
			return model;
		}

		public LanguageKey name;

		public string modelName;

		public int Price;

		public Characters.UnlockType unlockType;

		public LanguageKey symbolName;

		public string symbolSprite2dName;

		public TaskTarget taskTargetKey;

		public int freeReviveCount;

		public int order;
	}

	public enum UnlockType
	{
		free,
		symbols,
		coins,
		keys,
		subscription
	}
}
