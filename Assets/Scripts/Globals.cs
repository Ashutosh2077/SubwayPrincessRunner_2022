using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Globals
{
	public static string[] convertAllBoolToString(bool[] array)
	{
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].ToString();
		}
		return array2;
	}

	public static string[] convertAllIntToString(int[] array)
	{
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].ToString();
		}
		return array2;
	}

	public static bool[] convertAllStringToBool(string[] array)
	{
		bool[] array2 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = bool.Parse(array[i]);
		}
		return array2;
	}

	public static int[] convertAllStringToInt(string[] array)
	{
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = int.Parse(array[i]);
		}
		return array2;
	}

	public static string convertEnumBoolDictionaryToString<T>(Dictionary<T, bool> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, bool> keyValuePair in sourceDict)
		{
			string name = Enum.GetName(typeof(T), keyValuePair.Key);
			string arg = keyValuePair.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static string convertEnumIntArrayDictionaryToString<T>(Dictionary<T, int[]> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int[]> keyValuePair in sourceDict)
		{
			string name = Enum.GetName(typeof(T), keyValuePair.Key);
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < keyValuePair.Value.Length; i++)
			{
				stringBuilder2.AppendFormat("{0},", keyValuePair.Value[i]);
			}
			stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
			stringBuilder.AppendFormat("{0}-{1};", name, stringBuilder2.ToString());
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static string convertEnumIntDictionaryToString<T>(Dictionary<T, int> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int> keyValuePair in sourceDict)
		{
			string name = Enum.GetName(typeof(T), keyValuePair.Key);
			string arg = keyValuePair.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static string convertStringStringDictionaryToString(Dictionary<string, string> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in sourceDict)
		{
			string key = keyValuePair.Key;
			string value = keyValuePair.Value;
			stringBuilder.AppendFormat("{0},{1},", key, value);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static string convertStringIntDictionaryToString(Dictionary<string, int> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, int> keyValuePair in sourceDict)
		{
			string key = keyValuePair.Key;
			string arg = keyValuePair.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", key, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<T, bool> convertStringToEnumBoolDictionary<T>(string sourceString)
	{
		char[] separator = new char[]
		{
			','
		};
		string[] array = sourceString.Split(separator);
		Dictionary<T, bool> dictionary = new Dictionary<T, bool>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string value2 = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)((object)Enum.Parse(typeFromHandle, value, true));
				bool value3;
				if (bool.TryParse(value2, out value3))
				{
					dictionary[key] = value3;
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<T, int[]> convertStringToEnumIntArrayDictionary<T>(string sourceString)
	{
		char[] separator = new char[]
		{
			';'
		};
		string[] array = sourceString.Split(separator);
		Dictionary<T, int[]> dictionary = new Dictionary<T, int[]>(array.Length);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				char[] separator2 = new char[]
				{
					'-'
				};
				string[] array2 = array[i].Split(separator2);
				if (array2.Length == 2)
				{
					string value = array2[0];
					string text = array2[1];
					if (Enum.IsDefined(typeFromHandle, value))
					{
						T key = (T)((object)Enum.Parse(typeFromHandle, value, true));
						char[] separator3 = new char[]
						{
							','
						};
						string[] array3 = text.Split(separator3);
						int[] array4 = new int[array3.Length];
						for (int j = 0; j < array3.Length; j++)
						{
							int num;
							if (int.TryParse(array3[j], out num))
							{
								array4[j] = num;
							}
						}
						dictionary[key] = array4;
					}
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<T, int> convertStringToEnumIntDictionary<T>(string sourceString)
	{
		char[] separator = new char[]
		{
			','
		};
		string[] array = sourceString.Split(separator);
		Dictionary<T, int> dictionary = new Dictionary<T, int>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string s = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)((object)Enum.Parse(typeFromHandle, value, true));
				int value2;
				if (int.TryParse(s, out value2))
				{
					dictionary[key] = value2;
				}
				else
				{
					UnityEngine.Debug.LogError("Source string could not parse int", null);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, string> convertStringToStringStringDictionary(string sourceString)
	{
		char[] separator = new char[]
		{
			','
		};
		string[] array = sourceString.Split(separator);
		Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length / 2);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string key = array[i];
			string value = array[i + 1];
			dictionary[key] = value;
		}
		return dictionary;
	}

	public static Dictionary<string, int> convertStringToStringIntDictionary(string sourceString)
	{
		char[] separator = new char[]
		{
			','
		};
		string[] array = sourceString.Split(separator);
		Dictionary<string, int> dictionary = new Dictionary<string, int>(array.Length / 2);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string key = array[i];
			int value;
			if (int.TryParse(array[i + 1], out value))
			{
				dictionary[key] = value;
			}
		}
		return dictionary;
	}

	public static bool TryAddAnimationEvent(Animation animation, string clipName, AnimationEvent aniEvent)
	{
		int i = 0;
		int count = Globals.addedAnimEvents.Count;
		while (i < count)
		{
			if (Globals.addedAnimEvents[i].animation == animation && Globals.addedAnimEvents[i].clipName == clipName && Globals.addedAnimEvents[i].time == aniEvent.time && Globals.addedAnimEvents[i].functionName == aniEvent.functionName)
			{
				return false;
			}
			i++;
		}
		animation[clipName].clip.AddEvent(aniEvent);
		Globals.addedAnimEvents.Add(new Globals.AddedAnimationEventInfo(animation, clipName, aniEvent.time, aniEvent.functionName));
		return true;
	}

	public static string GetUserDataPath()
	{
		return Application.persistentDataPath;
	}

	public static string AppStoreURL
	{
		get
		{
			return "test";
		}
	}

	private static List<Globals.AddedAnimationEventInfo> addedAnimEvents = new List<Globals.AddedAnimationEventInfo>();

	private struct AddedAnimationEventInfo
	{
		public AddedAnimationEventInfo(Animation animation, string clipName, float time, string functionName)
		{
			this.animation = animation;
			this.clipName = clipName;
			this.time = time;
			this.functionName = functionName;
		}

		public Animation animation;

		public string clipName;

		public float time;

		public string functionName;
	}

	public enum LogInState
	{
		Offline,
		Facebook,
		GameCenter,
		Both
	}
}
