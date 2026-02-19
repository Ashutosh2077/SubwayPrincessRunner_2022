using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Achievements
{
	public static Task[] LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/Achievements");
		if (textAsset == null)
		{
			UnityEngine.Debug.Log("The file is not exist.");
			return null;
		}
		IList<object> list = Json.Deserialize(textAsset.text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			UnityEngine.Debug.Log("Error, the file is empty.");
			return null;
		}
		Achievements._achievementArray = new Task[Achievements.NUMBER_OF_ACHIEVEMENTS];
		Achievements._achievementInfos = new AchievementInfo[Achievements.NUMBER_OF_ACHIEVEMENTS];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			if (i >= Achievements.NUMBER_OF_ACHIEVEMENTS)
			{
				break;
			}
			string text = (string)list[i];
			string[] array = text.Split(new char[]
			{
				'-'
			});
			Achievements._achievementArray[i] = Task.Parse(array[0]);
			Achievements._achievementInfos[i] = new AchievementInfo().Parse(array[1]);
			i++;
		}
		while (i < Achievements.NUMBER_OF_ACHIEVEMENTS)
		{
			Achievements._achievementArray[i] = new Task();
			Achievements._achievementInfos[i] = new AchievementInfo();
			i++;
		}
		return Achievements._achievementArray;
	}

	public static void SaveFile()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Achievements._achievementArray.Length; i++)
		{
			list.Add(Task.ToJson(Achievements._achievementArray[i]) + "-" + Achievements._achievementInfos[i].ToJson());
		}
		string value = Json.Serialize(list);
		string path = Application.dataPath + "/Resources/Text/Task/Achievements.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static int getAchievementIndexInArray(TaskType type)
	{
		for (int i = 0; i < Achievements.NUMBER_OF_ACHIEVEMENTS; i++)
		{
			if (Achievements.achievementArray[i].type == type)
			{
				return i;
			}
		}
		return -1;
	}

	public static void Init()
	{
		Achievements._achievementArray = new Task[Achievements.NUMBER_OF_ACHIEVEMENTS];
		Achievements._achievementInfos = new AchievementInfo[Achievements.NUMBER_OF_ACHIEVEMENTS];
		for (int i = 0; i < Achievements.NUMBER_OF_ACHIEVEMENTS; i++)
		{
			Achievements._achievementArray[i] = new Task();
			Achievements._achievementInfos[i] = new AchievementInfo();
		}
		Achievements.SaveFile();
	}

	public static Task[] achievementArray
	{
		get
		{
			return Achievements._achievementArray;
		}
	}

	public static AchievementInfo[] achievementInfo
	{
		get
		{
			return Achievements._achievementInfos;
		}
	}

	private static Task[] _achievementArray;

	private static AchievementInfo[] _achievementInfos;

	private static Achievements _instance;

	public static int NUMBER_OF_ACHIEVEMENTS = 20;
}
