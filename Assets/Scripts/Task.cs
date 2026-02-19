using System;
using System.Collections.Generic;
using MiniJSONs;

public class Task
{
	public Task()
	{
	}

	public Task(TaskType type, int aim)
	{
		this.type = type;
		this.aim = aim;
	}

	public static string ToJson(Task task)
	{
		return Json.Serialize(new Dictionary<string, object>
		{
			{
				"type",
				task.type.ToString()
			},
			{
				"goal",
				task.aim
			}
		});
	}

	public static Task Parse(string json)
	{
		Task task = new Task();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary.ContainsKey("type"))
		{
			task.type = (TaskType)Enum.Parse(typeof(TaskType), (string)dictionary["type"]);
		}
		if (dictionary.ContainsKey("goal"))
		{
			task.aim = (int)((long)dictionary["goal"]);
		}
		return task;
	}

	public TaskType type;

	public int aim;
}
