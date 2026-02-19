using System;
using UnityEngine;

public class Strings
{
	public static Strings.DocumentFormat documentFormat
	{
		get
		{
			return Strings._documentFormat;
		}
		set
		{
			if (value == Strings.DocumentFormat.TXT)
			{
				Strings.LOCALE_FILES_FOLDER = "Text";
				Strings.LOCALE_FILES_MANDATORY_ENDING = "_locale";
			}
			else if (value == Strings.DocumentFormat.CSV)
			{
				Strings.LOCALE_FILES_FOLDER = "CSV";
				Strings.LOCALE_FILES_MANDATORY_ENDING = "_locale";
			}
			Strings._documentFormat = value;
		}
	}

	public static bool Exists(string keyString)
	{
		try
		{
			Strings.Get(keyString);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	public static string Get(string keyString)
	{
		return string.IsNullOrEmpty(keyString) ? null : Strings.Get((LanguageKey)((int)Enum.Parse(typeof(LanguageKey), keyString, true)));
	}

	public static string Get(LanguageKey key)
	{
		if (Strings.language == null)
		{
			Strings.LogWarning("Strings not loaded. Loading default language. Tried to get string: " + key, null);
			Strings.Language = "english";
		}
		return Strings.values[(int)key];
	}

	private static void Load(string language)
	{
		if (Strings.language != language)
		{
			Strings.language = language;
			if (Strings.values == null)
			{
				int[] array = (int[])Enum.GetValues(typeof(LanguageKey));
				Strings.values = new string[array[array.Length - 1] + 1];
			}
			else
			{
				for (int i = 0; i < Strings.values.Length; i++)
				{
					Strings.values[i] = null;
				}
			}
			TextAsset textAsset = (TextAsset)Resources.Load(Strings.LOCALE_FILES_FOLDER + "/" + language + Strings.LOCALE_FILES_MANDATORY_ENDING, typeof(TextAsset));
			string text = textAsset.text;
			int num = 0;
			string value;
			string text2;
			while ((num = StringUtility.GetNextKeyValuePair(text, num, out value, out text2)) >= 0)
			{
				int num2 = (int)Enum.Parse(typeof(LanguageKey), value, true);
				Strings.values[num2] = text2;
				if (num == text.Length)
				{
					break;
				}
			}
			if (Strings.values[0] != null)
			{
				throw new Exception("Strings.Load: String set for " + Enum.GetName(typeof(LanguageKey), 0));
			}
			for (int j = 1; j < Strings.values.Length; j++)
			{
				if (Strings.values[j] == null && Enum.IsDefined(typeof(LanguageKey), j))
				{
					throw new Exception("Strings.Load: String not set for " + Enum.GetName(typeof(LanguageKey), j));
				}
			}
		}
	}

	private static bool LoadCSV(string language)
	{
		if (!(Strings.language != language))
		{
			return true;
		}
		Strings.language = language;
		if (Strings.values == null)
		{
			int[] array = (int[])Enum.GetValues(typeof(LanguageKey));
			Strings.values = new string[array[array.Length - 1] + 1];
		}
		else
		{
			for (int i = 0; i < Strings.values.Length; i++)
			{
				Strings.values[i] = null;
			}
		}
		TextAsset textAsset = (TextAsset)Resources.Load(Strings.LOCALE_FILES_FOLDER + "/" + language + Strings.LOCALE_FILES_MANDATORY_ENDING, typeof(TextAsset));
		if (textAsset == null)
		{
			return false;
		}
		CSVReader csvreader = new CSVReader(textAsset.bytes);
		for (;;)
		{
			BetterList<string> betterList = csvreader.ReadLine();
			if (betterList == null || betterList.size == 0)
			{
				break;
			}
			if (!string.IsNullOrEmpty(betterList[0]))
			{
				int num = (int)Enum.Parse(typeof(LanguageKey), betterList[0]);
				Strings.values[num] = betterList[1];
			}
		}
		return true;
	}

	public static void LogWarning(string msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(msg, context);
	}

	public static string Language
	{
		get
		{
			return Strings.language;
		}
		set
		{
			if (Strings._documentFormat == Strings.DocumentFormat.CSV)
			{
				Strings.LoadCSV(value);
			}
			else if (Strings._documentFormat == Strings.DocumentFormat.TXT)
			{
				Strings.Load(value);
			}
		}
	}

	private static string language;

	public static string LOCALE_FILES_FOLDER = "Text";

	public static string LOCALE_FILES_MANDATORY_ENDING = "_locale";

	public static string[] values;

	private static Strings.DocumentFormat _documentFormat;

	public enum DocumentFormat
	{
		TXT,
		CSV
	}
}
