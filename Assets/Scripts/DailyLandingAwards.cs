using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class DailyLandingAwards
{
	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/dailyLanding");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		string text = textAsset.text;
		IList<object> list = Json.Deserialize(text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		DailyLandingAwards.awards = new DailyLandingAward[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			DailyLandingAwards.awards[i] = new DailyLandingAward();
			DailyLandingAwards.awards[i].Parse((string)list[i]);
		}
		return true;
	}

	public static bool SaveFile()
	{
		if (DailyLandingAwards.awards == null || DailyLandingAwards.awards.Length <= 0)
		{
			return false;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < DailyLandingAwards.awards.Length; i++)
		{
			list.Add(DailyLandingAwards.awards[i].ToJson());
		}
		string value = Json.Serialize(list);
		string text = Application.dataPath + "/Resources/Text/dailyLanding.txt";
		using (StreamWriter streamWriter = File.CreateText(text))
		{
			streamWriter.Write(value);
			streamWriter.Close();
		}
		return true;
	}

	public static DailyLandingAward GetDailyLandingAwardByID(int dayId)
	{
		if (dayId <= 0 || dayId > DailyLandingAwards.awards.Length)
		{
			return null;
		}
		return DailyLandingAwards.awards[dayId - 1];
	}

	public static DailyLandingAward[] awards;

	private const string path = "Text/dailyLanding";
}
