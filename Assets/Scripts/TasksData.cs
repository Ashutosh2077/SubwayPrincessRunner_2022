using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class TasksData
{
	public static void LoadFiles()
	{
		TasksData.LoadRepeatableTasks();
		TasksData.LoadStorylineTasks();
		TasksData.LoadTaskTemplates();
	}

	private static void SaveDyadicArrayFile(Task[][] taskss, string filename)
	{
		List<object> list = new List<object>();
		List<object> list2 = new List<object>();
		foreach (Task[] array in taskss)
		{
			list2.Add(Task.ToJson(array[0]));
			list2.Add(Task.ToJson(array[1]));
			list2.Add(Task.ToJson(array[2]));
			list.Add(Json.Serialize(list2));
			list2.Clear();
		}
		string value = Json.Serialize(list);
		string path = string.Concat(new object[]
		{
			Application.dataPath,
			"/Resources/Text/Task",
			Path.DirectorySeparatorChar,
			filename
		});
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static void SaveRepeatableTasks()
	{
		TasksData.SaveDyadicArrayFile(TasksData.repeatableTasks, "repeatableTasks.txt");
	}

	public static void SaveStorylineTasks()
	{
		TasksData.SaveDyadicArrayFile(TasksData.singleuseTasks, "storylineTasks.txt");
	}

	public static void SaveTaskTemplates()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<TaskType, TaskTemplate> keyValuePair in TasksData.taskTemplates)
		{
			dictionary.Add(keyValuePair.Key.ToString(), TaskTemplate.ToJson(keyValuePair.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Task/taskTemplates.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	private static void LoadDyadicArrayFile(out Task[][] taskss, string path)
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/" + path);
		if (textAsset == null)
		{
			taskss = new Task[0][];
			UnityEngine.Debug.LogError("The file is not exist.");
			return;
		}
		List<object> list = Json.Deserialize(textAsset.text) as List<object>;
		taskss = new Task[list.Count][];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			List<object> list2 = Json.Deserialize((string)list[i]) as List<object>;
			taskss[i] = new Task[]
			{
				Task.Parse((string)list2[0]),
				Task.Parse((string)list2[1]),
				Task.Parse((string)list2[2])
			};
			i++;
		}
	}

	public static void LoadRepeatableTasks()
	{
		TasksData.LoadDyadicArrayFile(out TasksData.repeatableTasks, "repeatableTasks");
	}

	public static void LoadStorylineTasks()
	{
		TasksData.LoadDyadicArrayFile(out TasksData.singleuseTasks, "storylineTasks");
	}

	public static void LoadTaskTemplates()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/taskTemplates");
		if (textAsset == null)
		{
			UnityEngine.Debug.LogError("The file is not exist.");
			return;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		TasksData.taskTemplates = new Dictionary<TaskType, TaskTemplate>();
		foreach (KeyValuePair<string, object> keyValuePair in dictionary)
		{
			TasksData.taskTemplates.Add((TaskType)Enum.Parse(typeof(TaskType), keyValuePair.Key), TaskTemplate.Parse((string)keyValuePair.Value));
		}
	}

	public static Dictionary<TaskType, TaskTemplate> taskTemplates;

	public static Task[][] repeatableTasks;

	public static Task[][] singleuseTasks;
}
