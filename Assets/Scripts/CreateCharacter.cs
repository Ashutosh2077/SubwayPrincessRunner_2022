using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateCharacter : Point
{
	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.InitializeModel(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.GoToNextPoint();
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
		this.InitializeModel(manager);
	}

	private void InitializeModel(PointsManager manager)
	{
		Characters.CharacterType currentCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		string modelName = Characters.characterData[currentCharacter].modelName;
		List<string> list = new List<string>();
		int i = 0;
		int num = this.characterName.Length;
		while (i < num)
		{
			string text = this.characterName[i];
			if (!modelName.Equals(text))
			{
				if (!PointsManager.hasShowedModels.Contains(text))
				{
					list.Add(text);
				}
			}
			i++;
		}
		if (list.Count <= 0)
		{
			UnityEngine.Debug.LogError("No one valid characterName");
			return;
		}
		int num2 = UnityEngine.Random.Range(0, list.Count);
		manager.InitializeCharacterModel(base.transform, this.characterName[num2], this.characterThemeId[num2]);
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	public string[] characterName;

	public int[] characterThemeId;
}
