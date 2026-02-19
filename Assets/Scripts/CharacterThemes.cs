using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class CharacterThemes
{
	public static bool SaveFile()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<Characters.CharacterType, List<CharacterTheme>> keyValuePair in CharacterThemes.characterCustomThemes)
		{
			List<object> list = new List<object>();
			foreach (CharacterTheme theme in keyValuePair.Value)
			{
				list.Add(CharacterTheme.ToJson(theme));
			}
			dictionary.Add(keyValuePair.Key.ToString(), Json.Serialize(list));
		}
		string value = Json.Serialize(dictionary);
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		string path = Application.dataPath + "/Resources/Text/CharacterThemes.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
		return true;
	}

	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/CharacterThemes");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null)
		{
			return false;
		}
		CharacterThemes.characterCustomThemes = new Dictionary<Characters.CharacterType, List<CharacterTheme>>(dictionary.Count);
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			Characters.CharacterType key;
			try
			{
				key = (Characters.CharacterType)Enum.Parse(typeof(Characters.CharacterType), keyValuePair.Key);
			}
			catch (ArgumentException)
			{
				return false;
			}
			IList<object> list = Json.Deserialize((string)keyValuePair.Value) as IList<object>;
			if (list == null)
			{
				CharacterThemes.characterCustomThemes.Add(key, new List<CharacterTheme>());
			}
			else
			{
				List<CharacterTheme> list2 = new List<CharacterTheme>(list.Count);
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					list2.Add(CharacterTheme.Parse((string)list[i]));
					i++;
				}
				CharacterThemes.characterCustomThemes.Add(key, list2);
			}
		}
		return true;
	}

	public static CharacterTheme GetThemeForCharacter(Characters.CharacterType charType, int index)
	{
		if (index != 0)
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(charType);
			if (list != null && list.Count >= index)
			{
				return list[index - 1];
			}
		}
		return null;
	}

	public static List<CharacterTheme> TryGetCustomThemesForChar(Characters.CharacterType charType)
	{
		List<CharacterTheme> result;
		CharacterThemes.characterCustomThemes.TryGetValue(charType, out result);
		return result;
	}

	public static Dictionary<Characters.CharacterType, List<CharacterTheme>> characterCustomThemes;
}
