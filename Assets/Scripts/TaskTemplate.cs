using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;

public class TaskTemplate
{
	public static string ToJson(TaskTemplate template)
	{
		return Json.Serialize(new Dictionary<string, object>
		{
			{
				"description",
				template.description.ToString()
			},
			{
				"descriptionSingle",
				template.descriptionSingle.ToString()
			},
			{
				"ultraShortDescription",
				template.ultraShortDescription.ToString()
			},
			{
				"ultraShortDescriptionSingle",
				template.ultraShortDescriptionSingle.ToString()
			},
			{
				"taskTarget",
				template.taskTarget.ToString()
			},
			{
				"singleRun",
				template.singleRun
			},
			{
				"completeIfLess",
				template.completeIfLess
			},
			{
				"completeIfEqual",
				template.completeIfEqual
			}
		});
	}

	public static TaskTemplate Parse(string json)
	{
		TaskTemplate taskTemplate = new TaskTemplate();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary.ContainsKey("description"))
		{
			taskTemplate.description = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["description"]);
		}
		if (dictionary.ContainsKey("descriptionSingle"))
		{
			taskTemplate.descriptionSingle = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["descriptionSingle"]);
		}
		if (dictionary.ContainsKey("ultraShortDescription"))
		{
			taskTemplate.ultraShortDescription = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["ultraShortDescription"]);
		}
		if (dictionary.ContainsKey("ultraShortDescriptionSingle"))
		{
			taskTemplate.ultraShortDescriptionSingle = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["ultraShortDescriptionSingle"]);
		}
		if (dictionary.ContainsKey("taskTarget"))
		{
			taskTemplate.taskTarget = (TaskTarget)Enum.Parse(typeof(TaskTarget), (string)dictionary["taskTarget"]);
		}
		if (dictionary.ContainsKey("singleRun"))
		{
			taskTemplate.singleRun = (bool)dictionary["singleRun"];
		}
		if (dictionary.ContainsKey("completeIfLess"))
		{
			taskTemplate.completeIfLess = (bool)dictionary["completeIfLess"];
		}
		if (dictionary.ContainsKey("completeIfEqual"))
		{
			taskTemplate.completeIfEqual = (bool)dictionary["completeIfEqual"];
		}
		return taskTemplate;
	}

	public LanguageKey description;

	public LanguageKey descriptionSingle;

	public LanguageKey ultraShortDescription;

	public LanguageKey ultraShortDescriptionSingle;

	public TaskTarget taskTarget;

	public bool singleRun;

	public bool completeIfLess;

	public bool completeIfEqual;
}
