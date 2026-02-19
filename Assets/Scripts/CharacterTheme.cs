using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;

public class CharacterTheme
{
	public static string ToJson(CharacterTheme theme)
	{
		return Json.Serialize(new Dictionary<string, object>
		{
			{
				"price",
				theme.price
			},
			{
				"unlockType",
				theme.unlockType.ToString()
			},
			{
				"buttonBgSpriteName",
				theme.buttonBgSpriteName
			},
			{
				"buttonIconSpriteName",
				theme.buttonIconSpriteName
			},
			{
				"unlockDescription",
				theme.unlockDescription.ToString()
			},
			{
				"title",
				theme.title.ToString()
			},
			{
				"description",
				theme.description
			},
			{
				"uiPriority",
				theme.uiPriority
			}
		});
	}

	public static CharacterTheme Parse(string json)
	{
		CharacterTheme characterTheme = new CharacterTheme();
		IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
		if (dictionary.ContainsKey("price"))
		{
			characterTheme.price = (int)((long)dictionary["price"]);
		}
		if (dictionary.ContainsKey("unlockType"))
		{
			try
			{
				Characters.UnlockType unlockType = (Characters.UnlockType)Enum.Parse(typeof(Characters.UnlockType), (string)dictionary["unlockType"]);
				characterTheme.unlockType = unlockType;
			}
			catch (ArgumentException)
			{
				characterTheme.unlockType = Characters.UnlockType.coins;
			}
		}
		if (dictionary.ContainsKey("buttonBgSpriteName"))
		{
			characterTheme.buttonBgSpriteName = (string)dictionary["buttonBgSpriteName"];
		}
		if (dictionary.ContainsKey("buttonIconSpriteName"))
		{
			characterTheme.buttonIconSpriteName = (string)dictionary["buttonIconSpriteName"];
		}
		if (dictionary.ContainsKey("unlockDescription"))
		{
			try
			{
				LanguageKey languageKey = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["unlockDescription"]);
				characterTheme.unlockDescription = languageKey;
			}
			catch (ArgumentException)
			{
				characterTheme.unlockDescription = LanguageKey.TAG_THEME_UNLOCK_DESCRIPTION;
			}
		}
		if (dictionary.ContainsKey("title"))
		{
			try
			{
				LanguageKey languageKey2 = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["title"]);
				characterTheme.title = languageKey2;
			}
			catch (ArgumentException)
			{
				characterTheme.title = LanguageKey.TAG_SMOOTH_THEME_TITLE;
			}
		}
		if (dictionary.ContainsKey("description"))
		{
			characterTheme.description = (string)dictionary["description"];
		}
		if (dictionary.ContainsKey("uiPriority"))
		{
			characterTheme.uiPriority = (int)((long)dictionary["uiPriority"]);
		}
		return characterTheme;
	}

	public static string Color32ToString(Color32 color)
	{
		return string.Format("{0},{1},{2},{3}", new object[]
		{
			color.r,
			color.g,
			color.b,
			color.a
		});
	}

	public static Color32 StringToColor32(string str)
	{
		string[] array = str.Split(new char[]
		{
			','
		});
		Color32 result = default(Color32);
		if (array.Length >= 4)
		{
			byte b;
			if (byte.TryParse(array[0], out b))
			{
				result.r = b;
			}
			if (byte.TryParse(array[1], out b))
			{
				result.g = b;
			}
			if (byte.TryParse(array[2], out b))
			{
				result.b = b;
			}
			if (byte.TryParse(array[3], out b))
			{
				result.a = b;
			}
		}
		return result;
	}

	public int price;

	public Characters.UnlockType unlockType;

	public string buttonBgSpriteName;

	public string buttonIconSpriteName;

	public LanguageKey unlockDescription;

	public LanguageKey title;

	public string description;

	public int uiPriority;
}
